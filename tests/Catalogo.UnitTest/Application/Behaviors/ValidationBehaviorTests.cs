using CatalogoItau.Application.Behaviors;

using ApplicationValidationException = CatalogoItau.Application.Exceptions.ValidationException;

using FluentAssertions;

using FluentValidation;

using MediatR;

namespace Catalogo.UnitTest.Application.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoValidatorsAreRegistered()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("ok");
        RequestHandlerDelegate<string> next = _ => Task.FromResult("processed");

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be("processed");
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenAnyValidatorFails()
    {
        // Arrange
        var validators = new IValidator<TestRequest>[] { new TestRequestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest(string.Empty);
        RequestHandlerDelegate<string> next = _ => Task.FromResult("processed");

        // Act
        Func<Task> act = () => behavior.Handle(request, next, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ApplicationValidationException>();
        exception.Which.Errors.Should().ContainSingle(x => x.PropertyName == nameof(TestRequest.Name));
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidationPasses()
    {
        // Arrange
        var validators = new IValidator<TestRequest>[] { new TestRequestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("valid");
        RequestHandlerDelegate<string> next = _ => Task.FromResult("processed");

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be("processed");
    }

    private sealed record TestRequest(string Name) : IRequest<string>;

    private sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
