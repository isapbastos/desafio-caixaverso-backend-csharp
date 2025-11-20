using InvestimentosJwt.Domain.Entities;

namespace InvestimentosJwt.Infra.Data.Sql.TelemetriaRepository;

public interface ITelemetriaRepository
{
    Task AdicionarRegistro(TelemetriaRegistro registro);
    Task<IEnumerable<TelemetriaRegistro>> ObterDadosPorPeriodo(DateTime inicio, DateTime fim);
}