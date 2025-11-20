using InvestimentosJwt.Application.SimulacaoService;
using InvestimentosJwt.Application.SimulacaoService.Models;
using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwt.Domain.Entities;
using InvestimentosJwtApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InvestimentosJwt.Tests.Controllers.Integration;
public class SimulacaoControllerIntegrationTests
{
    [Fact]
    public async Task SimularEObterSimulacoes_DeveFuncionar()
    {
        // Arrange
        var mockService = new Mock<ISimulacaoService>();
        var mockTelemetry = new Mock<ITelemetriaService>();

        var request = new SimulacaoRequest
        {
            ClienteId = 1,
            Valor = 10000,
            PrazoMeses = 12,
            TipoProduto = "CDB"
        };

        var retorno = RetornoSimulacao.SucessoRetorno(new { ValorFinal = 11200.00m });

        mockService.Setup(s => s.RealizarSimulacao(request)).ReturnsAsync(retorno);
        mockService.Setup(s => s.ObterTodasSimulacoes()).ReturnsAsync(new List<Domain.Entities.Simulacao>());

        var controller = new SimulacaoController(mockService.Object, mockTelemetry.Object);

        // Act
        var postResult = await controller.SimularInvestimento(request) as OkObjectResult;

        // Assert
        Assert.NotNull(postResult);
        Assert.Equal(11200.00m, ((dynamic)postResult.Value).ValorFinal);
    }
}
