using CDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CDN.Infrastructure.Persistence
{
    public class CDNDbContext : DbContext
    {
        public CDNDbContext()
        {
        }

        public CDNDbContext(DbContextOptions<CDNDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<NewsLetter> NewsLetters { get; set; }

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
