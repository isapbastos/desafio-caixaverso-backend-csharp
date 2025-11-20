using InvestimentosJwt.Application.ProdutoService;
using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwt.Domain.Entities;
using InvestimentosJwtApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InvestimentosJwt.Tests.Controllers.Integration;
public class ProdutoControllerIntegrationTests
{
    [Fact]
    public async Task ListarProdutos_DeveRetornarProdutos()
    {
        var mockService = new Mock<IProdutoService>();
        var mockTelemetry = new Mock<ITelemetriaService>();

        mockService.Setup(s => s.ListarProdutos()).ReturnsAsync(new List<Produto>
        {
            new Produto { Id = 1, Nome = "CDB Caixa 2026", Tipo = "CDB", Rentabilidade = 0.12m, Risco = "Baixo" }
        });

        var controller = new ProdutoController(mockService.Object, mockTelemetry.Object);

        var result = await controller.GetProdutos() as OkObjectResult;
        var produtos = result.Value as List<Produto>;

        Assert.Single(produtos);
        Assert.Equal("CDB Caixa 2026", produtos[0].Nome);
    }
}
