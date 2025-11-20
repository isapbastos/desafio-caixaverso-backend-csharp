using InvestimentosJwt.Domain.Entities;
using InvestimentosJwt.Infra.Data.Sql.TelemetriaRepository;

namespace InvestimentosJwt.Application.TelemetriaService;

/// <summary>
/// Serviço responsável por registrar e consultar dados de telemetria,
/// como tempo de resposta e quantidade de chamadas por serviço.
/// </summary>
public class TelemetriaService : ITelemetriaService
{
    private readonly ITelemetriaRepository _repository;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="TelemetriaService"/>.
    /// </summary>
    /// <param name="repository">Repositório responsável pela persistência dos dados de telemetria.</param>
    public TelemetriaService(ITelemetriaRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Registra uma nova chamada de serviço na telemetria.
    /// </summary>
    /// <param name="nomeServico">Nome do serviço chamado.</param>
    /// <param name="tempoRespostaMs">Tempo de resposta em milissegundos.</param>
    public async Task RegistrarChamada(string nomeServico, long tempoRespostaMs)
    {
        var registro = new TelemetriaRegistro(nomeServico, tempoRespostaMs);
        await _repository.AdicionarRegistro(registro);
    }

    /// <summary>
    /// Obtém um relatório agregado de telemetria com base em um período informado.
    /// </summary>
    /// <param name="inicio">Data inicial do período a ser consultado.</param>
    /// <param name="fim">Data final do período a ser consultado.</param>
    /// <returns>
    /// Um objeto contendo:
    /// <list type="bullet">
    /// <item><description>A lista de serviços com quantidade de chamadas e tempo médio de resposta.</description></item>
    /// <item><description>O período consultado formatado.</description></item>
    /// </list>
    /// </returns>
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
            periodo = new 
            { 
                inicio = inicio.ToString("yyyy-MM-dd"), 
                fim = fim.ToString("yyyy-MM-dd") 
            }
        };
    }
}
