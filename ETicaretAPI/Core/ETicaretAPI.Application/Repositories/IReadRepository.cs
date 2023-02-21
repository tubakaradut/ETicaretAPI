using ETicaretAPI.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    public interface IReadRepository<T>:IRepositroy<T> where  T: BaseEntity
    {
        //ChangeTracker İlişkisi
        //Entity Framework'ün tüm entity ve propertyler üzerinde uygulanan tüm değişiklikleri izleyerek, bu değişimleri veri kaynağına doğru bir şekilde yansıtabilmek ve uygun DML (Data Manipulation Language) ifadeleri oluşturabilmektir.Tracing sayesinde entitylerde yapılan değişikler izlenir, veri takibi yapılmış olunur.Core da default olarak tracking işlemi yapılır. ancak veri manipüle işlemleri yapılmadığı durumlarda tracking false çekilip maliyet azaltması yapılabilinir.

        IQueryable<T> GetAll(bool tracking=true);
        IQueryable<T> GetWhere(Expression<Func<T,bool>> expression, bool tracking = true);
        Task<T> GetSingleAsync(Expression<Func<T,bool>> expression, bool tracking = true);
        Task<T> GetByIdAsync(string id, bool tracking = true);


    }
}
