using InvestimentosJwt.Domain.Entities;
using InvestimentosJwt.Infra.Data.Sql.InvestimentoRepository;

namespace InvestimentosJwt.Application.InvestimentoService;
/// <summary>
/// Serviço de investimentos.
/// </summary>
public class InvestimentoService : IInvestimentoService
{
    private readonly IInvestimentoRepository _repository;
    public InvestimentoService(IInvestimentoRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retorna o histórico de investimentos de um cliente.
    /// </summary>
    public Task<List<Investimento>> ObterHistorico(int clienteId)
    {
        return _repository.ObterPorCliente(clienteId);
    }

    /// <summary>
    /// Registra um investimento realizado pelo cliente.
    /// </summary>
    public Task RegistrarInvestimento(Investimento investimento)
    {
        investimento.Data = DateTime.UtcNow;
        return _repository.AdicionarAsync(investimento);
    }
}
