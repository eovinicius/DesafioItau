using CatalogoItau.Application.Commands.Pedidos;
using CatalogoItau.Domain.Entities.Enums;

using FluentValidation.TestHelper;

namespace Catalogo.UnitTest.Application.Validators;

public sealed class PedidoValidatorsTests
{
    [Fact]
    public void CriarPedidoValidator_ShouldHaveErrors_ForInvalidCommand()
    {
        // Arrange
        var validator = new CriarPedidoValidator();
        var command = new CriarPedidoCommand
        {
            NumeroPedido = string.Empty,
            ClienteNome = new string('x', 151),
            ClienteEmail = "invalid-email",
            Itens =
            [
                new CriarPedidoCommand.ItemDto { ProdutoId = 0, Quantidade = 0 }
            ]
        };

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NumeroPedido);
        result.ShouldHaveValidationErrorFor(x => x.ClienteNome);
        result.ShouldHaveValidationErrorFor(x => x.ClienteEmail);
        result.ShouldHaveValidationErrorFor("Itens[0].ProdutoId");
        result.ShouldHaveValidationErrorFor("Itens[0].Quantidade");
    }

    [Fact]
    public void AtualizarStatusPedidoValidator_ShouldHaveError_WhenIdIsInvalid()
    {
        // Arrange
        var validator = new AtualizarStatusPedidoValidator();
        var command = new AtualizarStatusPedidoCommand
        {
            Id = 0,
            NovoStatus = StatusPedido.Pendente
        };

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ItemPedidoValidator_ShouldHaveErrors_ForInvalidItem()
    {
        // Arrange
        var validator = new ItemPedidoValidator();
        var item = new CriarPedidoCommand.ItemDto { ProdutoId = 0, Quantidade = 0 };

        // Act
        var result = validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProdutoId);
        result.ShouldHaveValidationErrorFor(x => x.Quantidade);
    }
}
