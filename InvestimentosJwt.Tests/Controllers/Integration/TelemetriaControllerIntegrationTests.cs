using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwtApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InvestimentosJwt.Tests.Controllers.Integration;
public class TelemetriaControllerIntegrationTests
{
    [Fact]
    public async Task GetTelemetria_DeveRetornarRelatorio()
    {
        var mockService = new Mock<ITelemetriaService>();

        var inicio = new DateTime(2025, 10, 1);
        var fim = new DateTime(2025, 10, 31);

        var relatorio = new
        {
            servicos = new[]
            {
                new { nome = "simular-investimento", quantidadeChamadas = 120, mediaTempoRespostaMs = 250 }
            },
            periodo = new { inicio = "2025-10-01", fim = "2025-10-31" }
        };

        mockService.Setup(s => s.ObterRelatorio(inicio, fim)).ReturnsAsync(relatorio);

        var controller = new TelemetriaController(mockService.Object);

        var result = await controller.GetTelemetria(inicio, fim) as OkObjectResult;
        dynamic obj = result.Value;

        Assert.Equal(120, obj.servicos[0].quantidadeChamadas);
        Assert.Equal("2025-10-01", obj.periodo.inicio);
    }
}
