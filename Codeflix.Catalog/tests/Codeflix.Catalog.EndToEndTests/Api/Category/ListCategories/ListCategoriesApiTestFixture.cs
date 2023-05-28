﻿using Codeflix.Catalog.EndToEndTests.Api.Category.Common;

namespace Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories;

[CollectionDefinition(nameof(ListCategoriesApiTestFixture))]
public class ListCategoriesApiTestFixtureColection : ICollectionFixture<ListCategoriesApiTestFixture> { }
public class ListCategoriesApiTestFixture : CategoryBaseFixture
{
}
