using InvestimentosJwt.Application.TelemetriaService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvestimentosJwtApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TelemetriaController : ControllerBase
{
    private readonly ITelemetriaService _service;

    public TelemetriaController(ITelemetriaService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retorna dados de telemetria com volumes e tempos médios por serviço.
    /// </summary>
    /// <param name="inicio">Data inicial do período (aaaa-mm-dd).</param>
    /// <param name="fim">Data final do período (aaaa-mm-dd).</param>
    /// <returns>Relatório de telemetria.</returns>
    /// <response code="200">Relatório de telemetria retornado com sucesso.</response>
    /// <response code="400">Parâmetros de data inválidos.</response>
    /// <response code="500">Erro interno ao gerar o relatório.</response>
    [HttpGet]
    public async Task<IActionResult> GetTelemetria([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
    {
        try
        {
            if (inicio == default || fim == default || inicio > fim)
                return BadRequest("As datas fornecidas são inválidas.");

            var inicioDia = inicio.Date;
            var fimDia = fim.Date.AddDays(1).AddTicks(-1);

            var relatorio = await _service.ObterRelatorio(inicioDia, fimDia);

            return Ok(relatorio);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao obter relatório de telemetria: {ex.Message}");
        }
    }
}
