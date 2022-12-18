using Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Catalog.UnitTests.Domain.Entity.Category;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{
    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture)
    {
        _categoryTestFixture=categoryTestFixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var dateTimeBefore = DateTime.Now;
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBe(default(DateTime));
        (category.CreatedAt > dateTimeBefore).Should().BeTrue();
        (category.CreatedAt <= dateTimeAfter).Should().BeTrue();  
        //Assert.True(category.CreatedAt > dateTimeBefore);
        //Assert.True(category.CreatedAt < dateTimeAfter);

    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var dateTimeBefore = DateTime.Now;
        
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);
        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBe(default(DateTime));
        (category.CreatedAt > dateTimeBefore).Should().BeTrue();
        (category.CreatedAt <= dateTimeAfter).Should().BeTrue();
        category.IsActive.Should().Be(isActive);

    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsNullOrEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsNullOrEmpty(string? name)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => new DomainEntity.Category(name!, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => new DomainEntity.Category(validCategory.Name, null!);

        action.Should()
          .Throw<EntityValidationException>()
          .WithMessage("Description should not be null");

    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    public void InstantiateErrorWhenNameLessThan3Characters(string name)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => new DomainEntity.Category(name, validCategory.Description);
        action.Should()
        .Throw<EntityValidationException>()
        .WithMessage("Name should have at least 3 characters");

    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameGreaterThan250Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameGreaterThan250Characters()
    {
        var name = String.Join(null, Enumerable.Range(0, 251).Select(_ => "a").ToArray());
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => new DomainEntity.Category(name, validCategory.Description);

        action.Should()
       .Throw<EntityValidationException>()
       .WithMessage("Name should have no more than 250 characters");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionGreaterThan10kCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionGreaterThan10kCharacters()
    {
        var description = String.Join(null, Enumerable.Range(0, 10001).Select(_ => "a").ToArray());
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => new DomainEntity.Category(validCategory.Name, description);
        
        action.Should()
         .Throw<EntityValidationException>()
         .WithMessage("Description should have no more than 10000 characters");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        validCategory.Activate();

        validCategory.IsActive.Should().BeTrue(); 
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, true);

        category.Deactivate();

        category.IsActive.Should().NotBe(true);
    }


    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var categoryWithNewValues = _categoryTestFixture.GetValidCategory(); 

        validCategory.Update(categoryWithNewValues.Name, categoryWithNewValues.Description);

        validCategory.Name.Should().Be(categoryWithNewValues.Name);
        validCategory.Description.Should().Be(categoryWithNewValues.Description);
    }


    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var description = validCategory.Description;

        var newName = _categoryTestFixture.GetValidCategoryName();

        validCategory.Update(newName);

        validCategory.Name.Should().Be(newName);
        validCategory.Description.Should().Be(description);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsNullOrEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateErrorWhenNameIsNullOrEmpty(string? name)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action = () => validCategory.Update(name!);

        action.Should()
          .Throw<EntityValidationException>()
          .WithMessage("Name should not be empty or null");

    }


    [Theory(DisplayName = nameof(UpdateErrorWhenNameLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [MemberData(nameof(GetNamesWithLessThan3Characters),parameters:6)]
    public void UpdateErrorWhenNameLessThan3Characters(string name)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => validCategory.Update(name!);

        action.Should()
           .Throw<EntityValidationException>()
           .WithMessage("Name should have at least 3 characters");
    }

    public static IEnumerable<object[]> GetNamesWithLessThan3Characters(int times)
    {
        var fixture = new CategoryTestFixture();

        for (int i = 0; i < times; i++)
        {
            var isOdd = i % 2 == 1;
            yield return new object[] { fixture.GetValidCategoryName()[..(isOdd ? 1: 2)] };
        }
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameGreaterThan250Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameGreaterThan250Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        //var name = String.Join(null, Enumerable.Range(0, 251).Select(_ => "a").ToArray());
        var name = _categoryTestFixture.Faker.Lorem.Letter(256);
      
        Action action = () => validCategory.Update(name!);

        action.Should()
         .Throw<EntityValidationException>()
         .WithMessage("Name should have no more than 250 characters");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionGreaterThan10kCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionGreaterThan10kCharacters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var description = _categoryTestFixture.Faker.Commerce.ProductDescription();

        while(description.Length <= 10000)
            description = $"{description} { _categoryTestFixture.Faker.Commerce.ProductDescription() }";

        Action action = () => validCategory.Update("name", description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should have no more than 10000 characters");

    }
}
