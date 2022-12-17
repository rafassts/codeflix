using Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
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

        category.Should().NotBeNull();
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBe(default(DateTime));
        (category.CreatedAt > dateTimeBefore).Should().BeTrue();
        (category.CreatedAt < dateTimeAfter).Should().BeTrue();  
        //Assert.True(category.CreatedAt > dateTimeBefore);
        //Assert.True(category.CreatedAt < dateTimeAfter);

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

        category.Should().NotBeNull();
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBe(default(DateTime));
        (category.CreatedAt > dateTimeBefore).Should().BeTrue();
        (category.CreatedAt < dateTimeAfter).Should().BeTrue();
        category.IsActive.Should().Be(isActive);

    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsNullOrEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsNullOrEmpty(string? name)
    {
        Action action = () => new DomainEntity.Category(name!, "description");

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        Action action = () => new DomainEntity.Category("name", null!);

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
        Action action = () => new DomainEntity.Category(name, "description");
        action.Should()
        .Throw<EntityValidationException>()
        .WithMessage("Name should have at least 3 characters");

    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameGreaterThan250Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameGreaterThan250Characters()
    {
        var name = String.Join(null, Enumerable.Range(0, 251).Select(_ => "a").ToArray());

        Action action = () => new DomainEntity.Category(name, "description");

        action.Should()
       .Throw<EntityValidationException>()
       .WithMessage("Name should have no more than 250 characters");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionGreaterThan10kCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionGreaterThan10kCharacters()
    {
        var description = String.Join(null, Enumerable.Range(0, 10001).Select(_ => "a").ToArray());

        Action action = () => new DomainEntity.Category("name", description);
        
        action.Should()
         .Throw<EntityValidationException>()
         .WithMessage("Description should have no more than 10000 characters");
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
        var category = new DomainEntity.Category(validData.Name, validData.Description, false);

        category.Activate();

        category.IsActive.Should().BeTrue(); 
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

        var category = new DomainEntity.Category(validData.Name, validData.Description, true);

        category.Deactivate();

        category.IsActive.Should().NotBe(true);
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

        category.Name.Should().Be(values.Name);
        category.Description.Should().Be(values.Description);
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

        category.Name.Should().Be(values.Name);
        category.Description.Should().Be(description);
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

        action.Should()
          .Throw<EntityValidationException>()
          .WithMessage("Name should not be empty or null");

    }


    [Theory(DisplayName = nameof(UpdateErrorWhenNameLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    public void UpdateErrorWhenNameLessThan3Characters(string name)
    {
        var category = new DomainEntity.Category("name", "description");
        Action action = () => category.Update(name!);

        action.Should()
           .Throw<EntityValidationException>()
           .WithMessage("Name should have at least 3 characters");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameGreaterThan250Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameGreaterThan250Characters()
    {
        var category = new DomainEntity.Category("name", "description");
        var name = String.Join(null, Enumerable.Range(0, 251).Select(_ => "a").ToArray());
      
        Action action = () => category.Update(name!);

        action.Should()
         .Throw<EntityValidationException>()
         .WithMessage("Name should have no more than 250 characters");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionGreaterThan10kCharacters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionGreaterThan10kCharacters()
    {
        var category = new DomainEntity.Category("name", "description");
        var description = String.Join(null, Enumerable.Range(0, 10001).Select(_ => "a").ToArray());

        Action action = () => category.Update("name", description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should have no more than 10000 characters");

    }
}
