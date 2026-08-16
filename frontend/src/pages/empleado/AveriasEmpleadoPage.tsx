import { useEffect, useState } from "react";
import { Box, FormControl, InputLabel, MenuItem, Select, Table, TableBody, TableCell, TableHead, TableRow, TextField, Typography } from "@mui/material";
import { api } from "../../api/client";
import { useAuthStore } from "../../store/authStore";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { StatusPill } from "../../components/StatusPill";
import { brand, glassFieldLight, glassTableSx, pillButtonSx } from "../../theme/brand";
import type { Maquina, ReporteAveria } from "../../types";

export function AveriasEmpleadoPage() {
  const locales = useAuthStore((s) => s.locales);
  const [localId, setLocalId] = useState(locales[0]?.id ?? "");
  const [maquinas, setMaquinas] = useState<Maquina[]>([]);
  const [reportes, setReportes] = useState<ReporteAveria[]>([]);
  const [maquinaId, setMaquinaId] = useState("");
  const [problema, setProblema] = useState("");
  const [descripcion, setDescripcion] = useState("");
  const [archivo, setArchivo] = useState<File | null>(null);
  const [saving, setSaving] = useState(false);

  const cargar = async () => {
    if (!localId) return;
    const [m, r] = await Promise.all([
      api.get<Maquina[]>("/maquinas", { params: { localId } }),
      api.get<ReporteAveria[]>(`/averias/local/${localId}`),
    ]);
    setMaquinas(m.data);
    setReportes(r.data);
  };

  useEffect(() => {
    cargar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [localId]);

  const enviar = async () => {
    setSaving(true);
    try {
      const formData = new FormData();
      formData.append("LocalId", localId);
      formData.append("MaquinaId", maquinaId);
      formData.append("Problema", problema);
      formData.append("Descripcion", descripcion);
      if (archivo) formData.append("evidencias", archivo);

      await api.post("/averias", formData, { headers: { "Content-Type": "multipart/form-data" } });
      setMaquinaId("");
      setProblema("");
      setDescripcion("");
      setArchivo(null);
      cargar();
    } finally {
      setSaving(false);
    }
  };

  return (
    <Box>
      <PageHeader
        title="Reportar avería"
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
        <TextField
          label="Problema"
          fullWidth
          margin="dense"
          value={problema}
          onChange={(e) => setProblema(e.target.value)}
          sx={glassFieldLight}
        />
        <TextField
          label="Descripción"
          fullWidth
          multiline
          minRows={2}
          margin="dense"
          value={descripcion}
          onChange={(e) => setDescripcion(e.target.value)}
          sx={glassFieldLight}
        />
        <Box
          component="label"
          sx={{
            display: "inline-flex",
            alignItems: "center",
            gap: 0.7,
            mt: 1.5,
            border: "1px solid rgba(14,23,48,0.16)",
            borderRadius: "999px",
            cursor: "pointer",
            fontSize: "0.85rem",
            fontWeight: 600,
            color: brand.ink,
            background: "rgba(255,255,255,0.5)",
            px: 2,
            py: 0.9,
          }}
        >
          {archivo ? archivo.name : "Adjuntar fotografía"}
          <input type="file" accept="image/*" hidden onChange={(e) => setArchivo(e.target.files?.[0] ?? null)} />
        </Box>
        <Box sx={{ mt: 2.5 }}>
          <Box component="button" onClick={enviar} disabled={!maquinaId || !problema || saving} sx={pillButtonSx}>
            {saving ? "Enviando..." : "Reportar avería"}
          </Box>
        </Box>
      </GlassCard>

      <Typography sx={{ fontWeight: 700, fontSize: "1.05rem", color: brand.ink, mb: 1.5 }}>
        Reportes recientes
      </Typography>
      <GlassCard sx={{ p: { xs: 1, sm: 2 }, overflowX: "auto" }}>
        <Table sx={glassTableSx}>
          <TableHead>
            <TableRow>
              <TableCell>Fecha</TableCell>
              <TableCell>Máquina</TableCell>
              <TableCell>Problema</TableCell>
              <TableCell>Reportó</TableCell>
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
                <TableCell>{r.empleadoNombre}</TableCell>
              </TableRow>
            ))}
            {reportes.length === 0 && (
              <TableRow>
                <TableCell colSpan={4} sx={{ textAlign: "center", color: brand.inkMuted, py: 4 }}>
                  Sin reportes registrados.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </GlassCard>
    </Box>
  );
}
