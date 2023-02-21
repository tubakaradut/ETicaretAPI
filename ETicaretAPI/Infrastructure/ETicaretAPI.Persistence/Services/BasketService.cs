using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Repositories.BasketItemRepo;
using ETicaretAPI.Application.Repositories.BasketRepo;
using ETicaretAPI.Application.Repositories.OrderRepo;
using ETicaretAPI.Application.ViewModels.Basket;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identiy;
using ETicaretAPI.Persistence.Repositories.OrderRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class BasketService : IBasketService
    {
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly IBasketItemReadRepository _basketItemReadRepository;
        readonly IBasketItemWriteRepository _basketItemWriteReadRepository;
        readonly IBasketWriteRepository _basketWriteRepository;
        readonly IBasketReadRepository _basketReadRepository;
        readonly UserManager<AppUser> _userManager;
        readonly IOrderReadRepository _orderReadRepository;


        public BasketService(IHttpContextAccessor httpContextAccessor, IBasketItemReadRepository basketItemReadRepository, IBasketItemWriteRepository basketItemWriteReadRepository, IBasketWriteRepository basketWriteRepository, IBasketReadRepository basketReadRepository, UserManager<AppUser> userManager, IOrderReadRepository orderReadRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _basketItemReadRepository = basketItemReadRepository;
            _basketItemWriteReadRepository = basketItemWriteReadRepository;
            _basketWriteRepository = basketWriteRepository;
            _basketReadRepository = basketReadRepository;
            _userManager = userManager;
            _orderReadRepository = orderReadRepository;
        }



        //sitemdeki kullanıcının kullanıcı bilgisi ve sepetini getirme
        private async Task<Basket?> ContextUserBasket()
        {
            var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                AppUser? user = await _userManager.Users.Include(u => u.Baskets).FirstOrDefaultAsync(x => x.UserName == userName);

                var _basket = from basket in user?.Baskets
                              join order in _orderReadRepository.Table
                              on
                              basket.Id equals order.Id
                              into BasketOrder
                              from order in BasketOrder.DefaultIfEmpty()
                              select new
                              {
                                  Basket = basket,
                                  Order = order
                              };

                Basket? targetBasket = null;
                if (_basket.Any(o => o.Order is null))
                {
                    targetBasket = _basket.FirstOrDefault(o => o.Order is null)?.Basket;  //kullanıcının mevcut olan sepeti yani sepet tamamlanmadığı için orderı oluşturulmamış
                }
                else
                {
                    targetBasket = new();
                    user?.Baskets.Add(targetBasket);
                }
                await _basketWriteRepository.SaveAsync();

                return targetBasket;

            }
            throw new Exception("Beklenmeyen bir hatayla karşılaşıldı...");
        }
        public Basket? GetUserActiveBasket
        {
            get
            {
                Basket? basket = ContextUserBasket().Result;
                return basket;
            }
        }

        //sepete itemları ekleme işlemi
        public async Task AddItemToBasketAsync(VMCreateBasketItem basketItem)
        {
            Basket? basket = await ContextUserBasket();

            if (basket != null)
            {
                BasketItem _basketItem = await _basketItemReadRepository.GetSingleAsync(bi => bi.BasketId == basket.Id && bi.ProductId == Guid.Parse(basketItem.ProductId));

                if (_basketItem != null)
                {
                    _basketItem.Quantity++;
                }
                else
                {
                    await _basketItemWriteReadRepository.AddAsync(new()
                    {
                        BasketId = basket.Id,
                        ProductId = Guid.Parse(basketItem.ProductId),
                        Quantity = basketItem.Quantity
                    });
                }
                await _basketItemWriteReadRepository.SaveAsync();
            }
        }

        //sepetlerin itemlarını getirme
        public async Task<List<BasketItem>> GetBasketItemsAsync()
        {
            Basket? basket = await ContextUserBasket();

            Basket? _basket = await _basketReadRepository.Table.Include(bi => bi.BasketItems).ThenInclude(p => p.Product).FirstOrDefaultAsync(b => b.Id == basket.Id);

            return _basket.BasketItems.ToList();

        }

        //sepetin itemlarını silme
        public async Task RemoveBasketItemAsync(string basketItemId)
        {
            BasketItem _basketItem = await _basketItemReadRepository.GetByIdAsync(basketItemId);
            if (_basketItem != null)
            {
                _basketItemWriteReadRepository.Remove(_basketItem);
                await _basketItemWriteReadRepository.SaveAsync();
            }
        }

        //sepetin itemların mikatarını güncelleme
        public async Task UpdateQuantityAsync(VMUpdateBasketItem basketItem)
        {
            BasketItem _basketItem = await _basketItemReadRepository.GetByIdAsync(basketItem.BasketItemId);
            if (_basketItem != null)
            {
                _basketItem.Quantity = basketItem.Quantity;
                await _basketItemWriteReadRepository.SaveAsync();
            }

        }


    }
}
