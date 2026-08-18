const ORDEN_TIPO: Record<string, number> = { Bolsa: 0, Moneda: 1, Premio: 2, Billete: 3 };

interface DetalleOrdenable {
  denominacionNombre?: string | null;
  premioNombre?: string | null;
  valorUnitario?: number;
}

function tipoDe(d: DetalleOrdenable): string {
  if (d.denominacionNombre?.startsWith("Bolsa")) return "Bolsa";
  if (d.denominacionNombre?.startsWith("Moneda")) return "Moneda";
  if (d.denominacionNombre?.startsWith("Billete")) return "Billete";
  if (d.premioNombre) return "Premio";
  return "Otro";
}

/** Bolsas -> Monedas -> Premios -> Billetes, ascendente por valor dentro de cada grupo. */
export function ordenarDetalles<T extends DetalleOrdenable>(detalles: T[]): T[] {
  return [...detalles].sort((a, b) => {
    const oa = ORDEN_TIPO[tipoDe(a)] ?? 99;
    const ob = ORDEN_TIPO[tipoDe(b)] ?? 99;
    if (oa !== ob) return oa - ob;
    return (a.valorUnitario ?? 0) - (b.valorUnitario ?? 0);
  });
}

/** Etiqueta legible de un concepto (antepone "Premio" cuando la línea es un premio). */
export function etiquetaConcepto(d: { denominacionNombre?: string | null; premioNombre?: string | null }): string {
  if (d.denominacionNombre) return d.denominacionNombre;
  if (d.premioNombre) return `Premio ${d.premioNombre}`;
  return "";
}
