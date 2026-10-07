"use client";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
    Card,
    CardContent,
    CardHeader
} from "@/components/ui/card";
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table";
import { Textarea } from "@/components/ui/textarea";
import api from "@/lib/api";
import type { AtualizarClienteRequest, ClienteDto, CriarClienteRequest } from "@/lib/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Pencil, Plus, Search, Trash2 } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

interface ClienteForm {
  id?: string;
  nome: string;
  email: string;
  telefone: string;
  cpf: string;
  endereco: string;
  observacoes: string;
}

const formInicial: ClienteForm = {
  nome: "",
  email: "",
  telefone: "",
  cpf: "",
  endereco: "",
  observacoes: "",
};

const somenteDigitos = (valor: string) => valor.replace(/\D/g, "");

const mascararCpf = (valor: string) => {
  const digitos = somenteDigitos(valor).slice(0, 11);
  return digitos
    .replace(/(\d{3})(\d)/, "$1.$2")
    .replace(/(\d{3})(\d)/, "$1.$2")
    .replace(/(\d{3})(\d{1,2})$/, "$1-$2");
};

const mascararTelefone = (valor: string) => {
  const digitos = somenteDigitos(valor).slice(0, 11);
  if (digitos.length <= 10) {
    return digitos
      .replace(/(\d{2})(\d)/, "($1) $2")
      .replace(/(\d{4})(\d)/, "$1-$2");
  }

  return digitos
    .replace(/(\d{2})(\d)/, "($1) $2")
    .replace(/(\d{5})(\d)/, "$1-$2");
};

const cpfValido = (cpf: string) => {
  const digitos = somenteDigitos(cpf);
  if (digitos.length !== 11) return false;
  if (/^(\d)\1+$/.test(digitos)) return false;

  const calcularDigito = (base: string, pesoInicial: number) => {
    const soma = base
      .split("")
      .reduce((acumulador, numero, indice) => acumulador + Number(numero) * (pesoInicial - indice), 0);
    const resto = (soma * 10) % 11;
    return resto === 10 ? 0 : resto;
  };

  const primeiroDigito = calcularDigito(digitos.slice(0, 9), 10);
  const segundoDigito = calcularDigito(digitos.slice(0, 10), 11);

  return primeiroDigito === Number(digitos[9]) && segundoDigito === Number(digitos[10]);
};

const telefoneValido = (telefone: string) => {
  const tamanho = somenteDigitos(telefone).length;
  return tamanho === 10 || tamanho === 11;
};

export default function ClientesPage() {
  const queryClient = useQueryClient();
  const [search, setSearch] = useState("");
  const [dialogOpen, setDialogOpen] = useState(false);
  const [modoEdicao, setModoEdicao] = useState(false);
  const [form, setForm] = useState<ClienteForm>(formInicial);

  const { data: clientes, isLoading } = useQuery({
    queryKey: ["clientes"],
    queryFn: () => api.get<ClienteDto[]>("/clientes").then((r) => r.data),
  });

  const criarMutation = useMutation({
    mutationFn: (data: CriarClienteRequest) => api.post("/clientes", data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["clientes"] });
      setDialogOpen(false);
      setForm(formInicial);
      toast.success("Cliente cadastrado com sucesso!");
    },
    onError: () => toast.error("Erro ao cadastrar cliente"),
  });

  const atualizarMutation = useMutation({
    mutationFn: (data: AtualizarClienteRequest) => api.put(`/clientes/${data.id}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["clientes"] });
      setDialogOpen(false);
      setModoEdicao(false);
      setForm(formInicial);
      toast.success("Cliente atualizado com sucesso!");
    },
    onError: () => toast.error("Erro ao atualizar cliente"),
  });

  const excluirMutation = useMutation({
    mutationFn: (id: string) => api.delete(`/clientes/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["clientes"] });
      toast.success("Cliente desativado com sucesso!");
    },
    onError: () => toast.error("Erro ao desativar cliente"),
  });

  const abrirNovoCliente = () => {
    setModoEdicao(false);
    setForm(formInicial);
    setDialogOpen(true);
  };

  const abrirEdicaoCliente = (cliente: ClienteDto) => {
    setModoEdicao(true);
    setForm({
      id: cliente.id,
      nome: cliente.nome,
      email: cliente.email,
      telefone: mascararTelefone(cliente.telefone),
      cpf: mascararCpf(cliente.cpf ?? ""),
      endereco: cliente.endereco ?? "",
      observacoes: cliente.observacoes ?? "",
    });
    setDialogOpen(true);
  };

  const confirmarExclusao = (cliente: ClienteDto) => {
    if (!cliente.ativo) {
      toast.info("Cliente já está inativo.");
      return;
    }

    if (window.confirm(`Deseja desativar o cliente ${cliente.nome}?`)) {
      excluirMutation.mutate(cliente.id);
    }
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!telefoneValido(form.telefone)) {
      toast.error("Telefone inválido. Informe 10 ou 11 dígitos.");
      return;
    }

    if (form.cpf && !cpfValido(form.cpf)) {
      toast.error("CPF inválido.");
      return;
    }

    const payloadBase = {
      nome: form.nome.trim(),
      email: form.email.trim(),
      telefone: somenteDigitos(form.telefone),
      cpf: somenteDigitos(form.cpf) || null,
      endereco: form.endereco.trim() || null,
      observacoes: form.observacoes.trim() || null,
    };

    if (modoEdicao && form.id) {
      atualizarMutation.mutate({ id: form.id, ...payloadBase });
      return;
    }

    criarMutation.mutate(payloadBase);
  };

  const filtered = clientes?.filter(
    (c) =>
      c.nome.toLowerCase().includes(search.toLowerCase()) ||
      c.email.toLowerCase().includes(search.toLowerCase()) ||
      (c.cpf ?? "").includes(search)
  );

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold">Clientes</h1>
          <p className="text-muted-foreground">Gerenciamento de clientes</p>
        </div>
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogTrigger render={<Button />} onClick={abrirNovoCliente}>
              <Plus className="mr-2 h-4 w-4" />
              Novo Cliente
          </DialogTrigger>
          <DialogContent className="max-w-lg">
            <DialogHeader>
              <DialogTitle>{modoEdicao ? "Editar Cliente" : "Cadastrar Cliente"}</DialogTitle>
            </DialogHeader>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="nome">Nome</Label>
                <Input id="nome" value={form.nome} onChange={(e) => setForm({ ...form, nome: e.target.value })} required />
              </div>
              <div className="space-y-2">
                <Label htmlFor="email">Email</Label>
                <Input id="email" type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required />
              </div>
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="space-y-2">
                  <Label htmlFor="telefone">Telefone</Label>
                  <Input
                    id="telefone"
                    value={form.telefone}
                    placeholder="(11) 99999-9999"
                    onChange={(e) => setForm({ ...form, telefone: mascararTelefone(e.target.value) })}
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="cpf">CPF</Label>
                  <Input
                    id="cpf"
                    value={form.cpf}
                    placeholder="000.000.000-00"
                    onChange={(e) => setForm({ ...form, cpf: mascararCpf(e.target.value) })}
                  />
                </div>
              </div>
              <div className="space-y-2">
                <Label htmlFor="endereco">Endereço</Label>
                <Input id="endereco" value={form.endereco} onChange={(e) => setForm({ ...form, endereco: e.target.value })} />
              </div>
              <div className="space-y-2">
                <Label htmlFor="observacoes">Observações</Label>
                <Textarea id="observacoes" value={form.observacoes} onChange={(e) => setForm({ ...form, observacoes: e.target.value })} />
              </div>
              <Button type="submit" className="w-full" disabled={criarMutation.isPending || atualizarMutation.isPending}>
                {criarMutation.isPending || atualizarMutation.isPending
                  ? "Salvando..."
                  : modoEdicao
                    ? "Salvar Alterações"
                    : "Cadastrar"}
              </Button>
            </form>
          </DialogContent>
        </Dialog>
      </div>

      <Card>
        <CardHeader>
          <div className="flex items-center gap-2">
            <Search className="h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Buscar por nome, email ou CPF..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="max-w-sm"
            />
          </div>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <p className="text-muted-foreground">Carregando...</p>
          ) : (
            <>
              {/* Mobile cards */}
              <div className="md:hidden space-y-3">
                {filtered?.map((cliente) => (
                  <div key={cliente.id} className="rounded-lg border bg-background p-4 space-y-2">
                    <div className="flex items-start justify-between gap-2">
                      <div>
                        <p className="font-medium">{cliente.nome}</p>
                        <p className="text-sm text-muted-foreground">{cliente.email}</p>
                      </div>
                      <Badge variant={cliente.ativo ? "default" : "secondary"}>
                        {cliente.ativo ? "Ativo" : "Inativo"}
                      </Badge>
                    </div>
                    <div className="flex flex-wrap gap-x-4 gap-y-1 text-sm text-muted-foreground">
                      {cliente.telefone && <span>{mascararTelefone(cliente.telefone)}</span>}
                      {cliente.cpf && <span>{mascararCpf(cliente.cpf)}</span>}
                    </div>
                    <div className="flex gap-2 pt-1">
                      <Button type="button" size="sm" variant="outline" onClick={() => abrirEdicaoCliente(cliente)}>
                        <Pencil className="h-4 w-4 mr-1" /> Editar
                      </Button>
                      <Button
                        type="button"
                        size="sm"
                        variant="destructive"
                        disabled={!cliente.ativo || excluirMutation.isPending}
                        onClick={() => confirmarExclusao(cliente)}
                      >
                        <Trash2 className="h-4 w-4 mr-1" /> Remover
                      </Button>
                    </div>
                  </div>
                ))}
                {filtered?.length === 0 && (
                  <p className="text-center text-muted-foreground py-4">Nenhum cliente encontrado</p>
                )}
              </div>
              {/* Desktop table */}
              <div className="hidden md:block overflow-x-auto">
              <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Nome</TableHead>
                  <TableHead>Email</TableHead>
                  <TableHead>Telefone</TableHead>
                  <TableHead>CPF</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Ações</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filtered?.map((cliente) => (
                  <TableRow key={cliente.id}>
                    <TableCell className="font-medium">{cliente.nome}</TableCell>
                    <TableCell>{cliente.email}</TableCell>
                    <TableCell>{cliente.telefone}</TableCell>
                    <TableCell>{cliente.cpf ? mascararCpf(cliente.cpf) : "-"}</TableCell>
                    <TableCell>
                      <Badge variant={cliente.ativo ? "default" : "secondary"}>
                        {cliente.ativo ? "Ativo" : "Inativo"}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <Button type="button" size="sm" variant="outline" onClick={() => abrirEdicaoCliente(cliente)}>
                          <Pencil className="h-4 w-4" />
                        </Button>
                        <Button
                          type="button"
                          size="sm"
                          variant="destructive"
                          disabled={!cliente.ativo || excluirMutation.isPending}
                          onClick={() => confirmarExclusao(cliente)}
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
                {filtered?.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={6} className="text-center text-muted-foreground">
                      Nenhum cliente encontrado
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
            </div>
            </>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
