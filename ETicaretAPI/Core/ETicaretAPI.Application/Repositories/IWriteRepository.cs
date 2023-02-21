using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    public  interface  IWriteRepository<T> : IRepositroy<T> where T : BaseEntity
    {
        Task<bool> AddAsync(T item);
        Task<bool> AddRangeAsync(List<T> items);
        bool Remove(T item);
        bool Remove(List<T> item);
        Task<bool> RemoveAsync(string id);
        bool Update(T item);

        Task<int> SaveAsync();
    }
}
