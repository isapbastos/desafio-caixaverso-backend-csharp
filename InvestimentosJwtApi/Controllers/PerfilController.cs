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
    [HttpGet("perfil-risco/{clienteId}")]
    public async Task<IActionResult> GetPerfilRiscoAsync(int clienteId)
    {
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


    /// <summary>
    /// Retorna produtos recomendados com base no perfil informado.
    /// </summary>
    /// <param name="perfil">Perfil do cliente (Conservador, Moderado, Agressivo).</param>
    /// <returns>Lista de produtos recomendados.</returns>
    /// <response code="200">Produtos recomendados retornados com sucesso.</response>
    [HttpGet("produtos-recomendados/{perfil}")]
    public async Task<IActionResult> GetProdutosRecomendados(string perfil)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var recomendados = await _perfilService.ObterProdutosRecomendados(perfil);
        stopwatch.Stop();
        var tempoRespostaMs = stopwatch.ElapsedMilliseconds;
        await _telemetriaService.RegistrarChamada("/api/Perfil/produtos-recomendados/{perfil}", tempoRespostaMs);
        return Ok(recomendados);
    }
}

