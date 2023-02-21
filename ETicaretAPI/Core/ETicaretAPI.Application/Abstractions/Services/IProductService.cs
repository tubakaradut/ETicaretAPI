using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.DTOs.Product;
using ETicaretAPI.Application.ViewModels.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IProductService
    {
        Task CreateProductAsync(CreateProductDTO createProduct);
        Task<List<ListProductDTO>> GetAllProductsAsync();
        Task<SingleProductDTO> GetOrderByIdAsync(string id);
        Task UpdateProductAsync (UpdateProductDTO updateProductDTO);
        Task RemoveProductAsync(string productId);

    }
}
