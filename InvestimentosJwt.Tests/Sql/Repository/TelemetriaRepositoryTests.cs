using InvestimentosJwt.Domain.Entities;
using InvestimentosJwt.Infra.Data.Sql;
using InvestimentosJwt.Infra.Data.Sql.TelemetriaRepository;
using Microsoft.EntityFrameworkCore;

namespace InvestimentosJwt.Tests.Sql.Repository;
public class TelemetriaRepositoryTests
{
    private AppDbContext ObterContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // banco isolado por teste
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AdicionarRegistro_DevePersistirRegistro()
    {
        // Arrange
        var context = ObterContexto();
        var repository = new TelemetriaRepository(context);
        var registro = new TelemetriaRegistro("simular-investimento", 200, DateTime.UtcNow);

        // Act
        await repository.AdicionarRegistro(registro);

        // Assert
        var registros = await context.TelemetriaRegistros.ToListAsync();
        Assert.Single(registros);
        Assert.Equal("simular-investimento", registros[0].NomeServico);
        Assert.Equal(200, registros[0].TempoRespostaMs);
    }

    [Fact]
    public async Task ObterDadosPorPeriodo_DeveRetornarRegistrosDentroDoPeriodo()
    {
        // Arrange
        var context = ObterContexto();
        var repository = new TelemetriaRepository(context);

        var registro1 = new TelemetriaRegistro("A", 100, new DateTime(2025, 10, 10));
        var registro2 = new TelemetriaRegistro("B", 150, new DateTime(2025, 10, 20));
        var registro3 = new TelemetriaRegistro("C", 200, new DateTime(2025, 11, 1));

        context.TelemetriaRegistros.AddRange(registro1, registro2, registro3);
        await context.SaveChangesAsync();

        // Act
        var resultados = (await repository.ObterDadosPorPeriodo(
            new DateTime(2025, 10, 1),
            new DateTime(2025, 10, 31))).ToList();

        // Assert
        Assert.Equal(2, resultados.Count);
        Assert.Contains(resultados, r => r.NomeServico == "A");
        Assert.Contains(resultados, r => r.NomeServico == "B");
        Assert.DoesNotContain(resultados, r => r.NomeServico == "C");
    }
}
