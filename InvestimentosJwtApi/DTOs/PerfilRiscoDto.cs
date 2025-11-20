namespace InvestimentosJwtApi.DTOs;
public class PerfilRiscoDto
{
    public int ClienteId { get; set; }
    public string Perfil { get; set; } = string.Empty;
    public double Pontuacao { get; set; }
    public string Descricao { get; set; } = string.Empty;
}

