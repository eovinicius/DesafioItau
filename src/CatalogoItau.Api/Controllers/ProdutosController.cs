using CatalogoItau.Application.Abstractions.Pagination;
using CatalogoItau.Application.Commands.Produtos;
using CatalogoItau.Application.Queries.Produtos;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace CatalogoItau.Api.Controllers;

/// <summary>
/// Controlador responsável pelo gerenciamento de produtos do catálogo.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Produtos")]
public sealed class ProdutosController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProdutosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém todos os produtos com paginação.
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Quantidade de itens por página (padrão: 10, máximo: 100)</param>
    /// <returns>Lista paginada de produtos</returns>
    /// <response code="200">Retorna a lista paginada de produtos ativos</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetProdutosPaginadosQuery(page, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Obtém um produto específico pelo ID.
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>Dados do produto solicitado</returns>
    /// <response code="200">Retorna o produto encontrado</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetProdutoByIdQuery { Id = id });

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo produto no catálogo.
    /// </summary>
    /// <param name="command">Dados do novo produto</param>
    /// <returns>ID do produto criado</returns>
    /// <response code="201">Produto criado com sucesso</response>
    /// <response code="400">Dados inválidos ou violação de regras de negócio</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CriarProdutoCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    /// <summary>
    /// Atualiza um produto existente.
    /// </summary>
    /// <param name="id">ID do produto a atualizar</param>
    /// <param name="command">Dados atualizados do produto</param>
    /// <response code="204">Produto atualizado com sucesso</response>
    /// <response code="400">Dados inválidos ou violação de regras de negócio</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] AtualizarProdutoCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Deleta um produto do catálogo (soft delete).
    /// </summary>
    /// <param name="id">ID do produto a deletar</param>
    /// <response code="204">Produto deletado com sucesso</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeletarProdutoCommand { Id = id });
        return NoContent();
    }
}
