using Codeflix.Catalog.Domain.SeedWork;
using Codeflix.Catalog.Domain.Validation;

namespace Codeflix.Catalog.Domain.Entity;
public class Genre : AggregateRoot
{
    public Genre(string name, bool isActive = true )
    {
        Name = name;
        IsActive = isActive;
        CreatedAt = DateTime.Now;
        Validate();
    }

    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public void Activate()
    {
        IsActive = true;
        Validate();
    }

    public void Deactivate()
    {
        IsActive = false;
        Validate();
    }

    public void Update(string name)
    {
        Name = name;
        Validate();
    }
       
    private void Validate()
        => DomainValidation.NotNullOrEmpty(Name, nameof(Name));
    
}
