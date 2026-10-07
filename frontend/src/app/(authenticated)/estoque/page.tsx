"use client";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
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
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import api from "@/lib/api";
import type { EstoqueItemDto, ProdutoDto, RegistrarEntradaEstoqueRequest } from "@/lib/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { AlertTriangle, Plus } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

export default function EstoquePage() {
  const queryClient = useQueryClient();
  const [dialogOpen, setDialogOpen] = useState(false);
  const [form, setForm] = useState<RegistrarEntradaEstoqueRequest>({
    produtoId: "",
    quantidade: 0,
    quantidadeMinima: 5,
    localizacao: "",
    dataValidade: null,
    observacao: "",
  });

  const { data: estoque, isLoading } = useQuery({
    queryKey: ["estoque"],
    queryFn: () => api.get<EstoqueItemDto[]>("/estoque").then((r) => r.data),
  });

  const { data: estoqueBaixo } = useQuery({
    queryKey: ["estoque-baixo"],
    queryFn: () => api.get<EstoqueItemDto[]>("/estoque/baixo").then((r) => r.data),
  });

  const { data: produtos } = useQuery({
    queryKey: ["produtos"],
    queryFn: () => api.get<ProdutoDto[]>("/produtos").then((r) => r.data),
  });

  const entradaMutation = useMutation({
    mutationFn: (data: RegistrarEntradaEstoqueRequest) => api.post("/estoque/entrada", data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["estoque"] });
      queryClient.invalidateQueries({ queryKey: ["estoque-baixo"] });
      setDialogOpen(false);
      setForm({ produtoId: "", quantidade: 0, quantidadeMinima: 5, localizacao: "", dataValidade: null, observacao: "" });
      toast.success("Entrada registrada com sucesso!");
    },
    onError: () => toast.error("Erro ao registrar entrada"),
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    entradaMutation.mutate(form);
  };

  const renderTable = (items: EstoqueItemDto[] | undefined) => (
    <>
      {/* Mobile cards */}
      <div className="md:hidden space-y-3">
        {items?.map((item) => (
          <div key={item.id} className="rounded-lg border bg-background p-4 space-y-2">
            <div className="flex items-start justify-between gap-2">
              <p className="font-medium">{item.produtoNome}</p>
              <div className="flex gap-1 flex-wrap justify-end">
                {item.estaBaixo && <Badge variant="destructive">Baixo</Badge>}
                {item.estaVencido && <Badge variant="destructive">Vencido</Badge>}
                {!item.estaBaixo && !item.estaVencido && <Badge variant="default">OK</Badge>}
              </div>
            </div>
            <div className="grid grid-cols-2 gap-x-4 gap-y-1 text-sm">
              <span className="text-muted-foreground">Quantidade:</span>
              <span className="font-medium">{item.quantidade}</span>
              <span className="text-muted-foreground">Mínimo:</span>
              <span>{item.quantidadeMinima}</span>
              <span className="text-muted-foreground">Localização:</span>
              <span>{item.localizacao || "-"}</span>
              {item.dataValidade && (
                <>
                  <span className="text-muted-foreground">Validade:</span>
                  <span>{new Date(item.dataValidade).toLocaleDateString("pt-BR")}</span>
                </>
              )}
            </div>
          </div>
        ))}
        {(!items || items.length === 0) && (
          <p className="text-center text-muted-foreground py-4">Nenhum item encontrado</p>
        )}
      </div>
      {/* Desktop table */}
      <div className="hidden md:block overflow-x-auto">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Produto</TableHead>
            <TableHead>Quantidade</TableHead>
            <TableHead>Mínimo</TableHead>
            <TableHead>Localização</TableHead>
            <TableHead>Validade</TableHead>
            <TableHead>Status</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {items?.map((item) => (
            <TableRow key={item.id}>
              <TableCell className="font-medium">{item.produtoNome}</TableCell>
              <TableCell>{item.quantidade}</TableCell>
              <TableCell>{item.quantidadeMinima}</TableCell>
              <TableCell>{item.localizacao}</TableCell>
              <TableCell>
                {item.dataValidade
                  ? new Date(item.dataValidade).toLocaleDateString("pt-BR")
                  : "-"}
              </TableCell>
              <TableCell className="flex gap-1">
                {item.estaBaixo && (
                  <Badge variant="destructive">Baixo</Badge>
                )}
                {item.estaVencido && (
                  <Badge variant="destructive">Vencido</Badge>
                )}
                {!item.estaBaixo && !item.estaVencido && (
                  <Badge variant="default">OK</Badge>
                )}
              </TableCell>
            </TableRow>
          ))}
          {(!items || items.length === 0) && (
            <TableRow>
              <TableCell colSpan={6} className="text-center text-muted-foreground">
                Nenhum item encontrado
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
      </div>
    </>
  );

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold">Estoque</h1>
          <p className="text-muted-foreground">Controle de estoque de produtos</p>
        </div>
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogTrigger render={<Button />}>
              <Plus className="mr-2 h-4 w-4" />
              Registrar Entrada
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Registrar Entrada de Estoque</DialogTitle>
            </DialogHeader>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Label>Produto</Label>
                <Select value={form.produtoId} onValueChange={(v: string | null) => setForm({ ...form, produtoId: v ?? '' })}>
                  <SelectTrigger className="w-full">
                    <SelectValue placeholder="Selecione um produto" />
                  </SelectTrigger>
                  <SelectContent>
                    {produtos?.map((p) => (
                      <SelectItem key={p.id} value={p.id}>{p.nome}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="space-y-2">
                  <Label htmlFor="quantidade">Quantidade</Label>
                  <Input id="quantidade" type="number" min="1" value={form.quantidade} onChange={(e) => setForm({ ...form, quantidade: Number(e.target.value) })} required />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="quantidadeMinima">Qtd. Mínima</Label>
                  <Input id="quantidadeMinima" type="number" min="0" value={form.quantidadeMinima} onChange={(e) => setForm({ ...form, quantidadeMinima: Number(e.target.value) })} />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="localizacao">Localização</Label>
                  <Input id="localizacao" value={form.localizacao} onChange={(e) => setForm({ ...form, localizacao: e.target.value })} />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="dataValidade">Validade</Label>
                  <Input id="dataValidade" type="date" value={form.dataValidade ?? ""} onChange={(e) => setForm({ ...form, dataValidade: e.target.value || null })} />
                </div>
              </div>
              <Button type="submit" className="w-full" disabled={entradaMutation.isPending}>
                {entradaMutation.isPending ? "Registrando..." : "Registrar Entrada"}
              </Button>
            </form>
          </DialogContent>
        </Dialog>
      </div>

      <Tabs defaultValue="todos">
        <TabsList>
          <TabsTrigger value="todos">Todos</TabsTrigger>
          <TabsTrigger value="baixo" className="flex items-center gap-1">
            <AlertTriangle className="h-3 w-3" />
            Estoque Baixo
            {estoqueBaixo && estoqueBaixo.length > 0 && (
              <Badge variant="destructive" className="ml-1 h-5 px-1.5">
                {estoqueBaixo.length}
              </Badge>
            )}
          </TabsTrigger>
        </TabsList>
        <TabsContent value="todos">
          <Card>
            <CardHeader>
              <CardTitle>Todos os Itens</CardTitle>
            </CardHeader>
            <CardContent>
              {isLoading ? <p className="text-muted-foreground">Carregando...</p> : renderTable(estoque)}
            </CardContent>
          </Card>
        </TabsContent>
        <TabsContent value="baixo">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <AlertTriangle className="h-5 w-5 text-destructive" />
                Itens com Estoque Baixo
              </CardTitle>
            </CardHeader>
            <CardContent>{renderTable(estoqueBaixo)}</CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
}
