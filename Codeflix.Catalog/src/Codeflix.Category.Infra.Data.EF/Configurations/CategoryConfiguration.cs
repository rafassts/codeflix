using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Category.Infra.Data.EF.Configurations;
internal class CategoryConfiguration
    : IEntityTypeConfiguration<DomainEntity.Category>
{
    public void Configure(EntityTypeBuilder<DomainEntity.Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(255);
        builder.Property(x => x.Description).HasMaxLength(10_000);
    }
}
