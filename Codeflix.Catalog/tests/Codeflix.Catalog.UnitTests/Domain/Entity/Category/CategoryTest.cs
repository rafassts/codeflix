using Codeflix.Catalog.Domain.Exceptions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameGreaterThan250Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameGreaterThan250Characters()
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

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var validData = new
        {
            Name = "categoria",
            Description = "description",
        };
        var dateTimeBefore = DateTime.Now;
        var category = new DomainEntity.Category(validData.Name, validData.Description, false);
        var dateTimeAfter = DateTime.Now;

        category.Activate();

        Assert.True(category.IsActive);
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var validData = new
        {
            Name = "categoria",
            Description = "description",
        };
        var dateTimeBefore = DateTime.Now;
        var category = new DomainEntity.Category(validData.Name, validData.Description, true);
        var dateTimeAfter = DateTime.Now;

        category.Deactivate();

        Assert.False(category.IsActive);
    }


    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        var category = new DomainEntity.Category("name", "description");

        var values = new 
        {
            Name = "New Name",
            Description = "New Description"
        }; 

        category.Update(values.Name, values.Description);

        Assert.Equal(values.Name, category.Name);
        Assert.Equal(values.Description, category.Description); 

    }


    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        var description = "description";
        var category = new DomainEntity.Category("name", description);

        var values = new
        {
            Name = "New Name"
        };

        category.Update(values.Name);

        Assert.Equal(values.Name, category.Name);
        Assert.Equal(description, category.Description);

    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsNullOrEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateErrorWhenNameIsNullOrEmpty(string? name)
    {
        var category = new DomainEntity.Category("name","description");

        Action action = () => category.Update(name!);
        var ex = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should not be empty or null", ex.Message);
    }


    [Theory(DisplayName = nameof(UpdateErrorWhenNameLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    public void UpdateErrorWhenNameLessThan3Characters(string name)
    {
        var category = new DomainEntity.Category("name", "description");
        Action action = () => category.Update(name!);
        var ex = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should have at least 3 characters", ex.Message);
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameGreaterThan250Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameGreaterThan250Characters()
    {
        var category = new DomainEntity.Category("name", "description");
        var name = String.Join(null, Enumerable.Range(0, 251).Select(_ => "a").ToArray());
      
        Action action = () => category.Update(name!);
        var ex = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should have no more than 250 characters", ex.Message);
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionGreaterThan10kCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionGreaterThan10kCharacters()
    {
        var category = new DomainEntity.Category("name", "description");
        var description = String.Join(null, Enumerable.Range(0, 10001).Select(_ => "a").ToArray());

        Action action = () => category.Update("name", description);
        var ex = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Description should have no more than 10000 characters", ex.Message);
    }
}
