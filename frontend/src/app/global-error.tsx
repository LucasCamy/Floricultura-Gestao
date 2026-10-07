"use client";

export default function GlobalError({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  return (
    <html>
      <body>
        <div style={{ padding: "2rem", fontFamily: "system-ui" }}>
          <h2>Algo deu errado!</h2>
          <pre style={{ background: "#f4f4f4", padding: "1rem", borderRadius: "4px", overflow: "auto" }}>
            {error?.message || "Erro desconhecido"}
            {"\n\n"}
            {error?.stack}
          </pre>
          <p>digest: {error?.digest}</p>
          <button onClick={reset} style={{ marginTop: "1rem", padding: "0.5rem 1rem" }}>
            Tentar novamente
          </button>
        </div>
      </body>
    </html>
  );
}
