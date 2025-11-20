namespace InvestimentosJwt.Application.TelemetriaService;
public interface ITelemetriaService
{
    Task RegistrarChamada(string nomeServico, long tempoRespostaMs);
    Task<object> ObterRelatorio(DateTime inicio, DateTime fim);
}