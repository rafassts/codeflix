namespace Codeflix.Catalog.Application.Exceptions;
public class NotFoundException : ApplicationException
{
    public NotFoundException(string? message) : base(message)
    {
    }

    //@ - usar palavra reservada como nome de variavel
    public static void ThrowIfNull(object? @object, string exceptionMessage)
    {
        if(@object == null) throw new NotFoundException(exceptionMessage);
    }
}
