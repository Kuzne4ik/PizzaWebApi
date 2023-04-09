using Mapster;
using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.ApiModels;

namespace PizzaWebApi.Core
{
    public class MapsterMapperSetup
    {
        public static TypeAdapterConfig GetTypeAdapterConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<Product, ProductDTO>();
            config.NewConfig<ProductDTO, Product>();

            config.NewConfig<Category, CategoryDTO>();
            config.NewConfig<CategoryDTO, Category>();

            config.NewConfig<Cart, CartDTO>();
            config.NewConfig<CartDTO, Cart>();

            config.NewConfig<CartItemDTO, CartItemDTO>();
            config.NewConfig<CartItemDTO, CartItemDTO>();

            config.NewConfig<Cart, CartDTO>();
            config.NewConfig<CartDTO, Cart>();

            return config;
        }
    }
}
