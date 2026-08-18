import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Box, Table, TableBody, TableCell, TableHead, TableRow, TextField, Typography } from "@mui/material";
import ArrowBackRoundedIcon from "@mui/icons-material/ArrowBackRounded";
import { api } from "../../api/client";
import { GlassCard } from "../../components/GlassCard";
import { brand, glassFieldLight, glassTableSx, pillButtonSx, pillOutlineButtonSx } from "../../theme/brand";
import type { CorteMaquina, Denominacion, Maquina, MaquinaPremioConfig } from "../../types";

function SectionLabel({ children }: { children: React.ReactNode }) {
  return (
    <Typography sx={{ fontSize: "0.75rem", fontWeight: 700, color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px", mb: 1, mt: 2.5 }}>
      {children}
    </Typography>
  );
}

const valorDenominacion = (d: Denominacion) => (d.tipo === "Bolsa" ? d.valorPorBolsa ?? 0 : d.valor);

export function CortesMaquinaAdminPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [maquina, setMaquina] = useState<Maquina | null>(null);
  const [premiosConfig, setPremiosConfig] = useState<MaquinaPremioConfig[]>([]);
  const [denominaciones, setDenominaciones] = useState<Denominacion[]>([]);
  const [cantidades, setCantidades] = useState<Record<string, number>>({});
  const [comentario, setComentario] = useState("");
  const [saving, setSaving] = useState(false);
  const [cortes, setCortes] = useState<CorteMaquina[]>([]);
  const [desde, setDesde] = useState("");
  const [hasta, setHasta] = useState("");

  useEffect(() => {
    if (!id) return;
    Promise.all([
      api.get<Maquina>(`/maquinas/${id}`),
      api.get<MaquinaPremioConfig[]>(`/maquinas/${id}/premios`),
      api.get<Denominacion[]>("/catalogos/denominaciones"),
    ]).then(([maquinaRes, premiosRes, denomRes]) => {
      setMaquina(maquinaRes.data);
      setPremiosConfig(premiosRes.data);
      setDenominaciones(denomRes.data);
    });
  }, [id]);

  const cargarCortes = () => {
    if (!id) return;
    api
      .get<CorteMaquina[]>(`/maquinas/${id}/cortes`, { params: { desde: desde || undefined, hasta: hasta || undefined } })
      .then((r) => setCortes(r.data));
  };

  useEffect(cargarCortes, [id, desde, hasta]);

  const totalCapturado = useMemo(() => {
    const totalPremios = premiosConfig.reduce((sum, p) => sum + (cantidades[p.premioId] ?? 0) * p.premioDenominacion, 0);
    const totalDenominaciones = denominaciones.reduce((sum, d) => sum + (cantidades[d.id] ?? 0) * valorDenominacion(d), 0);
    return totalPremios + totalDenominaciones;
  }, [premiosConfig, denominaciones, cantidades]);

  const totalHistorico = cortes.reduce((sum, c) => sum + c.total, 0);

  const registrar = async () => {
    if (!id) return;
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
      await api.post(`/maquinas/${id}/cortes`, {
        fecha: new Date().toISOString().slice(0, 10),
        comentario: comentario || null,
        lineas,
      });
      setCantidades({});
      setComentario("");
      cargarCortes();
    } finally {
      setSaving(false);
    }
  };

  return (
    <Box>
      <Box
        component="button"
        onClick={() => navigate("/admin/maquinas")}
        sx={{ ...pillOutlineButtonSx, mb: 2.5, py: 0.6, px: 1.4, fontSize: "0.78rem" }}
      >
        <ArrowBackRoundedIcon sx={{ fontSize: 16 }} />
        Máquinas
      </Box>

      <Typography sx={{ fontSize: { xs: "1.5rem", sm: "1.75rem" }, fontWeight: 800, color: brand.ink, mb: 3 }}>
        Cortes · {maquina?.nombre ?? "Máquina"}
      </Typography>

      <GlassCard sx={{ p: { xs: 2.5, sm: 3 }, mb: 3 }}>
        <Typography sx={{ fontWeight: 700, color: brand.ink, fontSize: "0.95rem" }}>Registrar nuevo corte</Typography>

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
          placeholder={`Notas sobre el corte de ${maquina?.nombre ?? "la máquina"}...`}
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

      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-end", flexWrap: "wrap", gap: 2, mb: 2 }}>
        <Typography sx={{ fontWeight: 700, color: brand.ink, fontSize: "1.05rem" }}>Historial de cortes</Typography>
        <Box sx={{ display: "flex", gap: 1.2, flexWrap: "wrap" }}>
          <TextField
            label="Desde"
            type="date"
            size="small"
            slotProps={{ inputLabel: { shrink: true } }}
            value={desde}
            onChange={(e) => setDesde(e.target.value)}
            sx={{ width: 165, ...glassFieldLight }}
          />
          <TextField
            label="Hasta"
            type="date"
            size="small"
            slotProps={{ inputLabel: { shrink: true } }}
            value={hasta}
            onChange={(e) => setHasta(e.target.value)}
            sx={{ width: 165, ...glassFieldLight }}
          />
        </Box>
      </Box>

      <GlassCard sx={{ p: { xs: 1, sm: 2 }, overflowX: "auto" }}>
        <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "baseline", px: 1, pt: 1, mb: 1 }}>
          <Typography sx={{ fontWeight: 700, color: brand.ink, fontSize: "0.95rem" }}>
            {cortes.length} corte{cortes.length === 1 ? "" : "s"}
          </Typography>
          <Typography sx={{ fontWeight: 800, color: "#1b7a4d", fontSize: "1.1rem" }}>
            Total: ${totalHistorico.toLocaleString()}
          </Typography>
        </Box>
        <Table sx={glassTableSx}>
          <TableHead>
            <TableRow>
              <TableCell>Fecha</TableCell>
              <TableCell>Registrado por</TableCell>
              <TableCell>Detalle</TableCell>
              <TableCell>Comentario</TableCell>
              <TableCell align="right">Total</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {cortes.map((c) => (
              <TableRow key={c.id}>
                <TableCell sx={{ whiteSpace: "nowrap" }}>{c.fecha}</TableCell>
                <TableCell>{c.registradoPorNombre}</TableCell>
                <TableCell sx={{ color: brand.inkMuted, fontSize: "0.78rem" }}>
                  {c.detalles.map((d, i) => (
                    <Box key={i}>
                      {d.premioNombre ?? d.denominacionNombre} × {d.cantidad}
                    </Box>
                  ))}
                </TableCell>
                <TableCell sx={{ color: brand.inkMuted, fontSize: "0.8rem", maxWidth: 220 }}>{c.comentario ?? "—"}</TableCell>
                <TableCell align="right" sx={{ fontWeight: 700 }}>
                  ${c.total.toLocaleString()}
                </TableCell>
              </TableRow>
            ))}
            {cortes.length === 0 && (
              <TableRow>
                <TableCell colSpan={5} sx={{ textAlign: "center", color: brand.inkMuted, py: 4 }}>
                  Sin cortes registrados en este rango.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </GlassCard>
    </Box>
  );
}
