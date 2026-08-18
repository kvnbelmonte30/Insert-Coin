import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Box, Table, TableBody, TableCell, TableHead, TableRow, TextField, Typography } from "@mui/material";
import ArrowBackRoundedIcon from "@mui/icons-material/ArrowBackRounded";
import { api } from "../../api/client";
import { GlassCard } from "../../components/GlassCard";
import { brand, glassFieldLight, glassTableSx, pillButtonSx, pillOutlineButtonSx } from "../../theme/brand";
import type { CorteMaquina, Maquina } from "../../types";

export function CortesMaquinaAdminPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [maquina, setMaquina] = useState<Maquina | null>(null);
  const [total, setTotal] = useState<number>(0);
  const [comentario, setComentario] = useState("");
  const [saving, setSaving] = useState(false);
  const [cortes, setCortes] = useState<CorteMaquina[]>([]);
  const [desde, setDesde] = useState("");
  const [hasta, setHasta] = useState("");

  useEffect(() => {
    if (!id) return;
    api.get<Maquina>(`/maquinas/${id}`).then((r) => setMaquina(r.data));
  }, [id]);

  const cargarCortes = () => {
    if (!id) return;
    api
      .get<CorteMaquina[]>(`/maquinas/${id}/cortes`, { params: { desde: desde || undefined, hasta: hasta || undefined } })
      .then((r) => setCortes(r.data));
  };

  useEffect(cargarCortes, [id, desde, hasta]);

  const totalHistorico = cortes.reduce((sum, c) => sum + c.total, 0);

  const registrar = async () => {
    if (!id || total <= 0) return;
    setSaving(true);
    try {
      await api.post(`/maquinas/${id}/cortes`, {
        fecha: new Date().toISOString().slice(0, 10),
        comentario: comentario || null,
        total,
      });
      setTotal(0);
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
        <Typography sx={{ fontWeight: 700, color: brand.ink, fontSize: "0.95rem", mb: 2 }}>Registrar nuevo corte</Typography>

        <TextField
          label="Total sacado de la máquina"
          type="number"
          fullWidth
          margin="dense"
          value={total || ""}
          onChange={(e) => setTotal(Number(e.target.value))}
          sx={glassFieldLight}
          autoFocus
        />
        <TextField
          label="Comentario (opcional)"
          fullWidth
          multiline
          minRows={2}
          margin="dense"
          placeholder={`Notas sobre el corte de ${maquina?.nombre ?? "la máquina"}...`}
          value={comentario}
          onChange={(e) => setComentario(e.target.value)}
          sx={glassFieldLight}
        />

        <Box sx={{ mt: 2.5 }}>
          <Box
            component="button"
            onClick={registrar}
            disabled={total <= 0 || saving}
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
              <TableCell>Comentario</TableCell>
              <TableCell align="right">Total</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {cortes.map((c) => (
              <TableRow key={c.id}>
                <TableCell sx={{ whiteSpace: "nowrap" }}>{c.fecha}</TableCell>
                <TableCell>{c.registradoPorNombre}</TableCell>
                <TableCell sx={{ color: brand.inkMuted, fontSize: "0.8rem", maxWidth: 320 }}>{c.comentario ?? "—"}</TableCell>
                <TableCell align="right" sx={{ fontWeight: 700 }}>
                  ${c.total.toLocaleString()}
                </TableCell>
              </TableRow>
            ))}
            {cortes.length === 0 && (
              <TableRow>
                <TableCell colSpan={4} sx={{ textAlign: "center", color: brand.inkMuted, py: 4 }}>
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
