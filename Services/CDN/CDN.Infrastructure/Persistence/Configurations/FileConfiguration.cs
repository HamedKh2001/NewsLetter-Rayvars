using CDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CDN.Infrastructure.Persistence.Configurations
{
    public class FileConfiguration : IEntityTypeConfiguration<NewsLetter>
    {
        public void Configure(EntityTypeBuilder<NewsLetter> builder)
        {
            builder.ToTable("File");
            builder.Property(f => f.FileName).IsRequired();
            builder.HasQueryFilter(f => f.IsDeleted == false);
        }
    }
}
