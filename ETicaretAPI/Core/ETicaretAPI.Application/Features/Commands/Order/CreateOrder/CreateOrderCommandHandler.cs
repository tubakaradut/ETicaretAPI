﻿using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Order.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommandRequest, CreateOrderCommandResponse>
    {
        readonly IOrderService _orderService;
        readonly IBasketService _basketService;

        public CreateOrderCommandHandler(IOrderService orderService, IBasketService basketService )
        {
            _orderService = orderService;
            _basketService = basketService;
        }

        public async Task<CreateOrderCommandResponse> Handle(CreateOrderCommandRequest request, CancellationToken cancellationToken)
        {
            var ordercode = (new Random().NextDouble() * 10000).ToString();
            ordercode = ordercode.Substring(ordercode.IndexOf(".")+1,ordercode.Length- ordercode.IndexOf(".") - 1);

            await _orderService.CreateOrderAsync(new()
            {
                Address = request.Address,
                Description = request.Description,
                BasketId = _basketService.GetUserActiveBasket?.Id.ToString(),
                OrderCode = ordercode

            });  

            return new()
            {
                Message="Şipariş oluşturuldu......"
            };

        }
    }
}
