using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Domain.Entities.Identiy;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Persistence.Contexts
{
    public class ETicaretAPIDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        //IoC containerda doldurulması için ctor yazılır
        public ETicaretAPIDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<Menuu> Menuus { get; set; }
        public DbSet<Endpoint> Endpoints { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Basket> Baskets { get; set; }

        public DbSet<CompletedOrder> CompletedOrders { get; set; }



        //Interceptor'lar belirli noktalarda metot çağrımları sırasında araya girerek bizlerin çakışan ilgilerimizi işletmemizi ve yönetmemizi sağlamakta. Böylece metotların çalışmasından önce veya sonra bir takım işlemleri gerçekleştirebilmekteyiz.bunun için savechanges methodu kullanarak veritabanına gitmeden yapmak istedğimiz değişiklerini yapabiliriz.

        //ChangeTracker Interceptor yöntemi ile tabloya ilgili kaydın kayıt tarihi, güncelleme tarihi, silinme tarihi gibi verilen otomatik kaydedilmesi konusunda fayda sağlıyor.change tracker entityler üzerinde yapılan değişiklerin yada yeni eklenen verinin yakalanmasını sağlayan yapıdır.
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var datas = ChangeTracker.Entries<BaseEntity>();
            foreach (var data in datas)
            {
                _ = data.State switch
                {
                    EntityState.Added => data.Entity.CreatedDate = DateTime.UtcNow,
                    EntityState.Modified => data.Entity.UpdatedDate = DateTime.UtcNow,
                    _ => DateTime.UtcNow
                };
            }

            return await base.SaveChangesAsync(cancellationToken);

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Order>().HasKey(o => o.Id);

            builder.Entity<Basket>()
                .HasOne(b => b.Order)
                .WithOne(o => o.Basket)
                .HasForeignKey<Order>(o =>o.Id);

            builder.Entity<Order>().HasIndex(o => o.Id).IsUnique();



            builder.Entity<Order>()
                .HasOne(c => c.CompletedOrder)
                .WithOne(o => o.Order)
                .HasForeignKey<CompletedOrder>(c => c.OrderId);

            base.OnModelCreating(builder);
        }
    }
}
