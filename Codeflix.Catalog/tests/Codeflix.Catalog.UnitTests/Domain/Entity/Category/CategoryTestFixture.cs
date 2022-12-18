using Codeflix.Catalog.UnitTests.Common;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Catalog.UnitTests.Domain.Entity.Category;
public class CategoryTestFixture : BaseFixture 
{
	public CategoryTestFixture() { }

	public string GetValidCategoryName()
	{
		var name = "";
		while (name.Length < 3)
			name = Faker.Commerce.Categories(1)[0];

		if (name.Length > 255)
			name = name[..255];

		return name;
	}

    public string GetValidCategoryDescription()
    {
        var description = Faker.Commerce.ProductDescription();

		if(description.Length > 10000)
			description = description[..10000]; 

        return description;
    }

    public DomainEntity.Category GetValidCategory() => new(
		GetValidCategoryName(), 
		GetValidCategoryDescription()
		);
}

[CollectionDefinition(nameof(CategoryTestFixture))]
public class CategoryTestFixtureCollection : ICollectionFixture<CategoryTestFixture> { }