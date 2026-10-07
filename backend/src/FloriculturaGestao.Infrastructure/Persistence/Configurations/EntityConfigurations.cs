using FloriculturaGestao.Domain.Auditoria;
using FloriculturaGestao.Domain.Clientes;
using FloriculturaGestao.Domain.Contas;
using FloriculturaGestao.Domain.Estoque;
using FloriculturaGestao.Domain.Fornecedores;
using FloriculturaGestao.Domain.Produtos;
using FloriculturaGestao.Domain.Usuarios;
using FloriculturaGestao.Domain.Vendas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FloriculturaGestao.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nome).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Telefone).HasMaxLength(20);
        builder.Property(c => c.Cpf).HasMaxLength(14);
        builder.Property(c => c.Endereco).HasMaxLength(500);
        builder.Property(c => c.Observacoes).HasMaxLength(1000);
        builder.HasIndex(c => c.Email).IsUnique();
        builder.HasIndex(c => c.Cpf).IsUnique().HasFilter("\"Cpf\" IS NOT NULL");
        builder.Ignore(c => c.HistoricoVendas);
        builder.Ignore(c => c.Eventos);
    }
}

public class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(EntityTypeBuilder<Fornecedor> builder)
    {
        builder.ToTable("fornecedores");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.RazaoSocial).HasMaxLength(300).IsRequired();
        builder.Property(f => f.NomeFantasia).HasMaxLength(300);
        builder.Property(f => f.Cnpj).HasMaxLength(18).IsRequired();
        builder.Property(f => f.Telefone).HasMaxLength(20);
        builder.Property(f => f.Email).HasMaxLength(200);
        builder.Property(f => f.Endereco).HasMaxLength(500);
        builder.HasIndex(f => f.Cnpj).IsUnique();
        builder.Ignore(f => f.Eventos);
    }
}

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("produtos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nome).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Descricao).HasMaxLength(1000);
        builder.Property(p => p.Sku).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Categoria).HasConversion<string>().HasMaxLength(30);
        builder.Property(p => p.PrecoCompra).HasPrecision(18, 2);
        builder.Property(p => p.PrecoVenda).HasPrecision(18, 2);
        builder.Property(p => p.UnidadeMedida).HasMaxLength(10);
        builder.HasIndex(p => p.Sku).IsUnique();
        builder.Ignore(p => p.Eventos);
    }
}

public class EstoqueItemConfiguration : IEntityTypeConfiguration<EstoqueItem>
{
    public void Configure(EntityTypeBuilder<EstoqueItem> builder)
    {
        builder.ToTable("estoque_itens");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Localizacao).HasMaxLength(100);
        builder.HasIndex(e => e.ProdutoId).IsUnique();
        builder.HasMany(e => e.Movimentacoes).WithOne().HasForeignKey(m => m.EstoqueItemId);
        builder.Ignore(e => e.Eventos);
    }
}

public class MovimentacaoEstoqueConfiguration : IEntityTypeConfiguration<MovimentacaoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoque> builder)
    {
        builder.ToTable("movimentacoes_estoque");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Tipo).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Observacao).HasMaxLength(500);
        builder.Ignore(m => m.Eventos);
    }
}

public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("vendas");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(v => v.ValorTotal).HasPrecision(18, 2);
        builder.Property(v => v.Desconto).HasPrecision(18, 2);
        builder.Property(v => v.Observacoes).HasMaxLength(1000);
        builder.HasMany(v => v.Itens).WithOne().HasForeignKey(i => i.VendaId);
        builder.Ignore(v => v.Eventos);
    }
}

public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVenda>
{
    public void Configure(EntityTypeBuilder<ItemVenda> builder)
    {
        builder.ToTable("itens_venda");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.NomeProduto).HasMaxLength(200);
        builder.Property(i => i.PrecoUnitario).HasPrecision(18, 2);
        builder.Ignore(i => i.Eventos);
    }
}

public class CaixaConfiguration : IEntityTypeConfiguration<Domain.Caixa.Caixa>
{
    public void Configure(EntityTypeBuilder<Domain.Caixa.Caixa> builder)
    {
        builder.ToTable("caixas");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.SaldoInicial).HasPrecision(18, 2);
        builder.Property(c => c.SaldoFinal).HasPrecision(18, 2);
        builder.Property(c => c.OperadorId).HasMaxLength(100);
        builder.HasMany(c => c.Movimentacoes).WithOne().HasForeignKey(m => m.CaixaId);
        builder.Ignore(c => c.Eventos);
    }
}

public class MovimentacaoCaixaConfiguration : IEntityTypeConfiguration<Domain.Caixa.MovimentacaoCaixa>
{
    public void Configure(EntityTypeBuilder<Domain.Caixa.MovimentacaoCaixa> builder)
    {
        builder.ToTable("movimentacoes_caixa");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Tipo).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Valor).HasPrecision(18, 2);
        builder.Property(m => m.Descricao).HasMaxLength(500);
        builder.Ignore(m => m.Eventos);
    }
}

public class ContaPagarConfiguration : IEntityTypeConfiguration<ContaPagar>
{
    public void Configure(EntityTypeBuilder<ContaPagar> builder)
    {
        builder.ToTable("contas_pagar");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Descricao).HasMaxLength(500).IsRequired();
        builder.Property(c => c.Valor).HasPrecision(18, 2);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Observacoes).HasMaxLength(1000);
        builder.Ignore(c => c.Eventos);
    }
}

public class ContaReceberConfiguration : IEntityTypeConfiguration<ContaReceber>
{
    public void Configure(EntityTypeBuilder<ContaReceber> builder)
    {
        builder.ToTable("contas_receber");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Descricao).HasMaxLength(500).IsRequired();
        builder.Property(c => c.Valor).HasPrecision(18, 2);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Observacoes).HasMaxLength(1000);
        builder.Ignore(c => c.Eventos);
    }
}

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Nome).HasMaxLength(200).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(200).IsRequired();
        builder.Property(u => u.SenhaHash).HasMaxLength(500).IsRequired();
        builder.Property(u => u.Perfil).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Ignore(u => u.Eventos);
    }
}

public class RegistroAuditoriaConfiguration : IEntityTypeConfiguration<RegistroAuditoria>
{
    public void Configure(EntityTypeBuilder<RegistroAuditoria> builder)
    {
        builder.ToTable("registros_auditoria");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Tabela).HasMaxLength(100).IsRequired();
        builder.Property(r => r.RegistroId).HasMaxLength(50).IsRequired();
        builder.Property(r => r.Acao).HasMaxLength(20).IsRequired();
        builder.Property(r => r.RealizadoPor).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Detalhes).HasColumnType("text");
    }
}
