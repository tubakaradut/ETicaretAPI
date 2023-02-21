using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ETicaretAPI.Persistence.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity

        //Core da default olarak tracking işlemi yapılır. ancak verilerde manipüle işlemleri yapılmadığı durumlarda tracking false çekilip maliyet azaltması yapılabilinir.
    {
        private readonly ETicaretAPIDbContext _contex;

        public ReadRepository(ETicaretAPIDbContext contex)
        {
            _contex = contex;
        }

        public DbSet<T> Table => _contex.Set<T>();

        public IQueryable<T> GetAll(bool tracking=true) 
        {
            var query = Table.AsQueryable();
            if (!tracking) // eğer tracking işlemi yapılmak istenmiyorsa  
                query = Table.AsNoTracking();
            return query;
        }
        public IQueryable<T> GetWhere(Expression<Func<T, bool>> expression, bool tracking = true)
        {
            var query = Table.Where(expression);
            if (!tracking)
                query = Table.AsNoTracking();
            return query;
        }
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> expression, bool tracking = true)
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = Table.AsNoTracking();

            return await Table.FirstOrDefaultAsync(expression);
        }


        public async Task<T> GetByIdAsync(string id, bool tracking = true)
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = Table.AsNoTracking();
            return await query.FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

        }
    }
}
