using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSO.Domain.Entities;

namespace SSO.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            builder.Property(u => u.UserName).HasMaxLength(50).IsRequired();
            builder.HasIndex(u => u.UserName).IsUnique();
            builder.Property(u => u.Mobile).HasMaxLength(20);
            builder.HasIndex(u => u.Mobile).IsUnique();
            builder.Property(u => u.Password).HasMaxLength(100).IsRequired();
            builder.HasIndex(u => new { u.UserName, u.Password }).IsUnique();
            builder.HasQueryFilter(u => u.IsDelete == false);
        }
    }
}
