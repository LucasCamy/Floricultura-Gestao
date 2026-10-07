"use client";

import { ClienteSearch } from "@/components/cliente-search";
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
import api from "@/lib/api";
import type { CaixaDto, ClienteDto, ProdutoDto, RegistrarVendaRequest, VendaDto } from "@/lib/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Plus, ShoppingCart, Trash2 } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

interface ItemForm {
  produtoId: string;
  quantidade: number;
}

export default function VendasPage() {
  const queryClient = useQueryClient();
  const [dialogOpen, setDialogOpen] = useState(false);
  const [clienteId, setClienteId] = useState("");
  const [caixaId, setCaixaId] = useState("");
  const [desconto, setDesconto] = useState(0);
  const [observacoes, setObservacoes] = useState("");
  const [itens, setItens] = useState<ItemForm[]>([]);
  const [inicio, setInicio] = useState(() => {
    const d = new Date();
    d.setDate(d.getDate() - 30);
    return d.toISOString().split("T")[0];
  });
  const [fim, setFim] = useState(() => new Date().toISOString().split("T")[0]);

  const { data: vendas, isLoading } = useQuery({
    queryKey: ["vendas", inicio, fim],
    queryFn: () => api.get<VendaDto[]>(`/vendas?inicio=${inicio}&fim=${fim}`).then((r) => r.data),
  });

  const { data: clientes } = useQuery({
    queryKey: ["clientes"],
    queryFn: () => api.get<ClienteDto[]>("/clientes").then((r) => r.data),
  });

  const { data: produtos } = useQuery({
    queryKey: ["produtos"],
    queryFn: () => api.get<ProdutoDto[]>("/produtos").then((r) => r.data),
  });

  const { data: caixas } = useQuery({
    queryKey: ["caixas-abertos"],
    queryFn: () => api.get<CaixaDto[]>("/caixa").then((r) => r.data).catch(() => []),
  });

  const vendaMutation = useMutation({
    mutationFn: (data: RegistrarVendaRequest) => api.post("/vendas", data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["vendas"] });
      queryClient.invalidateQueries({ queryKey: ["estoque"] });
      setDialogOpen(false);
      resetForm();
      toast.success("Venda registrada com sucesso!");
    },
    onError: () => toast.error("Erro ao registrar venda"),
  });

  const resetForm = () => {
    setClienteId("");
    setCaixaId("");
    setDesconto(0);
    setObservacoes("");
    setItens([]);
  };

  const addItem = () => setItens([...itens, { produtoId: "", quantidade: 1 }]);
  const removeItem = (index: number) => setItens(itens.filter((_, i) => i !== index));
  const updateItem = (index: number, field: keyof ItemForm, value: string | number) => {
    const updated = [...itens];
    updated[index] = { ...updated[index], [field]: value };
    setItens(updated);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (itens.length === 0) {
      toast.error("Adicione pelo menos um item");
      return;
    }
    vendaMutation.mutate({ clienteId, caixaId, itens, desconto, observacoes });
  };

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(value);

  const statusColor = (status: string) => {
    switch (status) {
      case "Finalizada": return "default" as const;
      case "Aberta": return "secondary" as const;
      case "Cancelada": return "destructive" as const;
      default: return "outline" as const;
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">Vendas</h1>
          <p className="text-muted-foreground">Registro e histórico de vendas</p>
        </div>
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogTrigger render={<Button />}>
              <ShoppingCart className="mr-2 h-4 w-4" />
              Nova Venda
          </DialogTrigger>
          <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle>Registrar Venda</DialogTitle>
            </DialogHeader>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="space-y-2">
                  <Label>Cliente</Label>
                  <ClienteSearch
                    value={clienteId}
                    onSelect={(id) => setClienteId(id)}
                    clientes={clientes}
                  />
                </div>
                <div className="space-y-2">
                  <Label>Caixa</Label>
                  <Select value={caixaId} onValueChange={(v: string | null) => setCaixaId(v ?? '')}>
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder="Selecione" />
                    </SelectTrigger>
                    <SelectContent>
                      {caixas?.map((c) => (
                        <SelectItem key={c.id} value={c.id}>{c.operador} - {c.status}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div className="space-y-3">
                <div className="flex items-center justify-between">
                  <Label>Itens</Label>
                  <Button type="button" variant="outline" size="sm" onClick={addItem}>
                    <Plus className="mr-1 h-3 w-3" /> Item
                  </Button>
                </div>
                {itens.map((item, index) => (
                  <div key={index} className="flex gap-2 items-end">
                    <div className="flex-1">
                      <Select value={item.produtoId} onValueChange={(v: string | null) => updateItem(index, "produtoId", v ?? '')}>
                        <SelectTrigger className="w-full">
                          <SelectValue placeholder="Produto" />
                        </SelectTrigger>
                        <SelectContent>
                          {produtos?.map((p) => (
                            <SelectItem key={p.id} value={p.id}>
                              {p.nome} - {formatCurrency(p.precoVenda)}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                    <Input
                      type="number"
                      min="1"
                      value={item.quantidade}
                      onChange={(e) => updateItem(index, "quantidade", Number(e.target.value))}
                      className="w-20"
                    />
                    <Button type="button" variant="ghost" size="icon" onClick={() => removeItem(index)}>
                      <Trash2 className="h-4 w-4 text-destructive" />
                    </Button>
                  </div>
                ))}
              </div>

              <div className="grid gap-4 sm:grid-cols-2">
                <div className="space-y-2">
                  <Label htmlFor="desconto">Desconto (R$)</Label>
                  <Input id="desconto" type="number" step="0.01" min="0" value={desconto} onChange={(e) => setDesconto(Number(e.target.value))} />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="obs">Observações</Label>
                  <Input id="obs" value={observacoes} onChange={(e) => setObservacoes(e.target.value)} />
                </div>
              </div>

              <Button type="submit" className="w-full" disabled={vendaMutation.isPending}>
                {vendaMutation.isPending ? "Registrando..." : "Finalizar Venda"}
              </Button>
            </form>
          </DialogContent>
        </Dialog>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Histórico de Vendas</CardTitle>
          <div className="flex flex-wrap gap-3 mt-2">
            <div className="flex items-center gap-2">
              <Label>De:</Label>
              <Input type="date" value={inicio} onChange={(e) => setInicio(e.target.value)} className="w-40" />
            </div>
            <div className="flex items-center gap-2">
              <Label>Até:</Label>
              <Input type="date" value={fim} onChange={(e) => setFim(e.target.value)} className="w-40" />
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <p className="text-muted-foreground">Carregando...</p>
          ) : (
            <>
              {/* Mobile cards */}
              <div className="md:hidden space-y-3">
                {vendas?.map((venda) => (
                  <div key={venda.id} className="rounded-lg border bg-background p-4 space-y-2">
                    <div className="flex items-start justify-between gap-2">
                      <div>
                        <p className="font-medium">{venda.clienteNome}</p>
                        <p className="text-xs text-muted-foreground">
                          {new Date(venda.criadoEm).toLocaleDateString("pt-BR")}
                        </p>
                      </div>
                      <Badge variant={statusColor(venda.status)}>{venda.status}</Badge>
                    </div>
                    <div className="grid grid-cols-2 gap-x-4 gap-y-1 text-sm">
                      <span className="text-muted-foreground">Itens:</span>
                      <span>{venda.itens.length}</span>
                      <span className="text-muted-foreground">Subtotal:</span>
                      <span>{formatCurrency(venda.subtotal)}</span>
                      {venda.desconto > 0 && (
                        <>
                          <span className="text-muted-foreground">Desconto:</span>
                          <span>{formatCurrency(venda.desconto)}</span>
                        </>
                      )}
                      <span className="text-muted-foreground font-medium">Total:</span>
                      <span className="font-bold">{formatCurrency(venda.total)}</span>
                    </div>
                  </div>
                ))}
                {(!vendas || vendas.length === 0) && (
                  <p className="text-center text-muted-foreground py-4">Nenhuma venda encontrada no período</p>
                )}
              </div>
              {/* Desktop table */}
              <div className="hidden md:block overflow-x-auto">
              <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Data</TableHead>
                  <TableHead>Cliente</TableHead>
                  <TableHead>Itens</TableHead>
                  <TableHead>Subtotal</TableHead>
                  <TableHead>Desconto</TableHead>
                  <TableHead>Total</TableHead>
                  <TableHead>Status</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {vendas?.map((venda) => (
                  <TableRow key={venda.id}>
                    <TableCell>{new Date(venda.criadoEm).toLocaleDateString("pt-BR")}</TableCell>
                    <TableCell>{venda.clienteNome}</TableCell>
                    <TableCell>{venda.itens.length}</TableCell>
                    <TableCell>{formatCurrency(venda.subtotal)}</TableCell>
                    <TableCell>{formatCurrency(venda.desconto)}</TableCell>
                    <TableCell className="font-bold">{formatCurrency(venda.total)}</TableCell>
                    <TableCell>
                      <Badge variant={statusColor(venda.status)}>{venda.status}</Badge>
                    </TableCell>
                  </TableRow>
                ))}
                {(!vendas || vendas.length === 0) && (
                  <TableRow>
                    <TableCell colSpan={7} className="text-center text-muted-foreground">
                      Nenhuma venda encontrada no período
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
