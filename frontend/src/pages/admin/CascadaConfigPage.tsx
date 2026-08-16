import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Box, Table, TableBody, TableCell, TableHead, TableRow, TextField, Typography } from "@mui/material";
import ArrowBackRoundedIcon from "@mui/icons-material/ArrowBackRounded";
import { api } from "../../api/client";
import { GlassCard } from "../../components/GlassCard";
import { StatusPill } from "../../components/StatusPill";
import { brand, glassFieldLight, glassTableSx, pillOutlineButtonSx } from "../../theme/brand";
import type { InventarioPremio, MaquinaPremioConfig, Premio } from "../../types";

export function CascadaConfigPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [config, setConfig] = useState<MaquinaPremioConfig[]>([]);
  const [premios, setPremios] = useState<Premio[]>([]);
  const [cantidades, setCantidades] = useState<Record<string, number>>({});
  const [inventarios, setInventarios] = useState<InventarioPremio[]>([]);

  const cargar = async () => {
    if (!id) return;
    const [configRes, premiosRes, inventariosRes] = await Promise.all([
      api.get<MaquinaPremioConfig[]>(`/maquinas/${id}/premios`),
      api.get<Premio[]>("/catalogos/premios"),
      api.get<InventarioPremio[]>(`/maquinas/${id}/inventarios`),
    ]);
    setConfig(configRes.data);
    setPremios(premiosRes.data);
    setInventarios(inventariosRes.data);
  };

  useEffect(() => {
    cargar();
  }, [id]);

  const guardar = async (premioId: string) => {
    await api.post(`/maquinas/${id}/premios`, { premioId, cantidadAsignada: cantidades[premioId] ?? 0 });
    cargar();
  };

  const cantidadActual = (premioId: string) => config.find((c) => c.premioId === premioId)?.cantidadAsignada ?? 0;

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
        Configuración de premios
      </Typography>

      <GlassCard sx={{ p: { xs: 2, sm: 3 }, mb: 3 }}>
        <Typography sx={{ fontWeight: 700, color: brand.ink, mb: 1.5 }}>Premios de la cascada</Typography>
        <Table sx={glassTableSx}>
          <TableHead>
            <TableRow>
              <TableCell>Premio</TableCell>
              <TableCell align="right">Cantidad asignada</TableCell>
              <TableCell align="right">Nueva cantidad</TableCell>
              <TableCell></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {premios.map((p) => (
              <TableRow key={p.id}>
                <TableCell sx={{ fontWeight: 600 }}>{p.nombre}</TableCell>
                <TableCell align="right">{cantidadActual(p.id)}</TableCell>
                <TableCell align="right">
                  <TextField
                    type="number"
                    size="small"
                    sx={{ width: 90, ...glassFieldLight }}
                    value={cantidades[p.id] ?? cantidadActual(p.id)}
                    onChange={(e) => setCantidades({ ...cantidades, [p.id]: Number(e.target.value) })}
                  />
                </TableCell>
                <TableCell align="right">
                  <Box
                    component="button"
                    onClick={() => guardar(p.id)}
                    sx={{ ...pillOutlineButtonSx, py: 0.5, px: 1.4, fontSize: "0.75rem" }}
                  >
                    Guardar
                  </Box>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </GlassCard>

      <GlassCard sx={{ p: { xs: 2, sm: 3 } }}>
        <Typography sx={{ fontWeight: 700, color: brand.ink, mb: 1.5 }}>Historial de conteos</Typography>
        {inventarios.map((inv) => (
          <Box key={inv.id} sx={{ mb: 2.5 }}>
            <Typography sx={{ fontSize: "0.8rem", color: brand.inkMuted, mb: 0.8 }}>
              {new Date(inv.fecha).toLocaleString()} — {inv.usuarioNombre}
            </Typography>
            <Table size="small" sx={glassTableSx}>
              <TableHead>
                <TableRow>
                  <TableCell>Premio</TableCell>
                  <TableCell align="right">Configurado</TableCell>
                  <TableCell align="right">Encontrado</TableCell>
                  <TableCell align="right">Diferencia</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {inv.detalles.map((d, i) => (
                  <TableRow key={i}>
                    <TableCell>{d.premioNombre}</TableCell>
                    <TableCell align="right">{d.cantidadConfigurada}</TableCell>
                    <TableCell align="right">{d.cantidadEncontrada}</TableCell>
                    <TableCell align="right">
                      {d.diferencia === 0 ? (
                        <StatusPill label="0" tone="success" />
                      ) : (
                        <StatusPill label={String(d.diferencia)} tone="warning" />
                      )}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </Box>
        ))}
        {inventarios.length === 0 && (
          <Typography sx={{ color: brand.inkMuted, fontSize: "0.9rem" }}>Sin conteos registrados.</Typography>
        )}
      </GlassCard>
    </Box>
  );
}
