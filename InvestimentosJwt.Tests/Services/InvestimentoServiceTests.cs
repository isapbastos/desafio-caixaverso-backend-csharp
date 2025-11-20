using InvestimentosJwt.Application.InvestimentoService;
using InvestimentosJwt.Application.SimulacaoService;
using InvestimentosJwt.Domain.Entities;
using InvestimentosJwt.Infra.Data.Sql.InvestimentoRepository;
using InvestimentosJwt.Infra.Data.Sql.SimulacaoRepository;
using Moq;

namespace InvestimentosJwt.Tests.Services;
public class InvestimentoServiceTests
{
    private readonly Mock<IInvestimentoRepository> _repo = new();
    private InvestimentoService CriarService() =>
    new InvestimentoService(_repo.Object);
    [Fact]
    public async Task ObterHistorico_DeveRetornarListaDeInvestimentos()
    {
        // Arrange
        int clienteId = 123;
        var investimentos = new List<Investimento>
        {
            new Investimento { Id = 1, ClienteId = clienteId, Tipo = "CDB", Valor = 10000, Rentabilidade = 0.12m, Data = DateTime.UtcNow },
            new Investimento { Id = 2, ClienteId = clienteId, Tipo = "Fundo", Valor = 5000, Rentabilidade = 0.08m, Data = DateTime.UtcNow }
        };

        _repo.Setup(r => r.ObterPorCliente(clienteId)).ReturnsAsync(investimentos);
        var service = new InvestimentoService(_repo.Object);

        // Act
        var resultado = await service.ObterHistorico(clienteId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, i => Assert.Equal(clienteId, i.ClienteId));
    }

    [Fact]
    public async Task RegistrarInvestimento_DeveChamarRepositorio()
    {
        // Arrange
        var investimento = new Investimento { ClienteId = 123, Tipo = "CDB", Valor = 10000, Rentabilidade = 0.12m };
        var service = new InvestimentoService(_repo.Object);

        // Act
        await service.RegistrarInvestimento(investimento);

        // Assert
        _repo.Verify(r => r.AdicionarAsync(It.Is<Investimento>(i => i == investimento)), Times.Once);
        Assert.True(investimento.Data <= DateTime.UtcNow);
    }
}


