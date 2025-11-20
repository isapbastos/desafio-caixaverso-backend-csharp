using System.ComponentModel.DataAnnotations;

namespace InvestimentosJwt.Domain.Entities;

public class TelemetriaRegistro
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Nome do serviço monitorado.
    /// </summary>
    public string NomeServico { get; set; } = string.Empty;

    /// <summary>
    /// Tempo de resposta em milissegundos.
    /// </summary>
    public long TempoRespostaMs { get; set; }

    /// <summary>
    /// Data e hora do registro.
    /// </summary>
    public DateTime DataRegistro { get; set; } = DateTime.Now;
    public TelemetriaRegistro() { }
    public TelemetriaRegistro(string servico, long tempoResposta)
    {
        NomeServico = servico;
        TempoRespostaMs = tempoResposta;
        DataRegistro = DateTime.Now;
    }

    public TelemetriaRegistro(string servico, long tempoResposta, DateTime dataRegistro)
    {
        NomeServico = servico;
        TempoRespostaMs = tempoResposta;
        DataRegistro = dataRegistro;
    }
}