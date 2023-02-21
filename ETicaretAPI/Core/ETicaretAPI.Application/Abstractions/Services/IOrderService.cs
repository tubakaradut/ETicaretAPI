using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface  IOrderService
    {
        Task CreateOrderAsync(CreateOrderDTO createOrder);
        Task<ListOrderDTO> GetAllOrdersAsync();
        Task<SingleOrderDTO> GetOrderByIdAsync(string id);
        Task<(bool, CompletedOrderDTO)> CompleteOrderAsync(string id);
    }
}
