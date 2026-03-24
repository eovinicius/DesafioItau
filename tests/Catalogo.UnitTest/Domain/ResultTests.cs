using CatalogoItau.Domain.Abstractions;

using FluentAssertions;

namespace Catalogo.UnitTest.Domain;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Arrange

        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        // Arrange
        var error = new Error("code", "name");

        // Act
        var result = Result.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenSuccessHasError()
    {
        // Arrange

        // Act
        Action act = () => _ = new Result(true, new Error("code", "name"));

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenFailureHasNoError()
    {
        // Arrange

        // Act
        Action act = () => _ = new Result(false, Error.None);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GenericSuccess_ShouldExposeValue()
    {
        // Arrange

        // Act
        var result = Result.Success("ok");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
    }

    [Fact]
    public void GenericFailure_ValueAccess_ShouldThrow()
    {
        // Arrange
        var result = Result.Failure<string>(new Error("code", "name"));

        // Act
        Action act = () => _ = result.Value;

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Create_ShouldReturnNullValueFailure_WhenValueIsNull()
    {
        // Arrange
        string? value = null;

        // Act
        var result = Result.Create(value);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }

    [Fact]
    public void ImplicitConversion_ShouldCreateResultFromValue()
    {
        // Arrange

        // Act
        Result<string> result = "value";

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("value");
    }
}
