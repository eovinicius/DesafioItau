using CatalogoItau.Domain.Entities.Enums;
using CatalogoItau.Domain.Exceptions;

namespace CatalogoItau.Domain.Entities;

/// <summary>
/// Entidade que representa um pedido de compra.
/// </summary>
public sealed class Pedido
{
    /// <summary>
    /// Identificador único do pedido.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Número único do pedido para referência.
    /// </summary>
    public string NumeroPedido { get; private set; }

    /// <summary>
    /// Nome do cliente que realizou o pedido.
    /// </summary>
    public string ClienteNome { get; private set; }

    /// <summary>
    /// Email do cliente.
    /// </summary>
    public string ClienteEmail { get; private set; }

    /// <summary>
    /// Data e hora de criação do pedido.
    /// </summary>
    public DateTime DataPedido { get; private set; }

    /// <summary>
    /// Status atual do pedido.
    /// </summary>
    public StatusPedido Status { get; private set; }

    /// <summary>
    /// Valor total do pedido calculado a partir dos itens.
    /// </summary>
    public decimal ValorTotal => Itens.Sum(i => i.Subtotal);

    /// <summary>
    /// Itens que compõem o pedido.
    /// </summary>
    public List<ItemPedido> Itens { get; private set; } = [];

    /// <summary>
    /// Inicializa uma nova instância de Pedido.
    /// </summary>
    /// <param name="numeroPedido">Número único do pedido</param>
    /// <param name="clienteNome">Nome do cliente</param>
    /// <param name="clienteEmail">Email do cliente</param>
    public Pedido(string numeroPedido, string clienteNome, string clienteEmail)
    {
        NumeroPedido = numeroPedido;
        ClienteNome = clienteNome;
        ClienteEmail = clienteEmail;
        DataPedido = DateTime.UtcNow;
        Status = StatusPedido.Pendente;
    }

    /// <summary>
    /// Adiciona um item ao pedido.
    /// </summary>
    /// <param name="produto">Produto a adicionar</param>
    /// <param name="quantidade">Quantidade do produto</param>
    /// <exception cref="DomainException">Lançada quando produto é nulo ou quantidade é inválida</exception>
    public void AdicionarItem(Produto produto, int quantidade)
    {
        if (produto == null)
            throw new DomainException("Produto inválido");

        if (quantidade < 1)
            throw new DomainException("Quantidade deve ser >= 1");

        var item = new ItemPedido(produto, quantidade);
        Itens.Add(item);
    }

    /// <summary>
    /// Atualiza o status do pedido.
    /// </summary>
    /// <param name="novoStatus">Novo status do pedido</param>
    /// <exception cref="DomainException">Lançada quando o pedido está cancelado</exception>
    public void AtualizarStatus(StatusPedido novoStatus)
    {
        if (Status == StatusPedido.Cancelado)
            throw new DomainException("Pedido cancelado não pode ser alterado");

        Status = novoStatus;
    }

    /// <summary>
    /// Cancela o pedido.
    /// </summary>
    /// <exception cref="DomainException">Lançada quando o pedido já foi entregue</exception>
    public void Cancelar()
    {
        if (Status == StatusPedido.Entregue)
            throw new DomainException("Pedido já entregue não pode ser cancelado");

        Status = StatusPedido.Cancelado;
    }
}