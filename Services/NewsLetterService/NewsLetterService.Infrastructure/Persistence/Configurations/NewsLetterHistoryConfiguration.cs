using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsLetterService.Domain.Entities;

namespace NewsLetterService.Infrastructure.Persistence.Configurations
{
    public class NewsLetterHistoryConfiguration : IEntityTypeConfiguration<NewsLetterHistory>
    {
        public void Configure(EntityTypeBuilder<NewsLetterHistory> builder)
        {
            builder.ToTable("NewsLetterHistory");
            builder.HasKey(nlh => nlh.Id);
            builder.HasIndex(nlh => new { nlh.NewsLetterId, nlh.PersonnelId, nlh.Act }).IsUnique();
            builder.Property(nlh => nlh.NewsLetterId).IsRequired();
            builder.Property(nlh => nlh.PersonnelId).IsRequired();
            builder.Property(nlh => nlh.Act).IsRequired();
        }
    }
}
