using System.Text;
using FloriculturaGestao.Application;
using FloriculturaGestao.Domain.Usuarios;
using FloriculturaGestao.Infrastructure;
using FloriculturaGestao.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();

// Auth
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"] ?? "FloriculturaChaveSecretaMuitoSegura2026!@#$%";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "FloriculturaGestao",
            ValidAudience = jwtSettings["Audience"] ?? "FloriculturaGestao",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Vendedor", policy => policy.RequireRole("Admin", "Vendedor"));
    options.AddPolicy("CaixaOperador", policy => policy.RequireRole("Admin", "Caixa"));
    options.AddPolicy("Estoquista", policy => policy.RequireRole("Admin", "Estoque"));
});

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Floricultura Gestão API",
        Version = "v1",
        Description = "API para gerenciamento de floricultura - Clientes, Produtos, Estoque, Caixa, Contas"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header. Exemplo: \"Bearer {token}\""
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var origins = (builder.Configuration["AllowedOrigins"] ?? "http://localhost:32400")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Run migrations in development or when explicitly enabled via configuration.
var applyMigrations = app.Configuration.GetValue<bool>("ApplyMigrations");
if (app.Environment.IsDevelopment() || applyMigrations)
{
    const int maxRetries = 10;
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FloriculturaDbContext>();
            if (applyMigrations)
            {
                await db.Database.EnsureCreatedAsync();
            }
            else
            {
                await db.Database.MigrateAsync();
            }

            // Ensure new tables exist for existing databases (EnsureCreatedAsync only runs on first init)
            await db.Database.ExecuteSqlRawAsync(@"
        CREATE TABLE IF NOT EXISTS usuarios (
            ""Id"" uuid NOT NULL PRIMARY KEY,
            ""Nome"" varchar(200) NOT NULL,
            ""Email"" varchar(200) NOT NULL,
            ""SenhaHash"" varchar(500) NOT NULL,
            ""Perfil"" varchar(20) NOT NULL,
            ""Ativo"" boolean NOT NULL DEFAULT true,
            ""CriadoEm"" timestamp with time zone NOT NULL,
            ""AtualizadoEm"" timestamp with time zone NULL
        );
        CREATE UNIQUE INDEX IF NOT EXISTS ix_usuarios_email ON usuarios (""Email"");

        CREATE TABLE IF NOT EXISTS registros_auditoria (
            ""Id"" uuid NOT NULL PRIMARY KEY,
            ""Tabela"" varchar(100) NOT NULL,
            ""RegistroId"" varchar(50) NOT NULL,
            ""Acao"" varchar(20) NOT NULL,
            ""RealizadoPor"" varchar(200) NOT NULL,
            ""RealizadoEm"" timestamp with time zone NOT NULL,
            ""Detalhes"" text NULL
        );
    ");

            // Create the first administrator only from deployment-provided credentials.
            var usuarioRepo = scope.ServiceProvider.GetRequiredService<IUsuarioRepositorio>();
            if (!await usuarioRepo.ExisteAdminAsync())
            {
                var adminEmail = Environment.GetEnvironmentVariable("BOOTSTRAP_ADMIN_EMAIL");
                var adminPassword = Environment.GetEnvironmentVariable("BOOTSTRAP_ADMIN_PASSWORD");
                if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
                    throw new InvalidOperationException("Set BOOTSTRAP_ADMIN_EMAIL and BOOTSTRAP_ADMIN_PASSWORD before first startup.");

                var adminName = Environment.GetEnvironmentVariable("BOOTSTRAP_ADMIN_NAME");
                var admin = Usuario.Criar(
                    nome: string.IsNullOrWhiteSpace(adminName) ? "Administrador" : adminName,
                    email: adminEmail,
                    senha: adminPassword,
                    perfil: PerfilUsuario.Admin
                );
                await usuarioRepo.AdicionarAsync(admin);
                var uow = scope.ServiceProvider.GetRequiredService<FloriculturaGestao.Domain.Common.IUnitOfWork>();
                await uow.SaveChangesAsync();
            }

            break; // success
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Tentativa {Attempt}/{Max} de conectar ao banco falhou: {Message}. Aguardando 5s...",
                attempt, maxRetries, ex.Message);
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Floricultura Gestão API v1"));
}

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
