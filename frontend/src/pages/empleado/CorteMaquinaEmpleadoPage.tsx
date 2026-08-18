import { useEffect, useMemo, useState } from "react";
import {
  Box,
  FormControl,
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
import ContentCutRoundedIcon from "@mui/icons-material/ContentCutRounded";
import { api } from "../../api/client";
import { useAuthStore } from "../../store/authStore";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { brand, glassFieldLight, glassTableSx, pillButtonSx } from "../../theme/brand";
import type { CorteMaquina, Denominacion, Maquina, MaquinaPremioConfig } from "../../types";

function SectionLabel({ children }: { children: React.ReactNode }) {
  return (
    <Typography sx={{ fontSize: "0.75rem", fontWeight: 700, color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px", mb: 1, mt: 2.5 }}>
      {children}
    </Typography>
  );
}

const valorDenominacion = (d: Denominacion) => (d.tipo === "Bolsa" ? d.valorPorBolsa ?? 0 : d.valor);

export function CorteMaquinaEmpleadoPage() {
  const locales = useAuthStore((s) => s.locales);
  const [localId, setLocalId] = useState(locales[0]?.id ?? "");
  const [maquinas, setMaquinas] = useState<Maquina[]>([]);
  const [maquinaId, setMaquinaId] = useState("");
  const [premiosConfig, setPremiosConfig] = useState<MaquinaPremioConfig[]>([]);
  const [denominaciones, setDenominaciones] = useState<Denominacion[]>([]);
  const [historial, setHistorial] = useState<CorteMaquina[]>([]);
  const [cantidades, setCantidades] = useState<Record<string, number>>({});
  const [comentario, setComentario] = useState("");
  const [resultado, setResultado] = useState<CorteMaquina | null>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (!localId) return;
    api.get<Maquina[]>("/maquinas", { params: { localId } }).then((r) => {
      setMaquinas(r.data);
      setMaquinaId(r.data[0]?.id ?? "");
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [localId]);

  useEffect(() => {
    if (!maquinaId) return;
    setResultado(null);
    setCantidades({});
    Promise.all([
      api.get<MaquinaPremioConfig[]>(`/maquinas/${maquinaId}/premios`),
      api.get<Denominacion[]>("/catalogos/denominaciones"),
      api.get<CorteMaquina[]>(`/maquinas/${maquinaId}/cortes`),
    ]).then(([premiosRes, denomRes, historialRes]) => {
      setPremiosConfig(premiosRes.data);
      setDenominaciones(denomRes.data);
      setHistorial(historialRes.data.slice(0, 5));
    });
  }, [maquinaId]);

  const totalCapturado = useMemo(() => {
    const totalPremios = premiosConfig.reduce((sum, p) => sum + (cantidades[p.premioId] ?? 0) * p.premioDenominacion, 0);
    const totalDenominaciones = denominaciones.reduce((sum, d) => sum + (cantidades[d.id] ?? 0) * valorDenominacion(d), 0);
    return totalPremios + totalDenominaciones;
  }, [premiosConfig, denominaciones, cantidades]);

  const registrar = async () => {
    const lineas = [
      ...premiosConfig
        .filter((p) => (cantidades[p.premioId] ?? 0) > 0)
        .map((p) => ({ premioId: p.premioId, cantidad: cantidades[p.premioId] })),
      ...denominaciones
        .filter((d) => (cantidades[d.id] ?? 0) > 0)
        .map((d) => ({ denominacionId: d.id, cantidad: cantidades[d.id] })),
    ];
    if (lineas.length === 0) return;

    setSaving(true);
    try {
      const { data } = await api.post<CorteMaquina>(`/maquinas/${maquinaId}/cortes`, {
        fecha: new Date().toISOString().slice(0, 10),
        comentario: comentario || null,
        lineas,
      });
      setResultado(data);
      setCantidades({});
      setComentario("");
      setHistorial((prev) => [data, ...prev].slice(0, 5));
    } finally {
      setSaving(false);
    }
  };

  const maquinaSeleccionada = maquinas.find((m) => m.id === maquinaId);

  return (
    <Box>
      <PageHeader
        title="Corte de máquina"
        subtitle="Registra lo que salió de una máquina específica: siempre se suma, nunca se resta."
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

      {maquinas.length === 0 ? (
        <GlassCard sx={{ p: 5, textAlign: "center" }}>
          <ContentCutRoundedIcon sx={{ fontSize: 38, color: brand.inkFaint, mb: 1.5 }} />
          <Typography sx={{ color: brand.inkMuted }}>Este local no tiene máquinas registradas.</Typography>
        </GlassCard>
      ) : (
        <GlassCard sx={{ p: { xs: 2.5, sm: 3 }, mb: 3 }}>
          <FormControl fullWidth margin="dense" sx={glassFieldLight}>
            <InputLabel>Máquina</InputLabel>
            <Select label="Máquina" value={maquinaId} onChange={(e) => setMaquinaId(e.target.value)}>
              {maquinas.map((m) => (
                <MenuItem key={m.id} value={m.id}>
                  {m.nombre} ({m.tipoMaquinaNombre})
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          {premiosConfig.length > 0 && (
            <>
              <SectionLabel>Premios que salieron de esta máquina</SectionLabel>
              <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1.2 }}>
                {premiosConfig.map((p) => (
                  <TextField
                    key={p.premioId}
                    label={p.premioNombre}
                    type="number"
                    size="small"
                    sx={{ width: 145, ...glassFieldLight }}
                    value={cantidades[p.premioId] ?? ""}
                    onChange={(e) => setCantidades({ ...cantidades, [p.premioId]: Number(e.target.value) })}
                  />
                ))}
              </Box>
            </>
          )}

          <SectionLabel>Efectivo extraído de esta máquina (si aplica)</SectionLabel>
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

          <SectionLabel>Comentario (opcional)</SectionLabel>
          <TextField
            fullWidth
            multiline
            minRows={2}
            placeholder={`Notas sobre el corte de ${maquinaSeleccionada?.nombre ?? "la máquina"}...`}
            value={comentario}
            onChange={(e) => setComentario(e.target.value)}
            sx={glassFieldLight}
          />

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
              Total del corte
            </Typography>
            <Typography sx={{ fontSize: "1.25rem", fontWeight: 800, color: brand.goldDark }}>
              ${totalCapturado.toLocaleString()}
            </Typography>
          </Box>

          <Box sx={{ mt: 2 }}>
            <Box
              component="button"
              onClick={registrar}
              disabled={totalCapturado <= 0 || saving}
              sx={{ ...pillButtonSx, py: 1.3, px: 3, fontSize: "0.95rem" }}
            >
              {saving ? "Registrando..." : "Registrar corte"}
            </Box>
          </Box>
        </GlassCard>
      )}

      {resultado && (
        <GlassCard sx={{ p: { xs: 2.5, sm: 3 }, mb: 3 }}>
          <Typography sx={{ fontWeight: 700, fontSize: "1.05rem", color: brand.ink, mb: 1.5 }}>
            Corte registrado · {resultado.maquinaNombre}
          </Typography>
          {resultado.detalles.map((d, i) => (
            <Box key={i} sx={{ display: "flex", justifyContent: "space-between", py: 0.5 }}>
              <Typography sx={{ fontSize: "0.85rem", color: brand.inkMuted }}>
                {d.premioNombre ?? d.denominacionNombre} × {d.cantidad}
              </Typography>
              <Typography sx={{ fontWeight: 600, color: brand.ink, fontSize: "0.85rem" }}>
                ${d.subtotal.toLocaleString()}
              </Typography>
            </Box>
          ))}
          <Box sx={{ display: "flex", justifyContent: "space-between", pt: 1.5, mt: 1, borderTop: "1px solid rgba(14,23,48,0.08)" }}>
            <Typography sx={{ fontWeight: 800, color: brand.ink }}>Total</Typography>
            <Typography sx={{ fontWeight: 800, color: brand.goldDark }}>${resultado.total.toLocaleString()}</Typography>
          </Box>
        </GlassCard>
      )}

      {maquinaId && historial.length > 0 && (
        <GlassCard sx={{ p: { xs: 1, sm: 2 }, overflowX: "auto" }}>
          <Typography sx={{ fontWeight: 700, color: brand.ink, px: 1, pt: 1, mb: 1, fontSize: "0.95rem" }}>
            Últimos cortes de esta máquina
          </Typography>
          <Table size="small" sx={glassTableSx}>
            <TableHead>
              <TableRow>
                <TableCell>Fecha</TableCell>
                <TableCell>Empleado</TableCell>
                <TableCell align="right">Total</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {historial.map((c) => (
                <TableRow key={c.id}>
                  <TableCell>{c.fecha}</TableCell>
                  <TableCell sx={{ color: brand.inkMuted }}>{c.empleadoNombre}</TableCell>
                  <TableCell align="right" sx={{ fontWeight: 700 }}>
                    ${c.total.toLocaleString()}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </GlassCard>
      )}
    </Box>
  );
}
