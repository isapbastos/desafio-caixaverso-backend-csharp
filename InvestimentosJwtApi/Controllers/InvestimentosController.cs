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
    public InvestimentosController(IInvestimentoService investimentoService, ITelemetriaService telemetriaService)
    {
        _investimentoService = investimentoService;
        _telemetriaService = telemetriaService;
    }

    /// <summary>
    /// Retorna o histórico de investimentos de um cliente.
    /// </summary>
    /// <param name="clienteId">Id do cliente</param>
    [HttpGet("{clienteId}")]
    public async Task<IActionResult> ObterHistorico(int clienteId)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var historico = await _investimentoService.ObterHistorico(clienteId);
        stopwatch.Stop();
        var tempoRespostaMs = stopwatch.ElapsedMilliseconds;
        await _telemetriaService.RegistrarChamada("/api/Investimento/{clienteId}", tempoRespostaMs);
        return Ok(historico);
    }

    /// <summary>
    /// Registra um novo investimento para um cliente.
    /// </summary>
    /// <param name="investimento">Objeto contendo clienteId, tipo, valor e rentabilidade</param>
    [HttpPost("investir")]
    public async Task<IActionResult> Investir([FromBody] Investimento investimento)
    {
        if (investimento == null || investimento.Valor <= 0)
            return BadRequest("Investimento inválido.");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await _investimentoService.RegistrarInvestimento(investimento);
        stopwatch.Stop();
        var tempoRespostaMs = stopwatch.ElapsedMilliseconds;
        await _telemetriaService.RegistrarChamada("/api/Investimento/investir", tempoRespostaMs);
        return CreatedAtAction(nameof(ObterHistorico), new { clienteId = investimento.ClienteId }, investimento);
    }
}

