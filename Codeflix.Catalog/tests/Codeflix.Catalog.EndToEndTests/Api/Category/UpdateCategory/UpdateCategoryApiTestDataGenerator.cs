
using Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;

namespace Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;
public class UpdateCategoryApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new UpdateCategoryApiTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 3;

        for (int index = 0; index < totalInvalidCases; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    var input1 = fixture.GetExampleInput(null);
                    input1.Name = fixture.GetInvalidNameTooShort();
                    invalidInputsList.Add(new object[] {
                        input1,
                        "Name should have at least 3 characters"
                    });
                    break;
                case 1:
                    var input2 = fixture.GetExampleInput(null);
                    input2.Name = fixture.GetInvalidNameTooLong();
                    invalidInputsList.Add(new object[] {
                        input2,
                       "Name should have no more than 250 characters"
                    });
                    break;
                case 2:
                    var input3 = fixture.GetExampleInput(null);
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
