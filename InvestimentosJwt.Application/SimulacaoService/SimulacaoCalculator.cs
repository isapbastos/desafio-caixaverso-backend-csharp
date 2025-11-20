namespace InvestimentosJwt.Application.SimulacaoService;

/// <summary>
/// Responsável por realizar os cálculos financeiros utilizados nas simulações de investimento.
/// Implementa o cálculo do valor final e da rentabilidade efetiva considerando juros compostos.
/// </summary>
public class SimulacaoCalculator : ISimulacaoCalculator
{
    /// <summary>
    /// Calcula o valor final de um investimento utilizando juros compostos.
    /// </summary>
    /// <param name="valorInicial">Valor inicial investido.</param>
    /// <param name="rentabilidadeAnual">Rentabilidade anual em formato decimal (ex.: 0.12 = 12%).</param>
    /// <param name="prazoMeses">Prazo total da aplicação em meses.</param>
    /// <returns>
    /// O valor final arredondado para 2 casas decimais.  
    /// Retorna <c>0</c> caso algum parâmetro seja inválido.
    /// </returns>
    public decimal CalcularValorFinal(decimal valorInicial, decimal rentabilidadeAnual, int prazoMeses)
    {
        if (valorInicial <= 0 || rentabilidadeAnual <= 0 || prazoMeses <= 0)
            return 0;

        // Converte a rentabilidade anual para mensal utilizando base exponencial
        decimal taxaMensal = (decimal)Math.Pow((double)(1 + rentabilidadeAnual), 1.0 / 12.0) - 1;

        // Aplica a fórmula de juros compostos
        decimal valorFinal = valorInicial * (decimal)Math.Pow((double)(1 + taxaMensal), prazoMeses);

        return Math.Round(valorFinal, 2);
    }

    /// <summary>
    /// Calcula a rentabilidade efetiva de um investimento no período informado.
    /// </summary>
    /// <param name="rentabilidadeAnual">Rentabilidade anual em formato decimal.</param>
    /// <param name="prazoMeses">Quantidade de meses do investimento.</param>
    /// <returns>
    /// A rentabilidade efetiva do período em formato decimal (ex.: 0.15 = 15%).  
    /// Retorna <c>0</c> caso algum parâmetro seja inválido.
    /// </returns>
    public decimal CalcularRentabilidadeEfetiva(decimal rentabilidadeAnual, int prazoMeses)
    {
        if (rentabilidadeAnual <= 0 || prazoMeses <= 0)
            return 0;

        // Converte a rentabilidade anual para mensal
        decimal taxaMensal = (decimal)Math.Pow((double)(1 + rentabilidadeAnual), 1.0 / 12.0) - 1;

        // Calcula o fator de crescimento
        decimal fator = (decimal)Math.Pow((double)(1 + taxaMensal), prazoMeses);

        return Math.Round(fator - 1, 4);
    }
}
