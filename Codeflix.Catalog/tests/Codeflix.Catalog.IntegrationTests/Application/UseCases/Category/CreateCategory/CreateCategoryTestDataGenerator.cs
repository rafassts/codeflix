namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory;
public class CreateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputList = new List<object[]>();
        var totalInvalidCases = 4;

        //nome não pode ser menor que 3 e maior que 255 caracteres
        //descrição não pode ser null e tamanho máximo 10k

        for (int index = 0; index < times; index++)
        {
            //0,1,2,3,0,1,2,3...
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
                        fixture.GetInvalidInputDescriptionNull(),
                        "Description should not be null"
                    });
                    break;
                case 3:
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
