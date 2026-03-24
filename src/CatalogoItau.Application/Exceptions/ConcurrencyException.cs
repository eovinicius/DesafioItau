namespace CatalogoItau.Application.Exceptions;

public sealed class ConcurrencyException : Exception
{
    public ConcurrencyException(string message) : base(message)
    {
    }
}