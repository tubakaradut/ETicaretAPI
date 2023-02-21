using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Product;
using ETicaretAPI.Application.Repositories.ProductRepo;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Product.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommandRequest, UpdateProductCommandResponse>

    {
        readonly IProductService _productService;
        readonly ILogger<UpdateProductCommandHandler> _logger;  //ürün güncellendiği durumlarda log almak için

        public UpdateProductCommandHandler(IProductService productService, ILogger<UpdateProductCommandHandler> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<UpdateProductCommandResponse> Handle(UpdateProductCommandRequest request, CancellationToken cancellationToken)
        {
            var dto = new UpdateProductDTO()
            {
                ProductId = request.Id,
                ProductName = request.Name,
                Price = request.Price,
                Stock = request.Stock
            };

            _logger.LogInformation("Ürün Güncellendi.");

            return new()
            {
                Price = dto.Price,
                ProductName = dto.ProductName,
                Stock = dto.Stock,
                ProductId = dto.ProductId
            };

        }
    }
}
