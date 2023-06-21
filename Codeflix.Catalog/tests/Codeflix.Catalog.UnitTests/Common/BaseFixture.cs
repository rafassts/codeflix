
using Bogus;

namespace Codeflix.Catalog.UnitTests.Common;
public abstract class BaseFixture
{
    public Faker Faker { get; set; }

    protected BaseFixture() => Faker = new Faker("pt_BR");

    public bool GetRandomIsActive() => new Random().NextDouble() < 0.5;
}
