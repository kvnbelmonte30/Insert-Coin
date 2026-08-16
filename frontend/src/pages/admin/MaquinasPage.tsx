import { useEffect, useState } from "react";
import {
  Box,
  Dialog,
  FormControl,
  IconButton,
  InputLabel,
  MenuItem,
  Select,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import AddRoundedIcon from "@mui/icons-material/AddRounded";
import CloseRoundedIcon from "@mui/icons-material/CloseRounded";
import CasinoRoundedIcon from "@mui/icons-material/CasinoRounded";
import PersonAddRoundedIcon from "@mui/icons-material/PersonAddRounded";
import { useNavigate } from "react-router-dom";
import { api } from "../../api/client";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { StatusPill } from "../../components/StatusPill";
import {
  brand,
  dialogBackdropSx,
  dialogPaperSx,
  glassFieldLight,
  glassTableSx,
  pillButtonSx,
  pillOutlineButtonSx,
} from "../../theme/brand";
import type { Local, Maquina, Propietario, TipoMaquina, EstadoMaquina } from "../../types";

const ESTADOS: EstadoMaquina[] = ["Funcional", "Reportada", "EnReparacion", "Reparada", "FueraDeServicio"];
const ESTADO_TONE: Record<EstadoMaquina, "success" | "warning" | "error" | "neutral"> = {
  Funcional: "success",
  Reportada: "error",
  EnReparacion: "warning",
  Reparada: "warning",
  FueraDeServicio: "neutral",
};

export function MaquinasPage() {
  const navigate = useNavigate();
  const [locales, setLocales] = useState<Local[]>([]);
  const [localId, setLocalId] = useState("");
  const [maquinas, setMaquinas] = useState<Maquina[]>([]);
  const [propietarios, setPropietarios] = useState<Propietario[]>([]);
  const [tipos, setTipos] = useState<TipoMaquina[]>([]);
  const [open, setOpen] = useState(false);
  const [propietarioOpen, setPropietarioOpen] = useState(false);
  const [nuevoPropietario, setNuevoPropietario] = useState("");
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState({
    nombre: "",
    tipoMaquinaId: "",
    propietarioId: "",
    marca: "",
    modelo: "",
    numeroSerie: "",
  });

  const cargarBase = async () => {
    const [l, p, t] = await Promise.all([
      api.get<Local[]>("/locales"),
      api.get<Propietario[]>("/propietarios"),
      api.get<TipoMaquina[]>("/maquinas/tipos"),
    ]);
    setLocales(l.data);
    setPropietarios(p.data);
    setTipos(t.data);
    if (!localId && l.data.length > 0) setLocalId(l.data[0].id);
  };

  const cargarMaquinas = async () => {
    if (!localId) return;
    const { data } = await api.get<Maquina[]>("/maquinas", { params: { localId } });
    setMaquinas(data);
  };

  useEffect(() => {
    cargarBase();
  }, []);

  useEffect(() => {
    cargarMaquinas();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [localId]);

  const crearMaquina = async () => {
    setSaving(true);
    try {
      await api.post("/maquinas", { ...form, localId });
      setOpen(false);
      setForm({ nombre: "", tipoMaquinaId: "", propietarioId: "", marca: "", modelo: "", numeroSerie: "" });
      cargarMaquinas();
    } finally {
      setSaving(false);
    }
  };

  const crearPropietario = async () => {
    setSaving(true);
    try {
      await api.post("/propietarios", { nombre: nuevoPropietario });
      setNuevoPropietario("");
      setPropietarioOpen(false);
      cargarBase();
    } finally {
      setSaving(false);
    }
  };

  const cambiarEstado = async (maquina: Maquina, estado: EstadoMaquina) => {
    await api.put(`/maquinas/${maquina.id}/estado`, { estado });
    cargarMaquinas();
  };

  return (
    <Box>
      <PageHeader
        title="Máquinas"
        subtitle={`${maquinas.length} máquina${maquinas.length === 1 ? "" : "s"} en este local`}
        action={
          <>
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
            <Box component="button" onClick={() => setPropietarioOpen(true)} sx={pillOutlineButtonSx}>
              <PersonAddRoundedIcon fontSize="small" />
              Propietario
            </Box>
            <Box component="button" onClick={() => setOpen(true)} sx={pillButtonSx}>
              <AddRoundedIcon fontSize="small" />
              Nueva máquina
            </Box>
          </>
        }
      />

      <GlassCard sx={{ p: { xs: 1, sm: 2 }, overflowX: "auto" }}>
        <Table sx={glassTableSx}>
          <TableHead>
            <TableRow>
              <TableCell>Nombre</TableCell>
              <TableCell>Tipo</TableCell>
              <TableCell>Propietario</TableCell>
              <TableCell>Marca / Modelo</TableCell>
              <TableCell>Estado</TableCell>
              <TableCell></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {maquinas.map((m) => (
              <TableRow key={m.id}>
                <TableCell sx={{ fontWeight: 600 }}>{m.nombre}</TableCell>
                <TableCell>{m.tipoMaquinaNombre}</TableCell>
                <TableCell>{m.propietarioNombre}</TableCell>
                <TableCell sx={{ color: brand.inkMuted }}>
                  {m.marca} {m.modelo}
                </TableCell>
                <TableCell>
                  <Select
                    size="small"
                    variant="standard"
                    disableUnderline
                    value={m.estado}
                    onChange={(e) => cambiarEstado(m, e.target.value as EstadoMaquina)}
                    renderValue={(v) => <StatusPill label={v as string} tone={ESTADO_TONE[v as EstadoMaquina]} />}
                  >
                    {ESTADOS.map((e) => (
                      <MenuItem key={e} value={e}>
                        <StatusPill label={e} tone={ESTADO_TONE[e]} />
                      </MenuItem>
                    ))}
                  </Select>
                </TableCell>
                <TableCell align="right">
                  {m.tipoMaquinaNombre === "Cascada" && (
                    <Box
                      component="button"
                      onClick={() => navigate(`/admin/maquinas/${m.id}/cascada`)}
                      sx={{ ...pillOutlineButtonSx, py: 0.5, px: 1.4, fontSize: "0.75rem" }}
                    >
                      <CasinoRoundedIcon sx={{ fontSize: 15 }} />
                      Premios
                    </Box>
                  )}
                </TableCell>
              </TableRow>
            ))}
            {maquinas.length === 0 && (
              <TableRow>
                <TableCell colSpan={6} sx={{ textAlign: "center", color: brand.inkMuted, py: 4 }}>
                  Sin máquinas registradas en este local.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </GlassCard>

      <Dialog
        open={open}
        onClose={() => setOpen(false)}
        fullWidth
        maxWidth="xs"
        slotProps={{ paper: { sx: dialogPaperSx }, backdrop: { sx: dialogBackdropSx } }}
      >
        <Box sx={{ p: 3.5 }}>
          <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 2.5 }}>
            <Box sx={{ display: "flex", alignItems: "center", gap: 1.2 }}>
              <Box
                sx={{
                  width: 38,
                  height: 38,
                  borderRadius: "12px",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  background: `linear-gradient(135deg, ${brand.navySoft} 0%, ${brand.navy} 100%)`,
                }}
              >
                <CasinoRoundedIcon sx={{ color: "#fff", fontSize: 20 }} />
              </Box>
              <Typography sx={{ fontWeight: 800, fontSize: "1.15rem", color: brand.ink }}>Nueva máquina</Typography>
            </Box>
            <IconButton size="small" onClick={() => setOpen(false)} sx={{ color: brand.inkFaint }}>
              <CloseRoundedIcon fontSize="small" />
            </IconButton>
          </Box>

          <TextField
            label="Nombre"
            fullWidth
            margin="dense"
            value={form.nombre}
            onChange={(e) => setForm({ ...form, nombre: e.target.value })}
            sx={glassFieldLight}
          />
          <FormControl fullWidth margin="dense" sx={glassFieldLight}>
            <InputLabel>Tipo</InputLabel>
            <Select label="Tipo" value={form.tipoMaquinaId} onChange={(e) => setForm({ ...form, tipoMaquinaId: e.target.value })}>
              {tipos.map((t) => (
                <MenuItem key={t.id} value={t.id}>
                  {t.nombre}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <FormControl fullWidth margin="dense" sx={glassFieldLight}>
            <InputLabel>Propietario</InputLabel>
            <Select
              label="Propietario"
              value={form.propietarioId}
              onChange={(e) => setForm({ ...form, propietarioId: e.target.value })}
            >
              {propietarios.map((p) => (
                <MenuItem key={p.id} value={p.id}>
                  {p.nombre}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <TextField
            label="Marca"
            fullWidth
            margin="dense"
            value={form.marca}
            onChange={(e) => setForm({ ...form, marca: e.target.value })}
            sx={glassFieldLight}
          />
          <TextField
            label="Modelo"
            fullWidth
            margin="dense"
            value={form.modelo}
            onChange={(e) => setForm({ ...form, modelo: e.target.value })}
            sx={glassFieldLight}
          />
          <TextField
            label="Número de serie"
            fullWidth
            margin="dense"
            value={form.numeroSerie}
            onChange={(e) => setForm({ ...form, numeroSerie: e.target.value })}
            sx={glassFieldLight}
          />

          <Box
            component="button"
            onClick={crearMaquina}
            disabled={!form.nombre || !form.tipoMaquinaId || !form.propietarioId || saving}
            sx={{ ...pillButtonSx, width: "100%", justifyContent: "center", mt: 3, py: 1.3, fontSize: "0.95rem" }}
          >
            {saving ? "Creando..." : "Crear máquina"}
          </Box>
        </Box>
      </Dialog>

      <Dialog
        open={propietarioOpen}
        onClose={() => setPropietarioOpen(false)}
        fullWidth
        maxWidth="xs"
        slotProps={{ paper: { sx: dialogPaperSx }, backdrop: { sx: dialogBackdropSx } }}
      >
        <Box sx={{ p: 3.5 }}>
          <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 2.5 }}>
            <Typography sx={{ fontWeight: 800, fontSize: "1.15rem", color: brand.ink }}>Nuevo propietario</Typography>
            <IconButton size="small" onClick={() => setPropietarioOpen(false)} sx={{ color: brand.inkFaint }}>
              <CloseRoundedIcon fontSize="small" />
            </IconButton>
          </Box>
          <TextField
            label="Nombre"
            fullWidth
            margin="dense"
            value={nuevoPropietario}
            onChange={(e) => setNuevoPropietario(e.target.value)}
            sx={glassFieldLight}
            autoFocus
          />
          <Box
            component="button"
            onClick={crearPropietario}
            disabled={!nuevoPropietario || saving}
            sx={{ ...pillButtonSx, width: "100%", justifyContent: "center", mt: 3, py: 1.3, fontSize: "0.95rem" }}
          >
            {saving ? "Creando..." : "Crear propietario"}
          </Box>
        </Box>
      </Dialog>
    </Box>
  );
}
