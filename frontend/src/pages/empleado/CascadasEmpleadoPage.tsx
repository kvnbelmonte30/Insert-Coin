import { useEffect, useState } from "react";
import { Box, FormControl, InputLabel, MenuItem, Select, Table, TableBody, TableCell, TableHead, TableRow, TextField, Typography } from "@mui/material";
import WavesRoundedIcon from "@mui/icons-material/WavesRounded";
import { api } from "../../api/client";
import { useAuthStore } from "../../store/authStore";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { brand, glassFieldLight, glassTableSx, pillButtonSx } from "../../theme/brand";
import type { Maquina, MaquinaPremioConfig, InventarioPremio } from "../../types";

export function CascadasEmpleadoPage() {
  const locales = useAuthStore((s) => s.locales);
  const [localId, setLocalId] = useState(locales[0]?.id ?? "");
  const [cascadas, setCascadas] = useState<Maquina[]>([]);
  const [maquinaId, setMaquinaId] = useState("");
  const [config, setConfig] = useState<MaquinaPremioConfig[]>([]);
  const [encontrado, setEncontrado] = useState<Record<string, number>>({});
  const [resultado, setResultado] = useState<InventarioPremio | null>(null);

  useEffect(() => {
    if (!localId) return;
    api.get<Maquina[]>("/maquinas", { params: { localId } }).then((r) => {
      const soloCascadas = r.data.filter((m) => m.tipoMaquinaNombre === "Cascada");
      setCascadas(soloCascadas);
      if (soloCascadas.length > 0) setMaquinaId(soloCascadas[0].id);
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [localId]);

  useEffect(() => {
    if (!maquinaId) return;
    setResultado(null);
    api.get<MaquinaPremioConfig[]>(`/maquinas/${maquinaId}/premios`).then((r) => setConfig(r.data));
  }, [maquinaId]);

  const registrar = async () => {
    const lineas = config.map((c) => ({ premioId: c.premioId, cantidadEncontrada: encontrado[c.premioId] ?? 0 }));
    const { data } = await api.post<InventarioPremio>(`/maquinas/${maquinaId}/inventarios`, { lineas });
    setResultado(data);
    setEncontrado({});
  };

  return (
    <Box>
      <PageHeader
        title="Conteo de premios en cascadas"
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

      {cascadas.length === 0 ? (
        <GlassCard sx={{ p: 5, textAlign: "center" }}>
          <WavesRoundedIcon sx={{ fontSize: 38, color: brand.inkFaint, mb: 1.5 }} />
          <Typography sx={{ color: brand.inkMuted }}>Este local no tiene máquinas de tipo Cascada.</Typography>
        </GlassCard>
      ) : (
        <GlassCard sx={{ p: { xs: 2.5, sm: 3 } }}>
          <FormControl fullWidth margin="dense" sx={glassFieldLight}>
            <InputLabel>Cascada</InputLabel>
            <Select label="Cascada" value={maquinaId} onChange={(e) => setMaquinaId(e.target.value)}>
              {cascadas.map((c) => (
                <MenuItem key={c.id} value={c.id}>
                  {c.nombre}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          <Box sx={{ overflowX: "auto", mt: 2 }}>
            <Table size="small" sx={glassTableSx}>
              <TableHead>
                <TableRow>
                  <TableCell>Premio</TableCell>
                  <TableCell align="right">Cantidad configurada</TableCell>
                  <TableCell align="right">Cantidad encontrada</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {config.map((c) => (
                  <TableRow key={c.premioId}>
                    <TableCell sx={{ fontWeight: 600 }}>{c.premioNombre}</TableCell>
                    <TableCell align="right">{c.cantidadAsignada}</TableCell>
                    <TableCell align="right">
                      <TextField
                        type="number"
                        size="small"
                        sx={{ width: 90, ...glassFieldLight }}
                        value={encontrado[c.premioId] ?? ""}
                        onChange={(e) => setEncontrado({ ...encontrado, [c.premioId]: Number(e.target.value) })}
                      />
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </Box>

          <Box sx={{ mt: 2.5 }}>
            <Box
              component="button"
              onClick={registrar}
              disabled={config.length === 0}
              sx={pillButtonSx}
            >
              Registrar conteo
            </Box>
          </Box>
        </GlassCard>
      )}

      {resultado && (
        <GlassCard sx={{ p: { xs: 2.5, sm: 3 }, mt: 3 }}>
          <Typography sx={{ fontWeight: 700, fontSize: "1.05rem", color: brand.ink, mb: 1.5 }}>
            Resultado del conteo
          </Typography>
          {resultado.detalles
            .filter((d) => d.diferencia !== 0)
            .map((d, i) => (
              <Typography key={i} sx={{ color: "#b06a00", fontSize: "0.9rem", fontWeight: 600 }}>
                Faltan {d.diferencia} de {d.premioNombre}
              </Typography>
            ))}
          {resultado.detalles.every((d) => d.diferencia === 0) && (
            <Typography sx={{ color: "#1b7a4d", fontSize: "0.9rem", fontWeight: 600 }}>
              Todo cuadra correctamente.
            </Typography>
          )}
        </GlassCard>
      )}
    </Box>
  );
}
