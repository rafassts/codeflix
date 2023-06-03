﻿using Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategories;

[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture> { }
public class ListCategoriesTestFixture : CategoryUseCasesBaseFixture
{
    public List<DomainEntity.Category> GetExampleCategoriesListWithNames(List<string> names)
       => names.Select(name =>
       {
           var category = GetExampleCategory();
           category.Update(name);
           return category;
       }).ToList();

    public List<DomainEntity.Category> CloneCategoriesListOrdered(
      List<DomainEntity.Category> categories,
      string orderBy,
      SearchOrder order)
    {
        var listClone = new List<DomainEntity.Category>(categories);

        //nova sintaxe do switch
        var orderedEnumerable = (orderBy, order) switch
        {
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdAt", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdAt", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id) //default
        };

        return orderedEnumerable.ToList();

    }
}
