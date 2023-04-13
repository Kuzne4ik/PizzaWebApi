using MapsterMapper;
using Moq;
using PizzaWebApi.Core.Models;
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
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.Requests;
using MediatR;

namespace PizzaWebApi.Tests.Services
{
    public class OrderServiceTest
    {
        readonly IMapper _mapper = null;
        private Mock<ILogger<OrderService>> _logger;
        private PromoCodeService _promoCodeService;
        Mock<IMediator> _mediatorMock;

        static Product product = new()
        {
            Id = 1,
            CategoryId = 1,
            Name = "Cat",
            Title = "Cat",
            Description = string.Empty,
            Price = 10
        };

        List<Cart> _cartsDB = new()
        {
            new Cart
            {
                Id = 1,
                UserId = 1,
                PromoCode = "ANY",
                CartItems = new List<CartItem>()
                {
                    new CartItem
                    {
                        Id = 1,
                        CartId = 1,
                        Created = DateTime.UtcNow,
                        ProductId = 1,
                        Quantity = 2,
                        Product = product
                    }
                }
            }
        };

        List<Order> _ordersDB = new()
        {
            new Order
            {
                Id = 1,
                UserId = 1,
                OrderItems = new List<OrderItem>()
            },
            new Order
            {
                Id = 2,
                UserId = 1,
                OrderItems = new List<OrderItem>()
            },
            new Order
            {
                Id = 3,
                UserId = 1,
                OrderItems = new List<OrderItem>()
            },
            new Order
            {
                Id = 4,
                UserId = 1,
                OrderItems = new List<OrderItem>()
            },
            new Order
            {
                Id = 5,
                UserId = 1,
                OrderItems = new List<OrderItem>()
            },
            new Order
            {
                Id = 6,
                UserId = 1,
                OrderItems = new List<OrderItem>()
            }
        };


        public OrderServiceTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _promoCodeService = new PromoCodeService();
            _mapper = new Mapper(MapsterMapperSetup.GetTypeAdapterConfig());
            _logger = new Mock<ILogger<OrderService>>();
        }

        [Fact]
        public async Task GetById()
        {
            // # Arrange
            var testId = _ordersDB.First().Id;
            var ordersMock = _ordersDB.BuildMock();

            Expression<Func<Order, bool>> predicate = t => t.Id == testId;

            var orderRepositoryMock = new Mock<IOrderRepository>();
            orderRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .Callback<Expression<Func<Order, bool>>>(s => predicate = s)
                .ReturnsAsync(ordersMock.Any(predicate));

            orderRepositoryMock
                .Setup(x => x.FindByConditionQuery(It.IsAny<Expression<Func<Order, bool>>>()))
                    .Returns(ordersMock.Where(t => t.Id == testId));

            var cartRepositoryMock = new Mock<ICartRepository>();
            var cartItemRepositoryMock = new Mock<ICartItemRepository>();
            var orderItemRepositoryMock = new Mock<IOrderItemRepository>();
            var eventPublisherMock = new Mock<IEventPublisher>();

            var orderService = new OrderService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                orderRepositoryMock.Object,
                orderItemRepositoryMock.Object,
                _promoCodeService,
                eventPublisherMock.Object,
                _mediatorMock.Object,
                _mapper,
                _logger.Object);

            // # Act
            var orderDTO = await orderService.GetByIdAsync(testId);

            // # Assert
            Assert.NotNull(orderDTO);
            Assert.NotNull(orderDTO.OrderItems);
        }

        [Fact]
        public async Task Checkout()
        {
            // # Arrange
            var cart = _cartsDB.First();
            var cartId = cart.Id;
            var newOrderId = 100;

            var orders = new List<Order>();

            var orderItems = new List<OrderItem>();

            var orderDetails = new OrderDetailsDTO()
            {
                UserId = 1,
                LastName = "Sid",
                FirstName = "Alex",
                SurName = "Sid",
                Phone = "77777777777",
                Email = "1@mail.ru",
                Address = "Town",
                DeliveryType = DeliveryType.SelfDelivery
            };

            var ordersMock = _ordersDB.BuildMock();
            var cartsMock = _cartsDB.BuildMock();
            var cartItems = _cartsDB.SelectMany(t => t.CartItems).ToList();
            var cartItemsMock = cartItems.BuildMock();

            var orderRepositoryMock = new Mock<IOrderRepository>();
            orderRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Order>()))
                .Callback((Order newOrder) => {
                    newOrder.Id = newOrderId;
                    orders.Add(newOrder);
                })
                .ReturnsAsync(true);

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                .ReturnsAsync(cartsMock.Any(t => t.Id == cartId));

            cartRepositoryMock
                .Setup(x => x.FindByIdAsync(cartId))
                .ReturnsAsync(_cartsDB.First(t => t.Id == cartId));

            cartRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Cart>()))
                .ReturnsAsync(true);

            cartRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                    .ReturnsAsync(cartsMock.Any(t => t.Id == cartId));

            cartRepositoryMock
                .Setup(x => x.ClearCart(It.IsAny<int>()))
                    .Callback((int cartId) =>
                    {
                        var targetCart = _cartsDB.First(t => t.Id == cartId);
                        targetCart.CartItems.Clear();
                    })
                .ReturnsAsync(true);


            var cartItemRepositoryMock = new Mock<ICartItemRepository>();
            cartItemRepositoryMock
                .Setup(x => x.GetCartItemsAsync(cartId))
                .ReturnsAsync(cartItems.Where(t => t.CartId == cartId).ToList());

            var orderItemRepositoryMock = new Mock<IOrderItemRepository>();

            orderItemRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<OrderItem>()))
                .Callback((OrderItem newOrderItem) => { 
                    newOrderItem.Id = 100;
                    orderItems.Add(newOrderItem); 
                })
                .ReturnsAsync(true);

            var eventPublisherMock = new Mock<IEventPublisher>();

            var orderService = new OrderService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                orderRepositoryMock.Object,
                orderItemRepositoryMock.Object,
                _promoCodeService,
                eventPublisherMock.Object,
                _mediatorMock.Object,
                _mapper,
                _logger.Object);

            // # Act
            var orderId = await orderService.CheckoutAsync(cartId, orderDetails);
            var newOrder = orders.FirstOrDefault();

            // # Assert
            Assert.NotNull(newOrder);
            Assert.Equal(newOrderId, orderId);
            Assert.Equal(newOrder.UserId, orderDetails.UserId);
            Assert.Equal(newOrder.LastName, orderDetails.LastName);
            Assert.Equal(newOrder.FirstName, orderDetails.FirstName);
            Assert.Equal(newOrder.SurName, orderDetails.SurName);
            Assert.Equal(newOrder.Phone, orderDetails.Phone);
            Assert.Equal(newOrder.Email, orderDetails.Email);
            Assert.Equal(newOrder.Address, orderDetails.Address);
            Assert.Equal(newOrder.DeliveryType, orderDetails.DeliveryType);
            Assert.True(newOrder.Total > 0);
            Assert.True(newOrder.State == OrderState.Paid);
        }

        [Fact]
        public async Task FindAll()
        {
            // # Arrange
            var ordersMock = _ordersDB.BuildMock();
            var limit = 5;

            var orderRepositoryMock = new Mock<IOrderRepository>();
            orderRepositoryMock
                .Setup(x => x.ListQuery())
                .Returns(ordersMock);

            var cartRepositoryMock = new Mock<ICartRepository>();
            var cartItemRepositoryMock = new Mock<ICartItemRepository>();
            var orderItemRepositoryMock = new Mock<IOrderItemRepository>();
            var eventPublisherMock = new Mock<IEventPublisher>();

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

            var orderService = new OrderService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                orderRepositoryMock.Object,
                orderItemRepositoryMock.Object,
                _promoCodeService,
                eventPublisherMock.Object,
                _mediatorMock.Object,
                _mapper,
                _logger.Object);

            // # Act
            var searchResultOrders = await orderService.FindAllAsync(pageCriteriaRequest1);

            // # Assert
            Assert.Equal(limit, searchResultOrders.Results.Count());
            Assert.Equal(_ordersDB.Count, searchResultOrders.Total);

            // # Act
            searchResultOrders = await orderService.FindAllAsync(pageCriteriaRequest2);

            // # Assert
            Assert.Equal(_ordersDB.Count - limit, searchResultOrders.Results.Count());
        }

        [Fact]
        public async Task SetOrderCompletedAsync()
        {
            // # Arrange
            int orderId = _ordersDB.First().Id;
            var ordersMock = _ordersDB.BuildMock();

            var orderRepositoryMock = new Mock<IOrderRepository>();
            orderRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(ordersMock.Any(t => t.Id == orderId));

            orderRepositoryMock
                .Setup(x => x.FindByIdAsync(orderId))
                .ReturnsAsync(() =>
                {
                    return _ordersDB.SingleOrDefault(t => t.Id == orderId);
                });

            orderRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Order>()))
                .ReturnsAsync(() =>
                    {
                        var targetOrder = _ordersDB.SingleOrDefault(t => t.Id == orderId);
                        targetOrder.State = OrderState.Completed;
                        return true;
                    });

            var cartRepositoryMock = new Mock<ICartRepository>();
            var cartItemRepositoryMock = new Mock<ICartItemRepository>();
            var orderItemRepositoryMock = new Mock<IOrderItemRepository>();
            var eventPublisherMock = new Mock<IEventPublisher>();

            var orderService = new OrderService(cartRepositoryMock.Object,
                cartItemRepositoryMock.Object,
                orderRepositoryMock.Object,
                orderItemRepositoryMock.Object,
                _promoCodeService,
                eventPublisherMock.Object,
                _mediatorMock.Object,
                _mapper,
                _logger.Object);

            // # Act
            var resullt = await orderService.SetOrderCompletedAsync(orderId);

            // # Assert
            Assert.True(resullt);

        }
    }
}
