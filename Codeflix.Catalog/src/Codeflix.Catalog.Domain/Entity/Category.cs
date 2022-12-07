using Codeflix.Catalog.Domain.Exceptions;

namespace Codeflix.Catalog.Domain.Entity;
public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Boolean IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category(string name, string description, bool isActive = true)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = DateTime.Now;
        Validate();
    }

    public void Validate()
    {
        if (String.IsNullOrWhiteSpace(Name))
            throw new EntityValidationException($"{nameof(Name)} should not be empty or null");

        if (Description == null)
            throw new EntityValidationException($"{nameof(Description)} should not be null");

        if (Name.Length < 3)
            throw new EntityValidationException($"{nameof(Name)} should have at least 3 characters");

        if (Name.Length > 250)
            throw new EntityValidationException($"{nameof(Name)} should have no more than 250 characters");

        if (Description.Length > 10000)
            throw new EntityValidationException($"{nameof(Description)} should have no more than 10000 characters");
    }   
}
