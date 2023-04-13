using MapsterMapper;
using Moq;
using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.Interfaces;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using PizzaWebApi.Core;
using Microsoft.Extensions.Logging;
using PizzaWebApi.Infrastructure.Services;
using MockQueryable.Moq;
using System;
using PizzaWebApi.Core.Requests;

namespace PizzaWebApi.Tests.Services
{
    public class CartServiceTest
    {
        readonly IMapper _mapper = null;
        private Mock<ILogger<CartService>> _logger;
        private IPromoCodeService _promoCodeService;

        List<Cart> _cartsDB = new()
        {
            new Cart
            {
                Id = 1,
                UserId = 1,
                PromoCode = "Any",
                CartItems = new List<CartItem>()
            }
            ,
            new Cart
            {
                Id = 2,
                UserId = 2,
                CartItems = new List<CartItem>()
            },
            new Cart
            {
                Id = 3,
                UserId = 3,
                PromoCode = "Any",
                CartItems = new List<CartItem>()
            }
            ,
            new Cart
            {
                Id = 4,
                UserId = 4,
                CartItems = new List<CartItem>()
            },
            new Cart
            {
                Id = 5,
                UserId = 5,
                PromoCode = "Any",
                CartItems = new List<CartItem>()
            }
            ,
            new Cart
            {
                Id = 6,
                UserId = 6,
                CartItems = new List<CartItem>()
            }
        };

        List<Product> _productsDB = new()
        {
            new Product
            {
                Id = 1,
                CategoryId = 1,
                Name = "Cat",
                Title = "Cat",
                Description = string.Empty,
                Price = 10
            },
            new Product
            {
                Id = 2,
                CategoryId = 1,
                Name = "Kitten",
                Title = "Kitten",
                Description = string.Empty,
                Price = 10
            }
        };

        public CartServiceTest()
        {
            _promoCodeService = new PromoCodeService();
            _mapper = new Mapper(MapsterMapperSetup.GetTypeAdapterConfig());
            _logger = new Mock<ILogger<CartService>>();
        }

        [Fact]
        public async Task GetById()
        {
            // # Arrange
            var cartsMock = _cartsDB.BuildMock();
            int cartId = _cartsDB.First().Id;

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock
                .Setup(x => x.FindByIdAsync(cartId))
                .ReturnsAsync(_cartsDB.FirstOrDefault(t => t.Id == cartId));

            cartRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .ReturnsAsync(cartsMock.Any(t => t.Id == cartId));

            cartRepositoryMock
                .Setup(x => x.FindByConditionQuery(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .Returns(cartsMock.Where(t => t.Id == cartId));

            var cartItemRepositoryMock = new Mock<ICartItemRepository>();

            var cartService = new CartService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                _promoCodeService,
                _mapper,
                _logger.Object);

            // # Act
            var cartDTO = await cartService.GetByIdAsync(cartId);

            // # Assert
            Assert.NotNull(cartDTO);
            Assert.NotNull(cartDTO.CartItems);
        }

        [Fact]
        public async Task GetCartByUserId()
        {
            // # Arrange
            int userId = 1;
            var cartsMock = _cartsDB.BuildMock();

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock
                .Setup(x => x.GetCartIdByUserId(It.IsAny<int>()))
                .Returns(_cartsDB.First(t => t.UserId == userId).Id);

            cartRepositoryMock
                .Setup(x => x.FindByConditionQuery(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .Returns(cartsMock.Where(t => t.UserId == userId));

            var cartItemRepositoryMock = new Mock<ICartItemRepository>();

            var cartService = new CartService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                _promoCodeService,
                _mapper,
                _logger.Object);

            // # Act
            var cartDTO = await cartService.GetCartByUserId(userId);

            // # Assert
            Assert.NotNull(cartDTO);
            Assert.NotNull(cartDTO.CartItems);
        }

        [Fact]
        public async Task AddItemItemToCart()
        {
            // # Arrange
            var cartsMock = _cartsDB.BuildMock();
            var cartItems = _cartsDB.SelectMany(t => t.CartItems).ToList();
            var cartItemsMock = cartItems.BuildMock();

            var targetCart = _cartsDB.First();
            var targetProduct = _productsDB.First();

            int cartId = targetCart.Id;
            int productId = targetProduct.Id;

            int cartItemNewId = 1;
            var newCartItem = new CartItem
            {
                Id = cartItemNewId,
                CartId = cartId,
                Created = DateTime.UtcNow,
                ProductId = productId,
                Product = targetProduct,
                Quantity = 1
            };

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .ReturnsAsync(cartsMock.Any(t => t.Id == cartId));
            cartRepositoryMock
                .Setup(x => x.FindByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(cartsMock.FirstOrDefault(t => t.Id == cartId));
            

            var cartItemRepositoryMock = new Mock<ICartItemRepository>();

            cartItemRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<CartItem, bool>>>()))
                    .ReturnsAsync(cartItemsMock.Any(t => t.CartId == cartId && t.ProductId == productId));

            cartItemRepositoryMock
                .Setup(x => x.GetCartItem(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(cartItems.SingleOrDefault(t => t.CartId == cartId && t.ProductId == productId));

            cartItemRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<CartItem>()))
                    .Callback( () => targetCart.CartItems.Add(newCartItem))
                    .ReturnsAsync(true);

            var newCartItems = new List<CartItem>();
            newCartItems.Add(newCartItem);
            
            cartItemRepositoryMock
                .Setup(x => x.FindByConditionQuery(It.IsAny<Expression<Func<CartItem, bool>>>()))
                    .Returns(newCartItems.BuildMock().Where(t => t.Id == cartItemNewId));


            var cartService = new CartService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                _promoCodeService,
                _mapper,
                _logger.Object);

            // # Act
            var cartItemDTO = await cartService.AddItemToCartAsync(cartId, productId);

            // # Assert
            Assert.NotNull(cartItemDTO);
            Assert.Equal(cartId, cartItemDTO.CartId);
            Assert.Equal(productId, cartItemDTO.ProductId);
            Assert.Equal(1, cartItemDTO.Quantity);

            var controlCartItem = _cartsDB.First(t => t.Id == cartId).CartItems.FirstOrDefault(t => t.ProductId == productId);
            Assert.NotNull(newCartItem);

            Assert.Equal(newCartItem.CartId, cartId);
            Assert.Equal(1, newCartItem.Quantity);
        }

        [Fact]
        public async Task RemoveItemItemFromCart()
        {
            // # Arrange
            int cartId = _cartsDB.First().Id;
            int productId = _productsDB.First().Id;

            var cartItem = new CartItem
            {
                Id = 1,
                CartId = cartId,
                ProductId = productId,
                Quantity = 1,
                Created = DateTime.UtcNow
            };
            _cartsDB.First(t => t.Id == cartId).CartItems.Add(cartItem);

            var cartsMock = _cartsDB.BuildMock();
            var cartItems = _cartsDB.SelectMany(t => t.CartItems).ToList();
            var cartItemsMock = cartItems.BuildMock();

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .ReturnsAsync(cartsMock.Any(t => t.Id == cartId));
            cartRepositoryMock
                .Setup(x => x.FindByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(cartsMock.FirstOrDefault(t => t.Id == cartId));

            var cartItemRepositoryMock = new Mock<ICartItemRepository>();

            cartItemRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<CartItem, bool>>>()))
                    .ReturnsAsync(cartItemsMock.Any(t => t.CartId == cartId && t.ProductId == productId));

            cartItemRepositoryMock
                .Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<int>()))
                    .Callback((int cartId, int productId) =>
                    {
                        var targetCart = _cartsDB.First(t => t.Id == cartId);
                        var cartItemToDelete = targetCart.CartItems.First(t => t.ProductId == productId);
                        targetCart.CartItems.Remove(cartItemToDelete);
                    })
                .ReturnsAsync(true);

            var cartService = new CartService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                _promoCodeService,
                _mapper,
                _logger.Object);

            // # Act
            var result = await cartService.RemoveItemFromCartAsync(cartId, productId);

            // # Assert
            Assert.True(result);
            Assert.Equal(0, _cartsDB.First(t => t.Id == cartId).CartItems.Count);
        }

        [Fact]
        public async Task UpdateItem()
        {
            // # Arrange
            int cartId = _cartsDB.First().Id;
            int productId = _productsDB.First().Id;
            int quantity = 2;

            var cartItem = new CartItem
            {
                Id = 1,
                CartId = cartId,
                ProductId = productId,
                Quantity = 1,
                Created = DateTime.UtcNow
            };
            _cartsDB.First(t => t.Id == cartId).CartItems.Add(cartItem);

            var cartsMock = _cartsDB.BuildMock();
            var cartItems = _cartsDB.SelectMany(t => t.CartItems).ToList();
            var cartItemsMock = cartItems.BuildMock();

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .ReturnsAsync(cartsMock.Any(t => t.Id == cartId));
            cartRepositoryMock
                .Setup(x => x.FindByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(cartsMock.FirstOrDefault(t => t.Id == cartId));

            var cartItemRepositoryMock = new Mock<ICartItemRepository>();
            cartItemRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<CartItem, bool>>>()))
                    .ReturnsAsync(cartItemsMock.Any(t => t.CartId == cartId && t.ProductId == productId));
            cartItemRepositoryMock
                .Setup(x => x.GetCartItem(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(cartItems.SingleOrDefault(t => t.CartId == cartId && t.ProductId == productId));

            cartItemRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<CartItem>()))
                    .ReturnsAsync(() =>
                    {
                        var targetCartItem = cartItems.SingleOrDefault(t => t.CartId == cartId && t.ProductId == productId);

                        targetCartItem.Quantity = quantity;
                        return true;

                    });

            var cartService = new CartService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                _promoCodeService,
                _mapper,
                _logger.Object);

            // # Act
            var resullt = await cartService.UpdateItemAsync(cartId, productId, quantity);

            // # Assert
            Assert.True(resullt);
        }

        [Fact]
        public async Task ClearCart()
        {
            // # Arrange
            int cartId = _cartsDB.First().Id;
            int productId = _productsDB.First().Id;

            var cartItem = new CartItem
            {
                Id = 1,
                CartId = cartId,
                ProductId = productId,
                Created = DateTime.UtcNow
            };
            _cartsDB.First(t => t.Id == cartId).CartItems.Add(cartItem);

            var cartsMock = _cartsDB.BuildMock();
            var cartItems = _cartsDB.SelectMany(t => t.CartItems).ToList();
            var cartItemsMock = cartItems.BuildMock();

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .ReturnsAsync(cartsMock.Any(t => t.Id == cartId));

            cartRepositoryMock
                .Setup(x => x.FindByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(cartsMock.FirstOrDefault(t => t.Id == cartId));

            cartRepositoryMock
                .Setup(x => x.ClearCart(It.IsAny<int>()))
                    .Callback((int cartId) =>
                    {
                        var targetCart = _cartsDB.First(t => t.Id == cartId);
                        targetCart.CartItems.Clear();
                    })
                .ReturnsAsync(true);

            var cartItemRepositoryMock = new Mock<ICartItemRepository>();


            var cartService = new CartService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                _promoCodeService,
                _mapper,
                _logger.Object);

            // # Act
            var result = await cartService.ClearCart(cartId);

            // # Assert
            Assert.True(result);
            Assert.Equal(0, _cartsDB.First(t => t.Id == cartId).CartItems.Count);
        }

        [Fact]
        public async Task SetPromocode()
        {
            // # Arrange
            int cartId = _cartsDB.First().Id;
            var promocode = "any";
            var cartsMock = _cartsDB.BuildMock();

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .ReturnsAsync(cartsMock.Any(t => t.Id == cartId));
            cartRepositoryMock
                .Setup(x => x.FindByIdAsync(cartId))
                .ReturnsAsync(_cartsDB.FirstOrDefault(t => t.Id == cartId));

            cartRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Cart>()))
                .ReturnsAsync(true);

            var cartItemRepositoryMock = new Mock<ICartItemRepository>();

            var cartService = new CartService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                _promoCodeService,
                _mapper,
                _logger.Object);

            // # Act
            var result = await cartService.SetPromocode(cartId, promocode);

            // # Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetTotal()
        {
            // # Arrange
            var targetCart = _cartsDB.First();
            targetCart.PromoCode = "any";

            int cartId = targetCart.Id;

            var product = new Product
            {
                Id = 1,
                CategoryId = 1,
                Name = "Fisrt",
                Title = "First",
                Description = String.Empty,
                Price = 10
            };

            var cartItem = new CartItem
            {
                Id = 1,
                CartId = cartId,
                ProductId = product.Id,
                Quantity = 1,
                Created = DateTime.UtcNow,
                Product = product
            };
            targetCart.CartItems.Add(cartItem);

            var cartsMock = _cartsDB.BuildMock();
            var cartItems = _cartsDB.SelectMany(t => t.CartItems).ToList();
            var cartItemsMock = cartItems.BuildMock();

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .ReturnsAsync(cartsMock.Any(t => t.Id == cartId));
            cartRepositoryMock
                .Setup(x => x.FindByConditionQuery(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .Returns(cartsMock.Where(t => t.Id == cartId));

            var sum = cartItems.Where(t => t.CartId == cartId).Sum(t => t.Quantity * t.Product.Price);
            var cartItemRepositoryMock = new Mock<ICartItemRepository>();
            cartItemRepositoryMock
                .Setup(x => x.GetSum(It.IsAny<int>()))
                    .ReturnsAsync(sum);

            var cartService = new CartService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                _promoCodeService,
                _mapper,
                _logger.Object);

            // # Act
            var total = await cartService.GetTotal(cartId);

            // # Assert
            var controlValueWithoutPromoCode = targetCart.CartItems.Sum(x => x.Product.Price);
            Assert.True(controlValueWithoutPromoCode > total);
        }

        [Fact]
        public async Task FindAll()
        {
            // # Arrange

            var cartsMock = _cartsDB.BuildMock();

            var limit = 5;

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock
                .Setup(x => x.ListQuery())
                .Returns(cartsMock);

            var cartItemRepositoryMock = new Mock<ICartItemRepository>();

            var cartService = new CartService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                _promoCodeService,
                _mapper,
                _logger.Object);

            var pageCriteriaRequest1 = new PageCriteriaRequest()
            {
                Page = 1,
                PageSize = limit,
            };

            var pageCriteriaRequest2 = new PageCriteriaRequest()
            {
                Page = 2,
                PageSize = limit,
            };

            // # Act
            var cartsSearchResult = await cartService.FindAllAsync(pageCriteriaRequest1);

            // # Assert
            Assert.Equal(limit, cartsSearchResult.Results.Count());
            Assert.Equal(_cartsDB.Count, cartsSearchResult.Total);

            
            // # Act
            cartsSearchResult = await cartService.FindAllAsync(pageCriteriaRequest2);

            // # Assert
            Assert.Equal(_cartsDB.Count - limit, cartsSearchResult.Results.Count());
        }
    }
}
