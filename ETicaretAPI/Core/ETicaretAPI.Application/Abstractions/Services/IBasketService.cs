using ETicaretAPI.Application.ViewModels.Basket;
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IBasketService
    {
        Task<List<BasketItem>> GetBasketItemsAsync();
        Task AddItemToBasketAsync(VMCreateBasketItem basketItem);
        Task UpdateQuantityAsync(VMUpdateBasketItem basketItem);
        Task RemoveBasketItemAsync(string basketItemId);
        Basket? GetUserActiveBasket { get; }

    }
}
