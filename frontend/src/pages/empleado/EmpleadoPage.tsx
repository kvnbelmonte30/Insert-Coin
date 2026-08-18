import { useEffect, useMemo, useState } from "react";
import {
  Box,
  Dialog,
  FormControl,
  IconButton,
  InputLabel,
  MenuItem,
  Select,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import AddRoundedIcon from "@mui/icons-material/AddRounded";
import CloseRoundedIcon from "@mui/icons-material/CloseRounded";
import ReceiptLongRoundedIcon from "@mui/icons-material/ReceiptLongRounded";
import WarningAmberRoundedIcon from "@mui/icons-material/WarningAmberRounded";
import { api } from "../../api/client";
import { useAuthStore } from "../../store/authStore";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { StatusPill } from "../../components/StatusPill";
import { brand, dialogBackdropSx, dialogPaperSx, glassFieldLight, glassTableSx, pillButtonSx, pillOutlineButtonSx } from "../../theme/brand";
import { etiquetaConcepto, ordenarDetalles } from "../../utils/cuenta";
import type { Cuenta, CategoriaGasto, Denominacion, Gasto, Premio, CierreDiario } from "../../types";

function SectionLabel({ children }: { children: React.ReactNode }) {
  return (
    <Typography sx={{ fontSize: "0.75rem", fontWeight: 700, color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px", mb: 1, mt: 2.5 }}>
      {children}
    </Typography>
  );
}

const valorDenominacion = (d: Denominacion) => (d.tipo === "Bolsa" ? d.valorPorBolsa ?? 0 : d.valor);

export function EmpleadoPage() {
  const locales = useAuthStore((s) => s.locales);
  const [localId, setLocalId] = useState(locales[0]?.id ?? "");
  const [cuenta, setCuenta] = useState<Cuenta | null>(null);
  const [denominaciones, setDenominaciones] = useState<Denominacion[]>([]);
  const [premios, setPremios] = useState<Premio[]>([]);
  const [gastos, setGastos] = useState<Gasto[]>([]);
  const [cantidades, setCantidades] = useState<Record<string, number>>({});
  const [terminal, setTerminal] = useState<number>(0);
  const [transferencia, setTransferencia] = useState<number>(0);
  const [gastoOpen, setGastoOpen] = useState(false);
  const [resultado, setResultado] = useState<CierreDiario | null>(null);
  const [error, setError] = useState<string | null>(null);

  const cargar = async () => {
    if (!localId) return;
    setError(null);
    try {
      const [cuentaRes, denomRes, premioRes, gastosRes] = await Promise.all([
        api.get<Cuenta>(`/cuentas/local/${localId}/actual`),
        api.get<Denominacion[]>("/catalogos/denominaciones"),
        api.get<Premio[]>("/catalogos/premios"),
        api.get<Gasto[]>(`/gastos/local/${localId}`, { params: { soloSinCierre: true } }),
      ]);
      setCuenta(cuentaRes.data);
      setDenominaciones(denomRes.data);
      setPremios(premioRes.data);
      setGastos(gastosRes.data);
    } catch {
      setCuenta(null);
      setError("Este local todavía no tiene una cuenta configurada por el administrador.");
    }
  };

  useEffect(() => {
    cargar();
    setResultado(null);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [localId]);

  const totalCapturado = useMemo(() => {
    const totalDenominaciones = denominaciones.reduce(
      (sum, d) => sum + (cantidades[d.id] ?? 0) * valorDenominacion(d),
      0
    );
    const totalPremios = premios.reduce((sum, p) => sum + (cantidades[p.id] ?? 0) * p.denominacion, 0);
    const totalGastos = gastos.reduce((sum, g) => sum + g.monto, 0);
    return totalDenominaciones + totalPremios + (terminal || 0) + (transferencia || 0) + totalGastos;
  }, [denominaciones, premios, cantidades, terminal, transferencia, gastos]);

  const registrarCierre = async () => {
    const lineas = [
      ...denominaciones
        .filter((d) => (cantidades[d.id] ?? 0) > 0)
        .map((d) => ({ concepto: d.tipo, denominacionId: d.id, cantidad: cantidades[d.id] })),
      ...premios
        .filter((p) => (cantidades[p.id] ?? 0) > 0)
        .map((p) => ({ concepto: "Premio", premioId: p.id, cantidad: cantidades[p.id] })),
      ...(terminal > 0 ? [{ concepto: "Terminal", cantidad: 1, monto: terminal }] : []),
      ...(transferencia > 0 ? [{ concepto: "Transferencia", cantidad: 1, monto: transferencia }] : []),
    ];

    const { data } = await api.post<CierreDiario>(`/cierres-diarios/local/${localId}`, {
      fecha: new Date().toISOString().slice(0, 10),
      lineas,
      gastoIds: gastos.map((g) => g.id),
    });

    setResultado(data);
    setCantidades({});
    setTerminal(0);
    setTransferencia(0);
    cargar();
  };

  return (
    <Box>
      <PageHeader
        title="Mi cuenta"
        action={
          locales.length > 1 ? (
            <FormControl size="small" sx={{ minWidth: 190, ...glassFieldLight }}>
              <InputLabel>Local</InputLabel>
              <Select label="Local" value={localId} onChange={(e) => setLocalId(e.target.value)}>
                {locales.map((l) => (
                  <MenuItem key={l.id} value={l.id}>
                    {l.nombre}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          ) : undefined
        }
      />

      {error && (
        <Box sx={{ display: "flex", alignItems: "flex-start", gap: 1, mb: 3, p: 1.8, borderRadius: "14px", background: "rgba(240,180,41,0.16)", color: "#b06a00" }}>
          <WarningAmberRoundedIcon fontSize="small" sx={{ mt: 0.1 }} />
          <Typography sx={{ fontSize: "0.85rem" }}>{error}</Typography>
        </Box>
      )}

      {cuenta && (
        <GlassCard sx={{ p: { xs: 2.5, sm: 3 }, mb: 3 }}>
          <Typography sx={{ fontWeight: 700, fontSize: "1.05rem", color: brand.ink, mb: 0.3 }}>
            Cuenta asignada
          </Typography>
          <Typography sx={{ color: brand.inkMuted, fontSize: "0.82rem", mb: 2 }}>
            Semana #{cuenta.semanaNumero} · solo lectura
          </Typography>
          <Box sx={{ overflowX: "auto" }}>
            <Table size="small" sx={glassTableSx}>
              <TableHead>
                <TableRow>
                  <TableCell>Concepto</TableCell>
                  <TableCell align="right">Cantidad</TableCell>
                  <TableCell align="right">Subtotal</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {ordenarDetalles(cuenta.detalles).map((d) => (
                  <TableRow key={d.id}>
                    <TableCell sx={{ fontWeight: 600 }}>{etiquetaConcepto(d)}</TableCell>
                    <TableCell align="right">{d.cantidad}</TableCell>
                    <TableCell align="right">${d.subtotal.toLocaleString()}</TableCell>
                  </TableRow>
                ))}
                <TableRow>
                  <TableCell colSpan={2} sx={{ fontWeight: 800, color: brand.ink }}>
                    Total acumulado
                  </TableCell>
                  <TableCell align="right" sx={{ fontWeight: 800, color: brand.goldDark, fontSize: "1rem" }}>
                    ${cuenta.totalAcumulado.toLocaleString()}
                  </TableCell>
                </TableRow>
              </TableBody>
            </Table>
          </Box>
        </GlassCard>
      )}

      {cuenta && (
        <GlassCard sx={{ p: { xs: 2.5, sm: 3 }, mb: 3 }}>
          <Typography sx={{ fontWeight: 700, fontSize: "1.05rem", color: brand.ink }}>Cierre diario</Typography>

          <SectionLabel>Bolsas / Monedas / Billetes contados</SectionLabel>
          <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1.2 }}>
            {denominaciones.map((d) => (
              <TextField
                key={d.id}
                label={`${d.tipo} $${d.valor}`}
                type="number"
                size="small"
                sx={{ width: 145, ...glassFieldLight }}
                value={cantidades[d.id] ?? ""}
                onChange={(e) => setCantidades({ ...cantidades, [d.id]: Number(e.target.value) })}
              />
            ))}
          </Box>

          <SectionLabel>Otros</SectionLabel>
          <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1.2 }}>
            <TextField
              label="Terminal"
              type="number"
              size="small"
              sx={{ width: 145, ...glassFieldLight }}
              value={terminal || ""}
              onChange={(e) => setTerminal(Number(e.target.value))}
            />
            <TextField
              label="Transferencias"
              type="number"
              size="small"
              sx={{ width: 145, ...glassFieldLight }}
              value={transferencia || ""}
              onChange={(e) => setTransferencia(Number(e.target.value))}
            />
          </Box>

          <SectionLabel>Premios encontrados</SectionLabel>
          <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1.2 }}>
            {premios.map((p) => (
              <TextField
                key={p.id}
                label={p.nombre}
                type="number"
                size="small"
                sx={{ width: 145, ...glassFieldLight }}
                value={cantidades[p.id] ?? ""}
                onChange={(e) => setCantidades({ ...cantidades, [p.id]: Number(e.target.value) })}
              />
            ))}
          </Box>

          <Box sx={{ mt: 3 }}>
            <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 1 }}>
              <Typography sx={{ fontSize: "0.75rem", fontWeight: 700, color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px" }}>
                Gastos del día
              </Typography>
              <Box component="button" onClick={() => setGastoOpen(true)} sx={{ ...pillOutlineButtonSx, py: 0.5, px: 1.3, fontSize: "0.75rem" }}>
                <AddRoundedIcon sx={{ fontSize: 15 }} />
                Agregar gasto
              </Box>
            </Box>
            <Box sx={{ overflowX: "auto" }}>
              <Table size="small" sx={glassTableSx}>
                <TableBody>
                  {gastos.map((g) => (
                    <TableRow key={g.id}>
                      <TableCell sx={{ fontWeight: 600 }}>
                        {g.descripcion}
                        <Typography component="div" sx={{ fontSize: "0.72rem", color: brand.inkMuted, fontWeight: 400 }}>
                          {g.categoriaGastoNombre}
                        </Typography>
                      </TableCell>
                      <TableCell align="right">${g.monto.toLocaleString()}</TableCell>
                      <TableCell align="right">
                        <StatusPill label={g.tieneEvidencia ? "Con evidencia" : "Sin evidencia"} tone={g.tieneEvidencia ? "success" : "neutral"} />
                      </TableCell>
                    </TableRow>
                  ))}
                  {gastos.length === 0 && (
                    <TableRow>
                      <TableCell colSpan={3} sx={{ textAlign: "center", color: brand.inkMuted, py: 3 }}>
                        Sin gastos registrados hoy.
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </Box>
          </Box>

          <Box
            sx={{
              mt: 3,
              pt: 2,
              borderTop: "1px solid rgba(14,23,48,0.08)",
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Typography sx={{ fontSize: "0.75rem", fontWeight: 700, color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px" }}>
              Total capturado hasta ahora
            </Typography>
            <Typography sx={{ fontSize: "1.25rem", fontWeight: 800, color: brand.goldDark }}>
              ${totalCapturado.toLocaleString()}
            </Typography>
          </Box>

          <Box sx={{ mt: 2 }}>
            <Box component="button" onClick={registrarCierre} sx={{ ...pillButtonSx, py: 1.3, px: 3, fontSize: "0.95rem" }}>
              Registrar cierre diario
            </Box>
          </Box>
        </GlassCard>
      )}

      {resultado && (
        <GlassCard sx={{ p: { xs: 2.5, sm: 3 } }}>
          <Typography sx={{ fontWeight: 700, fontSize: "1.05rem", color: brand.ink, mb: 2 }}>
            Resultado del cierre
          </Typography>
          <Box sx={{ display: "flex", gap: 4, flexWrap: "wrap", alignItems: "flex-start" }}>
            <Box>
              <Typography sx={{ fontSize: "0.68rem", color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px" }}>
                Total esperado
              </Typography>
              <Typography sx={{ fontSize: "1.15rem", fontWeight: 800, color: brand.ink }}>
                ${resultado.totalEsperado.toLocaleString()}
              </Typography>
            </Box>
            <Box>
              <Typography sx={{ fontSize: "0.68rem", color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px" }}>
                Total reportado
              </Typography>
              <Typography sx={{ fontSize: "1.15rem", fontWeight: 800, color: brand.ink }}>
                ${resultado.totalReportado.toLocaleString()}
              </Typography>
            </Box>
            <Box>
              <Typography sx={{ fontSize: "0.68rem", color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px" }}>
                Diferencia
              </Typography>
              <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                <Typography sx={{ fontSize: "1.15rem", fontWeight: 800, color: brand.ink }}>
                  ${resultado.diferencia.toLocaleString()}
                </Typography>
                <StatusPill label={resultado.estado} tone={resultado.estado === "Correcto" ? "success" : "warning"} />
              </Box>
            </Box>
          </Box>
        </GlassCard>
      )}

      <GastoDialog
        open={gastoOpen}
        localId={localId}
        onClose={() => setGastoOpen(false)}
        onGuardado={() => {
          setGastoOpen(false);
          cargar();
        }}
      />
    </Box>
  );
}

function GastoDialog({
  open,
  localId,
  onClose,
  onGuardado,
}: {
  open: boolean;
  localId: string;
  onClose: () => void;
  onGuardado: () => void;
}) {
  const [descripcion, setDescripcion] = useState("");
  const [categorias, setCategorias] = useState<CategoriaGasto[]>([]);
  const [categoriaGastoId, setCategoriaGastoId] = useState("");
  const [monto, setMonto] = useState<number>(0);
  const [archivo, setArchivo] = useState<File | null>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (!open) return;
    api.get<CategoriaGasto[]>("/catalogos/categorias-gasto").then((r) => {
      setCategorias(r.data);
      setCategoriaGastoId((prev) => prev || r.data[0]?.id || "");
    });
  }, [open]);

  const guardar = async () => {
    setSaving(true);
    try {
      const formData = new FormData();
      formData.append("LocalId", localId);
      formData.append("Descripcion", descripcion);
      formData.append("CategoriaGastoId", categoriaGastoId);
      formData.append("Monto", String(monto));
      formData.append("Fecha", new Date().toISOString().slice(0, 10));
      if (archivo) formData.append("evidencias", archivo);

      await api.post("/gastos", formData, { headers: { "Content-Type": "multipart/form-data" } });
      setDescripcion("");
      setMonto(0);
      setArchivo(null);
      onGuardado();
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog
      open={open}
      onClose={onClose}
      fullWidth
      maxWidth="xs"
      slotProps={{ paper: { sx: dialogPaperSx }, backdrop: { sx: dialogBackdropSx } }}
    >
      <Box sx={{ p: 3.5 }}>
        <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 2.5 }}>
          <Box sx={{ display: "flex", alignItems: "center", gap: 1.2 }}>
            <Box
              sx={{
                width: 38,
                height: 38,
                borderRadius: "12px",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                background: `linear-gradient(135deg, ${brand.navySoft} 0%, ${brand.navy} 100%)`,
              }}
            >
              <ReceiptLongRoundedIcon sx={{ color: "#fff", fontSize: 20 }} />
            </Box>
            <Typography sx={{ fontWeight: 800, fontSize: "1.15rem", color: brand.ink }}>Registrar gasto</Typography>
          </Box>
          <IconButton size="small" onClick={onClose} sx={{ color: brand.inkFaint }}>
            <CloseRoundedIcon fontSize="small" />
          </IconButton>
        </Box>

        <TextField
          label="Descripción"
          fullWidth
          margin="dense"
          value={descripcion}
          onChange={(e) => setDescripcion(e.target.value)}
          sx={glassFieldLight}
          autoFocus
        />
        <FormControl fullWidth margin="dense" sx={glassFieldLight}>
          <InputLabel>Tipo de gasto</InputLabel>
          <Select label="Tipo de gasto" value={categoriaGastoId} onChange={(e) => setCategoriaGastoId(e.target.value)}>
            {categorias.map((c) => (
              <MenuItem key={c.id} value={c.id}>
                {c.nombre}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
        <TextField
          label="Monto"
          type="number"
          fullWidth
          margin="dense"
          value={monto || ""}
          onChange={(e) => setMonto(Number(e.target.value))}
          sx={glassFieldLight}
        />
        <Box component="label" sx={{ ...pillOutlineButtonSx, mt: 1.5, width: "100%", justifyContent: "center" }}>
          {archivo ? archivo.name : "Adjuntar evidencia (foto del ticket)"}
          <input type="file" accept="image/*" hidden onChange={(e) => setArchivo(e.target.files?.[0] ?? null)} />
        </Box>

        <Box
          component="button"
          onClick={guardar}
          disabled={!descripcion || !categoriaGastoId || monto <= 0 || saving}
          sx={{ ...pillButtonSx, width: "100%", justifyContent: "center", mt: 3, py: 1.3, fontSize: "0.95rem" }}
        >
          {saving ? "Guardando..." : "Guardar gasto"}
        </Box>
      </Box>
    </Dialog>
  );
}
