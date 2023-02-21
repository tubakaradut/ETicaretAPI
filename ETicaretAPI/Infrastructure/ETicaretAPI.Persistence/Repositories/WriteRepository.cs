using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
    {
        private readonly ETicaretAPIDbContext _contex;

        public WriteRepository(ETicaretAPIDbContext contex)
        {
            _contex = contex;
        }

        public DbSet<T> Table => _contex.Set<T>();

        public async Task<int> SaveAsync() => await _contex.SaveChangesAsync();

        public async Task<bool> AddAsync(T item)
        {
            EntityEntry<T> entityEntry = await Table.AddAsync(item);
           
            return entityEntry.State==EntityState.Added;
        }

        public async Task<bool> AddRangeAsync(List<T> item)
        {
           await Table.AddRangeAsync(item);
            return true;
        }

        public bool Remove(T item)
        {
            EntityEntry<T> entityEntry=Table.Remove(item);
            return entityEntry.State==EntityState.Deleted;

        }

        public bool Remove(List<T> item)
        {
            Table.RemoveRange(item);
            return true;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            T item=await Table.FirstOrDefaultAsync(x=>x.Id==Guid.Parse(id));
            return Remove(item);
        }

        

        public bool Update(T item)
        {
            EntityEntry<T> entityEntry=Table.Update(item);
            return entityEntry.State== EntityState.Modified;
        }
    }
}
