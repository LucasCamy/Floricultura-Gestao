"use client";

import { Input } from "@/components/ui/input";
import type { ClienteDto } from "@/lib/types";
import { useState } from "react";

interface ClienteSearchProps {
  value: string;
  onSelect: (id: string) => void;
  clientes?: ClienteDto[];
  placeholder?: string;
}

export function ClienteSearch({
  value,
  onSelect,
  clientes,
  placeholder = "Buscar cliente por nome ou CPF...",
}: ClienteSearchProps) {
  const [inputValue, setInputValue] = useState("");
  const [open, setOpen] = useState(false);

  const selectedNome = clientes?.find((c) => c.id === value)?.nome ?? "";

  const filtered =
    inputValue.trim()
      ? (clientes ?? [])
          .filter(
            (c) =>
              c.nome.toLowerCase().includes(inputValue.toLowerCase()) ||
              (c.cpf ?? "").includes(inputValue)
          )
          .slice(0, 8)
      : [];

  const displayValue = open ? inputValue : selectedNome;

  return (
    <div className="relative w-full">
      <Input
        value={displayValue}
        autoComplete="off"
        placeholder={placeholder}
        className="w-full"
        onChange={(e) => {
          setInputValue(e.target.value);
          setOpen(true);
          if (!e.target.value) onSelect("");
        }}
        onFocus={() => {
          setInputValue("");
          setOpen(true);
        }}
        onBlur={() => {
          setOpen(false);
        }}
      />
      {open && filtered.length > 0 && (
        <div className="absolute z-50 mt-1 w-full rounded-md border bg-popover text-popover-foreground shadow-md max-h-48 overflow-y-auto">
          {filtered.map((c) => (
            <button
              key={c.id}
              type="button"
              className="flex w-full items-center gap-2 px-3 py-2 text-sm text-left hover:bg-accent cursor-pointer"
              onMouseDown={(e) => e.preventDefault()}
              onClick={() => {
                onSelect(c.id);
                setInputValue("");
                setOpen(false);
              }}
            >
              <span className="font-medium">{c.nome}</span>
              {c.cpf && (
                <span className="ml-auto text-xs text-muted-foreground">{c.cpf}</span>
              )}
            </button>
          ))}
        </div>
      )}
      {open && inputValue.trim() && filtered.length === 0 && (
        <div className="absolute z-50 mt-1 w-full rounded-md border bg-popover shadow-md px-3 py-2 text-sm text-muted-foreground">
          Nenhum cliente encontrado
        </div>
      )}
    </div>
  );
}
