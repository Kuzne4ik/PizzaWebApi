using Microsoft.EntityFrameworkCore;
using PizzaWebApi.Infrastructure.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Hangfire;
using PizzaWebApi.Web.Extentions;
using PizzaWebApi.Web;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using PizzaWebApi.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using PizzaWebApi.Web.Filters.ExceptionFilters;
using MapsterMapper;
using PizzaWebApi.Core.Validators;
using PizzaWebApi.Core;
using PizzaWebApi.Infrastructure.Mediators.Handlers;
using MediatR;
using System.Reflection;
using Hangfire.SqlServer;
using PizzaWebApi.Core.Events;
using PizzaWebApi.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers(opt => 
    {
        // Применение фильтров глобально (не к конкретному контроллеру или Action а ко всему приложению)
        opt.Filters.Add<ApplicationExceptionFilter>();
        opt.Filters.Add<ArgumentExceptionFilter>();
        opt.Filters.Add<AuthenticationExceptionFilter>();
        opt.Filters.Add<KeyNotFoundExceptionFilter>();
        opt.Filters.Add<OrderStateConflictExceptionFilter>();
        opt.Filters.Add<SecurityTokenExceptionFilter>();
    })
    .AddFluentValidation(fluent =>
    {
        // Stop validation on first error
        fluent.ValidatorOptions.DefaultClassLevelCascadeMode = CascadeMode.Stop;
        // Fluent validation only (without attributes validation)
        fluent.DisableDataAnnotationsValidation = true;
        // Validate childs
        fluent.ImplicitlyValidateChildProperties = true;
        // ToDo: Нужно посмотреть
        fluent.LocalizationEnabled = false;
        
    })
    .AddNewtonsoftJson();

// MS SQL Local DB
builder.Services.AddDbContextPool<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MsSQLOnDockerConnection")));
// EF Migration
builder.Services.BuildServiceProvider().GetService<AppDbContext>().Database.Migrate();

builder.Services.AddDbContext<AppIdentityDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MsSQLOnDockerConnection")))
                .AddIdentityCore<ApplicationUser>(
                    opt =>
                    {
                        opt.Password.RequiredLength = 8;
                        opt.Password.RequireNonAlphanumeric = false;
                    })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.AddHttpContextAccessor()
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // укзывает, будет ли валидироваться издатель при валидации токена
                            ValidateIssuer = true,
                            // строка, представляющая издателя
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],

                            // будет ли валидироваться потребитель токена
                            ValidateAudience = true,
                            // установка потребителя токена
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            // будет ли валидироваться время существования
                            ValidateLifetime = true,

                            // установка ключа безопасности
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                            // валидация ключа безопасности
                            ValidateIssuerSigningKey = true,
                        };
                    });

// EF Migration
builder.Services.BuildServiceProvider().GetService<AppIdentityDbContext>().Database.Migrate();



builder.Services.AddEndpointsApiExplorer();

// Serilog
builder.Host.UseSerilog((context, configuration) => configuration
    //.Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithThreadName()
    .ReadFrom.Configuration(context.Configuration)
);

// Регистрируем валиадаторы, используются вместо аттриубутов [Required]
// Find all Fluent Validators in assembly
// найти все классы с валидацией в сборке где есть указанный класс отнаследованные от FluentValidation.AbstractValidator<T>
builder.Services.AddValidatorsFromAssemblyContaining<CategoryDTOValidator>();

//Mapster (маппинг)
builder.Services.AddSingleton(MapsterMapperSetup.GetTypeAdapterConfig());
builder.Services.AddScoped<IMapper, ServiceMapper>();

// Add service health check include SqlServer with EF check
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

// Разрешить кросдоменное взаимодействие CORS со стороны этого API
// Браузер отправит "предваоительный" запрос OPTIONS для проверки: разрешено да/нет кросдоменное взаимодействие CORS
// Необходимо для работы с Angular
builder.Services.AddCors();

// Использовать специальный HttpClient для Core API IHttpClientFactory
builder.Services.AddHttpClient();

builder.Services.AddSwaggerGen(options =>
{
    // Add comments 
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Add FV Rules to swagger
builder.Services.AddFluentValidationRulesToSwagger();


#region Hangfire - как Cron. Создает таблицы в БД с данными для работы. Проблемы с либой EF при развертывании в Production (помогает выключить совсем)

// Add Hangfire services.
builder.Services.AddHangfire(t => t
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(
        builder.Configuration.GetConnectionString("MsSQLOnDockerConnection"),
        new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

// Register Handfire Jobs
builder.Services.AddHostedService<HandfireRegisterJobsHostedService>();

#endregion

// MediatR (работает с событиями основной вариант)
builder.Services.AddMediatR(typeof(GetAllProductsHandler));

#region Events Для работы с событиями альтернативный вариант с помощью DI

// Events
builder.Services.AddScoped<IEventPublisher, EventPublisher>();

// Find all and Add to service collection IEnumerable<IEventListener>
// Найти все Классы-слушатели событий-перехватчики в проекте,
// которые наследуются от IEventListener<T> и добавить их в IServiceCollection как синглтоны
builder.Services.Scan(scan => scan.FromAssemblyOf<IEventListener>()
    .AddClasses(classes => classes.AssignableTo<IEventListener>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime());


#endregion

// Add Services (пример как вынести часть кода в отедельный класс)
builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

// For Angular
app.UseCors(x => x
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());

// Url to check health status string Healthy/Degraded/Unhealthy
app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    AllowCachingResponses = false
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Request info in log event
app.UseSerilogRequestLogging();


//Hangfire Dashboard http://localhost:5118/hangfire
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    //Подключение авторизации для UI Hangfire
    Authorization = new[] { new HangfireDashboardAuthorization() }
});

// StaticFileMiddleware файлы картинок, css
app.UseStaticFiles();

// Для проброса заголовков https и IP адрес клиента сковзь прокси серверер
// Чтобы не замещались "пустыми" данными прокси серевера
app.UseForwardedHeaders(new ForwardedHeadersOptions()
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// EndpointRoutingMiddleware 
app.UseRouting();

// AuthenticationMiddleware
app.UseAuthentication();
// AuthorizationMiddleware
app.UseAuthorization();

app.MapControllers();

app.Run();
// Develop: Comment to delete 3 develop v. 2.0