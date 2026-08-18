import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Box, Table, TableBody, TableCell, TableHead, TableRow, TextField, Typography } from "@mui/material";
import ArrowBackRoundedIcon from "@mui/icons-material/ArrowBackRounded";
import { api } from "../../api/client";
import { GlassCard } from "../../components/GlassCard";
import { brand, glassFieldLight, glassTableSx, pillOutlineButtonSx } from "../../theme/brand";
import type { CorteMaquina, Maquina } from "../../types";

export function CortesMaquinaAdminPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [maquina, setMaquina] = useState<Maquina | null>(null);
  const [cortes, setCortes] = useState<CorteMaquina[]>([]);
  const [desde, setDesde] = useState("");
  const [hasta, setHasta] = useState("");

  useEffect(() => {
    if (!id) return;
    api.get<Maquina>(`/maquinas/${id}`).then((r) => setMaquina(r.data));
  }, [id]);

  useEffect(() => {
    if (!id) return;
    api
      .get<CorteMaquina[]>(`/maquinas/${id}/cortes`, { params: { desde: desde || undefined, hasta: hasta || undefined } })
      .then((r) => setCortes(r.data));
  }, [id, desde, hasta]);

  const total = cortes.reduce((sum, c) => sum + c.total, 0);

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

      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-end", flexWrap: "wrap", gap: 2, mb: 3 }}>
        <Typography sx={{ fontSize: { xs: "1.5rem", sm: "1.75rem" }, fontWeight: 800, color: brand.ink }}>
          Cortes · {maquina?.nombre ?? "Máquina"}
        </Typography>
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
            Total: ${total.toLocaleString()}
          </Typography>
        </Box>
        <Table sx={glassTableSx}>
          <TableHead>
            <TableRow>
              <TableCell>Fecha</TableCell>
              <TableCell>Empleado</TableCell>
              <TableCell>Detalle</TableCell>
              <TableCell>Comentario</TableCell>
              <TableCell align="right">Total</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {cortes.map((c) => (
              <TableRow key={c.id}>
                <TableCell sx={{ whiteSpace: "nowrap" }}>{c.fecha}</TableCell>
                <TableCell>{c.empleadoNombre}</TableCell>
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
