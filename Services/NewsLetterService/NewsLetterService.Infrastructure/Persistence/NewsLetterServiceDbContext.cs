using Microsoft.EntityFrameworkCore;
using NewsLetterService.Domain.Entities;
using System.Reflection;

namespace NewsLetterService.Infrastructure.Persistence
{
    public class NewsLetterServiceDbContext : DbContext
    {
        public NewsLetterServiceDbContext()
        {
        }

        public NewsLetterServiceDbContext(DbContextOptions<NewsLetterServiceDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Personnel> Personnels { get; set; }
        public virtual DbSet<NewsLetterHistory> NewsLetterHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        public virtual void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }
    }
}
