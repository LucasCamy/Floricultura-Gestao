import axios from "axios";

const api = axios.create({
  baseURL: "/api",
  headers: { "Content-Type": "application/json" },
});

interface ResultadoOperacao<T = unknown> {
  sucesso: boolean;
  dados: T | null;
  mensagem?: string;
  erros?: string[];
}

const isResultadoOperacao = (value: unknown): value is ResultadoOperacao => {
  if (!value || typeof value !== "object") return false;
  return "sucesso" in value && "dados" in value;
};

api.interceptors.request.use((config) => {
  if (typeof window !== "undefined") {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  }
  return config;
});

api.interceptors.response.use(
  (response) => {
    if (isResultadoOperacao(response.data)) {
      response.data = response.data.dados;
    }
    return response;
  },
  (error) => {
    const isLoginRequest = error.config?.url?.includes("/auth/login");
    if (
      error.response?.status === 401 &&
      typeof window !== "undefined" &&
      !isLoginRequest
    ) {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  },
);

export default api;
