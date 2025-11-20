using InvestimentosJwt.Domain.Entities;
using InvestimentosJwt.Infra.Data.Sql.TelemetriaRepository;

namespace InvestimentosJwt.Application.TelemetriaService;
public class TelemetriaService : ITelemetriaService
{
    private readonly ITelemetriaRepository _repository;

    public TelemetriaService(ITelemetriaRepository repository)
    {
        _repository = repository;
    }

    public async Task RegistrarChamada(string nomeServico, long tempoRespostaMs)
    {
        var registro = new TelemetriaRegistro(nomeServico,tempoRespostaMs);
        await _repository.AdicionarRegistro(registro);
    }

    public async Task<object> ObterRelatorio(DateTime inicio, DateTime fim)
    {
        var dados = await _repository.ObterDadosPorPeriodo(inicio, fim);
        var agrupado = dados
            .GroupBy(d => d.NomeServico)
            .Select(g => new
            {
                nome = g.Key,
                quantidadeChamadas = g.Count(),
                mediaTempoRespostaMs = g.Average(x => x.TempoRespostaMs)
            })
            .ToList();

        return new
        {
            servicos = agrupado,
            periodo = new { inicio = inicio.ToString("yyyy-MM-dd"), fim = fim.ToString("yyyy-MM-dd") }
        };
    }
}
