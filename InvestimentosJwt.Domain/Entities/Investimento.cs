namespace InvestimentosJwt.Domain.Entities;
/// <summary>
/// Representa um investimento realizado por um cliente.
/// </summary>
public class Investimento
{
    /// <summary>
    /// Identificador do investimento.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador do cliente.
    /// </summary>
    public int ClienteId { get; set; }

    /// <summary>
    /// Tipo de investimento (Ex: CDB, Fundo, Tesouro).
    /// </summary>
    public string Tipo { get; set; }

    /// <summary>
    /// Valor investido.
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Rentabilidade esperada ou aplicada.
    /// </summary>
    public decimal Rentabilidade { get; set; }

    /// <summary>
    /// Data do investimento.
    /// </summary>
    public DateTime Data { get; set; }
}

