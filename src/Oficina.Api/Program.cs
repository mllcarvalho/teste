using DotNetEnv;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Oficina.Atendimento.Application.IServices;
using Oficina.Atendimento.Application.Services;
using Oficina.Atendimento.Domain.IRepository;
using Oficina.Atendimento.Infrastructure.Data;
using Oficina.Atendimento.Infrastructure.Repositories;
using Oficina.Common.Application.IServices;
using Oficina.Common.Application.Services;
using Oficina.Common.Domain.IRepository;
using Oficina.Common.Domain.ISecurity;
using Oficina.Common.Domain.IHelper;
using Oficina.Common.Infrastructure.Data;
using Oficina.Common.Infrastructure.Repositories;
using Oficina.Common.Infrastructure.Security;
using Oficina.Common.Infrastructure.Helper;
using Oficina.Estoque.Application.IServices;
using Oficina.Estoque.Application.Services;
using Oficina.Estoque.Domain.IRepositories;
using Oficina.Estoque.Domain.Services;
using Oficina.Estoque.Infrastructure.Data;
using Oficina.Estoque.Infrastructure.Repositories;
using System.Text;

// Detectar se está rodando em container Docker
var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// Tentar carregar o .env de diferentes localizações
var possibleEnvPaths = new[]
{
    "/app/.env", // Container Docker
    ".env", // Raiz do projeto
    Path.Combine(Directory.GetCurrentDirectory(), ".env"), // Diretório atual
    Path.Combine(
        Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? "")?.FullName ?? "",
        ".env"
    ) // Duas pastas acima (desenvolvimento local)
};

foreach (var envPath in possibleEnvPaths)
{
    if (File.Exists(envPath))
    {
        Env.Load(envPath);
        Console.WriteLine($"[INFO] Arquivo .env carregado de: {envPath}");
        break;
    }
}

var builder = WebApplication.CreateBuilder(args);

// Obter connection string do .env
var connectionString = Env.GetString("CONNECTIONSTRINGS__DEFAULTCONNECTION");

/// Se estiver em container, substituir localhost por postgres
if (isRunningInContainer && !string.IsNullOrEmpty(connectionString))
    connectionString = connectionString.Replace("Host=localhost", "Host=postgres");

var isTesting = builder.Environment.IsEnvironment("Testing");

// =======================================
// Injecao de Dependencias
// =======================================
#region Atendimento
// Repositories
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IOrdemDeServicoRepository, OrdemRepository>();
builder.Services.AddScoped<IServicoRepository, ServicoRepository>();
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();
builder.Services.AddScoped<IOrcamentoRepository, OrcamentoRepository>();

// App Services
builder.Services.AddScoped<IOrdemServicoAppService, OrdemServicoAppService>();
builder.Services.AddScoped<IVeiculoAppService, VeiculoAppService>();
builder.Services.AddScoped<IClienteAppService, ClienteAppService>();
builder.Services.AddScoped<IServicoAppService, ServicoAppService>();

#endregion

#region Estoque
// Repositories
builder.Services.AddScoped<IPecaRepository, PecaRepository>();
builder.Services.AddScoped<IEstoqueRepository, EstoqueRepository>();

// App Services
builder.Services.AddScoped<IEstoqueAppService, EstoqueAppService>();
builder.Services.AddScoped<IPecaAppService, PecaAppService>();

// Domain Services
builder.Services.AddScoped<EstoqueDomainService>();

#endregion

#region Common
//Common - Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IEmailHelper, EmailHelper>();

// Common - App Services
builder.Services.AddScoped<IAuthAppService, AuthAppService>();

#endregion

# region DbContexts
if (isTesting)
{
    builder.Services.AddDbContext<AtendimentoDbContext>(options => options.UseInMemoryDatabase("AtendimentoTestDb"));
    builder.Services.AddDbContext<EstoqueDbContext>(options => options.UseInMemoryDatabase("EstoqueTestDb"));
    builder.Services.AddDbContext<CommonDbContext>(options => options.UseInMemoryDatabase("CommonTestDb"));
}
else
{
    builder.Services.AddDbContext<AtendimentoDbContext>(options => options.UseNpgsql(connectionString));
    builder.Services.AddDbContext<EstoqueDbContext>(options => options.UseNpgsql(connectionString));
    builder.Services.AddDbContext<CommonDbContext>(options => options.UseNpgsql(connectionString));
}
#endregion

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var bearer = "Bearer";
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Oficina API", Version = "v1" });

        // Configuração do Bearer Token
        c.AddSecurityDefinition(bearer, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "JWT Authorization header usando o Bearer. Ex: 'Bearer {seu_token}'",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = bearer
        });

        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = bearer
                    }
                },
                new string[] {}
            }
        });
    });
builder.Services.AddOpenApi();

var jwtKey = Env.GetString("JWT__KEY");
var jwtIssuer = Env.GetString("JWT__ISSUER");
var jwtAudience = Env.GetString("JWT__AUDIENCE");

builder.Services.AddAuthentication(bearer)
    .AddJwtBearer(bearer, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RolePolicy", policy =>
        policy.RequireAssertion(context =>
        {
            // Se o usuário for Admin, passa direto
            if (context.User.IsInRole("Admin"))
                return true;

            // Recupera as roles permitidas da rota
            var rolesClaim = context.Resource as IEnumerable<string>;
            if (rolesClaim == null)
                return false;

            return rolesClaim.Any(r => context.User.IsInRole(r));
        }));
});

var app = builder.Build();

// Migrations para os contextos (somente quando relacional)
using (var scope = app.Services.CreateScope())
{
    var atendimentoDb = scope.ServiceProvider.GetRequiredService<AtendimentoDbContext>();
    if (atendimentoDb.Database.IsRelational())
        atendimentoDb.Database.Migrate();

    var estoqueDb = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
    if (estoqueDb.Database.IsRelational())
        estoqueDb.Database.Migrate();

    var commonDb = scope.ServiceProvider.GetRequiredService<CommonDbContext>();
    if (commonDb.Database.IsRelational())
        commonDb.Database.Migrate();
}

// Rodar seeder de users
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        DataSeeder.SeedUsers(scope.ServiceProvider);
    }
}
// Middleware global de tratamento de erros
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        context.Response.StatusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var errorObj = new { error = exception?.Message };
        var json = System.Text.Json.JsonSerializer.Serialize(errorObj);
        await context.Response.WriteAsync(json);
    });
});

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();