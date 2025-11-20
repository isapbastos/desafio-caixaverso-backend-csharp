using InvestimentosJwt.Application.ProdutoService;
using InvestimentosJwt.Application.TelemetriaService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InvestimentosJwtApi.Controllers;

/// <summary>
/// Controlador responsável por operações relacionadas aos produtos de investimento.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly ITelemetriaService _telemetriaService;

    public ProdutoController(IProdutoService produtoService, ITelemetriaService telemetriaService)
    {
        _produtoService = produtoService;
        _telemetriaService = telemetriaService;
    }

    /// <summary>
    /// Retorna a lista de todos os produtos de investimento disponíveis.
    /// </summary>
    /// <returns>Lista de produtos.</returns>
    /// <response code="200">Lista retornada com sucesso.</response>
    /// <response code="500">Erro interno ao buscar a lista de produtos.</response>
    [HttpGet("produtos")]
    public async Task<IActionResult> GetProdutos()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            var produtos = await _produtoService.ListarProdutos();

            return Ok(produtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar produtos: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
            await _telemetriaService.RegistrarChamada("/api/Produto/produtos", stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Retorna as informações de um produto específico.
    /// </summary>
    /// <param name="id">ID do produto desejado.</param>
    /// <returns>Objeto contendo os dados do produto.</returns>
    /// <response code="200">Produto encontrado.</response>
    /// <response code="404">Produto não encontrado.</response>
    /// <response code="400">ID inválido.</response>
    /// <response code="500">Erro interno ao buscar o produto.</response>
    [HttpGet("produtos/{id}")]
    public async Task<IActionResult> GetProduto(int id)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            var produto = await _produtoService.ObterProduto(id);

            if (produto == null)
                return NotFound("Produto não encontrado.");

            return Ok(produto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar produto: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
            // ==== IMPORTANTE PARA PASSAR NO TESTE ====
            await _telemetriaService.RegistrarChamada("/api/Produto/produtos/{id}", stopwatch.ElapsedMilliseconds);
        }
    }
}
