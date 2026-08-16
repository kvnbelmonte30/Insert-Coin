import { useEffect, useState } from "react";
import {
  Box,
  Checkbox,
  Dialog,
  FormControl,
  IconButton,
  InputLabel,
  ListItemText,
  MenuItem,
  OutlinedInput,
  Select,
  Switch,
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
import GroupRoundedIcon from "@mui/icons-material/GroupRounded";
import { api } from "../../api/client";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { StatusPill } from "../../components/StatusPill";
import { brand, dialogBackdropSx, dialogPaperSx, glassFieldLight, glassTableSx, pillButtonSx } from "../../theme/brand";
import type { Local, Usuario } from "../../types";

export function UsuariosPage() {
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const [locales, setLocales] = useState<Local[]>([]);
  const [open, setOpen] = useState(false);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState({
    userName: "",
    email: "",
    password: "",
    nombre: "",
    rol: "Empleado",
    localIds: [] as string[],
  });

  const cargar = async () => {
    const [u, l] = await Promise.all([api.get<Usuario[]>("/usuarios"), api.get<Local[]>("/locales")]);
    setUsuarios(u.data);
    setLocales(l.data);
  };

  useEffect(() => {
    cargar();
  }, []);

  const crear = async () => {
    setSaving(true);
    try {
      await api.post("/usuarios", form);
      setOpen(false);
      setForm({ userName: "", email: "", password: "", nombre: "", rol: "Empleado", localIds: [] });
      cargar();
    } finally {
      setSaving(false);
    }
  };

  const toggleActivo = async (usuario: Usuario) => {
    await api.put(`/usuarios/${usuario.id}/estado`, { activo: !usuario.activo });
    cargar();
  };

  const nombreLocal = (id: string) => locales.find((l) => l.id === id)?.nombre ?? id;

  return (
    <Box>
      <PageHeader
        title="Usuarios"
        subtitle={`${usuarios.length} usuario${usuarios.length === 1 ? "" : "s"}`}
        action={
          <Box component="button" onClick={() => setOpen(true)} sx={pillButtonSx}>
            <AddRoundedIcon fontSize="small" />
            Nuevo usuario
          </Box>
        }
      />

      <GlassCard sx={{ p: { xs: 1, sm: 2 }, overflowX: "auto" }}>
        <Table sx={glassTableSx}>
          <TableHead>
            <TableRow>
              <TableCell>Nombre</TableCell>
              <TableCell>Usuario</TableCell>
              <TableCell>Rol</TableCell>
              <TableCell>Locales</TableCell>
              <TableCell align="right">Estado</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {usuarios.map((u) => (
              <TableRow key={u.id}>
                <TableCell sx={{ fontWeight: 600 }}>{u.nombre}</TableCell>
                <TableCell>{u.userName}</TableCell>
                <TableCell>
                  {u.roles.map((r) => (
                    <StatusPill key={r} label={r} tone={r === "Administrador" ? "info" : "neutral"} />
                  ))}
                </TableCell>
                <TableCell sx={{ color: brand.inkMuted }}>{u.localIds.map(nombreLocal).join(", ") || "—"}</TableCell>
                <TableCell align="right">
                  <Switch checked={u.activo} onChange={() => toggleActivo(u)} />
                </TableCell>
              </TableRow>
            ))}
            {usuarios.length === 0 && (
              <TableRow>
                <TableCell colSpan={5} sx={{ textAlign: "center", color: brand.inkMuted, py: 4 }}>
                  Sin usuarios registrados.
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
                <GroupRoundedIcon sx={{ color: "#fff", fontSize: 20 }} />
              </Box>
              <Typography sx={{ fontWeight: 800, fontSize: "1.15rem", color: brand.ink }}>Nuevo usuario</Typography>
            </Box>
            <IconButton size="small" onClick={() => setOpen(false)} sx={{ color: brand.inkFaint }}>
              <CloseRoundedIcon fontSize="small" />
            </IconButton>
          </Box>

          <TextField
            label="Nombre completo"
            fullWidth
            margin="dense"
            value={form.nombre}
            onChange={(e) => setForm({ ...form, nombre: e.target.value })}
            sx={glassFieldLight}
          />
          <TextField
            label="Usuario (login)"
            fullWidth
            margin="dense"
            value={form.userName}
            onChange={(e) => setForm({ ...form, userName: e.target.value })}
            sx={glassFieldLight}
          />
          <TextField
            label="Correo"
            fullWidth
            margin="dense"
            value={form.email}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
            sx={glassFieldLight}
          />
          <TextField
            label="Contraseña"
            type="password"
            fullWidth
            margin="dense"
            value={form.password}
            onChange={(e) => setForm({ ...form, password: e.target.value })}
            sx={glassFieldLight}
          />
          <FormControl fullWidth margin="dense" sx={glassFieldLight}>
            <InputLabel>Rol</InputLabel>
            <Select label="Rol" value={form.rol} onChange={(e) => setForm({ ...form, rol: e.target.value })}>
              <MenuItem value="Administrador">Administrador</MenuItem>
              <MenuItem value="Empleado">Empleado</MenuItem>
            </Select>
          </FormControl>
          <FormControl fullWidth margin="dense" sx={glassFieldLight}>
            <InputLabel>Locales asignados</InputLabel>
            <Select
              multiple
              value={form.localIds}
              onChange={(e) =>
                setForm({ ...form, localIds: typeof e.target.value === "string" ? [] : (e.target.value as string[]) })
              }
              input={<OutlinedInput label="Locales asignados" />}
              renderValue={(selected) => (selected as string[]).map(nombreLocal).join(", ")}
            >
              {locales.map((l) => (
                <MenuItem key={l.id} value={l.id}>
                  <Checkbox checked={form.localIds.includes(l.id)} />
                  <ListItemText primary={l.nombre} />
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          <Box
            component="button"
            onClick={crear}
            disabled={!form.userName || !form.password || !form.nombre || saving}
            sx={{ ...pillButtonSx, width: "100%", justifyContent: "center", mt: 3, py: 1.3, fontSize: "0.95rem" }}
          >
            {saving ? "Creando..." : "Crear usuario"}
          </Box>
        </Box>
      </Dialog>
    </Box>
  );
}
