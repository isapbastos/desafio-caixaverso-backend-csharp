using Moq;
using Microsoft.AspNetCore.Mvc;
using InvestimentosJwt.Application.InvestimentoService;
using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwt.Domain.Entities;
using InvestimentosJwtApi.Controllers;

namespace InvestimentosJwt.Tests.Controllers;
public class InvestimentosControllerTests
{
    [Fact]
    public async Task ObterHistorico_DeveRetornarOkComLista()
    {
        // Arrange
        var mockService = new Mock<IInvestimentoService>();
        var mockTelemetria = new Mock<ITelemetriaService>();

        var historico = new List<Investimento>
        {
            new Investimento { Id = 1, ClienteId = 123, Tipo = "CDB", Valor = 10000m, Rentabilidade = 0.12m, Data = DateTime.UtcNow },
            new Investimento { Id = 2, ClienteId = 123, Tipo = "Fundo", Valor = 5000m, Rentabilidade = 0.08m, Data = DateTime.UtcNow }
        };

        mockService.Setup(s => s.ObterHistorico(123)).ReturnsAsync(historico);

        var controller = new InvestimentosController(mockService.Object, mockTelemetria.Object);

        // Act
        var result = await controller.ObterHistorico(123) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var list = Assert.IsType<List<Investimento>>(result.Value);
        Assert.Equal(2, list.Count);
        mockTelemetria.Verify(t => t.RegistrarChamada("/api/Investimento/{clienteId}", It.IsAny<long>()), Times.Once);
    }

    [Fact]
    public async Task Investir_InvestimentoValido_DeveRetornarCreatedAtAction()
    {
        // Arrange
        var mockService = new Mock<IInvestimentoService>();
        var mockTelemetria = new Mock<ITelemetriaService>();

        var investimento = new Investimento
        {
            Id = 1,
            ClienteId = 123,
            Tipo = "CDB",
            Valor = 10000m,
            Rentabilidade = 0.12m,
            Data = DateTime.UtcNow
        };

        mockService.Setup(s => s.RegistrarInvestimento(investimento)).Returns(Task.CompletedTask);

        var controller = new InvestimentosController(mockService.Object, mockTelemetria.Object);

        // Act
        var result = await controller.Investir(investimento) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nameof(InvestimentosController.ObterHistorico), result.ActionName);
        var obj = Assert.IsType<Investimento>(result.Value);
        Assert.Equal(123, obj.ClienteId);
        mockTelemetria.Verify(t => t.RegistrarChamada("/api/Investimento/investir", It.IsAny<long>()), Times.Once);
    }

    [Fact]
    public async Task Investir_InvestimentoInvalido_DeveRetornarBadRequest()
    {
        // Arrange
        var mockService = new Mock<IInvestimentoService>();
        var mockTelemetria = new Mock<ITelemetriaService>();

        var investimento = new Investimento { ClienteId = 123, Tipo = "CDB", Valor = 0, Rentabilidade = 0.12m };

        var controller = new InvestimentosController(mockService.Object, mockTelemetria.Object);

        // Act
        var result = await controller.Investir(investimento) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Investimento inválido.", result.Value);
        mockTelemetria.Verify(t => t.RegistrarChamada("/api/Investimento/investir", It.IsAny<long>()), Times.Never);
    }
}
