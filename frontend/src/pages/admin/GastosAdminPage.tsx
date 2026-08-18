import { useEffect, useState } from "react";
import { Box, FormControl, InputLabel, MenuItem, Select, Table, TableBody, TableCell, TableHead, TableRow } from "@mui/material";
import { api } from "../../api/client";
import { EvidenciaLink } from "../../components/EvidenciaLink";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { StatusPill } from "../../components/StatusPill";
import { brand, glassFieldLight, glassTableSx } from "../../theme/brand";
import { TIPO_GASTO_LABEL } from "../../utils/cuenta";
import type { Gasto, Local } from "../../types";

export function GastosAdminPage() {
  const [locales, setLocales] = useState<Local[]>([]);
  const [localId, setLocalId] = useState("");
  const [gastos, setGastos] = useState<Gasto[]>([]);

  useEffect(() => {
    api.get<Local[]>("/locales").then((r) => {
      setLocales(r.data);
      if (r.data.length > 0) setLocalId(r.data[0].id);
    });
  }, []);

  useEffect(() => {
    if (!localId) return;
    api.get<Gasto[]>(`/gastos/local/${localId}`).then((r) => setGastos(r.data));
  }, [localId]);

  const total = gastos.reduce((sum, g) => sum + g.monto, 0);

  return (
    <Box>
      <PageHeader
        title="Gastos"
        subtitle={`${gastos.length} gasto${gastos.length === 1 ? "" : "s"} · $${total.toLocaleString()} acumulado`}
        action={
          <FormControl size="small" sx={{ minWidth: 200, ...glassFieldLight }}>
            <InputLabel>Local</InputLabel>
            <Select label="Local" value={localId} onChange={(e) => setLocalId(e.target.value)}>
              {locales.map((l) => (
                <MenuItem key={l.id} value={l.id}>
                  {l.nombre}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        }
      />

      <GlassCard sx={{ p: { xs: 1, sm: 2 }, overflowX: "auto" }}>
        <Table sx={glassTableSx}>
          <TableHead>
            <TableRow>
              <TableCell>Fecha</TableCell>
              <TableCell>Descripción</TableCell>
              <TableCell>Tipo</TableCell>
              <TableCell align="right">Monto</TableCell>
              <TableCell>Empleado</TableCell>
              <TableCell>Evidencia</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {gastos.map((g) => (
              <TableRow key={g.id}>
                <TableCell sx={{ color: brand.inkMuted, whiteSpace: "nowrap" }}>{g.fecha}</TableCell>
                <TableCell sx={{ fontWeight: 600 }}>{g.descripcion}</TableCell>
                <TableCell sx={{ color: brand.inkMuted, whiteSpace: "nowrap" }}>{TIPO_GASTO_LABEL[g.tipo]}</TableCell>
                <TableCell align="right" sx={{ fontWeight: 700 }}>
                  ${g.monto.toLocaleString()}
                </TableCell>
                <TableCell>{g.empleadoNombre}</TableCell>
                <TableCell>
                  {g.evidenciaUrls.length === 0 && <StatusPill label="Sin evidencia" tone="neutral" />}
                  {g.evidenciaUrls.map((url, i) => (
                    <EvidenciaLink key={i} url={url} label={`Foto ${i + 1}`} />
                  ))}
                </TableCell>
              </TableRow>
            ))}
            {gastos.length === 0 && (
              <TableRow>
                <TableCell colSpan={6} sx={{ textAlign: "center", color: brand.inkMuted, py: 4 }}>
                  Sin gastos registrados en este local.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </GlassCard>
    </Box>
  );
}
