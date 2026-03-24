using CatalogoItau.Application.Abstractions.Pagination;
using CatalogoItau.Application.Commands.Pedidos;
using CatalogoItau.Application.Queries.Pedidos;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace CatalogoItau.Api.Controllers;

/// <summary>
/// Controlador responsável pelo gerenciamento de pedidos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Pedidos")]
public sealed class PedidosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PedidosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém todos os pedidos com paginação.
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Quantidade de itens por página (padrão: 10, máximo: 100)</param>
    /// <returns>Lista paginada de pedidos</returns>
    /// <response code="200">Retorna a lista paginada de pedidos</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetPedidosPaginadosQuery(page, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Obtém um pedido específico pelo ID.
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <returns>Dados do pedido solicitado</returns>
    /// <response code="200">Retorna o pedido encontrado</response>
    /// <response code="404">Pedido não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetPedidoByIdQuery { Id = id });

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo pedido com itens e realiza automaticamente a depleção do estoque.
    /// </summary>
    /// <param name="command">Dados do novo pedido incluindo itens</param>
    /// <returns>Pedido criado com todos os seus dados</returns>
    /// <response code="201">Pedido criado com sucesso</response>
    /// <response code="400">Dados inválidos, estoque insuficiente ou número de pedido duplicado</response>
    /// <response code="404">Produto não encontrado</response>
    /// <response code="409">Conflito de concorrência (estoque foi modificado simultaneamente)</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CriarPedidoCommand command)
    {
        var pedido = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedido);
    }

    /// <summary>
    /// Atualiza o status de um pedido existente.
    /// </summary>
    /// <param name="id">ID do pedido a atualizar</param>
    /// <param name="command">Novo status do pedido</param>
    /// <response code="204">Status do pedido atualizado com sucesso</response>
    /// <response code="400">Dados inválidos ou transição de status inválida</response>
    /// <response code="404">Pedido não encontrado</response>
    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] AtualizarStatusPedidoCommand command)
    {
        command.Id = id;

        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Cancela um pedido existente.
    /// </summary>
    /// <param name="id">ID do pedido a cancelar</param>
    /// <response code="204">Pedido cancelado com sucesso</response>
    /// <response code="404">Pedido não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id)
    {
        await _mediator.Send(new CancelarPedidoCommand { Id = id });

        return NoContent();
    }
}