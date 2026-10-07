"use client";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
    Card,
    CardContent,
    CardHeader,
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
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select";
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
import type { CriarProdutoRequest, ProdutoDto } from "@/lib/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Plus, Search } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

const categorias = [
  { value: "0", label: "Flor" },
  { value: "1", label: "Planta" },
  { value: "2", label: "Arranjo" },
  { value: "3", label: "Vaso" },
  { value: "4", label: "Insumo" },
  { value: "5", label: "Acessório" },
  { value: "6", label: "Outro" },
];

const getCategoriaLabel = (cat: string) =>
  categorias.find((c) => c.value === cat || c.label === cat)?.label ?? cat;

export default function ProdutosPage() {
  const queryClient = useQueryClient();
  const [search, setSearch] = useState("");
  const [dialogOpen, setDialogOpen] = useState(false);
  const [form, setForm] = useState<CriarProdutoRequest>({
    nome: "",
    descricao: "",
    sku: "",
    categoria: 0,
    precoCusto: 0,
    precoVenda: 0,
  });

  const { data: produtos, isLoading } = useQuery({
    queryKey: ["produtos"],
    queryFn: () => api.get<ProdutoDto[]>("/produtos").then((r) => r.data),
  });

  const criarMutation = useMutation({
    mutationFn: (data: CriarProdutoRequest) => api.post("/produtos", data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["produtos"] });
      setDialogOpen(false);
      setForm({ nome: "", descricao: "", sku: "", categoria: 0, precoCusto: 0, precoVenda: 0 });
      toast.success("Produto cadastrado com sucesso!");
    },
    onError: () => toast.error("Erro ao cadastrar produto"),
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    criarMutation.mutate(form);
  };

  const filtered = produtos?.filter(
    (p) =>
      p.nome.toLowerCase().includes(search.toLowerCase()) ||
      p.sku.toLowerCase().includes(search.toLowerCase())
  );

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(value);

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold">Produtos</h1>
          <p className="text-muted-foreground">Catálogo de flores, plantas e acessórios</p>
        </div>
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogTrigger render={<Button />}>
              <Plus className="mr-2 h-4 w-4" />
              Novo Produto
          </DialogTrigger>
          <DialogContent className="max-w-lg">
            <DialogHeader>
              <DialogTitle>Cadastrar Produto</DialogTitle>
            </DialogHeader>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="space-y-2">
                  <Label htmlFor="nome">Nome</Label>
                  <Input id="nome" value={form.nome} onChange={(e) => setForm({ ...form, nome: e.target.value })} required />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="sku">SKU</Label>
                  <Input id="sku" value={form.sku} onChange={(e) => setForm({ ...form, sku: e.target.value })} required />
                </div>
                <div className="space-y-2">
                  <Label>Categoria</Label>
                  <Select value={String(form.categoria)} onValueChange={(v: string | null) => setForm({ ...form, categoria: Number(v ?? '0') })}>
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder="Selecione" />
                    </SelectTrigger>
                    <SelectContent>
                      {categorias.map((c) => (
                        <SelectItem key={c.value} value={c.value}>{c.label}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div className="space-y-2">
                  <Label htmlFor="precoCusto">Preço de Custo</Label>
                  <Input id="precoCusto" type="number" step="0.01" min="0" value={form.precoCusto} onChange={(e) => setForm({ ...form, precoCusto: Number(e.target.value) })} required />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="precoVenda">Preço de Venda</Label>
                  <Input id="precoVenda" type="number" step="0.01" min="0" value={form.precoVenda} onChange={(e) => setForm({ ...form, precoVenda: Number(e.target.value) })} required />
                </div>
              </div>
              <div className="space-y-2">
                <Label htmlFor="descricao">Descrição</Label>
                <Textarea id="descricao" value={form.descricao} onChange={(e) => setForm({ ...form, descricao: e.target.value })} />
              </div>
              <Button type="submit" className="w-full" disabled={criarMutation.isPending}>
                {criarMutation.isPending ? "Salvando..." : "Cadastrar"}
              </Button>
            </form>
          </DialogContent>
        </Dialog>
      </div>

      <Card>
        <CardHeader>
          <div className="flex items-center gap-2">
            <Search className="h-4 w-4 text-muted-foreground" />
            <Input placeholder="Buscar por nome ou SKU..." value={search} onChange={(e) => setSearch(e.target.value)} className="max-w-sm" />
          </div>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <p className="text-muted-foreground">Carregando...</p>
          ) : (
            <>
              {/* Mobile cards */}
              <div className="md:hidden space-y-3">
                {filtered?.map((produto) => (
                  <div key={produto.id} className="rounded-lg border bg-background p-4 space-y-2">
                    <div className="flex items-start justify-between gap-2">
                      <div>
                        <p className="font-medium">{produto.nome}</p>
                        <p className="text-xs text-muted-foreground">{produto.sku}</p>
                      </div>
                      <div className="flex flex-col items-end gap-1">
                        <Badge variant={produto.ativo ? "default" : "secondary"}>
                          {produto.ativo ? "Ativo" : "Inativo"}
                        </Badge>
                        <Badge variant="outline">{getCategoriaLabel(produto.categoria)}</Badge>
                      </div>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-muted-foreground">Custo: {formatCurrency(produto.precoCusto)}</span>
                      <span className="font-medium">Venda: {formatCurrency(produto.precoVenda)}</span>
                    </div>
                  </div>
                ))}
                {filtered?.length === 0 && (
                  <p className="text-center text-muted-foreground py-4">Nenhum produto encontrado</p>
                )}
              </div>
              {/* Desktop table */}
              <div className="hidden md:block overflow-x-auto">
              <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Nome</TableHead>
                  <TableHead>SKU</TableHead>
                  <TableHead>Categoria</TableHead>
                  <TableHead>Custo</TableHead>
                  <TableHead>Venda</TableHead>
                  <TableHead>Status</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filtered?.map((produto) => (
                  <TableRow key={produto.id}>
                    <TableCell className="font-medium">{produto.nome}</TableCell>
                    <TableCell>{produto.sku}</TableCell>
                    <TableCell>
                      <Badge variant="outline">{getCategoriaLabel(produto.categoria)}</Badge>
                    </TableCell>
                    <TableCell>{formatCurrency(produto.precoCusto)}</TableCell>
                    <TableCell>{formatCurrency(produto.precoVenda)}</TableCell>
                    <TableCell>
                      <Badge variant={produto.ativo ? "default" : "secondary"}>
                        {produto.ativo ? "Ativo" : "Inativo"}
                      </Badge>
                    </TableCell>
                  </TableRow>
                ))}
                {filtered?.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={6} className="text-center text-muted-foreground">
                      Nenhum produto encontrado
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
