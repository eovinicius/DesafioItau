using CatalogoItau.Domain.Exceptions;

namespace CatalogoItau.Domain.Entities;

/// <summary>
/// Entidade que representa um item dentro de um pedido.
/// </summary>
public sealed class ItemPedido
{
    /// <summary>
    /// Identificador único do item pedido.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// ID do pedido que contém este item.
    /// </summary>
    public int PedidoId { get; private set; }

    /// <summary>
    /// ID do produto.
    /// </summary>
    public int ProdutoId { get; private set; }

    /// <summary>
    /// Quantidade de unidades do produto.
    /// </summary>
    public int Quantidade { get; private set; }

    /// <summary>
    /// Preço unitário do produto no momento da compra.
    /// </summary>
    public decimal PrecoUnitario { get; private set; }

    /// <summary>
    /// Resultado da multiplicação de quantidade pelo preço unitário.
    /// </summary>
    public decimal Subtotal => Quantidade * PrecoUnitario;

    private ItemPedido() { }

    /// <summary>
    /// Inicializa uma nova instância de ItemPedido.
    /// </summary>
    /// <param name="produto">Produto a adicionar ao pedido</param>
    /// <param name="quantidade">Quantidade desejada</param>
    /// <exception cref="DomainException">Lançada quando produto é nulo ou quantidade é inválida</exception>
    public ItemPedido(Produto produto, int quantidade)
    {
        if (produto == null)
            throw new DomainException("Produto inválido");

        if (quantidade < 1)
            throw new DomainException("Quantidade deve ser >= 1");

        ProdutoId = produto.Id;
        Quantidade = quantidade;
        PrecoUnitario = produto.Preco;
    }
}
