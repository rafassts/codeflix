using Microsoft.EntityFrameworkCore;
using Codeflix.Catalog.Infra.Data.EF.Configurations;
using Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Catalog.Infra.Data.EF;
public class CodeflixCategoryDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public CodeflixCategoryDbContext(DbContextOptions<CodeflixCategoryDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CategoryConfiguration());
    }
   
}
