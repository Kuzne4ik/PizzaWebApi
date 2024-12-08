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
        // ���������� �������� ��������� (�� � ����������� ����������� ��� Action � �� ����� ����������)
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
        // ToDo: ����� ����������
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
                            // ��������, ����� �� �������������� �������� ��� ��������� ������
                            ValidateIssuer = true,
                            // ������, �������������� ��������
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],

                            // ����� �� �������������� ����������� ������
                            ValidateAudience = true,
                            // ��������� ����������� ������
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            // ����� �� �������������� ����� �������������
                            ValidateLifetime = true,

                            // ��������� ����� ������������
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                            // ��������� ����� ������������
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

// ������������ �����������, ������������ ������ ����������� [Required]
// Find all Fluent Validators in assembly
// ����� ��� ������ � ���������� � ������ ��� ���� ��������� ����� ��������������� �� FluentValidation.AbstractValidator<T>
builder.Services.AddValidatorsFromAssemblyContaining<CategoryDTOValidator>();

//Mapster (�������)
builder.Services.AddSingleton(MapsterMapperSetup.GetTypeAdapterConfig());
builder.Services.AddScoped<IMapper, ServiceMapper>();

// Add service health check include SqlServer with EF check
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

// ��������� ������������ �������������� CORS �� ������� ����� API
// ������� �������� "���������������" ������ OPTIONS ��� ��������: ��������� ��/��� ������������ �������������� CORS
// ���������� ��� ������ � Angular
builder.Services.AddCors();

// ������������ ����������� HttpClient ��� Core API IHttpClientFactory
builder.Services.AddHttpClient();

builder.Services.AddSwaggerGen(options =>
{
    // Add comments 
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Add FV Rules to swagger
builder.Services.AddFluentValidationRulesToSwagger();


#region Hangfire - ��� Cron. ������� ������� � �� � ������� ��� ������. �������� � ����� EF ��� ������������� � Production (�������� ��������� ������)

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

// MediatR (�������� � ��������� �������� �������)
builder.Services.AddMediatR(typeof(GetAllProductsHandler));

#region Events ��� ������ � ��������� �������������� ������� � ������� DI

// Events
builder.Services.AddScoped<IEventPublisher, EventPublisher>();

// Find all and Add to service collection IEnumerable<IEventListener>
// ����� ��� ������-��������� �������-������������ � �������,
// ������� ����������� �� IEventListener<T> � �������� �� � IServiceCollection ��� ���������
builder.Services.Scan(scan => scan.FromAssemblyOf<IEventListener>()
    .AddClasses(classes => classes.AssignableTo<IEventListener>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime());


#endregion

// Add Services (������ ��� ������� ����� ���� � ���������� �����)
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
    //����������� ����������� ��� UI Hangfire
    Authorization = new[] { new HangfireDashboardAuthorization() }
});

// StaticFileMiddleware ����� ��������, css
app.UseStaticFiles();

// ��� �������� ���������� https � IP ����� ������� ������ ������ ��������
// ����� �� ���������� "�������" ������� ������ ��������
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