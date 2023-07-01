using Microsoft.EntityFrameworkCore;
using Codeflix.Catalog.Infra.Data.EF.Configurations;
using Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Infra.Data.EF.Models;

namespace Codeflix.Catalog.Infra.Data.EF;
public class CodeflixCatalogDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<GenresCategories> GenresCategories => Set<GenresCategories>();

    public CodeflixCatalogDbContext(DbContextOptions<CodeflixCatalogDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CategoryConfiguration());
        builder.ApplyConfiguration(new GenreConfiguration());

        builder.ApplyConfiguration(new GenresCategoriesConfiguration());
    }
   
}
