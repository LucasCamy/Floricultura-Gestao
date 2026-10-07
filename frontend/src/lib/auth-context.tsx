"use client";

import api from "@/lib/api";
import type { LoginResponse } from "@/lib/types";
import { useRouter } from "next/navigation";
import { createContext, useCallback, useContext, useEffect, useState, useSyncExternalStore } from "react";

interface AuthUser {
  nome: string;
  perfil: string;
  token: string;
}

interface AuthContextType {
  user: AuthUser | null;
  loading: boolean;
  login: (email: string, senha: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

// --- External store (localStorage) for useSyncExternalStore ---
let authListeners: Array<() => void> = [];

function subscribeAuth(callback: () => void) {
  authListeners = [...authListeners, callback];
  return () => { authListeners = authListeners.filter(l => l !== callback); };
}

function notifyAuthListeners() {
  for (const listener of authListeners) listener();
}

let cachedAuthSnapshot: AuthUser | null = null;

function getAuthSnapshot(): AuthUser | null {
  const token = localStorage.getItem("token");
  const nome = localStorage.getItem("nome");
  const perfil = localStorage.getItem("perfil");
  if (token && nome && perfil) {
    if (
      cachedAuthSnapshot?.token === token &&
      cachedAuthSnapshot?.nome === nome &&
      cachedAuthSnapshot?.perfil === perfil
    ) {
      return cachedAuthSnapshot;
    }
    cachedAuthSnapshot = { token, nome, perfil };
    return cachedAuthSnapshot;
  }
  cachedAuthSnapshot = null;
  return null;
}

function getAuthServerSnapshot(): AuthUser | null {
  return null;
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const user = useSyncExternalStore(subscribeAuth, getAuthSnapshot, getAuthServerSnapshot);
  const [mounted, setMounted] = useState(false);
  const router = useRouter();

  useEffect(() => {
    setMounted(true);
  }, []);

  const login = useCallback(async (email: string, senha: string) => {
    const { data } = await api.post<LoginResponse>("/auth/login", { email, senha });
    localStorage.setItem("token", data.token);
    localStorage.setItem("nome", data.nome);
    localStorage.setItem("perfil", data.role);
    notifyAuthListeners();
    router.push("/");
  }, [router]);

  const logout = useCallback(() => {
    localStorage.removeItem("token");
    localStorage.removeItem("nome");
    localStorage.removeItem("perfil");
    notifyAuthListeners();
    router.push("/login");
  }, [router]);

  return (
    <AuthContext.Provider value={{ user, loading: !mounted, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
}
