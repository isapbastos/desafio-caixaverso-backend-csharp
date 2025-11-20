using InvestimentosJwt.Application.InvestimentoService;
using InvestimentosJwt.Application.PerfilService;
using InvestimentosJwt.Application.ProdutoService;
using InvestimentosJwt.Application.SimulacaoService;
using InvestimentosJwt.Application.TelemetriaService;
using InvestimentosJwt.Infra.Data.Sql;
using InvestimentosJwt.Infra.Data.Sql.InvestimentoRepository;
using InvestimentosJwt.Infra.Data.Sql.ProdutoRepository;
using InvestimentosJwt.Infra.Data.Sql.SimulacaoRepository;
using InvestimentosJwt.Infra.Data.Sql.TelemetriaRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ------------------------------
// DATABASE
// ------------------------------

// Caminho do banco de dados
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "data", "app.db");

// Cria a pasta se não existir
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

// Configura DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}",
    x => x.MigrationsAssembly("InvestimentosJwt.Infra.Data")
    ));
//builder.Services.AddDbContext<AppDbContext>(options =>
//options.UseSqlite(
//        configuration.GetConnectionString("DefaultConnection"),
//        x => x.MigrationsAssembly("InvestimentosJwt.Infra.Data")
//    ));

// ------------------------------
// DEPENDENCY INJECTION
// ------------------------------
builder.Services.AddScoped<IInvestimentoRepository, InvestimentoRepository>();
builder.Services.AddScoped<IInvestimentoService, InvestimentoService>();

builder.Services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();
builder.Services.AddScoped<ISimulacaoService, SimulacaoService>();

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

builder.Services.AddScoped<ITelemetriaRepository, TelemetriaRepository>();
builder.Services.AddScoped<ITelemetriaService, TelemetriaService>();

builder.Services.AddScoped<ISimulacaoCalculator, SimulacaoCalculator>();
builder.Services.AddScoped<IPerfilService, PerfilService>();

builder.Services.AddControllers();

// ------------------------------
// JWT CONFIG
// ------------------------------
var jwtSection = configuration.GetSection("Jwt");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = signingKey
        };
    });

// ------------------------------
// SWAGGER + JWT
// ------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Investimentos API",
        Version = "v1"
    });

    // Carregar documentação XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    // JWT no Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Insira apenas o token JWT (sem 'Bearer ')",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// ------------------------------
// BUILD APP
// ------------------------------
var app = builder.Build();

// DB seed
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    ctx.Database.EnsureCreated();
    DataSeed.Seed(ctx);
}

// ------------------------------
// MIDDLEWARE
// ------------------------------
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
