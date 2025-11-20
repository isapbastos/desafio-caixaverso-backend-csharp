using Moq;
using Microsoft.AspNetCore.Mvc;
using InvestimentosJwt.Application.ProdutoService;
using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwt.Domain.Entities;
using InvestimentosJwtApi.Controllers;

namespace InvestimentosJwt.Tests.Controllers;

public class ProdutoControllerTests
{
    [Fact]
    public async Task GetProdutos_DeveRetornarOkComLista()
    {
        // Arrange
        var mockProdutoService = new Mock<IProdutoService>();
        var mockTelemetria = new Mock<ITelemetriaService>();

        var produtos = new List<Produto>
        {
            new Produto { Id = 1, Nome = "CDB Caixa 2026", Tipo = "CDB", Rentabilidade = 0.12m, Risco = "Baixo" },
            new Produto { Id = 2, Nome = "Fundo XPTO", Tipo = "Fundo", Rentabilidade = 0.18m, Risco = "Alto" }
        };

        mockProdutoService.Setup(s => s.ListarProdutos()).ReturnsAsync(produtos);

        var controller = new ProdutoController(mockProdutoService.Object, mockTelemetria.Object);

        // Act
        var result = await controller.GetProdutos() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var list = Assert.IsType<List<Produto>>(result.Value);
        Assert.Equal(2, list.Count);
        mockTelemetria.Verify(t => t.RegistrarChamada("/api/Produto/produtos", It.IsAny<long>()), Times.Once);
    }

    [Fact]
    public async Task GetProduto_ProdutoExistente_DeveRetornarOk()
    {
        // Arrange
        var mockProdutoService = new Mock<IProdutoService>();
        var mockTelemetria = new Mock<ITelemetriaService>();

        var produto = new Produto { Id = 1, Nome = "CDB Caixa 2026", Tipo = "CDB", Rentabilidade = 0.12m, Risco = "Baixo" };

        mockProdutoService.Setup(s => s.ObterProduto(1)).ReturnsAsync(produto);

        var controller = new ProdutoController(mockProdutoService.Object, mockTelemetria.Object);

        // Act
        var result = await controller.GetProduto(1) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var obj = Assert.IsType<Produto>(result.Value);
        Assert.Equal("CDB Caixa 2026", obj.Nome);
        mockTelemetria.Verify(t => t.RegistrarChamada("/api/Produto/produtos/{id}", It.IsAny<long>()), Times.Once);
    }

    [Fact]
    public async Task GetProduto_ProdutoInexistente_DeveRetornarNotFound()
    {
        // Arrange
        var mockProdutoService = new Mock<IProdutoService>();
        var mockTelemetria = new Mock<ITelemetriaService>();

        mockProdutoService.Setup(s => s.ObterProduto(99)).ReturnsAsync((Produto)null);

        var controller = new ProdutoController(mockProdutoService.Object, mockTelemetria.Object);

        // Act
        var result = await controller.GetProduto(99) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Produto não encontrado.", result.Value);
        mockTelemetria.Verify(t => t.RegistrarChamada("/api/Produto/produtos/{id}", It.IsAny<long>()), Times.Once);
    }
}
