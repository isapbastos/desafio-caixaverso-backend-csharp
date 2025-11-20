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

    /// <summary>
    /// Realiza a simulação de um investimento com base nos parâmetros informados. 
    /// </summary> 
    /// <param name="request">Objeto contendo ClienteId, Valor, PrazoMeses e TipoProduto.</param> 
    /// <returns>Retorna o produto validado e o resultado da simulação.</returns> 
    /// <response code="200">Simulação realizada com sucesso.</response> 
    /// <response code="400">Dados inválidos para simulação.</response> 
    /// <response code="404">Produto não encontrado.</response>
    /// <response code="500">Erro interno ao realizar a simulação.</response>
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

    /// <summary>
    /// Retorna todas as simulações realizadas.
    /// </summary>
    /// <response code="200">Lista de simulações retornada com sucesso.</response>
    /// <response code="500">Erro interno ao buscar as simulações.</response>
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

    /// <summary>
    /// Retorna agregação de simulações por produto e dia.
    /// </summary>
    /// <response code="200">Agregação retornada com sucesso.</response>
    /// <response code="500">Erro interno ao buscar agregação.</response>
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
