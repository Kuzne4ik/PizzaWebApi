using FluentValidation;
using Hangfire;
using MapsterMapper;
using PizzaWebApi.Core;
using PizzaWebApi.Core.Events;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Infrastructure.Data;
using PizzaWebApi.Infrastructure.Mediators.Handlers;
using PizzaWebApi.Infrastructure.Services;
using MediatR;
using PizzaWebApi.Core.Validators;
using Hangfire.SqlServer;
using System.Reflection;

namespace PizzaWebApi.Web.Extentions
{
    /// <summary>
    /// Add Services
    /// </summary>
    public static class WebApplicationBuilderExtentions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSwaggerGen(options =>
            {
                // Add comments 
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            // Add Hangfire services.
            services.AddHangfire(t => t
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
                    config.GetConnectionString("MsSQLOnDockerConnection"),
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            // Register Handfire Jobs
            services.AddHostedService<HandfireRegisterJobsHostedService>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPromoCodeService, PromoCodeService>();
            services.AddScoped<ITelegramService, TelegramService>();
            services.AddScoped<IAccountService, AccountService>();

            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IOrderItemRepository, OrderItemRepository>();
            services.AddTransient<ICartRepository, CartRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ICartItemRepository, CartItemRepository>();

            // Events
            services.AddScoped<IEventPublisher, EventPublisher>();

            // Get IEnumerable<IEventListener>
            services.Scan(scan => scan.FromAssemblyOf<IEventListener>()
                .AddClasses(classes => classes.AssignableTo<IEventListener>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

            // Find all Fluent Validators in assembly
            services.AddValidatorsFromAssemblyContaining<CategoryDTOValidator>();

            //Mapster
            services.AddSingleton(MapsterMapperSetup.GetTypeAdapterConfig());
            services.AddScoped<IMapper, ServiceMapper>();

            // MediatR
            services.AddMediatR(typeof(GetAllProductsHandler));

            return services;
        }
    }
}
