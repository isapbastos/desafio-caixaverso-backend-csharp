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
        // Arrange
        var mockService = new Mock<ITelemetriaService>();

        var inicio = new DateTime(2025, 10, 1);
        var fim = new DateTime(2025, 10, 31);

        // O controller ajusta assim:
        var inicioEsperado = inicio.Date;
        var fimEsperado = fim.Date.AddDays(1).AddTicks(-1);

        // Relatório esperado
        var relatorio = new
        {
            servicos = new[]
            {
                new { nome = "simular-investimento", quantidadeChamadas = 120, mediaTempoRespostaMs = 250 }
            },
            periodo = new { inicio = "2025-10-01", fim = "2025-10-31" }
        };

        // Mock com os valores ajustados
        mockService
            .Setup(s => s.ObterRelatorio(inicioEsperado, fimEsperado))
            .ReturnsAsync(relatorio);

        var controller = new TelemetriaController(mockService.Object);

        // Act
        var actionResult = await controller.GetTelemetria(inicio, fim);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(ok.Value);

        dynamic obj = ok.Value;

        Assert.Single(obj.servicos);
        Assert.Equal(120, obj.servicos[0].quantidadeChamadas);
        Assert.Equal("2025-10-01", obj.periodo.inicio);
        Assert.Equal("2025-10-31", obj.periodo.fim);
    }
}
