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
using PizzaWebApi.Web.Filters.ExceptionFilters;

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
        //Stop validation on first error
        fluent.ValidatorOptions.CascadeMode = CascadeMode.Stop;
        //Fluent validation only (without MVC validation)
        fluent.DisableDataAnnotationsValidation = true;
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
builder.Host.UseSerilog((context, services, configuration) => configuration
                    .Enrich.FromLogContext()
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .WriteTo.Console());

// Add Servicess
builder.Services.AddServices(builder.Configuration);

// Add service health check include SqlServer with EF check
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

// For Angular
builder.Services.AddCors();

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

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
// Develop: Comment to delete 3 develop v. 2.0