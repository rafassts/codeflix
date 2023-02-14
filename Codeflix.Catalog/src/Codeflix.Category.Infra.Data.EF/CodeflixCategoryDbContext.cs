using DomainEntity = Codeflix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Codeflix.Category.Infra.Data.EF.Configurations;

namespace Codeflix.Category.Infra.Data.EF;
public class CodeflixCategoryDbContext : DbContext
{
    public DbSet<DomainEntity.Category> Categories => Set<DomainEntity.Category>();
    public CodeflixCategoryDbContext(DbContextOptions<CodeflixCategoryDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CategoryConfiguration());
    }
}
