import { useEffect, useState } from "react";
import { Box, FormControl, InputLabel, MenuItem, Select, Table, TableBody, TableCell, TableHead, TableRow, TextField } from "@mui/material";
import { api } from "../../api/client";
import { EvidenciaLink } from "../../components/EvidenciaLink";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { StatusPill } from "../../components/StatusPill";
import { brand, glassFieldLight, glassTableSx } from "../../theme/brand";
import type { Local, ReporteAveria } from "../../types";

export function AveriasAdminPage() {
  const [locales, setLocales] = useState<Local[]>([]);
  const [localId, setLocalId] = useState("");
  const [reportes, setReportes] = useState<ReporteAveria[]>([]);
  const [desde, setDesde] = useState("");
  const [hasta, setHasta] = useState("");

  useEffect(() => {
    api.get<Local[]>("/locales").then((r) => {
      setLocales(r.data);
      if (r.data.length > 0) setLocalId(r.data[0].id);
    });
  }, []);

  useEffect(() => {
    if (!localId) return;
    api
      .get<ReporteAveria[]>(`/averias/local/${localId}`, { params: { desde: desde || undefined, hasta: hasta || undefined } })
      .then((r) => setReportes(r.data));
  }, [localId, desde, hasta]);

  return (
    <Box>
      <PageHeader
        title="Averías reportadas"
        subtitle={`${reportes.length} reporte${reportes.length === 1 ? "" : "s"}`}
        action={
          <>
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
            <TextField
              label="Desde"
              type="date"
              size="small"
              slotProps={{ inputLabel: { shrink: true } }}
              sx={{ width: 165, ...glassFieldLight }}
              value={desde}
              onChange={(e) => setDesde(e.target.value)}
            />
            <TextField
              label="Hasta"
              type="date"
              size="small"
              slotProps={{ inputLabel: { shrink: true } }}
              sx={{ width: 165, ...glassFieldLight }}
              value={hasta}
              onChange={(e) => setHasta(e.target.value)}
            />
          </>
        }
      />

      <GlassCard sx={{ p: { xs: 1, sm: 2 }, overflowX: "auto" }}>
        <Table sx={glassTableSx}>
          <TableHead>
            <TableRow>
              <TableCell>Fecha</TableCell>
              <TableCell>Máquina</TableCell>
              <TableCell>Problema</TableCell>
              <TableCell>Descripción</TableCell>
              <TableCell>Reportó</TableCell>
              <TableCell>Evidencia</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {reportes.map((r) => (
              <TableRow key={r.id}>
                <TableCell sx={{ color: brand.inkMuted, whiteSpace: "nowrap" }}>
                  {new Date(r.fecha).toLocaleString()}
                </TableCell>
                <TableCell sx={{ fontWeight: 600 }}>{r.maquinaNombre}</TableCell>
                <TableCell>
                  <StatusPill label={r.problema} tone="error" />
                </TableCell>
                <TableCell sx={{ color: brand.inkMuted, maxWidth: 260 }}>{r.descripcion}</TableCell>
                <TableCell>{r.empleadoNombre}</TableCell>
                <TableCell>
                  {r.evidenciaUrls.map((url, i) => (
                    <EvidenciaLink key={i} url={url} label={`Foto ${i + 1}`} />
                  ))}
                </TableCell>
              </TableRow>
            ))}
            {reportes.length === 0 && (
              <TableRow>
                <TableCell colSpan={6} sx={{ textAlign: "center", color: brand.inkMuted, py: 4 }}>
                  Sin averías reportadas en este rango.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </GlassCard>
    </Box>
  );
}
