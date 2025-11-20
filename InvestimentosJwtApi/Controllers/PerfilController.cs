using InvestimentosJwt.Application.PerfilService;
using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwtApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InvestimentosJwtApi.Controllers;

/// <summary>
/// Controlador responsável por operações relacionadas ao perfil de risco e produtos recomendados.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PerfilController : ControllerBase
{
    private readonly IPerfilService _perfilService;
    private readonly ITelemetriaService _telemetriaService;

    public PerfilController(IPerfilService perfilService, ITelemetriaService telemetriaService)
    {
        _perfilService = perfilService;
        _telemetriaService = telemetriaService;
    }

    /// <summary>
    /// Retorna o perfil de risco do cliente com base em dados financeiros simulados.
    /// </summary>
    /// <param name="clienteId">ID do cliente.</param>
    /// <returns>Perfil de risco com pontuação e descrição.</returns>
    /// <response code="200">Perfil encontrado com sucesso.</response>
    /// <response code="400">ID do cliente inválido.</response>
    /// <response code="500">Erro ao processar o perfil de risco.</response>
    [HttpGet("perfil-risco/{clienteId}")]
    public async Task<IActionResult> GetPerfilRiscoAsync(int clienteId)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            if (clienteId <= 0)
                return BadRequest("ID do cliente inválido.");

            var (perfil, pontuacao, descricao) = await _perfilService.ObterPerfilRisco(clienteId);

            var dto = new PerfilRiscoDto
            {
                ClienteId = clienteId,
                Perfil = perfil,
                Pontuacao = pontuacao,
                Descricao = descricao
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao obter perfil de risco: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
            await _telemetriaService.RegistrarChamada(
                "/api/Perfil/perfil-risco/{clienteId}",
                stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Retorna produtos recomendados com base no perfil informado.
    /// </summary>
    /// <param name="perfil">Perfil do cliente (Conservador, Moderado, Agressivo).</param>
    /// <returns>Lista de produtos recomendados.</returns>
    /// <response code="200">Produtos recomendados retornados com sucesso.</response>
    /// <response code="400">Perfil informado é inválido.</response>
    /// <response code="500">Erro ao buscar recomendações.</response>
    [HttpGet("produtos-recomendados/{perfil}")]
    public async Task<IActionResult> GetProdutosRecomendados(string perfil)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            if (string.IsNullOrWhiteSpace(perfil))
                return BadRequest("Perfil não pode ser vazio.");

            var recomendados = await _perfilService.ObterProdutosRecomendados(perfil);

            return Ok(recomendados);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao obter produtos recomendados: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
            await _telemetriaService.RegistrarChamada(
                "/api/Perfil/produtos-recomendados/{perfil}",
                stopwatch.ElapsedMilliseconds);
        }
    }
}
