namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory;
public class UpdateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetGategoriesToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestFixture();
        for (int i = 0; i < times; i++)
        {
            //vai ser categoria "gravada no banco"
            var exampleCategory = fixture.GetExampleCategory();

            //vai ser os dados para atualizar a categoria
            var exampleInput = fixture.GetValidInput(exampleCategory.Id);

            yield return new object[]
            {
                exampleCategory,
                exampleInput
            };
        }
    }

    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var fixture = new UpdateCategoryTestFixture();
        var invalidInputList = new List<object[]>();
        var totalInvalidCases = 3;

        //nome não pode ser menor que 3 e maior que 255 caracteres
        //descrição não pode ser null e tamanho máximo 10k

        for (int index = 0; index < times; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputList.Add(new object[]
                    {
                        fixture.GetInvalidInputShortName(),
                        "Name should have at least 3 characters"
                    });
                    break;
                case 1:
                    invalidInputList.Add(new object[]
                    {
                        fixture.GetInvalidInputTooLongName(),
                        "Name should have no more than 250 characters"
                    });
                    break;
                case 2:
                    invalidInputList.Add(new object[]
                    {
                        fixture.GetInvalidInputDescriptionTooLong(),
                        "Description should have no more than 10000 characters"
                    });
                    break;
                default:
                    break;
            }
        }

        return invalidInputList;
    }
}
