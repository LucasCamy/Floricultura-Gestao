"use client";

import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from "@/components/ui/card";
import api from "@/lib/api";
import type { ClienteDto, EstoqueItemDto, ProdutoDto, VendaDto } from "@/lib/types";
import { useQuery } from "@tanstack/react-query";
import { AlertTriangle, Package, ShoppingCart, Users } from "lucide-react";

export default function DashboardPage() {
  const { data: clientes } = useQuery({
    queryKey: ["clientes"],
    queryFn: () => api.get<ClienteDto[]>("/clientes").then((r) => r.data),
  });

  const { data: produtos } = useQuery({
    queryKey: ["produtos"],
    queryFn: () => api.get<ProdutoDto[]>("/produtos").then((r) => r.data),
  });

  const { data: estoqueBaixo } = useQuery({
    queryKey: ["estoque-baixo"],
    queryFn: () => api.get<EstoqueItemDto[]>("/estoque/baixo").then((r) => r.data),
  });

  const { data: vendas } = useQuery({
    queryKey: ["vendas-hoje"],
    queryFn: () => {
      const hoje = new Date().toISOString().split("T")[0];
      return api
        .get<VendaDto[]>(`/vendas?inicio=${hoje}&fim=${hoje}`)
        .then((r) => r.data);
    },
  });

  const stats = [
    {
      title: "Clientes",
      value: clientes?.length ?? 0,
      icon: Users,
      description: "Total de clientes cadastrados",
    },
    {
      title: "Produtos",
      value: produtos?.length ?? 0,
      icon: Package,
      description: "Produtos no catálogo",
    },
    {
      title: "Vendas Hoje",
      value: vendas?.length ?? 0,
      icon: ShoppingCart,
      description: "Vendas realizadas hoje",
    },
    {
      title: "Estoque Baixo",
      value: estoqueBaixo?.length ?? 0,
      icon: AlertTriangle,
      description: "Itens com estoque baixo",
    },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">Dashboard</h1>
        <p className="text-muted-foreground">
          Visão geral da floricultura
        </p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {stats.map((stat) => (
          <Card key={stat.title}>
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium">{stat.title}</CardTitle>
              <stat.icon className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{stat.value}</div>
              <CardDescription>{stat.description}</CardDescription>
            </CardContent>
          </Card>
        ))}
      </div>

      {estoqueBaixo && estoqueBaixo.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <AlertTriangle className="h-5 w-5 text-destructive" />
              Alertas de Estoque Baixo
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-2">
              {estoqueBaixo.map((item) => (
                <div
                  key={item.id}
                  className="flex items-center justify-between rounded-md border p-3"
                >
                  <span className="font-medium">{item.produtoNome}</span>
                  <span className="text-sm text-destructive">
                    {item.quantidade} unidades (mín: {item.quantidadeMinima})
                  </span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
