using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Infrastructure.Data;
using PizzaWebApi.Infrastructure.Services;
using PizzaWebApi.Web.Filters.ActionFilters;

namespace PizzaWebApi.Web.Extentions
{
    /// <summary>
    /// Add Services
    /// </summary>
    public static class WebApplicationBuilderExtentions
    {
        /// <summary>
        /// Add some Services
        /// </summary>
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
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

            // Если атрибут для фильтра унаследован от ServiceFilterAttribute, то фильтр должен быть зарегситрирован в ServiceCollection
            // Если атрибут для фильтра унаследован от TypeFilterAttribute, то фильтр не должен быть зарегситрирован в ServiceCollection
            services.AddTransient<EnsureCartExistsActionFilter>();
            services.AddTransient<EnsureProductExistsActionFilter>();
            services.AddTransient<EnsureOrderExistsActionFilter>();

            

            return services;
        }
    }
}
