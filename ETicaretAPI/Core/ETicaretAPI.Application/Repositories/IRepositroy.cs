using ETicaretAPI.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Application.Repositories
{
    public interface IRepositroy<T> where T : BaseEntity
    {   
        // BaseEntity yazarak Marker Pattern kullanmış olduk. derleyicinin nesneler hakkında ek bilgilere sahip olabilmesini ve böylece ilgili nesnenin kullanılacağı noktaları derleme sürecinde kurallar eşliğinde belirleyerek, kodu runtime’da olası hatalardan arındırmamızı sağlayan bir pattern’dır.
        DbSet<T> Table { get; } //veritabında t tablaya eş değer olduğu için table yazıldı
    }
}
