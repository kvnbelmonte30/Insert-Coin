import { useEffect, useMemo, useState } from "react";
import { Box, FormControl, MenuItem, Select, Table, TableBody, TableCell, TableHead, TableRow, Typography } from "@mui/material";
import StorefrontRoundedIcon from "@mui/icons-material/StorefrontRounded";
import CasinoRoundedIcon from "@mui/icons-material/CasinoRounded";
import GroupRoundedIcon from "@mui/icons-material/GroupRounded";
import ReceiptLongRoundedIcon from "@mui/icons-material/ReceiptLongRounded";
import CheckCircleRoundedIcon from "@mui/icons-material/CheckCircleRounded";
import ReportProblemRoundedIcon from "@mui/icons-material/ReportProblemRounded";
import BuildRoundedIcon from "@mui/icons-material/BuildRounded";
import PersonRoundedIcon from "@mui/icons-material/PersonRounded";
import ScheduleRoundedIcon from "@mui/icons-material/ScheduleRounded";
import { api } from "../../api/client";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { StatusPill } from "../../components/StatusPill";
import { DonutChart } from "../../components/charts/DonutChart";
import { BarRows } from "../../components/charts/BarRows";
import { TrendChart } from "../../components/charts/TrendChart";
import { brand, glassFieldLight, glassTableSx } from "../../theme/brand";
import { useAuthStore } from "../../store/authStore";
import type { DashboardData, PropietarioDashboardData, PropietarioResumen } from "../../types";

const ESTADO_COLOR: Record<string, string> = {
  Funcional: "#1b7a4d",
  Reportada: "#c02b3c",
  "En reparación": "#b06a00",
  Reparada: "#2158b0",
  "Fuera de servicio": "rgba(14,23,48,0.35)",
};

const ESTADO_TONE: Record<string, "success" | "warning" | "error" | "neutral" | "info"> = {
  Funcional: "success",
  Reportada: "error",
  "En reparación": "warning",
  Reparada: "info",
  "Fuera de servicio": "neutral",
};

function StatCard({
  icon,
  label,
  value,
  tone = "neutral",
}: {
  icon: React.ReactElement;
  label: string;
  value: number | string;
  tone?: "success" | "warning" | "error" | "info" | "neutral";
}) {
  const toneColor: Record<string, string> = {
    success: "#1b7a4d",
    warning: "#b06a00",
    error: "#c02b3c",
    info: "#2158b0",
    neutral: brand.ink,
  };
  return (
    <GlassCard sx={{ p: 2.2, display: "flex", flexDirection: "column", gap: 1, height: "100%" }}>
      <Box
        sx={{
          width: 34,
          height: 34,
          borderRadius: "10px",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          background: "rgba(14,23,48,0.05)",
          color: toneColor[tone],
        }}
      >
        {icon}
      </Box>
      <Typography sx={{ fontSize: "1.6rem", fontWeight: 800, color: toneColor[tone], lineHeight: 1 }}>{value}</Typography>
      <Typography sx={{ fontSize: "0.78rem", color: brand.inkMuted }}>{label}</Typography>
    </GlassCard>
  );
}

function SectionTitle({ children }: { children: React.ReactNode }) {
  return (
    <Typography sx={{ fontSize: "1.05rem", fontWeight: 800, color: brand.ink, mb: 2 }}>{children}</Typography>
  );
}

export function DashboardPage() {
  const { nombre } = useAuthStore();
  const [data, setData] = useState<DashboardData | null>(null);

  const [propietarios, setPropietarios] = useState<PropietarioResumen[]>([]);
  const [propietarioId, setPropietarioId] = useState("");
  const [propData, setPropData] = useState<PropietarioDashboardData | null>(null);
  const [propLoading, setPropLoading] = useState(false);
  const [miPropietarioId, setMiPropietarioId] = useState<string | null>(null);

  useEffect(() => {
    api.get<DashboardData>("/dashboard").then((r) => setData(r.data));
    api.get<PropietarioResumen[]>("/dashboard/propietarios").then((r) => setPropietarios(r.data));

    api
      .get<PropietarioDashboardData>("/dashboard/mi-propietario")
      .then((r) => {
        setMiPropietarioId(r.data.propietarioId);
        setPropietarioId(r.data.propietarioId);
      })
      .catch(() => {
        // el admin actual no tiene propietario asociado; se deja que elija manualmente
      });
  }, []);

  useEffect(() => {
    if (!propietarioId) return;
    setPropLoading(true);
    api
      .get<PropietarioDashboardData>(`/dashboard/propietario/${propietarioId}`)
      .then((r) => setPropData(r.data))
      .finally(() => setPropLoading(false));
  }, [propietarioId]);

  useEffect(() => {
    if (!propietarioId && propietarios.length > 0 && !miPropietarioId) {
      setPropietarioId(propietarios[0].id);
    }
  }, [propietarios, propietarioId, miPropietarioId]);

  const maquinasPorEstadoSegments = useMemo(
    () =>
      (propData?.maquinasPorEstado ?? []).map((c) => ({
        label: c.etiqueta,
        value: c.cantidad,
        color: ESTADO_COLOR[c.etiqueta] ?? brand.gold,
      })),
    [propData]
  );

  const tendenciaPoints = useMemo(
    () => (propData?.averiasPorSemana ?? []).map((t) => ({ label: t.periodo, value: t.cantidad })),
    [propData]
  );

  const esMiPropietario = miPropietarioId !== null && propietarioId === miPropietarioId;

  return (
    <Box>
      <PageHeader title="Dashboard" subtitle={`Bienvenido, ${nombre ?? "administrador"}`} />

      {/* Resumen general del sistema */}
      {data && (
        <Box sx={{ mb: 4.5 }}>
          <SectionTitle>Resumen general</SectionTitle>
          <Box
            sx={{
              display: "grid",
              gridTemplateColumns: { xs: "repeat(2, 1fr)", sm: "repeat(4, 1fr)" },
              gap: 1.6,
              mb: 2,
            }}
          >
            <StatCard icon={<StorefrontRoundedIcon fontSize="small" />} label="Locales" value={data.locales} />
            <StatCard icon={<CasinoRoundedIcon fontSize="small" />} label="Máquinas" value={data.maquinas} />
            <StatCard icon={<GroupRoundedIcon fontSize="small" />} label="Empleados" value={data.empleados} />
            <StatCard
              icon={<ReceiptLongRoundedIcon fontSize="small" />}
              label="Gastos sin evidencia"
              value={data.gastosPendientesEvidencia}
              tone="warning"
            />
            <StatCard
              icon={<CheckCircleRoundedIcon fontSize="small" />}
              label="Cierres correctos"
              value={data.cierresCorrectos}
              tone="success"
            />
            <StatCard
              icon={<ReportProblemRoundedIcon fontSize="small" />}
              label="Cierres con diferencia"
              value={data.cierresConDiferencia}
              tone="warning"
            />
            <StatCard
              icon={<ReportProblemRoundedIcon fontSize="small" />}
              label="Máquinas averiadas"
              value={data.maquinasAveriadas}
              tone="error"
            />
            <StatCard
              icon={<BuildRoundedIcon fontSize="small" />}
              label="En reparación"
              value={data.maquinasEnReparacion}
              tone="warning"
            />
          </Box>

          <GlassCard sx={{ p: { xs: 2, sm: 2.5 } }}>
            <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "baseline", mb: 2 }}>
              <Typography sx={{ fontWeight: 700, color: brand.ink, fontSize: "0.95rem" }}>
                Cuenta de la semana por local
              </Typography>
              <Box sx={{ textAlign: "right" }}>
                <Typography sx={{ fontSize: "0.68rem", color: brand.inkMuted, textTransform: "uppercase", letterSpacing: "0.4px" }}>
                  Total general
                </Typography>
                <Typography sx={{ fontWeight: 800, color: "#1b7a4d", fontSize: "1.3rem", lineHeight: 1.1 }}>
                  ${data.totalAcumuladoSemanaActual.toLocaleString()}
                </Typography>
              </Box>
            </Box>
            {data.totalesPorLocal.map((t) => (
              <Box key={t.localId} sx={{ display: "flex", justifyContent: "space-between", py: 0.7 }}>
                <Typography sx={{ fontSize: "0.85rem", color: brand.inkMuted }}>
                  {t.localNombre} (semana #{t.semanaNumero})
                </Typography>
                <Typography sx={{ fontWeight: 700, color: brand.ink, fontSize: "0.85rem" }}>
                  ${t.totalAcumulado.toLocaleString()}
                </Typography>
              </Box>
            ))}
            {data.totalesPorLocal.length === 0 && (
              <Typography sx={{ color: brand.inkMuted, fontSize: "0.85rem" }}>
                Ningún local tiene cuenta configurada todavía.
              </Typography>
            )}
          </GlassCard>
        </Box>
      )}

      {/* Vista ejecutiva por propietario */}
      <Box>
        <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", flexWrap: "wrap", gap: 1.5, mb: 2 }}>
          <SectionTitle>
            {esMiPropietario ? "Mis máquinas" : "Máquinas por propietario"}
          </SectionTitle>
          <FormControl size="small" sx={{ minWidth: 220, ...glassFieldLight }}>
            <Select
              value={propietarioId}
              onChange={(e) => setPropietarioId(e.target.value)}
              displayEmpty
              renderValue={(v) => {
                const p = propietarios.find((x) => x.id === v);
                const propioSufijo = v === miPropietarioId ? " (yo)" : "";
                return p ? `${p.nombre}${propioSufijo}` : "Selecciona un propietario";
              }}
            >
              {propietarios.map((p) => (
                <MenuItem key={p.id} value={p.id}>
                  <PersonRoundedIcon sx={{ fontSize: 16, mr: 1, color: brand.inkFaint }} />
                  {p.nombre}
                  {p.id === miPropietarioId ? " (yo)" : ""} · {p.totalMaquinas} máquina{p.totalMaquinas === 1 ? "" : "s"}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Box>

        {!propietarioId && (
          <GlassCard sx={{ p: 3, textAlign: "center" }}>
            <Typography sx={{ color: brand.inkMuted }}>Aún no hay propietarios registrados.</Typography>
          </GlassCard>
        )}

        {propietarioId && propData && (
          <>
            <Box
              sx={{
                display: "grid",
                gridTemplateColumns: { xs: "repeat(2, 1fr)", sm: "repeat(3, 1fr)" },
                gap: 1.6,
                mb: 2,
                opacity: propLoading ? 0.6 : 1,
                transition: "opacity 0.15s ease",
              }}
            >
              <StatCard icon={<CasinoRoundedIcon fontSize="small" />} label="Máquinas activas" value={propData.totalMaquinas} />
              <StatCard
                icon={<ReportProblemRoundedIcon fontSize="small" />}
                label="Averías (30 días)"
                value={propData.averiasUltimos30Dias}
                tone={propData.averiasUltimos30Dias > 0 ? "error" : "success"}
              />
              <StatCard
                icon={<ScheduleRoundedIcon fontSize="small" />}
                label="Tiempo prom. reparación"
                value={propData.tiempoPromedioReparacionHoras !== null ? `${propData.tiempoPromedioReparacionHoras} h` : "—"}
                tone="info"
              />
            </Box>

            <Box sx={{ display: "grid", gridTemplateColumns: { xs: "1fr", md: "1fr 1fr" }, gap: 2, mb: 2, "& > *": { minWidth: 0 } }}>
              <GlassCard sx={{ p: { xs: 2, sm: 2.5 } }}>
                <Typography sx={{ fontWeight: 700, color: brand.ink, mb: 2, fontSize: "0.95rem" }}>
                  Estado de sus máquinas
                </Typography>
                <DonutChart segments={maquinasPorEstadoSegments} centerLabel="Máquinas" centerValue={propData.totalMaquinas} />
              </GlassCard>

              <GlassCard sx={{ p: { xs: 2, sm: 2.5 } }}>
                <Typography sx={{ fontWeight: 700, color: brand.ink, mb: 2, fontSize: "0.95rem" }}>
                  Averías reportadas · últimas 8 semanas
                </Typography>
                <TrendChart points={tendenciaPoints} />
              </GlassCard>
            </Box>

            <Box sx={{ display: "grid", gridTemplateColumns: { xs: "1fr", md: "1fr 1fr" }, gap: 2, mb: 2, "& > *": { minWidth: 0 } }}>
              <GlassCard sx={{ p: { xs: 2, sm: 2.5 } }}>
                <Typography sx={{ fontWeight: 700, color: brand.ink, mb: 2, fontSize: "0.95rem" }}>
                  Máquinas por tipo
                </Typography>
                <BarRows rows={propData.maquinasPorTipo.map((c) => ({ label: c.etiqueta, value: c.cantidad }))} />
              </GlassCard>

              <GlassCard sx={{ p: { xs: 2, sm: 2.5 } }}>
                <Typography sx={{ fontWeight: 700, color: brand.ink, mb: 2, fontSize: "0.95rem" }}>
                  Máquinas con más reportes (30 días)
                </Typography>
                <BarRows
                  rows={propData.maquinas
                    .filter((m) => m.averiasUltimos30Dias > 0)
                    .slice(0, 5)
                    .map((m) => ({ label: m.nombre, value: m.averiasUltimos30Dias, sublabel: "reportes" }))}
                  emptyLabel="Sin averías reportadas en los últimos 30 días."
                />
              </GlassCard>
            </Box>

            <GlassCard sx={{ p: { xs: 1, sm: 2 }, overflowX: "auto" }}>
              <Typography sx={{ fontWeight: 700, color: brand.ink, px: 1, pt: 1, mb: 1, fontSize: "0.95rem" }}>
                Sus máquinas
              </Typography>
              <Table sx={glassTableSx}>
                <TableHead>
                  <TableRow>
                    <TableCell>Nombre</TableCell>
                    <TableCell>Tipo</TableCell>
                    <TableCell>Local</TableCell>
                    <TableCell>Estado</TableCell>
                    <TableCell align="right">Averías (30 días)</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {propData.maquinas.map((m) => (
                    <TableRow key={m.id}>
                      <TableCell sx={{ fontWeight: 600 }}>{m.nombre}</TableCell>
                      <TableCell>{m.tipoNombre}</TableCell>
                      <TableCell sx={{ color: brand.inkMuted }}>{m.localNombre}</TableCell>
                      <TableCell>
                        <StatusPill label={m.estado} tone={ESTADO_TONE[m.estado] ?? "neutral"} />
                      </TableCell>
                      <TableCell align="right" sx={{ fontWeight: m.averiasUltimos30Dias > 0 ? 700 : 400 }}>
                        {m.averiasUltimos30Dias}
                      </TableCell>
                    </TableRow>
                  ))}
                  {propData.maquinas.length === 0 && (
                    <TableRow>
                      <TableCell colSpan={5} sx={{ textAlign: "center", color: brand.inkMuted, py: 4 }}>
                        Este propietario no tiene máquinas activas.
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </GlassCard>
          </>
        )}
      </Box>
    </Box>
  );
}
