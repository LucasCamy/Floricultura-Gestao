"use client";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
    Card,
    CardContent,
    CardDescription,
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
import api from "@/lib/api";
import type { CaixaDto } from "@/lib/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { DollarSign, Lock, Unlock } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

export default function CaixaPage() {
  const queryClient = useQueryClient();
  const [abrirOpen, setAbrirOpen] = useState(false);
  const [fecharOpen, setFecharOpen] = useState(false);
  const [saldoInicial, setSaldoInicial] = useState(0);
  const [caixaFecharId, setCaixaFecharId] = useState("");

  const { data: caixas, isLoading } = useQuery({
    queryKey: ["caixas"],
    queryFn: () => api.get<CaixaDto[]>("/caixa").then((r) => r.data).catch(() => []),
  });

  const abrirMutation = useMutation({
    mutationFn: (saldoInicial: number) => api.post("/caixa/abrir", { saldoInicial }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["caixas"] });
      setAbrirOpen(false);
      setSaldoInicial(0);
      toast.success("Caixa aberto com sucesso!");
    },
    onError: () => toast.error("Erro ao abrir caixa"),
  });

  const fecharMutation = useMutation({
    mutationFn: (caixaId: string) => api.post("/caixa/fechar", { caixaId }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["caixas"] });
      setFecharOpen(false);
      setCaixaFecharId("");
      toast.success("Caixa fechado com sucesso!");
    },
    onError: () => toast.error("Erro ao fechar caixa"),
  });

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(value);

  const caixasAbertos = caixas?.filter((c) => c.status === "Aberto") ?? [];
  const caixasFechados = caixas?.filter((c) => c.status === "Fechado") ?? [];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">Caixa</h1>
          <p className="text-muted-foreground">Abertura e fechamento de caixa</p>
        </div>
        <div className="flex gap-2">
          <Dialog open={abrirOpen} onOpenChange={setAbrirOpen}>
            <DialogTrigger render={<Button />}>
                <Unlock className="mr-2 h-4 w-4" />
                Abrir Caixa
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Abrir Caixa</DialogTitle>
              </DialogHeader>
              <div className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="saldoInicial">Saldo Inicial (R$)</Label>
                  <Input
                    id="saldoInicial"
                    type="number"
                    step="0.01"
                    min="0"
                    value={saldoInicial}
                    onChange={(e) => setSaldoInicial(Number(e.target.value))}
                  />
                </div>
                <Button className="w-full" onClick={() => abrirMutation.mutate(saldoInicial)} disabled={abrirMutation.isPending}>
                  {abrirMutation.isPending ? "Abrindo..." : "Confirmar Abertura"}
                </Button>
              </div>
            </DialogContent>
          </Dialog>

          {caixasAbertos.length > 0 && (
            <Dialog open={fecharOpen} onOpenChange={setFecharOpen}>
              <DialogTrigger render={<Button variant="outline" />}>
                  <Lock className="mr-2 h-4 w-4" />
                  Fechar Caixa
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Fechar Caixa</DialogTitle>
                </DialogHeader>
                <div className="space-y-4">
                  <p className="text-sm text-muted-foreground">
                    Selecione o caixa para fechar:
                  </p>
                  {caixasAbertos.map((caixa) => (
                    <Card
                      key={caixa.id}
                      className={`cursor-pointer transition-colors ${caixaFecharId === caixa.id ? "border-primary" : ""}`}
                      onClick={() => setCaixaFecharId(caixa.id)}
                    >
                      <CardContent className="p-4">
                        <div className="flex justify-between">
                          <span className="font-medium">{caixa.operador}</span>
                          <span>{formatCurrency(caixa.saldoInicial)}</span>
                        </div>
                        <p className="text-xs text-muted-foreground">
                          Aberto em: {new Date(caixa.dataAbertura).toLocaleString("pt-BR")}
                        </p>
                      </CardContent>
                    </Card>
                  ))}
                  <Button
                    className="w-full"
                    onClick={() => fecharMutation.mutate(caixaFecharId)}
                    disabled={!caixaFecharId || fecharMutation.isPending}
                  >
                    {fecharMutation.isPending ? "Fechando..." : "Confirmar Fechamento"}
                  </Button>
                </div>
              </DialogContent>
            </Dialog>
          )}
        </div>
      </div>

      {/* Caixas abertos */}
      {caixasAbertos.length > 0 && (
        <div className="space-y-4">
          <h2 className="text-xl font-semibold flex items-center gap-2">
            <Unlock className="h-5 w-5 text-green-600" />
            Caixas Abertos
          </h2>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {caixasAbertos.map((caixa) => (
              <Card key={caixa.id}>
                <CardHeader>
                  <CardTitle className="flex items-center justify-between">
                    <span>{caixa.operador}</span>
                    <Badge>Aberto</Badge>
                  </CardTitle>
                  <CardDescription>
                    Aberto em: {new Date(caixa.dataAbertura).toLocaleString("pt-BR")}
                  </CardDescription>
                </CardHeader>
                <CardContent className="space-y-2">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Saldo Inicial:</span>
                    <span className="font-medium">{formatCurrency(caixa.saldoInicial)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Entradas:</span>
                    <span className="font-medium text-green-600">{formatCurrency(caixa.totalEntradas)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Saídas:</span>
                    <span className="font-medium text-red-600">{formatCurrency(caixa.totalSaidas)}</span>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      )}

      {/* Histórico */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <DollarSign className="h-5 w-5" />
            Histórico de Caixas
          </CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <p className="text-muted-foreground">Carregando...</p>
          ) : caixasFechados.length === 0 ? (
            <p className="text-muted-foreground text-center py-4">Nenhum caixa fechado encontrado</p>
          ) : (
            <div className="space-y-3">
              {caixasFechados.map((caixa) => (
                <div key={caixa.id} className="flex items-center justify-between rounded-md border p-4">
                  <div>
                    <p className="font-medium">{caixa.operador}</p>
                    <p className="text-xs text-muted-foreground">
                      {new Date(caixa.dataAbertura).toLocaleDateString("pt-BR")} -{" "}
                      {caixa.dataFechamento ? new Date(caixa.dataFechamento).toLocaleDateString("pt-BR") : ""}
                    </p>
                  </div>
                  <div className="text-right">
                    <p className="font-bold">{formatCurrency(caixa.saldoFinal ?? 0)}</p>
                    <Badge variant="secondary">Fechado</Badge>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
