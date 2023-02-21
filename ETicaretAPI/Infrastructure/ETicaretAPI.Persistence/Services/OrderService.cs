using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories.CompletedOrderRepo;
using ETicaretAPI.Application.Repositories.OrderRepo;
using ETicaretAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class OrderService : IOrderService
    {
        readonly IOrderReadRepository _orderReadRepository;
        readonly IOrderWriteRepository _orderWriteRepository;
        readonly ICompletedOrderReadRepository _completedOrderReadRepository;
        readonly ICompletedOrderWriteRepository _completedOrderWriteRepository;


        public OrderService(IOrderReadRepository orderReadRepository, IOrderWriteRepository orderWriteRepository, ICompletedOrderReadRepository completedOrderReadRepository, ICompletedOrderWriteRepository completedOrderWriteRepository)
        {
            _orderReadRepository = orderReadRepository;
            _orderWriteRepository = orderWriteRepository;
            _completedOrderReadRepository = completedOrderReadRepository;
            _completedOrderWriteRepository = completedOrderWriteRepository;
        }


        public async Task CreateOrderAsync(CreateOrderDTO createOrder)
        {
            await _orderWriteRepository.AddAsync(new()
            {
                Address = createOrder.Address,
                Description = createOrder.Description,
                Id = Guid.Parse(createOrder.BasketId)
            });
            await _completedOrderWriteRepository.SaveAsync();

        }

        public async Task<ListOrderDTO> GetAllOrdersAsync()
        {
            var datas = _orderReadRepository.Table
                .Include(o => o.Basket)
                .ThenInclude(u => u.AppUser)
                .Include(o => o.Basket)
                .ThenInclude(bi => bi.BasketItems)
                .ThenInclude(p => p.Product);


            var datas2 = from order in datas
                      join completedOrder in _completedOrderReadRepository.Table
                      on order.Id equals completedOrder.OrderId into co
                      from CompletedOrder in co.DefaultIfEmpty()
                      select new
                      {
                          order.Id,
                          order.CreatedDate,
                          order.OrderCode,
                          order.Basket,
                          Completed = CompletedOrder != null ? true : false
                      };

            return new()
            {
                TotalOrderCount = await datas.CountAsync(),
                Orders = await datas2.Select(o => new
                {
                    Id = o.Id,
                    CreatedDate = o.CreatedDate,
                    OrderCode = o.OrderCode,
                    TotalPrice = o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                    UserName = o.Basket.AppUser.UserName,
                    o.Completed
                }).ToListAsync()
            };

        }

        public async Task<SingleOrderDTO> GetOrderByIdAsync(string id)
        {
            var data = _orderReadRepository.Table
                .Include(b => b.Basket)
                .ThenInclude(bi => bi.BasketItems)
                .ThenInclude(p => p.Product);


            var data2 = await (from order in data
                               join completedOrder in _completedOrderReadRepository.Table
                                    on order.Id equals completedOrder.OrderId into co
                               from CompletedOrder in co.DefaultIfEmpty()
                               select new
                               {
                                   Id = order.Id,
                                   CreatedDate = order.CreatedDate,
                                   OrderCode = order.OrderCode,
                                   Basket = order.Basket,
                                   Completed = CompletedOrder != null ? true : false,
                                   Address = order.Address,
                                   Description = order.Description
                               }).FirstOrDefaultAsync(o => o.Id == Guid.Parse(id));

            return new()
            {
                OrderId = data2.Id.ToString(),
                BasketItems = data2.Basket.BasketItems.Select(bi => new
                {
                    bi.Product.ProductName,
                    bi.Product.Price,
                    bi.Quantity
                }),
                Address = data2.Address,
                CreatedDate = data2.CreatedDate,
                Description = data2.Description,
                OrderCode = data2.OrderCode,
                Completed = data2.Completed
            };

        }

        public async Task<(bool, CompletedOrderDTO)> CompleteOrderAsync(string id)
        {
            Order? order = await _orderReadRepository.Table
                .Include(o => o.Basket)
                .ThenInclude(b => b.AppUser)
                .FirstOrDefaultAsync(o => o.Id == Guid.Parse(id));

            if (order != null)
            {
                await _completedOrderWriteRepository.AddAsync(new() { OrderId = Guid.Parse(id) });

                return (await _completedOrderWriteRepository.SaveAsync() > 0, new()
                {
                    OrderCode = order.OrderCode,
                    OrderDate = order.CreatedDate,
                    Username = order.Basket.AppUser.UserName, 
                    EMail = order.Basket.AppUser.Email
                });
            }
            return (false, null);
        }
    }
}
