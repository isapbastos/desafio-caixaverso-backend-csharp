using InvestimentosJwt.Domain.Entities;
using InvestimentosJwt.Infra.Data.Sql;
using InvestimentosJwt.Infra.Data.Sql.InvestimentoRepository;
using Microsoft.EntityFrameworkCore;

namespace InvestimentosJwt.Tests.Sql.Repository;
public class InvestimentoRepositoryTests
{
    private AppDbContext ObterContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Banco em memória único para cada teste
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarInvestimento()
    {
        // Arrange
        var contexto = ObterContextoEmMemoria();
        var repo = new InvestimentoRepository(contexto);

        var investimento = new Investimento
        {
            ClienteId = 123,
            Tipo = "CDB",
            Valor = 10000,
            Rentabilidade = 0.12m,
            Data = DateTime.UtcNow
        };

        // Act
        await repo.AdicionarAsync(investimento);

        // Assert
        var lista = await contexto.Investimentos.ToListAsync();
        Assert.Single(lista);
        Assert.Equal("CDB", lista.First().Tipo);
        Assert.Equal(123, lista.First().ClienteId);
    }

    [Fact]
    public async Task ObterPorCliente_DeveRetornarInvestimentosDoCliente()
    {
        // Arrange
        var contexto = ObterContextoEmMemoria();
        var repo = new InvestimentoRepository(contexto);

        contexto.Investimentos.AddRange(
            new Investimento { ClienteId = 123, Tipo = "CDB", Valor = 10000, Rentabilidade = 0.12m, Data = DateTime.UtcNow.AddDays(-1) },
            new Investimento { ClienteId = 123, Tipo = "Fundo", Valor = 5000, Rentabilidade = 0.08m, Data = DateTime.UtcNow },
            new Investimento { ClienteId = 999, Tipo = "CDB", Valor = 8000, Rentabilidade = 0.10m, Data = DateTime.UtcNow }
        );
        await contexto.SaveChangesAsync();

        // Act
        var resultado = await repo.ObterPorCliente(123);

        // Assert
        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, i => Assert.Equal(123, i.ClienteId));
        Assert.Equal("Fundo", resultado.First().Tipo); // Verifica ordenação por Data DESC
    }
}
