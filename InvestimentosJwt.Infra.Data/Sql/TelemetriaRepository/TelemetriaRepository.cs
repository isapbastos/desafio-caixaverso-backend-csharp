using InvestimentosJwt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestimentosJwt.Infra.Data.Sql.TelemetriaRepository;
public class TelemetriaRepository : ITelemetriaRepository
{
    private readonly AppDbContext _context;

    public TelemetriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarRegistro(TelemetriaRegistro registro)
    {
        _context.TelemetriaRegistros.Add(registro);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TelemetriaRegistro>> ObterDadosPorPeriodo(DateTime inicio, DateTime fim)
    {
        var resultado = await _context.TelemetriaRegistros
            .Where(r => r.DataRegistro >= inicio && r.DataRegistro <= fim)
            .ToListAsync();
        return resultado;
    }
}