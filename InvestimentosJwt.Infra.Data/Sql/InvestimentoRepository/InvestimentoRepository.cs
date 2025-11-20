using InvestimentosJwt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestimentosJwt.Infra.Data.Sql.InvestimentoRepository;
/// <summary>
/// Repositório para persistência e consulta de investimentos.
/// </summary>
public class InvestimentoRepository : IInvestimentoRepository
{
    private readonly AppDbContext _context;
    public InvestimentoRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtém histórico de investimentos de um cliente.
    /// </summary>
    /// <param name="clienteId">Id do cliente</param>
    /// <returns>Lista de investimentos</returns>
    public async Task<List<Investimento>> ObterPorCliente(int clienteId)
    {
        return await _context.Investimentos
                             .Where(i => i.ClienteId == clienteId)
                             .OrderByDescending(i => i.Data)
                             .ToListAsync();
    }

    /// <summary>
    /// Adiciona um investimento ao histórico.
    /// </summary>
    /// <param name="investimento">Investimento a ser adicionado</param>
    public async Task AdicionarAsync(Investimento investimento)
    {
        _context.Investimentos.Add(investimento);
        await _context.SaveChangesAsync();
    }
}
