using Bogus;
using Codeflix.Catalog.Domain.Exceptions;
using Codeflix.Catalog.Domain.Validation;
using Codeflix.Catalog.UnitTests.Domain.Entity.Category;
using FluentAssertions;

namespace Codeflix.Catalog.UnitTests.Domain.Validation;
public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();

    [Fact(DisplayName = nameof(NotNullOK))]
    [Trait("Domain", "Domain Validation - Validation")]
    public void NotNullOK()
    {
        var value = Faker.Commerce.ProductName();
        Action action =
            () => DomainValidation.NotNull(value, "Value");

        action.Should().NotThrow();
    }

    [Fact(DisplayName = nameof(NotNullThrow))]
    [Trait("Domain", "Domain Validation - Validation")]
    public void NotNullThrow()
    {
        string value = null;
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        Action action =
            () => DomainValidation.NotNull(value, fieldName);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be null");
    }

    [Theory(DisplayName = nameof(NotNulllOrEmptyThrow))]
    [Trait("Domain", "Domain Validation - Validation")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void NotNulllOrEmptyThrow(string? target)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.NotNullOrEmpty(target, fieldName);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be null or empty");
    }

    [Fact(DisplayName = nameof(NotNulllOrEmptyOk))]
    [Trait("Domain", "Domain Validation - Validation")]
    public void NotNulllOrEmptyOk()
    {

        var value = Faker.Commerce.ProductName();
        Action action =
            () => DomainValidation.NotNullOrEmpty(value, "Value");

        action.Should().NotThrow();
    }

    [Theory(DisplayName = nameof(MinLengthThrow))]
    [Trait("Domain", "Domain Validation - Validation")]
    [MemberData(nameof(GetValuesSmallerThanMin), parameters: 5)]
    public void MinLengthThrow(string target, int minLength)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
          () => DomainValidation.MinLength(target, minLength, fieldName);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should have at least {minLength} characters");
    }

    public static IEnumerable<object[]> GetValuesSmallerThanMin(int times)
    {
        yield return new object[] { "123456", 10  };
        var faker = new Faker();
        for (int i = 0; i < times; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length + (new Random()).Next(1,20);
            yield return new object[] { example, minLength };
        }
    }

    [Theory(DisplayName = nameof(MinLengthOk))]
    [Trait("Domain", "Domain Validation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMin), parameters: 5)]
    public void MinLengthOk(string target, int minLength)
    {
        Action action =
          () => DomainValidation.MinLength(target, minLength, "fieldName");

        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMin(int times)
    {
        yield return new object[] { "123456", 6 };
        var faker = new Faker();
        for (int i = 0; i < times; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length - (new Random()).Next(1, 5);
            yield return new object[] { example, minLength };
        }
    }

    [Theory(DisplayName = nameof(MaxLengthThrow))]
    [Trait("Domain", "Domain Validation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMax), parameters: 5)]
    public void MaxLengthThrow(string target, int maxLengh)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
          () => DomainValidation.MaxLength(target, maxLengh, fieldName);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should have no more than {maxLengh} characters");
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMax(int times)
    {
        yield return new object[] { "123456", 3 };
        var faker = new Faker();
        for (int i = 0; i < times; i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLengh = example.Length - (new Random()).Next(1, 20);
            yield return new object[] { example, maxLengh };
        }
    }

    [Theory(DisplayName = nameof(MaxLengthOk))]
    [Trait("Domain", "Domain Validation - Validation")]
    [MemberData(nameof(GetValuesSmallerThanMax), parameters: 5)]
    public void MaxLengthOk(string target, int maxLengh)
    {
        Action action =
          () => DomainValidation.MaxLength(target, maxLengh, "fieldName");

        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValuesSmallerThanMax(int times)
    {
        yield return new object[] { "123456", 13 };
        var faker = new Faker();
        for (int i = 0; i < times; i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLengh = example.Length + (new Random()).Next(1, 20);
            yield return new object[] { example, maxLengh };
        }
    }




}
