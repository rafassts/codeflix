using Codeflix.Catalog.Domain.Exceptions;
using Codeflix.Catalog.Domain.SeedWork;
using Codeflix.Catalog.Domain.Validation;

namespace Codeflix.Catalog.Domain.Entity;
public class Category : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category(string name, string description, bool isActive = true) 
    {
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = DateTime.Now;
        Validate();
    }

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
    
    public void Update(string name, string? description = null)
    {
        Name= name;
        Description = description ?? Description;
        Validate();
    }

    public void Validate()
    {
        DomainValidation.NotNullOrEmpty(Name, nameof(Name));  
        DomainValidation.NotNull(Description, nameof(Description));   
        DomainValidation.MinLength(Name,3,nameof(Name));
        DomainValidation.MaxLength(Name, 250, nameof(Name));
        DomainValidation.MaxLength(Description, 10000, nameof(Description));
    }   

}
