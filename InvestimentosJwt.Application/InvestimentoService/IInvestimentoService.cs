using InvestimentosJwt.Domain.Entities;

namespace InvestimentosJwt.Application.InvestimentoService;
public interface IInvestimentoService
{
    Task<List<Investimento>> ObterHistorico(int clienteId);
    Task RegistrarInvestimento(Investimento investimento);
}
