using InvestimentosCaixa.Api.Controllers;
using InvestimentosCaixa.Api.DTOs;
using InvestimentosJwt.Domain.Entities;
using InvestimentosJwt.Infra.Data.Sql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InvestimentosJwt.Tests.Controllers;
public class AuthControllerTests
{
    private AppDbContext CriarDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private IConfiguration CriarConfiguracaoJwt()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:Key", "12345678901234567890123456789012" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" },
            { "Jwt:ExpiresMinutes", "60" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    // -------------------------
    //  TESTE: Registro OK
    // -------------------------
    [Fact]
    public async Task Register_DeveRetornarOk_QuandoUsuarioNovo()
    {
        var db = CriarDbContext();
        var config = CriarConfiguracaoJwt();
        var controller = new AuthController(db, config);

        var dto = new RegisterDto
        {
            Email = "novo@email.com",
            Password = "123"
        };

        var resultado = await controller.Register(dto);

        Assert.IsType<OkObjectResult>(resultado);
        Assert.Single(db.Users);
    }

    // -------------------------
    //  TESTE: Registro duplicado
    // -------------------------
    [Fact]
    public async Task Register_DeveRetornarBadRequest_QuandoEmailDuplicado()
    {
        var db = CriarDbContext();
        db.Users.Add(new User { Email = "existe@email.com", Password = "123" });
        db.SaveChanges();

        var controller = new AuthController(db, CriarConfiguracaoJwt());

        var dto = new RegisterDto
        {
            Email = "existe@email.com",
            Password = "senha"
        };

        var resultado = await controller.Register(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Email já registrado.", badRequest.Value);
    }

    // -------------------------
    //  TESTE: Login OK
    // -------------------------
    [Fact]
    public async Task Login_DeveRetornarToken_QuandoCredenciaisValidas()
    {
        var db = CriarDbContext();
        db.Users.Add(new User { Email = "teste@email.com", Password = "123" });
        db.SaveChanges();

        var controller = new AuthController(db, CriarConfiguracaoJwt());

        var dto = new LoginDto
        {
            Email = "teste@email.com",
            Password = "123"
        };

        var resultado = await controller.Login(dto);

        var ok = Assert.IsType<OkObjectResult>(resultado);

        var resp = Assert.IsType<AuthResponseDto>(ok.Value);

        Assert.False(string.IsNullOrEmpty(resp.Token));
    }

    // -------------------------
    // TESTE: Login inválido
    // -------------------------
    [Fact]
    public async Task Login_DeveRetornarUnauthorized_QuandoCredenciaisInvalidas()
    {
        var db = CriarDbContext();
        db.Users.Add(new User { Email = "teste@email.com", Password = "123" });
        db.SaveChanges();

        var controller = new AuthController(db, CriarConfiguracaoJwt());

        var dto = new LoginDto
        {
            Email = "teste@email.com",
            Password = "senha_errada"
        };

        var resultado = await controller.Login(dto);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(resultado);
        Assert.Equal("Invalid credentials.", unauthorized.Value);
    }
}
