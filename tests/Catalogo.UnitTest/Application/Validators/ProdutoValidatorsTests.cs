using CatalogoItau.Application.Commands.Produtos;

using FluentValidation.TestHelper;

namespace Catalogo.UnitTest.Application.Validators;

public sealed class ProdutoValidatorsTests
{
    [Fact]
    public void CriarProdutoValidator_ShouldHaveErrors_ForInvalidCommand()
    {
        // Arrange
        var validator = new CriarProdutoValidator();
        var command = new CriarProdutoCommand(string.Empty, new string('x', 501), 0m, -1);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Nome);
        result.ShouldHaveValidationErrorFor(x => x.Descricao);
        result.ShouldHaveValidationErrorFor(x => x.Preco);
        result.ShouldHaveValidationErrorFor(x => x.Estoque);
    }

    [Fact]
    public void AtualizarProdutoValidator_ShouldHaveErrors_ForInvalidCommand()
    {
        // Arrange
        var validator = new AtualizarProdutoValidator();
        var command = new AtualizarProdutoCommand
        {
            Id = 0,
            Nome = string.Empty,
            Descricao = new string('x', 501),
            Preco = 0m,
            Estoque = -1
        };

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.Nome);
        result.ShouldHaveValidationErrorFor(x => x.Descricao);
        result.ShouldHaveValidationErrorFor(x => x.Preco);
        result.ShouldHaveValidationErrorFor(x => x.Estoque);
    }
}
