using InvestimentosJwt.Domain.Entities;

namespace InvestimentosJwt.Infra.Data.Sql.InvestimentoRepository;
public interface IInvestimentoRepository
{
    Task<List<Investimento>> ObterPorCliente(int clienteId);
    Task AdicionarAsync(Investimento investimento);
}
