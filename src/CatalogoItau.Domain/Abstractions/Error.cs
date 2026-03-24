namespace CatalogoItau.Domain.Abstractions;

public sealed record Error(string Code, string Name)
{
    public static Error None = new Error(string.Empty, string.Empty);
    public static Error NullValue = new Error("Error.NullValue", "Null value was provided");
}