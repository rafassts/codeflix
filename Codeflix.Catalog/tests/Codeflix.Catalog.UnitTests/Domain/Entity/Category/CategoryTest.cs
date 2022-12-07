using Codeflix.Catalog.Domain.Exceptions;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Catalog.UnitTests.Domain.Entity.Category;
public class CategoryTest
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        var validData = new
        {
            Name = "categoria",
            Description = "description",
        };
        var dateTimeBefore = DateTime.Now;
        var category = new DomainEntity.Category(validData.Name, validData.Description);
        var dateTimeAfter = DateTime.Now;
        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(default(Guid), category.Id);
        Assert.NotEqual(default(DateTime), category.CreatedAt);
        Assert.True(category.CreatedAt > dateTimeBefore);
        Assert.True(category.CreatedAt < dateTimeAfter);
        Assert.True(category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validData = new
        {
            Name = "categoria",
            Description = "description",
        };
        var dateTimeBefore = DateTime.Now;
        var category = new DomainEntity.Category(validData.Name, validData.Description, isActive);
        var dateTimeAfter = DateTime.Now;
     
        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(default(Guid), category.Id);
        Assert.NotEqual(default(DateTime), category.CreatedAt);
        Assert.True(category.CreatedAt > dateTimeBefore);
        Assert.True(category.CreatedAt < dateTimeAfter);
        Assert.Equal(category.IsActive, isActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsNullOrEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsNullOrEmpty(string? name)
    {
        Action action = () => new DomainEntity.Category(name!, "description");
        var ex = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should not be empty or null", ex.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        Action action = () => new DomainEntity.Category("name", null!);
        var ex = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Description should not be null", ex.Message);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    public void InstantiateErrorWhenNameLessThan3Characters(string name)
    {
        Action action = () => new DomainEntity.Category(name, "description");
        var ex = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should have at least 3 characters", ex.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameGreaterThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameGreaterThan3Characters()
    {
        var name = String.Join(null, Enumerable.Range(0, 251).Select(_ => "a").ToArray());

        Action action = () => new DomainEntity.Category(name, "description");
        var ex = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should have no more than 250 characters", ex.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionGreaterThan10kCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionGreaterThan10kCharacters()
    {
        var description = String.Join(null, Enumerable.Range(0, 10001).Select(_ => "a").ToArray());

        Action action = () => new DomainEntity.Category("name", description);
        var ex = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Description should have no more than 10000 characters", ex.Message);
    }


}
