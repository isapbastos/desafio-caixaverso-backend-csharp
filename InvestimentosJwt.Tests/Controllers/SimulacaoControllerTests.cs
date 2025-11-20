using InvestimentosJwt.Application.SimulacaoService;
using InvestimentosJwt.Application.SimulacaoService.Models;
using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwt.Domain.Entities;
using InvestimentosJwtApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InvestimentosJwt.Tests.Controllers;
public class SimulacaoControllerTests
{
    [Fact]
    public async Task SimularInvestimento_Sucesso_RetornaOkComDados()
    {
        // Arrange
        var mockSimulacao = new Mock<ISimulacaoService>();
        var mockTelemetria = new Mock<ITelemetriaService>();

        var request = new SimulacaoRequest
        {
            ClienteId = 123,
            Valor = 10000,
            PrazoMeses = 12,
            TipoProduto = "CDB"
        };

        var retornoDados = new Simulacao
        {
            Id = 1,
            ClienteId = 123,
            Produto = "CDB Caixa 2026",
            ValorInvestido = 10000,
            ValorFinal = 11200,
            PrazoMeses = 12,
            DataSimulacao = DateTime.UtcNow
        };

        mockSimulacao.Setup(s => s.RealizarSimulacao(request))
            .ReturnsAsync(RetornoSimulacao.SucessoRetorno(retornoDados));

        var controller = new SimulacaoController(mockSimulacao.Object, mockTelemetria.Object);

        // Act
        var result = await controller.SimularInvestimento(request) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(retornoDados, result.Value);
        mockTelemetria.Verify(t => t.RegistrarChamada("/api/Simulacao/simular-investimento", It.IsAny<long>()), Times.Once);
    }

    [Fact]
    public async Task SimularInvestimento_Erro_RetornaBadRequest()
    {
        // Arrange
        var mockSimulacao = new Mock<ISimulacaoService>();
        var mockTelemetria = new Mock<ITelemetriaService>();

        var request = new SimulacaoRequest { ClienteId = 123, Valor = 0, PrazoMeses = 12, TipoProduto = "CDB" };

        mockSimulacao.Setup(s => s.RealizarSimulacao(request))
            .ReturnsAsync(RetornoSimulacao.Erro("Valor inválido"));

        var controller = new SimulacaoController(mockSimulacao.Object, mockTelemetria.Object);

        // Act
        var result = await controller.SimularInvestimento(request) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Valor inválido", result.Value);
        mockTelemetria.Verify(t => t.RegistrarChamada("/api/Simulacao/simular-investimento", It.IsAny<long>()), Times.Once);
    }

    [Fact]
    public async Task GetSimulacoes_RetornaOkComLista()
    {
        // Arrange
        var mockSimulacao = new Mock<ISimulacaoService>();
        var mockTelemetria = new Mock<ITelemetriaService>();

        var listaSimulacoes = new List<Simulacao>
        {
            new Simulacao { Id = 1, ClienteId = 123, Produto = "CDB Caixa 2026", ValorInvestido = 10000, ValorFinal = 11200, PrazoMeses = 12, DataSimulacao = DateTime.UtcNow },
            new Simulacao { Id = 2, ClienteId = 124, Produto = "Fundo XPTO", ValorInvestido = 5000, ValorFinal = 5800, PrazoMeses = 6, DataSimulacao = DateTime.UtcNow }
        };

        mockSimulacao.Setup(s => s.ObterTodasSimulacoes()).ReturnsAsync(listaSimulacoes);

        var controller = new SimulacaoController(mockSimulacao.Object, mockTelemetria.Object);

        // Act
        var result = await controller.GetSimulacoes() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var list = Assert.IsType<List<Simulacao>>(result.Value);
        Assert.Equal(2, list.Count);
        mockTelemetria.Verify(t => t.RegistrarChamada("/api/Simulacao/simulacoes", It.IsAny<long>()), Times.Once);
    }

    [Fact]
    public async Task GetSimulacoesPorProdutoDia_RetornaOkComLista()
    {
        // Arrange
        var mockSimulacao = new Mock<ISimulacaoService>();
        var mockTelemetria = new Mock<ITelemetriaService>();

        var listaAgregada = new List<SimulacaoPorProdutoDiaDto>
        {
            new SimulacaoPorProdutoDiaDto { Produto = "CDB Caixa 2026", Data = DateTime.UtcNow.Date, QuantidadeSimulacoes = 10, MediaValorFinal = 11050m }
        };

        mockSimulacao.Setup(s => s.ObterSimulacoesPorProdutoDia()).ReturnsAsync(listaAgregada);

        var controller = new SimulacaoController(mockSimulacao.Object, mockTelemetria.Object);

        // Act
        var result = await controller.GetSimulacoesPorProdutoDia() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var list = Assert.IsType<List<SimulacaoPorProdutoDiaDto>>(result.Value);
        Assert.Single(list);
        Assert.Equal("CDB Caixa 2026", list[0].Produto);
        mockTelemetria.Verify(t => t.RegistrarChamada("/api/Simulacao/simulacoes/por-produto-dia", It.IsAny<long>()), Times.Once);
    }
}
