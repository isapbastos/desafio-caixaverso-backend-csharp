using InvestimentosJwt.Application.PerfilService;
using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwt.Domain.Entities;
using InvestimentosJwtApi.Controllers;
using InvestimentosJwtApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InvestimentosJwt.Tests.Controllers.Integration;
public class PerfilControllerIntegrationTests
{
    [Fact]
    public async Task ObterPerfilEProdutosRecomendados_DeveFuncionar()
    {
        var mockPerfil = new Mock<IPerfilService>();
        var mockTelemetry = new Mock<ITelemetriaService>();

        mockPerfil.Setup(p => p.ObterPerfilRisco(1)).ReturnsAsync(("Moderado", 65, "Perfil equilibrado entre segurança e rentabilidade."));
        mockPerfil.Setup(p => p.ObterProdutosRecomendados("Moderado")).ReturnsAsync(new List<Produto>
        {
            new Produto { Id = 101, Nome = "CDB Caixa 2026", Tipo = "CDB", Rentabilidade = 0.12m, Risco = "Baixo" }
        });

        var controller = new PerfilController(mockPerfil.Object, mockTelemetry.Object);

        var perfilResult = await controller.GetPerfilRiscoAsync(1) as OkObjectResult;
        var perfil = perfilResult.Value as PerfilRiscoDto;

        Assert.Equal("Moderado", perfil.Perfil);
        Assert.Equal(65, perfil.Pontuacao);


        var produtosResult = await controller.GetProdutosRecomendados("Moderado") as OkObjectResult;
        var produtos = produtosResult.Value as List<Produto>;
        Assert.Single(produtos);
    }
}
