using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.Order.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQueryRequest, GetOrderByIdQueryResponse>
    {
        readonly IOrderService _orderService;

        public GetOrderByIdQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<GetOrderByIdQueryResponse> Handle(GetOrderByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var data = await _orderService.GetOrderByIdAsync(request.OrderId);
            return new()
            {
                OrderId = data.OrderId,
                OrderCode = data.OrderCode,
                Description = data.Description,
                Address = data.Address,
                CreatedDate = data.CreatedDate,
                BasketItems = data.BasketItems,
                Completed = data.Completed
            };
        }
    }
}
