using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Product;
using ETicaretAPI.Application.Repositories.ProductRepo;
using ETicaretAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class ProductService : IProductService
    {
        readonly IProductReadRepository _productReadRepository;
        readonly IProductWriteRepository _productWriteRepository;

        public ProductService(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
        }

        public async Task CreateProductAsync(CreateProductDTO createProduct)
        {
            await _productWriteRepository.AddAsync(new()
            {
                Id = Guid.NewGuid(),
                ProductName = createProduct.ProductName,
                Price = createProduct.Price,
                Stock = createProduct.Stock
            });
            await _productWriteRepository.SaveAsync();
        }

        public async Task<List<ListProductDTO>> GetAllProductsAsync()
        {
            var products = _productReadRepository.GetAll();

            return await products.Select(product => new ListProductDTO()
            {
                Id = product.Id.ToString(),
                ProductName = product.ProductName,
                Price = product.Price,
            }).ToListAsync();
        }

        public async Task<SingleProductDTO> GetOrderByIdAsync(string id)
        {
           var product=await _productReadRepository.GetByIdAsync(id);

            return new SingleProductDTO()
            {
                Price = product.Price,
                ProductId = product.Id.ToString(),
                ProductName= product.ProductName,
                Stock=product.Stock
            };
        }

        public async Task RemoveProductAsync(string productId)
        {
            Product product = await _productReadRepository.GetByIdAsync(productId);
            _productWriteRepository.Remove(product);
            await _productWriteRepository.SaveAsync();
        }

        public async Task UpdateProductAsync(UpdateProductDTO updateProductDTO)
        {
            var product = await _productReadRepository.GetSingleAsync(x=>x.Id==Guid.Parse(updateProductDTO.ProductId));

            product.Price = updateProductDTO.Price;
            product.Stock= updateProductDTO.Stock;
            product.ProductName= updateProductDTO.ProductName;
             
             
            _productWriteRepository.Update(product); 
            await _productWriteRepository.SaveAsync();

        }
    }
}
