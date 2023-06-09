﻿using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Events;
using PizzaWebApi.Core.Exceptions;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Core.Response;
using PizzaWebApi.Infrastructure.Mediators.Notifications;

namespace PizzaWebApi.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private IPromoCodeService _promoCodeService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        

        public OrderService(ICartRepository cartRepository, 
            ICartItemRepository cartItemRepository, 
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IPromoCodeService promoCodeService,
            IEventPublisher eventPublisher,
            IMediator mediator,
            IMapper mapper, 
            ILogger<OrderService> logger)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _promoCodeService = promoCodeService;

            _eventPublisher = eventPublisher;
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all
        /// </summary>
        /// <param name="pageCriteriaRequest"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<SearchResult<OrderDTO>> FindAllAsync(PageCriteriaRequest pageCriteriaRequest)
        {
            _logger.LogInformation($"{nameof(FindAllAsync)} run");

            try
            {
                var allQuery = _orderRepository.ListQuery();
                var query = allQuery.Skip(pageCriteriaRequest.Skip).Take(pageCriteriaRequest.PageSize);
                
                var count = await allQuery.CountAsync();
                var items = await query.ProjectToType<OrderDTO>().ToListAsync();

                return new SearchResult<OrderDTO>(items, count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(FindAllAsync)} exception");
                throw new ApplicationException("Get Orders failed");
            }
        }

        /// <summary>
        /// Create new Order
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        /// <param name="orderDetails">Order details</param>
        /// <returns>Order ID</returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> CheckoutAsync(int cartId, OrderDetailsDTO orderDetails)
        {
            _logger.LogInformation($"{nameof(CheckoutAsync)} run");

            Cart cart = null;
            try
            {
                cart = await _cartRepository.FindByIdAsync(cartId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CheckoutAsync)} exception");
                throw new ApplicationException("Get cart failed");
            }

            if (cart.UserId != orderDetails.UserId)
            {
                throw new ArgumentException($"Arg {nameof(orderDetails)} is wrong");
            }

            IList<CartItem> cartItems = null;

            try 
            {
                cartItems = await _cartItemRepository.GetCartItemsAsync(cartId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CheckoutAsync)} exception");
                throw new ApplicationException("Get cart items failed");
            }

            if(!cartItems.Any())
            {
                throw new Exception($"Cart items is empty");
            }
            try
            { 
                var total = 0m;
                var sum = cartItems.Sum(x => x.Product.Price * x.Quantity);

                if (!string.IsNullOrEmpty(cart.PromoCode))
                {
                    var discount = _promoCodeService.CarculateDiscount(cart.PromoCode, sum);
                    total = sum - discount;
                }
                else
                    total = sum;

                var order = new Order
                {
                    UserId = cart.UserId,
                    CreatedBy = cart.CreatedBy,
                    LastName = orderDetails.LastName,
                    FirstName = orderDetails.FirstName,
                    SurName = orderDetails.SurName,
                    Phone = orderDetails.Phone,
                    Email = orderDetails.Email,
                    Address = orderDetails.Address,
                    DeliveryType = orderDetails.DeliveryType,
                    State = OrderState.Paid,
                    Total = total
                };
                var res = await _orderRepository.AddAsync(order);
                if (!res)
                    throw new Exception("Create Order failed");

                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Product.Price
                    };
                    _ = await _orderItemRepository.AddAsync(orderItem);
                }
                _ = await _cartRepository.ClearCart(cartId);

                // Skip cart promocode
                cart.PromoCode = string.Empty;
                _ = await _cartRepository.UpdateAsync(cart);

                // Publish Event for listeners
                _eventPublisher.Publish(new OrderStateChagedEvent(order.Id, order.State));
                // Publish Request for Handler
                _ = _mediator.Publish(new OrderStateChangedNotification(order.Id, order.State)).ConfigureAwait(false);

                return order.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CheckoutAsync)} exception");
                throw new ApplicationException("Create Order failed");
            }
        }

        public async Task<bool> SetOrderCompletedAsync(int orderId)
        {
            _logger.LogInformation($"{nameof(SetOrderCompletedAsync)} run");

            Order order;
            try 
            { 
                order = await _orderRepository.FindByIdAsync(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SetOrderCompletedAsync)} exception");
                throw new ApplicationException($"Get Order {orderId} failed");
            }
            if (order.State == OrderState.Completed)
            {
                throw new OrderStateConflictException($"The Order {orderId} already has state Completed");
            } 
            try 
            {
                order.State = OrderState.Completed;
                order.UpdatedBy = order.CreatedBy;
                order.Updated = DateTime.UtcNow;
                var res = await _orderRepository.UpdateAsync(order);
                // Publish Event for listeners
                _eventPublisher.Publish(new OrderStateChagedEvent(order.Id, order.State));
                // Publish Request for Handler
                _mediator.Publish(new OrderStateChangedNotification(order.Id, order.State)).ConfigureAwait(false);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SetOrderCompletedAsync)} exception");
                throw new ApplicationException($"Update Order {orderId} failed");
            }
        }

        public async Task<OrderDTO> GetByIdAsync(int id)
        {
            _logger.LogInformation($"{nameof(GetByIdAsync)} run");
            
            try
            {
                return await _orderRepository.FindByConditionQuery(t => t.Id == id).ProjectToType<OrderDTO>().SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetByIdAsync)} exception");
                throw new ApplicationException("Get Order failed");
            }
        }

        /// <summary>
        /// Order is exists by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Is exists</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<bool> OrderIsExistById(int id)
        {
            _logger.LogInformation($"{nameof(OrderIsExistById)} run");
            try
            {
                return await _orderRepository.AnyAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(OrderIsExistById)} exception");
                throw new ApplicationException("Any failed");
            }
        }
    }
}
