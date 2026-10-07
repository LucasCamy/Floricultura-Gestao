"use client";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
    Dialog,
    DialogContent,
    DialogFooter,
    DialogHeader,
    DialogTitle,
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
import { useAuth } from "@/lib/auth-context";
import type {
    AtualizarUsuarioRequest,
    CriarUsuarioRequest,
    UsuarioDto,
} from "@/lib/types";
import { KeyRound, Pencil, Plus, PowerOff, RefreshCw, UserCheck } from "lucide-react";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

const PERFIS = ["Admin", "Vendedor", "Caixa", "Estoque"];

const perfilLabel: Record<string, string> = {
  Admin: "Administrador",
  Vendedor: "Vendedor",
  Caixa: "Caixa",
  Estoque: "Estoquista",
};

export default function UsuariosPage() {
  const { user } = useAuth();
  const router = useRouter();
  const [usuarios, setUsuarios] = useState<UsuarioDto[]>([]);
  const [loading, setLoading] = useState(true);

  // Modal criar/editar
  const [modalAberto, setModalAberto] = useState(false);
  const [editando, setEditando] = useState<UsuarioDto | null>(null);
  const [form, setForm] = useState({ nome: "", email: "", senha: "", perfil: "Vendedor" });
  const [salvando, setSalvando] = useState(false);
  const [erroForm, setErroForm] = useState("");

  // Modal redefinir senha
  const [modalSenha, setModalSenha] = useState(false);
  const [usuarioSenha, setUsuarioSenha] = useState<UsuarioDto | null>(null);
  const [novaSenha, setNovaSenha] = useState("");
  const [salvandoSenha, setSalvandoSenha] = useState(false);
  const [erroSenha, setErroSenha] = useState("");

  useEffect(() => {
    if (user?.perfil !== "Admin") {
      router.replace("/");
    }
  }, [user, router]);

  const carregarUsuarios = async () => {
    setLoading(true);
    try {
      const { data } = await api.get<UsuarioDto[]>("/usuarios");
      setUsuarios(data ?? []);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user?.perfil === "Admin") carregarUsuarios();
  }, [user]);

  const abrirCriar = () => {
    setEditando(null);
    setForm({ nome: "", email: "", senha: "", perfil: "Vendedor" });
    setErroForm("");
    setModalAberto(true);
  };

  const abrirEditar = (u: UsuarioDto) => {
    setEditando(u);
    setForm({ nome: u.nome, email: u.email, senha: "", perfil: u.perfil });
    setErroForm("");
    setModalAberto(true);
  };

  const salvar = async () => {
    if (!form.nome.trim() || !form.email.trim() || !form.perfil) {
      setErroForm("Preencha nome, e-mail e perfil.");
      return;
    }
    if (!editando && form.senha.length < 6) {
      setErroForm("A senha deve ter no mínimo 6 caracteres.");
      return;
    }
    setSalvando(true);
    setErroForm("");
    try {
      if (editando) {
        const body: AtualizarUsuarioRequest = {
          id: editando.id,
          nome: form.nome,
          email: form.email,
          perfil: form.perfil,
        };
        await api.put(`/usuarios/${editando.id}`, body);
      } else {
        const body: CriarUsuarioRequest = {
          nome: form.nome,
          email: form.email,
          senha: form.senha,
          perfil: form.perfil,
        };
        await api.post("/usuarios", body);
      }
      setModalAberto(false);
      carregarUsuarios();
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { mensagem?: string; erros?: string[] } } };
      const msg =
        axiosErr?.response?.data?.mensagem ||
        axiosErr?.response?.data?.erros?.join(", ") ||
        "Erro ao salvar.";
      setErroForm(msg);
    } finally {
      setSalvando(false);
    }
  };

  const toggleAtivo = async (u: UsuarioDto) => {
    try {
      if (u.ativo) {
        await api.delete(`/usuarios/${u.id}`);
      } else {
        await api.post(`/usuarios/${u.id}/ativar`);
      }
      carregarUsuarios();
    } catch {
      /* ignore */
    }
  };

  const abrirSenha = (u: UsuarioDto) => {
    setUsuarioSenha(u);
    setNovaSenha("");
    setErroSenha("");
    setModalSenha(true);
  };

  const salvarSenha = async () => {
    if (novaSenha.length < 6) {
      setErroSenha("A senha deve ter no mínimo 6 caracteres.");
      return;
    }
    setSalvandoSenha(true);
    setErroSenha("");
    try {
      await api.post(`/usuarios/${usuarioSenha!.id}/redefinir-senha`, {
        novaSenha,
      });
      setModalSenha(false);
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { mensagem?: string } } };
      setErroSenha(axiosErr?.response?.data?.mensagem || "Erro ao redefinir senha.");
    } finally {
      setSalvandoSenha(false);
    }
  };

  if (user?.perfil !== "Admin") return null;

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold">Controle de Acesso</h1>
          <p className="text-muted-foreground">
            Gerencie os usuários do sistema
          </p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" size="sm" onClick={carregarUsuarios}>
            <RefreshCw className="mr-2 h-4 w-4" />
            Atualizar
          </Button>
          <Button onClick={abrirCriar}>
            <Plus className="mr-2 h-4 w-4" />
            Novo Usuário
          </Button>
        </div>
      </div>

      <div className="rounded-lg border bg-card">
        {/* Mobile cards */}
        <div className="md:hidden p-3 space-y-3">
          {loading ? (
            <p className="text-center text-muted-foreground py-4">Carregando...</p>
          ) : usuarios.length === 0 ? (
            <p className="text-center text-muted-foreground py-4">Nenhum usuário encontrado.</p>
          ) : (
            usuarios.map((u) => (
              <div key={u.id} className={`rounded-lg border bg-background p-4 space-y-2 ${!u.ativo ? "opacity-60" : ""}`}>
                <div className="flex items-start justify-between gap-2">
                  <div>
                    <p className="font-medium">{u.nome}</p>
                    <p className="text-sm text-muted-foreground">{u.email}</p>
                  </div>
                  <div className="flex flex-col items-end gap-1">
                    <Badge variant={u.ativo ? "default" : "secondary"}>{u.ativo ? "Ativo" : "Inativo"}</Badge>
                    <Badge variant="outline">{perfilLabel[u.perfil] ?? u.perfil}</Badge>
                  </div>
                </div>
                <p className="text-xs text-muted-foreground">
                  Criado em {new Date(u.criadoEm).toLocaleDateString("pt-BR")}
                </p>
                <div className="flex flex-wrap gap-2 pt-1">
                  <Button size="sm" variant="outline" onClick={() => abrirEditar(u)}>
                    <Pencil className="h-4 w-4 mr-1" /> Editar
                  </Button>
                  <Button size="sm" variant="outline" onClick={() => abrirSenha(u)}>
                    <KeyRound className="h-4 w-4 mr-1" /> Senha
                  </Button>
                  <Button
                    size="sm"
                    variant={u.ativo ? "destructive" : "outline"}
                    onClick={() => toggleAtivo(u)}
                  >
                    {u.ativo ? (
                      <><PowerOff className="h-4 w-4 mr-1" /> Desativar</>
                    ) : (
                      <><UserCheck className="h-4 w-4 mr-1" /> Ativar</>
                    )}
                  </Button>
                </div>
              </div>
            ))
          )}
        </div>
        {/* Desktop table */}
        <div className="hidden md:block overflow-x-auto">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Nome</TableHead>
              <TableHead>E-mail</TableHead>
              <TableHead>Perfil</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Criado em</TableHead>
              <TableHead className="text-right">Ações</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={6} className="text-center py-8 text-muted-foreground">
                  Carregando...
                </TableCell>
              </TableRow>
            ) : usuarios.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} className="text-center py-8 text-muted-foreground">
                  Nenhum usuário encontrado.
                </TableCell>
              </TableRow>
            ) : (
              usuarios.map((u) => (
                <TableRow key={u.id} className={!u.ativo ? "opacity-60" : undefined}>
                  <TableCell className="font-medium">{u.nome}</TableCell>
                  <TableCell>{u.email}</TableCell>
                  <TableCell>
                    <Badge variant="outline">{perfilLabel[u.perfil] ?? u.perfil}</Badge>
                  </TableCell>
                  <TableCell>
                    <Badge variant={u.ativo ? "default" : "secondary"}>
                      {u.ativo ? "Ativo" : "Inativo"}
                    </Badge>
                  </TableCell>
                  <TableCell>
                    {new Date(u.criadoEm).toLocaleDateString("pt-BR")}
                  </TableCell>
                  <TableCell className="text-right">
                    <div className="flex justify-end gap-1">
                      <Button
                        variant="ghost"
                        size="icon"
                        title="Editar"
                        onClick={() => abrirEditar(u)}
                      >
                        <Pencil className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="icon"
                        title="Redefinir senha"
                        onClick={() => abrirSenha(u)}
                      >
                        <KeyRound className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="icon"
                        title={u.ativo ? "Desativar" : "Ativar"}
                        onClick={() => toggleAtivo(u)}
                      >
                        {u.ativo ? (
                          <PowerOff className="h-4 w-4 text-destructive" />
                        ) : (
                          <UserCheck className="h-4 w-4 text-green-600" />
                        )}
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
        </div>
      </div>

      {/* Modal criar/editar */}
      <Dialog open={modalAberto} onOpenChange={setModalAberto}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>{editando ? "Editar Usuário" : "Novo Usuário"}</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-2">
            <div className="space-y-2">
              <Label htmlFor="nome">Nome</Label>
              <Input
                id="nome"
                value={form.nome}
                onChange={(e) => setForm({ ...form, nome: e.target.value })}
                placeholder="Nome completo"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="email">E-mail</Label>
              <Input
                id="email"
                type="email"
                value={form.email}
                onChange={(e) => setForm({ ...form, email: e.target.value })}
                placeholder="email@exemplo.com"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="perfil">Perfil</Label>
              <Select
                value={form.perfil}
                onValueChange={(v) => v && setForm({ ...form, perfil: v })}
              >
                <SelectTrigger className="w-full" id="perfil">
                  <SelectValue placeholder="Selecione o perfil" />
                </SelectTrigger>
                <SelectContent>
                  {PERFIS.map((p) => (
                    <SelectItem key={p} value={p}>
                      {perfilLabel[p]}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            {!editando && (
              <div className="space-y-2">
                <Label htmlFor="senha">Senha</Label>
                <Input
                  id="senha"
                  type="password"
                  value={form.senha}
                  onChange={(e) => setForm({ ...form, senha: e.target.value })}
                  placeholder="Mínimo 6 caracteres"
                />
              </div>
            )}
            {erroForm && (
              <p className="text-sm text-destructive">{erroForm}</p>
            )}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setModalAberto(false)}>
              Cancelar
            </Button>
            <Button onClick={salvar} disabled={salvando}>
              {salvando ? "Salvando..." : "Salvar"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Modal redefinir senha */}
      <Dialog open={modalSenha} onOpenChange={setModalSenha}>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>Redefinir Senha</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-2">
            <p className="text-sm text-muted-foreground">
              Definir nova senha para <strong>{usuarioSenha?.nome}</strong>.
            </p>
            <div className="space-y-2">
              <Label htmlFor="novaSenha">Nova Senha</Label>
              <Input
                id="novaSenha"
                type="password"
                value={novaSenha}
                onChange={(e) => setNovaSenha(e.target.value)}
                placeholder="Mínimo 6 caracteres"
              />
            </div>
            {erroSenha && (
              <p className="text-sm text-destructive">{erroSenha}</p>
            )}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setModalSenha(false)}>
              Cancelar
            </Button>
            <Button onClick={salvarSenha} disabled={salvandoSenha}>
              {salvandoSenha ? "Salvando..." : "Redefinir"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
