namespace Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;
public class CreateCategoryApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryApiTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 3;

        for (int index = 0; index < totalInvalidCases; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    var input1 = fixture.getExampleInput();
                    input1.Name = fixture.GetInvalidNameTooShort();
                    invalidInputsList.Add(new object[] {
                        input1,
                        "Name should have at least 3 characters"
                    });
                    break;
                case 1:
                    var input2 = fixture.getExampleInput();
                    input2.Name = fixture.GetInvalidNameTooLong();
                    invalidInputsList.Add(new object[] {
                        input2,
                       "Name should have no more than 250 characters"
                    });
                    break;
                case 2:
                    var input3 = fixture.getExampleInput();
                    input3.Description = fixture.GetInvalidDescriptionTooLong();
                    invalidInputsList.Add(new object[] {
                        input3,
                        "Description should have no more than 10000 characters"
                    });
                    break;
                default:
                    break;
            }
        }

        return invalidInputsList;
    }
}
