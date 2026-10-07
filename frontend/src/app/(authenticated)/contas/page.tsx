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
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import api from "@/lib/api";
import type { ClienteDto, ContaPagarDto, ContaReceberDto } from "@/lib/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowDownCircle, ArrowUpCircle, Plus } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

export default function ContasPage() {
  const queryClient = useQueryClient();
  const [pagarDialogOpen, setPagarDialogOpen] = useState(false);
  const [receberDialogOpen, setReceberDialogOpen] = useState(false);

  const [formPagar, setFormPagar] = useState({
    descricao: "",
    valor: 0,
    dataVencimento: "",
    fornecedorId: "",
  });

  const [formReceber, setFormReceber] = useState({
    descricao: "",
    valor: 0,
    dataVencimento: "",
    clienteId: "",
  });

  const { data: contasPagar } = useQuery({
    queryKey: ["contas-pagar"],
    queryFn: () => api.get<ContaPagarDto[]>("/contas/pagar").then((r) => r.data).catch(() => []),
  });

  const { data: contasReceber } = useQuery({
    queryKey: ["contas-receber"],
    queryFn: () => api.get<ContaReceberDto[]>("/contas/receber").then((r) => r.data).catch(() => []),
  });

  const { data: clientes } = useQuery({
    queryKey: ["clientes"],
    queryFn: () => api.get<ClienteDto[]>("/clientes").then((r) => r.data).catch(() => []),
  });

  const criarPagarMutation = useMutation({
    mutationFn: (data: typeof formPagar) => api.post("/contas/pagar", data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["contas-pagar"] });
      setPagarDialogOpen(false);
      setFormPagar({ descricao: "", valor: 0, dataVencimento: "", fornecedorId: "" });
      toast.success("Conta a pagar criada!");
    },
    onError: () => toast.error("Erro ao criar conta"),
  });

  const criarReceberMutation = useMutation({
    mutationFn: (data: typeof formReceber) => api.post("/contas/receber", data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["contas-receber"] });
      setReceberDialogOpen(false);
      setFormReceber({ descricao: "", valor: 0, dataVencimento: "", clienteId: "" });
      toast.success("Conta a receber criada!");
    },
    onError: () => toast.error("Erro ao criar conta"),
  });

  const pagarContaMutation = useMutation({
    mutationFn: (contaId: string) => api.post(`/contas/pagar/${contaId}/pagar`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["contas-pagar"] });
      toast.success("Pagamento registrado!");
    },
    onError: () => toast.error("Erro ao registrar pagamento"),
  });

  const receberContaMutation = useMutation({
    mutationFn: (contaId: string) => api.post(`/contas/receber/${contaId}/receber`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["contas-receber"] });
      toast.success("Recebimento registrado!");
    },
    onError: () => toast.error("Erro ao registrar recebimento"),
  });

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(value);

  const statusBadge = (status: string) => {
    switch (status) {
      case "Paga":
        return <Badge>Paga</Badge>;
      case "Recebida":
        return <Badge>Recebida</Badge>;
      case "Vencida":
        return <Badge variant="destructive">Vencida</Badge>;
      default:
        return <Badge variant="secondary">Pendente</Badge>;
    }
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">Contas</h1>
        <p className="text-muted-foreground">Contas a pagar e a receber</p>
      </div>

      <Tabs defaultValue="pagar">
        <TabsList>
          <TabsTrigger value="pagar" className="flex items-center gap-1">
            <ArrowUpCircle className="h-4 w-4" />
            A Pagar
          </TabsTrigger>
          <TabsTrigger value="receber" className="flex items-center gap-1">
            <ArrowDownCircle className="h-4 w-4" />
            A Receber
          </TabsTrigger>
        </TabsList>

        <TabsContent value="pagar">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <CardTitle>Contas a Pagar</CardTitle>
              <Dialog open={pagarDialogOpen} onOpenChange={setPagarDialogOpen}>
                <DialogTrigger render={<Button size="sm" />}>
                    <Plus className="mr-2 h-4 w-4" />
                    Nova Conta
                </DialogTrigger>
                <DialogContent>
                  <DialogHeader>
                    <DialogTitle>Nova Conta a Pagar</DialogTitle>
                  </DialogHeader>
                  <form onSubmit={(e) => { e.preventDefault(); criarPagarMutation.mutate(formPagar); }} className="space-y-4">
                    <div className="space-y-2">
                      <Label>Descrição</Label>
                      <Input value={formPagar.descricao} onChange={(e) => setFormPagar({ ...formPagar, descricao: e.target.value })} required />
                    </div>
                    <div className="grid gap-4 sm:grid-cols-2">
                      <div className="space-y-2">
                        <Label>Valor</Label>
                        <Input type="number" step="0.01" min="0" value={formPagar.valor} onChange={(e) => setFormPagar({ ...formPagar, valor: Number(e.target.value) })} required />
                      </div>
                      <div className="space-y-2">
                        <Label>Vencimento</Label>
                        <Input type="date" value={formPagar.dataVencimento} onChange={(e) => setFormPagar({ ...formPagar, dataVencimento: e.target.value })} required />
                      </div>
                    </div>
                    <Button type="submit" className="w-full" disabled={criarPagarMutation.isPending}>
                      {criarPagarMutation.isPending ? "Salvando..." : "Criar Conta"}
                    </Button>
                  </form>
                </DialogContent>
              </Dialog>
            </CardHeader>
            <CardContent>
              {/* Mobile cards */}
              <div className="md:hidden space-y-3">
                {contasPagar?.map((conta) => (
                  <div key={conta.id} className="rounded-lg border bg-background p-4 space-y-2">
                    <div className="flex items-start justify-between gap-2">
                      <div>
                        <p className="font-medium">{conta.descricao}</p>
                        {conta.fornecedorNome && (
                          <p className="text-xs text-muted-foreground">{conta.fornecedorNome}</p>
                        )}
                      </div>
                      {statusBadge(conta.status)}
                    </div>
                    <div className="flex items-center justify-between text-sm">
                      <span className="text-muted-foreground">
                        Vence: {new Date(conta.dataVencimento).toLocaleDateString("pt-BR")}
                      </span>
                      <span className="font-bold">{formatCurrency(conta.valor)}</span>
                    </div>
                    {conta.status !== "Paga" && (
                      <Button
                        size="sm"
                        variant="outline"
                        className="w-full"
                        onClick={() => pagarContaMutation.mutate(conta.id)}
                        disabled={pagarContaMutation.isPending}
                      >
                        Pagar
                      </Button>
                    )}
                  </div>
                ))}
                {(!contasPagar || contasPagar.length === 0) && (
                  <p className="text-center text-muted-foreground py-4">Nenhuma conta a pagar</p>
                )}
              </div>
              {/* Desktop table */}
              <div className="hidden md:block overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Descrição</TableHead>
                    <TableHead>Valor</TableHead>
                    <TableHead>Vencimento</TableHead>
                    <TableHead>Fornecedor</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Ações</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {contasPagar?.map((conta) => (
                    <TableRow key={conta.id}>
                      <TableCell className="font-medium">{conta.descricao}</TableCell>
                      <TableCell>{formatCurrency(conta.valor)}</TableCell>
                      <TableCell>{new Date(conta.dataVencimento).toLocaleDateString("pt-BR")}</TableCell>
                      <TableCell>{conta.fornecedorNome || "-"}</TableCell>
                      <TableCell>{statusBadge(conta.status)}</TableCell>
                      <TableCell>
                        {conta.status !== "Paga" && (
                          <Button
                            size="sm"
                            variant="outline"
                            onClick={() => pagarContaMutation.mutate(conta.id)}
                            disabled={pagarContaMutation.isPending}
                          >
                            Pagar
                          </Button>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                  {(!contasPagar || contasPagar.length === 0) && (
                    <TableRow>
                      <TableCell colSpan={6} className="text-center text-muted-foreground">
                        Nenhuma conta a pagar
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="receber">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <CardTitle>Contas a Receber</CardTitle>
              <Dialog open={receberDialogOpen} onOpenChange={setReceberDialogOpen}>
                <DialogTrigger render={<Button size="sm" />}>
                    <Plus className="mr-2 h-4 w-4" />
                    Nova Conta
                </DialogTrigger>
                <DialogContent>
                  <DialogHeader>
                    <DialogTitle>Nova Conta a Receber</DialogTitle>
                  </DialogHeader>
                  <form onSubmit={(e) => { e.preventDefault(); criarReceberMutation.mutate(formReceber); }} className="space-y-4">
                    <div className="space-y-2">
                      <Label>Descrição</Label>
                      <Input value={formReceber.descricao} onChange={(e) => setFormReceber({ ...formReceber, descricao: e.target.value })} required />
                    </div>
                    <div className="grid gap-4 sm:grid-cols-2">
                      <div className="space-y-2">
                        <Label>Valor</Label>
                        <Input type="number" step="0.01" min="0" value={formReceber.valor} onChange={(e) => setFormReceber({ ...formReceber, valor: Number(e.target.value) })} required />
                      </div>
                      <div className="space-y-2">
                        <Label>Vencimento</Label>
                        <Input type="date" value={formReceber.dataVencimento} onChange={(e) => setFormReceber({ ...formReceber, dataVencimento: e.target.value })} required />
                      </div>
                    </div>
                    <div className="space-y-2">
                      <Label>Cliente (opcional)</Label>
                      <ClienteSearch
                        value={formReceber.clienteId}
                        onSelect={(id) => setFormReceber({ ...formReceber, clienteId: id })}
                        clientes={clientes}
                      />
                    </div>
                    <Button type="submit" className="w-full" disabled={criarReceberMutation.isPending}>
                      {criarReceberMutation.isPending ? "Salvando..." : "Criar Conta"}
                    </Button>
                  </form>
                </DialogContent>
              </Dialog>
            </CardHeader>
            <CardContent>
              {/* Mobile cards */}
              <div className="md:hidden space-y-3">
                {contasReceber?.map((conta) => (
                  <div key={conta.id} className="rounded-lg border bg-background p-4 space-y-2">
                    <div className="flex items-start justify-between gap-2">
                      <div>
                        <p className="font-medium">{conta.descricao}</p>
                        {conta.clienteNome && (
                          <p className="text-xs text-muted-foreground">{conta.clienteNome}</p>
                        )}
                      </div>
                      {statusBadge(conta.status)}
                    </div>
                    <div className="flex items-center justify-between text-sm">
                      <span className="text-muted-foreground">
                        Vence: {new Date(conta.dataVencimento).toLocaleDateString("pt-BR")}
                      </span>
                      <span className="font-bold">{formatCurrency(conta.valor)}</span>
                    </div>
                    {conta.status !== "Recebida" && (
                      <Button
                        size="sm"
                        variant="outline"
                        className="w-full"
                        onClick={() => receberContaMutation.mutate(conta.id)}
                        disabled={receberContaMutation.isPending}
                      >
                        Receber
                      </Button>
                    )}
                  </div>
                ))}
                {(!contasReceber || contasReceber.length === 0) && (
                  <p className="text-center text-muted-foreground py-4">Nenhuma conta a receber</p>
                )}
              </div>
              {/* Desktop table */}
              <div className="hidden md:block overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Descrição</TableHead>
                    <TableHead>Valor</TableHead>
                    <TableHead>Vencimento</TableHead>
                    <TableHead>Cliente</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Ações</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {contasReceber?.map((conta) => (
                    <TableRow key={conta.id}>
                      <TableCell className="font-medium">{conta.descricao}</TableCell>
                      <TableCell>{formatCurrency(conta.valor)}</TableCell>
                      <TableCell>{new Date(conta.dataVencimento).toLocaleDateString("pt-BR")}</TableCell>
                      <TableCell>{conta.clienteNome || "-"}</TableCell>
                      <TableCell>{statusBadge(conta.status)}</TableCell>
                      <TableCell>
                        {conta.status !== "Recebida" && (
                          <Button
                            size="sm"
                            variant="outline"
                            onClick={() => receberContaMutation.mutate(conta.id)}
                            disabled={receberContaMutation.isPending}
                          >
                            Receber
                          </Button>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                  {(!contasReceber || contasReceber.length === 0) && (
                    <TableRow>
                      <TableCell colSpan={6} className="text-center text-muted-foreground">
                        Nenhuma conta a receber
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
              </div>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
}
