using CatalogoItau.Domain.Exceptions;

namespace CatalogoItau.Domain.Entities;

/// <summary>
/// Entidade que representa um produto no catálogo.
/// </summary>
public sealed class Produto
{
    /// <summary>
    /// Identificador único do produto.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Nome do produto.
    /// </summary>
    public string Nome { get; private set; }

    /// <summary>
    /// Descrição detalhada do produto.
    /// </summary>
    public string? Descricao { get; private set; }

    /// <summary>
    /// Preço unitário do produto em reais.
    /// </summary>
    public decimal Preco { get; private set; }

    /// <summary>
    /// Quantidade em estoque.
    /// </summary>
    public int Estoque { get; private set; }

    /// <summary>
    /// Data de criação do produto.
    /// </summary>
    public DateTime DataCriacao { get; private set; }

    /// <summary>
    /// Data da última atualização (usada para controle otimista de concorrência).
    /// </summary>
    public DateTime AtualizadoEm { get; private set; }

    /// <summary>
    /// Indica se o produto está ativo (soft delete).
    /// </summary>
    public bool Ativo { get; private set; }

    /// <summary>
    /// Inicializa uma nova instância de Produto.
    /// </summary>
    /// <param name="nome">Nome do produto</param>
    /// <param name="preco">Preço unitário</param>
    /// <param name="estoque">Quantidade inicial em estoque</param>
    /// <param name="descricao">Descrição opcional do produto</param>
    public Produto(string nome, decimal preco, int estoque, string? descricao = null)
    {
        Nome = nome;
        Preco = preco;
        Estoque = estoque;
        Descricao = descricao;
        DataCriacao = DateTime.UtcNow;
        AtualizadoEm = DateTime.UtcNow;
        Ativo = true;
    }

    /// <summary>
    /// Atualiza os dados do produto.
    /// </summary>
    /// <param name="nome">Novo nome</param>
    /// <param name="preco">Novo preço</param>
    /// <param name="estoque">Nova quantidade em estoque</param>
    /// <param name="descricao">Nova descrição</param>
    public void Atualizar(string nome, decimal preco, int estoque, string? descricao)
    {
        Nome = nome;
        Preco = preco;
        Estoque = estoque;
        Descricao = descricao;
        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Inativa o produto (soft delete).
    /// </summary>
    public void Inativar()
    {
        Ativo = false;
    }

    /// <summary>
    /// Reduz o estoque do produto pela quantidade especificada.
    /// </summary>
    /// <param name="quantidade">Quantidade a ser debitada do estoque</param>
    /// <exception cref="DomainException">Lançada quando a quantidade é inválida ou estoque é insuficiente</exception>
    public void BaixarEstoque(int quantidade)
    {
        if (quantidade < 1)
            throw new DomainException("Quantidade deve ser maior ou igual a 1");

        if (Estoque < quantidade)
            throw new DomainException($"Estoque insuficiente. Disponível: {Estoque}, Solicitado: {quantidade}");

        Estoque -= quantidade;
        AtualizadoEm = DateTime.UtcNow;
    }
}