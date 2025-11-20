using InvestimentosJwt.Application.InvestimentoService;
using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwt.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InvestimentosJwtApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InvestimentosController : ControllerBase
{
    private readonly IInvestimentoService _investimentoService;
    private readonly ITelemetriaService _telemetriaService;

    public InvestimentosController(
        IInvestimentoService investimentoService,
        ITelemetriaService telemetriaService)
    {
        _investimentoService = investimentoService;
        _telemetriaService = telemetriaService;
    }

    /// <summary>
    /// Retorna o histórico de investimentos de um cliente.
    /// </summary>
    /// <param name="clienteId">Id do cliente</param>
    /// <returns>Lista de investimentos</returns>
    [HttpGet("{clienteId}")]
    [ProducesResponseType(typeof(IEnumerable<Investimento>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterHistorico(int clienteId)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var historico = await _investimentoService.ObterHistorico(clienteId);

        stopwatch.Stop();
        await _telemetriaService
            .RegistrarChamada("/api/Investimento/{clienteId}", stopwatch.ElapsedMilliseconds);

        if (historico == null || !historico.Any())
            return NotFound("Nenhum investimento encontrado.");

        return Ok(historico);
    }

    /// <summary>
    /// Registra um novo investimento para um cliente.
    /// </summary>
    /// <param name="investimento">Objeto contendo clienteId, tipo, valor e rentabilidade</param>
    [HttpPost("investir")]
    [ProducesResponseType(typeof(Investimento), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Investir([FromBody] Investimento investimento)
    {
        if (investimento == null || investimento.Valor <= 0)
            return BadRequest("Investimento inválido.");

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        await _investimentoService.RegistrarInvestimento(investimento);

        stopwatch.Stop();
        await _telemetriaService
            .RegistrarChamada("/api/Investimento/investir", stopwatch.ElapsedMilliseconds);

        return CreatedAtAction(nameof(ObterHistorico), new { clienteId = investimento.ClienteId }, investimento);
    }
}
