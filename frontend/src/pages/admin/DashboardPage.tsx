import { useEffect, useState } from "react";
import { Box, Grid, Paper, Typography } from "@mui/material";
import { api } from "../../api/client";
import type { DashboardData } from "../../types";

function Stat({ label, value, color }: { label: string; value: number | string; color?: string }) {
  return (
    <Paper sx={{ p: 2, textAlign: "center" }}>
      <Typography variant="h4" color={color}>
        {value}
      </Typography>
      <Typography variant="body2" color="text.secondary">
        {label}
      </Typography>
    </Paper>
  );
}

export function DashboardPage() {
  const [data, setData] = useState<DashboardData | null>(null);

  useEffect(() => {
    api.get<DashboardData>("/dashboard").then((r) => setData(r.data));
  }, []);

  if (!data) return null;

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>

      <Grid container spacing={2}>
        <Grid size={{ xs: 6, sm: 3 }}>
          <Stat label="Locales" value={data.locales} />
        </Grid>
        <Grid size={{ xs: 6, sm: 3 }}>
          <Stat label="Máquinas" value={data.maquinas} />
        </Grid>
        <Grid size={{ xs: 6, sm: 3 }}>
          <Stat label="Empleados" value={data.empleados} />
        </Grid>
        <Grid size={{ xs: 6, sm: 3 }}>
          <Stat label="Gastos sin evidencia" value={data.gastosPendientesEvidencia} color="warning.main" />
        </Grid>
        <Grid size={{ xs: 6, sm: 3 }}>
          <Stat label="Cierres correctos" value={data.cierresCorrectos} color="success.main" />
        </Grid>
        <Grid size={{ xs: 6, sm: 3 }}>
          <Stat label="Cierres con diferencia" value={data.cierresConDiferencia} color="warning.main" />
        </Grid>
        <Grid size={{ xs: 6, sm: 3 }}>
          <Stat label="Máquinas averiadas" value={data.maquinasAveriadas} color="error.main" />
        </Grid>
        <Grid size={{ xs: 6, sm: 3 }}>
          <Stat label="En reparación" value={data.maquinasEnReparacion} color="warning.main" />
        </Grid>
      </Grid>

      <Paper sx={{ p: 3, mt: 3 }}>
        <Typography variant="h6" gutterBottom>
          Total acumulado por local (semana actual)
        </Typography>
        {data.totalesPorLocal.map((t) => (
          <Box key={t.localId} sx={{ display: "flex", justifyContent: "space-between", py: 0.5 }}>
            <Typography>
              {t.localNombre} (semana #{t.semanaNumero})
            </Typography>
            <Typography sx={{ fontWeight: "bold" }}>${t.totalAcumulado.toLocaleString()}</Typography>
          </Box>
        ))}
        {data.totalesPorLocal.length === 0 && (
          <Typography color="text.secondary">Ningún local tiene cuenta configurada todavía.</Typography>
        )}
      </Paper>
    </Box>
  );
}
