using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsLetterService.Domain.Entities;

namespace NewsLetterService.Infrastructure.Persistence.Configurations
{
    public class PersonnelConfiguration : IEntityTypeConfiguration<Personnel>
    {
        public void Configure(EntityTypeBuilder<Personnel> builder)
        {
            builder.ToTable("Personnel");
            builder.HasIndex(c => c.Id).IsUnique();
            builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        }
    }
}
