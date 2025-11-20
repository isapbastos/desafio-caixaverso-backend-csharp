using Moq;
using Microsoft.AspNetCore.Mvc;
using InvestimentosJwt.Application.PerfilService;
using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwt.Domain.Entities;
using InvestimentosJwtApi.Controllers;

namespace InvestimentosJwt.Tests.Controllers;
public class PerfilControllerTests
{
    [Fact]
    public async Task GetPerfilRiscoAsync_DeveRetornarOkComPerfilNotNull()
    {
        // Arrange
        var mockPerfilService = new Mock<IPerfilService>();
        var mockTelemetria = new Mock<ITelemetriaService>();

        mockPerfilService.Setup(s => s.ObterPerfilRisco(123))
            .ReturnsAsync(("Moderado", 65, "Perfil equilibrado entre segurança e rentabilidade."));

        var controller = new PerfilController(mockPerfilService.Object, mockTelemetria.Object);

        // Act
        var result = await controller.GetPerfilRiscoAsync(123) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetProdutosRecomendados_DeveRetornarOkComLista()
    {
        // Arrange
        var mockPerfilService = new Mock<IPerfilService>();
        var mockTelemetria = new Mock<ITelemetriaService>();

        var produtos = new List<Produto>
        {
            new Produto { Id = 1, Nome = "CDB Caixa 2026", Tipo = "CDB", Rentabilidade = 0.12m, Risco = "Baixo" },
            new Produto { Id = 2, Nome = "Fundo XPTO", Tipo = "Fundo", Rentabilidade = 0.18m, Risco = "Alto" }
        };

        mockPerfilService.Setup(s => s.ObterProdutosRecomendados("Moderado"))
            .ReturnsAsync(produtos);

        var controller = new PerfilController(mockPerfilService.Object, mockTelemetria.Object);

        // Act
        var result = await controller.GetProdutosRecomendados("Moderado") as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var lista = Assert.IsType<List<Produto>>(result.Value);
        Assert.Equal(2, lista.Count);
        Assert.Contains(lista, p => p.Nome == "CDB Caixa 2026");
        Assert.Contains(lista, p => p.Nome == "Fundo XPTO");

        mockTelemetria.Verify(t => t.RegistrarChamada("/api/Perfil/produtos-recomendados/{perfil}", It.IsAny<long>()), Times.Once);
    }
}
