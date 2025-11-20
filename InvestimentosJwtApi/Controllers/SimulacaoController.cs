using InvestimentosJwt.Application.SimulacaoService;
using InvestimentosJwt.Application.SimulacaoService.Models;
using InvestimentosJwt.Application.TelemetriaService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InvestimentosJwtApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SimulacaoController : ControllerBase
{
    private readonly ISimulacaoService _simulacaoService;
    private readonly ITelemetriaService _telemetriaService;

    public SimulacaoController(ISimulacaoService simulacaoService, ITelemetriaService telemetriaService)
    {
        _simulacaoService = simulacaoService;
        _telemetriaService = telemetriaService;
    }

    [HttpPost("simular-investimento")]
    public async Task<IActionResult> SimularInvestimento([FromBody] SimulacaoRequest request)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            var resultado = await _simulacaoService.RealizarSimulacao(request);

            stopwatch.Stop();
            await _telemetriaService.RegistrarChamada("/api/Simulacao/simular-investimento", stopwatch.ElapsedMilliseconds);

            if (!resultado.Sucesso)
                return BadRequest(resultado.Mensagem);

            return Ok(resultado.Dados);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            await _telemetriaService.RegistrarChamada("/api/Simulacao/simular-investimento", stopwatch.ElapsedMilliseconds);

            return StatusCode(500, $"Erro interno ao realizar simulação: {ex.Message}");
        }
    }

    [HttpGet("simulacoes")]
    public async Task<IActionResult> GetSimulacoes()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            var simulacoes = await _simulacaoService.ObterTodasSimulacoes();

            stopwatch.Stop();
            await _telemetriaService.RegistrarChamada("/api/Simulacao/simulacoes", stopwatch.ElapsedMilliseconds);

            return Ok(simulacoes);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            await _telemetriaService.RegistrarChamada("/api/Simulacao/simulacoes", stopwatch.ElapsedMilliseconds);

            return StatusCode(500, $"Erro ao buscar simulações: {ex.Message}");
        }
    }

    [HttpGet("simulacoes/por-produto-dia")]
    public async Task<IActionResult> GetSimulacoesPorProdutoDia()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            var agregacao = await _simulacaoService.ObterSimulacoesPorProdutoDia();

            stopwatch.Stop();
            await _telemetriaService.RegistrarChamada("/api/Simulacao/simulacoes/por-produto-dia", stopwatch.ElapsedMilliseconds);

            return Ok(agregacao);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            await _telemetriaService.RegistrarChamada("/api/Simulacao/simulacoes/por-produto-dia", stopwatch.ElapsedMilliseconds);

            return StatusCode(500, $"Erro ao buscar agregação: {ex.Message}");
        }
    }
}
