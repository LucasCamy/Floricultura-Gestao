using FloriculturaGestao.Domain.Caixa;
using FloriculturaGestao.Domain.Clientes;
using FloriculturaGestao.Domain.Common;
using FloriculturaGestao.Domain.Contas;
using FloriculturaGestao.Domain.Estoque;
using FloriculturaGestao.Domain.Fornecedores;
using FloriculturaGestao.Domain.Produtos;
using FloriculturaGestao.Domain.Usuarios;
using FloriculturaGestao.Domain.Vendas;
using FloriculturaGestao.Infrastructure.Persistence;
using FloriculturaGestao.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FloriculturaGestao.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' not found.");

        services.AddDbContext<FloriculturaDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<FloriculturaDbContext>());

        // Repositories
        services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
        services.AddScoped<IFornecedorRepositorio, FornecedorRepositorio>();
        services.AddScoped<IProdutoRepositorio, ProdutoRepositorio>();
        services.AddScoped<IEstoqueRepositorio, EstoqueRepositorio>();
        services.AddScoped<IVendaRepositorio, VendaRepositorio>();
        services.AddScoped<ICaixaRepositorio, CaixaRepositorio>();
        services.AddScoped<IContaPagarRepositorio, ContaPagarRepositorio>();
        services.AddScoped<IContaReceberRepositorio, ContaReceberRepositorio>();
        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

        return services;
    }
}
