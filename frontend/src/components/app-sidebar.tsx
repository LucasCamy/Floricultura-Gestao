"use client";

import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { useAuth } from "@/lib/auth-context";
import { useTheme } from "@/lib/theme-context";
import { cn } from "@/lib/utils";
import {
    DollarSign,
    FileText,
    Flower2,
    LogOut,
    Menu,
    Moon,
    Package,
    ShieldCheck,
    ShoppingCart,
    Sun,
    Users,
    Warehouse,
    X,
} from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useState } from "react";

const navItems = [
  { href: "/", label: "Dashboard", icon: Flower2 },
  { href: "/clientes", label: "Clientes", icon: Users },
  { href: "/produtos", label: "Produtos", icon: Package },
  { href: "/estoque", label: "Estoque", icon: Warehouse },
  { href: "/vendas", label: "Vendas", icon: ShoppingCart },
  { href: "/caixa", label: "Caixa", icon: DollarSign },
  { href: "/contas", label: "Contas", icon: FileText },
];

export function AppSidebar() {
  const pathname = usePathname();
  const { user, logout } = useAuth();
  const { resolvedTheme, setTheme } = useTheme();
  const [open, setOpen] = useState(false);

  if (!user) return null;

  return (
    <>
      {/* Mobile hamburger - only visible when sidebar is closed */}
      <Button
        variant="ghost"
        size="icon"
        className="fixed top-3 left-3 z-30 md:hidden"
        onClick={() => setOpen(true)}
        aria-label="Abrir menu"
      >
        <Menu className="h-5 w-5" />
      </Button>

      {/* Overlay - z-40 covers the hamburger button */}
      {open && (
        <div
          className="fixed inset-0 z-40 bg-black/50 md:hidden"
          onClick={() => setOpen(false)}
        />
      )}

      {/* Sidebar - z-50 on top of everything */}
      <aside
        className={cn(
          "fixed left-0 top-0 z-50 flex h-screen w-64 flex-col border-r bg-card transition-transform duration-300 md:translate-x-0",
          open ? "translate-x-0" : "-translate-x-full"
        )}
      >
        <div className="flex items-center justify-between px-4 py-4 md:px-6 md:py-5">
          <div className="flex items-center gap-2">
            <Flower2 className="h-7 w-7 text-primary" />
            <span className="text-lg font-bold">Floricultura Gestão</span>
          </div>
          {/* X button inside sidebar - only on mobile */}
          <Button
            variant="ghost"
            size="icon"
            className="md:hidden -mr-1"
            onClick={() => setOpen(false)}
            aria-label="Fechar menu"
          >
            <X className="h-5 w-5" />
          </Button>
        </div>

        <Separator />

        <nav className="flex-1 space-y-1 px-3 py-4">
          {navItems.map((item) => {
            const isActive =
              item.href === "/"
                ? pathname === "/"
                : pathname.startsWith(item.href);
            return (
              <Link
                key={item.href}
                href={item.href}
                onClick={() => setOpen(false)}
                className={cn(
                  "flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors",
                  isActive
                    ? "bg-primary text-primary-foreground"
                    : "text-muted-foreground hover:bg-accent hover:text-accent-foreground"
                )}
              >
                <item.icon className="h-4 w-4" />
                {item.label}
              </Link>
            );
          })}
          {user.perfil === "Admin" && (
            <Link
              href="/usuarios"
              onClick={() => setOpen(false)}
              className={cn(
                "flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors",
                pathname.startsWith("/usuarios")
                  ? "bg-primary text-primary-foreground"
                  : "text-muted-foreground hover:bg-accent hover:text-accent-foreground"
              )}
            >
              <ShieldCheck className="h-4 w-4" />
              Controle de Acesso
            </Link>
          )}
        </nav>

        <Separator />

        <div className="px-3 py-4">
          <div className="mb-2 px-3 text-xs text-muted-foreground truncate">
            {user.nome} ({user.perfil})
          </div>
          <Button
            variant="ghost"
            className="w-full justify-start gap-3 mb-1"
            onClick={() => setTheme(resolvedTheme === "dark" ? "light" : "dark")}
          >
            {resolvedTheme === "dark" ? (
              <Sun className="h-4 w-4" />
            ) : (
              <Moon className="h-4 w-4" />
            )}
            {resolvedTheme === "dark" ? "Modo Claro" : "Modo Escuro"}
          </Button>
          <Button
            variant="ghost"
            className="w-full justify-start gap-3"
            onClick={logout}
          >
            <LogOut className="h-4 w-4" />
            Sair
          </Button>
        </div>
      </aside>
    </>
  );
}
