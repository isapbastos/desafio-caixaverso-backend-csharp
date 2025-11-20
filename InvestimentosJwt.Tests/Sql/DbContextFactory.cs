using InvestimentosJwt.Infra.Data.Sql;
using Microsoft.EntityFrameworkCore;

namespace InvestimentosJwt.Tests.Sql;
public static class DbContextFactory
{
    public static AppDbContext CreateContext(string dbName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
