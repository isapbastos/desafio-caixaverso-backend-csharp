using Moq;
using Microsoft.AspNetCore.Mvc;
using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwtApi.Controllers;

namespace InvestimentosJwt.Tests.Controllers;
public class TelemetriaControllerTests
{
    [Fact]
    public async Task GetTelemetria_DeveRetornarOkComRelatorio()
    {
        // Arrange
        var mockService = new Mock<ITelemetriaService>();

        var inicio = new DateTime(2025, 10, 1);
        var fim = new DateTime(2025, 10, 31);

        var relatorio = new
        {
            servicos = new List<object>
            {
                new { nome = "simular-investimento", quantidadeChamadas = 120, mediaTempoRespostaMs = 250 },
                new { nome = "perfil-risco", quantidadeChamadas = 80, mediaTempoRespostaMs = 180 }
            },
            periodo = new { inicio = "2025-10-01", fim = "2025-10-31" }
        };

        mockService.Setup(s => s.ObterRelatorio(inicio, fim)).ReturnsAsync(relatorio);

        var controller = new TelemetriaController(mockService.Object);

        // Act
        var result = await controller.GetTelemetria(inicio, fim) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        dynamic obj = result.Value;
        Assert.Equal(2, obj.servicos.Count);
        Assert.Equal("simular-investimento", obj.servicos[0].nome);
        Assert.Equal("2025-10-01", obj.periodo.inicio);
        Assert.Equal("2025-10-31", obj.periodo.fim);
    }
}
