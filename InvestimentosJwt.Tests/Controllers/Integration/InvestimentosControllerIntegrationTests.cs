using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using InvestimentosJwtApi.Controllers;
using InvestimentosJwt.Application.InvestimentoService;
using InvestimentosJwt.Infra.Data.Sql.InvestimentoRepository;
using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvestimentosJwt.Tests.Sql;
namespace InvestimentosJwt.Tests.Controllers.Integration;
public class InvestimentosControllerIntegrationTests
{
    [Fact]
    public async Task InvestirEObterHistorico_DeveFuncionar()
    {
        // Arrange
        var context = DbContextFactory.CreateContext();
        var repo = new InvestimentoRepository(context);
        var service = new InvestimentoService(repo);
        var mockTelemetry = new Mock<ITelemetriaService>();
        var controller = new InvestimentosController(service, mockTelemetry.Object);

        var investimento = new Investimento
        {
            ClienteId = 1,
            Tipo = "CDB",
            Valor = 10000,
            Rentabilidade = 0.12m,
            Data = DateTime.UtcNow
        };

        // Act
        var postResult = await controller.Investir(investimento) as CreatedAtActionResult;

        // Assert POST
        Assert.NotNull(postResult);

        // Act GET
        var getResult = await controller.ObterHistorico(1) as OkObjectResult;
        var historico = getResult.Value as List<Investimento>;

        // Assert GET
        Assert.Single(historico);
        Assert.Equal("CDB", historico[0].Tipo);
    }
}

