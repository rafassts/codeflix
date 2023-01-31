using Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using FluentValidation;

namespace Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryInputValidatorTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryInputValidatorTest(UpdateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(InvalidWhenEmptyGuidId))]
    [Trait("Application", "UpdateCategoryInputValidation - Usecases")]
    public void InvalidWhenEmptyGuidId()
    {
        var input = _fixture.GetValidInput(Guid.Empty);

        var validator = new UpdateCategoryInputValidator();

        var validationResult = validator.Validate(input);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
    }

    [Fact(DisplayName = nameof(ValidationOk))]
    [Trait("Application", "UpdateCategoryInputValidation - Usecases")]
    public void ValidationOk()
    {
        var input = _fixture.GetValidInput();

        var validator = new UpdateCategoryInputValidator();

        var validationResult = validator.Validate(input);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().HaveCount(0);
    }
}
