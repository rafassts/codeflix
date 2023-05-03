using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using Codeflix.Catalog.Domain.Exceptions;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using DomainEntity = Codeflix.Catalog.Domain.Entity;
using UseCase = Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
        => _fixture=fixture;

    [Theory(DisplayName = nameof(UpdateCategory))]
    [Trait("Integration/Application", "Update Category - Use cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetGategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async void UpdateCategory(DomainEntity.Category exampleCategory, UpdateCategoryInput input)
    {
        var dbContext = _fixture.CreateDbContext();
        
        //insere alguns itens no banco e depois o examplecategory, que é  o que vai ser atualizado.
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        dbContext.SaveChanges();
        trackingInfo.State = EntityState.Detached; //desatacha manualmente o tracking depois de salvar
        //acontece pois estamos forçando primeiro um insert e logo depois um update, o que não é comum

        var repo = new CategoryRepository(dbContext);
        var uow = new UnitOfWork(dbContext);
        var useCase = new UseCase.UpdateCategory(repo, uow);

        var output = await useCase.Handle(input, CancellationToken.None);

        //lê do banco para verificar se deu certo, novo contexto para não usar o tracking  
        var dbContext2 = _fixture.CreateDbContext(true);
        
        var dbCategory = await dbContext2
            .Categories
            .FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool) input.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

    }

    [Theory(DisplayName = nameof(UpdateCategoryWithoutIsActive))]
    [Trait("Integration/Application", "Update Category - Use cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetGategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async void UpdateCategoryWithoutIsActive(DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
        var dbContext = _fixture.CreateDbContext();

        //insere alguns itens no banco e depois o examplecategory, que é  o que vai ser atualizado.
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        dbContext.SaveChanges();
        trackingInfo.State = EntityState.Detached; //desatacha manualmente o tracking depois de salvar
        //acontece pois estamos forçando primeiro um insert e logo depois um update, o que não é comum

        var repo = new CategoryRepository(dbContext);
        var uow = new UnitOfWork(dbContext);
        var useCase = new UseCase.UpdateCategory(repo, uow);

        var output = await useCase.Handle(input, CancellationToken.None);

        //lê do banco para verificar se deu certo, novo contexto para não usar o tracking  
        var dbContext2 = _fixture.CreateDbContext(true);

        var dbCategory = await dbContext2
            .Categories
            .FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

    }

    [Theory(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("Integration/Application", "Update Category - Use cases")]
    [MemberData(
       nameof(UpdateCategoryTestDataGenerator.GetGategoriesToUpdate),
       parameters: 5,
       MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async void UpdateCategoryOnlyName(DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name);
        var dbContext = _fixture.CreateDbContext();

        //insere alguns itens no banco e depois o examplecategory, que é  o que vai ser atualizado.
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        dbContext.SaveChanges();
        trackingInfo.State = EntityState.Detached; //desatacha manualmente o tracking depois de salvar
        //acontece pois estamos forçando primeiro um insert e logo depois um update, o que não é comum

        var repo = new CategoryRepository(dbContext);
        var uow = new UnitOfWork(dbContext);
        var useCase = new UseCase.UpdateCategory(repo, uow);

        var output = await useCase.Handle(input, CancellationToken.None);

        //lê do banco para verificar se deu certo, novo contexto para não usar o tracking  
        var dbContext2 = _fixture.CreateDbContext(true);

        var dbCategory = await dbContext2
            .Categories
            .FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

    }

    [Fact(DisplayName = nameof(UpdateThrowsWhenCategoryNotFound))]
    [Trait("Integration/Application", "Update Category - Use cases")]
    public async void UpdateThrowsWhenCategoryNotFound()
    {
        var input = _fixture.GetValidInput(); //guid rand
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        dbContext.SaveChanges();

        var repo = new CategoryRepository(dbContext);
        var uow = new UnitOfWork(dbContext);
        var useCase = new UseCase.UpdateCategory(repo, uow);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found");

    }

    [Theory(DisplayName = nameof(UpdateThrowsWhenCantInstantiateCategory))]
    [Trait("Integration/Application", "Update Category - Use cases")]
    [MemberData(
    nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
    parameters: 6,
    MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async void UpdateThrowsWhenCantInstantiateCategory(UpdateCategoryInput input, string expectedExceptionMessage)
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategories = _fixture.GetExampleCategoriesList();
        //insere alguns itens no banco e depois o examplecategory, que é  o que vai ser atualizado.
        await dbContext.AddRangeAsync(exampleCategories);
        dbContext.SaveChanges();

        var repo = new CategoryRepository(dbContext);
        var uow = new UnitOfWork(dbContext);
        var useCase = new UseCase.UpdateCategory(repo, uow);

        //pega o id de algum da lista para tentar atualizar com dados incorretos
        input.Id = exampleCategories[0].Id;
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(expectedExceptionMessage);
    }

}
