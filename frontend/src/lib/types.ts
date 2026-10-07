// Clientes
export interface ClienteDto {
  id: string;
  nome: string;
  email: string;
  telefone: string;
  cpf: string | null;
  endereco: string | null;
  observacoes: string | null;
  ativo: boolean;
  criadoEm: string;
}

export interface CriarClienteRequest {
  nome: string;
  email: string;
  telefone: string;
  cpf?: string | null;
  endereco?: string | null;
  observacoes?: string | null;
}

export interface AtualizarClienteRequest extends CriarClienteRequest {
  id: string;
}

// Produtos
export interface ProdutoDto {
  id: string;
  nome: string;
  descricao: string;
  sku: string;
  categoria: string;
  precoCusto: number;
  precoVenda: number;
  ativo: boolean;
  criadoEm: string;
}

export interface CriarProdutoRequest {
  nome: string;
  descricao: string;
  sku: string;
  categoria: number;
  precoCusto: number;
  precoVenda: number;
}

// Estoque
export interface EstoqueItemDto {
  id: string;
  produtoId: string;
  produtoNome: string;
  quantidade: number;
  quantidadeMinima: number;
  localizacao: string;
  dataValidade: string | null;
  estaBaixo: boolean;
  estaVencido: boolean;
}

export interface RegistrarEntradaEstoqueRequest {
  produtoId: string;
  quantidade: number;
  quantidadeMinima: number;
  localizacao: string;
  dataValidade: string | null;
  observacao: string;
}

// Vendas
export interface VendaDto {
  id: string;
  clienteId: string;
  clienteNome: string;
  itens: ItemVendaDto[];
  subtotal: number;
  desconto: number;
  total: number;
  status: string;
  observacoes: string;
  criadoEm: string;
}

export interface ItemVendaDto {
  produtoId: string;
  produtoNome: string;
  quantidade: number;
  precoUnitario: number;
  subtotal: number;
}

export interface RegistrarVendaRequest {
  clienteId: string;
  caixaId: string;
  itens: { produtoId: string; quantidade: number }[];
  desconto: number;
  observacoes: string;
}

// Caixa
export interface CaixaDto {
  id: string;
  operador: string;
  dataAbertura: string;
  dataFechamento: string | null;
  saldoInicial: number;
  saldoFinal: number | null;
  totalEntradas: number;
  totalSaidas: number;
  status: string;
}

// Contas
export interface ContaPagarDto {
  id: string;
  descricao: string;
  valor: number;
  dataVencimento: string;
  dataPagamento: string | null;
  fornecedorId: string;
  fornecedorNome: string;
  status: string;
}

export interface ContaReceberDto {
  id: string;
  descricao: string;
  valor: number;
  dataVencimento: string;
  dataRecebimento: string | null;
  clienteId: string;
  clienteNome: string;
  status: string;
}

// Auth
export interface LoginRequest {
  email: string;
  senha: string;
}

export interface LoginResponse {
  token: string;
  nome: string;
  email: string;
  role: string;
  expiracao: string;
}

// Usuarios
export interface UsuarioDto {
  id: string;
  nome: string;
  email: string;
  perfil: string;
  ativo: boolean;
  criadoEm: string;
  atualizadoEm: string | null;
}

export interface CriarUsuarioRequest {
  nome: string;
  email: string;
  senha: string;
  perfil: string;
}

export interface AtualizarUsuarioRequest {
  id: string;
  nome: string;
  email: string;
  perfil: string;
}

// Result wrapper
export interface ResultadoOperacao<T> {
  sucesso: boolean;
  dados: T | null;
  mensagem: string;
  erros: string[];
}
