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
import EditRoundedIcon from "@mui/icons-material/EditRounded";
import KeyRoundedIcon from "@mui/icons-material/KeyRounded";
import { api } from "../../api/client";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { StatusPill } from "../../components/StatusPill";
import { brand, dialogBackdropSx, dialogPaperSx, glassFieldLight, glassTableSx, pillButtonSx } from "../../theme/brand";
import type { Local, Propietario, Usuario } from "../../types";

const emptyForm = {
  userName: "",
  email: "",
  password: "",
  nombre: "",
  rol: "Empleado",
  localIds: [] as string[],
  propietarioId: "",
};

export function UsuariosPage() {
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const [locales, setLocales] = useState<Local[]>([]);
  const [propietarios, setPropietarios] = useState<Propietario[]>([]);
  const [open, setOpen] = useState(false);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState(emptyForm);

  const [editTarget, setEditTarget] = useState<Usuario | null>(null);
  const [editForm, setEditForm] = useState({
    nombre: "",
    email: "",
    rol: "Empleado",
    localIds: [] as string[],
    propietarioId: "",
  });

  const [resetTarget, setResetTarget] = useState<Usuario | null>(null);
  const [resetPassword, setResetPassword] = useState("");

  const cargar = async () => {
    const [u, l, p] = await Promise.all([
      api.get<Usuario[]>("/usuarios"),
      api.get<Local[]>("/locales"),
      api.get<Propietario[]>("/propietarios"),
    ]);
    setUsuarios(u.data);
    setLocales(l.data);
    setPropietarios(p.data);
  };

  useEffect(() => {
    cargar();
  }, []);

  const crear = async () => {
    setSaving(true);
    try {
      await api.post("/usuarios", { ...form, propietarioId: form.propietarioId || null });
      setOpen(false);
      setForm(emptyForm);
      cargar();
    } finally {
      setSaving(false);
    }
  };

  const toggleActivo = async (usuario: Usuario) => {
    await api.put(`/usuarios/${usuario.id}/estado`, { activo: !usuario.activo });
    cargar();
  };

  const abrirEditar = (usuario: Usuario) => {
    setEditTarget(usuario);
    setEditForm({
      nombre: usuario.nombre,
      email: usuario.email,
      rol: usuario.roles[0] ?? "Empleado",
      localIds: usuario.localIds,
      propietarioId: usuario.propietarioId ?? "",
    });
  };

  const guardarEdicion = async () => {
    if (!editTarget) return;
    setSaving(true);
    try {
      await api.put(`/usuarios/${editTarget.id}`, { ...editForm, propietarioId: editForm.propietarioId || null });
      setEditTarget(null);
      cargar();
    } finally {
      setSaving(false);
    }
  };

  const guardarReset = async () => {
    if (!resetTarget) return;
    setSaving(true);
    try {
      await api.post(`/usuarios/${resetTarget.id}/restablecer-contrasena`, { nuevaContrasena: resetPassword });
      setResetTarget(null);
      setResetPassword("");
    } finally {
      setSaving(false);
    }
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
              <TableCell>Propietario</TableCell>
              <TableCell align="right">Estado</TableCell>
              <TableCell align="right"></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {usuarios.map((u) => (
              <TableRow key={u.id}>
                <TableCell sx={{ fontWeight: 600, whiteSpace: "nowrap" }}>{u.nombre}</TableCell>
                <TableCell>{u.userName}</TableCell>
                <TableCell>
                  {u.roles.map((r) => (
                    <StatusPill key={r} label={r} tone={r === "Administrador" ? "info" : "neutral"} />
                  ))}
                </TableCell>
                <TableCell sx={{ color: brand.inkMuted }}>{u.localIds.map(nombreLocal).join(", ") || "—"}</TableCell>
                <TableCell sx={{ color: brand.inkMuted }}>{u.propietarioNombre ?? "—"}</TableCell>
                <TableCell align="right">
                  <Switch checked={u.activo} onChange={() => toggleActivo(u)} />
                </TableCell>
                <TableCell align="right">
                  <Box sx={{ display: "flex", gap: 0.5, justifyContent: "flex-end" }}>
                    <IconButton size="small" onClick={() => abrirEditar(u)} sx={{ color: brand.inkMuted }}>
                      <EditRoundedIcon fontSize="small" />
                    </IconButton>
                    <IconButton size="small" onClick={() => setResetTarget(u)} sx={{ color: brand.inkMuted }}>
                      <KeyRoundedIcon fontSize="small" />
                    </IconButton>
                  </Box>
                </TableCell>
              </TableRow>
            ))}
            {usuarios.length === 0 && (
              <TableRow>
                <TableCell colSpan={7} sx={{ textAlign: "center", color: brand.inkMuted, py: 4 }}>
                  Sin usuarios registrados.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </GlassCard>

      {/* Crear usuario */}
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
          <FormControl fullWidth margin="dense" sx={glassFieldLight}>
            <InputLabel>Propietario asociado (opcional)</InputLabel>
            <Select
              label="Propietario asociado (opcional)"
              value={form.propietarioId}
              onChange={(e) => setForm({ ...form, propietarioId: e.target.value })}
            >
              <MenuItem value="">Sin propietario asociado</MenuItem>
              {propietarios.map((p) => (
                <MenuItem key={p.id} value={p.id}>
                  {p.nombre}
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

      {/* Editar usuario */}
      <Dialog
        open={!!editTarget}
        onClose={() => setEditTarget(null)}
        fullWidth
        maxWidth="xs"
        slotProps={{ paper: { sx: dialogPaperSx }, backdrop: { sx: dialogBackdropSx } }}
      >
        <Box sx={{ p: 3.5 }}>
          <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 2.5 }}>
            <Typography sx={{ fontWeight: 800, fontSize: "1.15rem", color: brand.ink }}>
              Editar usuario {editTarget ? `· ${editTarget.userName}` : ""}
            </Typography>
            <IconButton size="small" onClick={() => setEditTarget(null)} sx={{ color: brand.inkFaint }}>
              <CloseRoundedIcon fontSize="small" />
            </IconButton>
          </Box>

          <TextField
            label="Nombre completo"
            fullWidth
            margin="dense"
            value={editForm.nombre}
            onChange={(e) => setEditForm({ ...editForm, nombre: e.target.value })}
            sx={glassFieldLight}
          />
          <TextField
            label="Correo"
            fullWidth
            margin="dense"
            value={editForm.email}
            onChange={(e) => setEditForm({ ...editForm, email: e.target.value })}
            sx={glassFieldLight}
          />
          <FormControl fullWidth margin="dense" sx={glassFieldLight}>
            <InputLabel>Rol</InputLabel>
            <Select label="Rol" value={editForm.rol} onChange={(e) => setEditForm({ ...editForm, rol: e.target.value })}>
              <MenuItem value="Administrador">Administrador</MenuItem>
              <MenuItem value="Empleado">Empleado</MenuItem>
            </Select>
          </FormControl>
          <FormControl fullWidth margin="dense" sx={glassFieldLight}>
            <InputLabel>Locales asignados</InputLabel>
            <Select
              multiple
              value={editForm.localIds}
              onChange={(e) =>
                setEditForm({
                  ...editForm,
                  localIds: typeof e.target.value === "string" ? [] : (e.target.value as string[]),
                })
              }
              input={<OutlinedInput label="Locales asignados" />}
              renderValue={(selected) => (selected as string[]).map(nombreLocal).join(", ")}
            >
              {locales.map((l) => (
                <MenuItem key={l.id} value={l.id}>
                  <Checkbox checked={editForm.localIds.includes(l.id)} />
                  <ListItemText primary={l.nombre} />
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <FormControl fullWidth margin="dense" sx={glassFieldLight}>
            <InputLabel>Propietario asociado (opcional)</InputLabel>
            <Select
              label="Propietario asociado (opcional)"
              value={editForm.propietarioId}
              onChange={(e) => setEditForm({ ...editForm, propietarioId: e.target.value })}
            >
              <MenuItem value="">Sin propietario asociado</MenuItem>
              {propietarios.map((p) => (
                <MenuItem key={p.id} value={p.id}>
                  {p.nombre}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          <Box
            component="button"
            onClick={guardarEdicion}
            disabled={!editForm.nombre || saving}
            sx={{ ...pillButtonSx, width: "100%", justifyContent: "center", mt: 3, py: 1.3, fontSize: "0.95rem" }}
          >
            {saving ? "Guardando..." : "Guardar cambios"}
          </Box>
        </Box>
      </Dialog>

      {/* Restablecer contraseña */}
      <Dialog
        open={!!resetTarget}
        onClose={() => {
          setResetTarget(null);
          setResetPassword("");
        }}
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
                <KeyRoundedIcon sx={{ color: "#fff", fontSize: 20 }} />
              </Box>
              <Typography sx={{ fontWeight: 800, fontSize: "1.1rem", color: brand.ink }}>
                Restablecer contraseña
              </Typography>
            </Box>
            <IconButton
              size="small"
              onClick={() => {
                setResetTarget(null);
                setResetPassword("");
              }}
              sx={{ color: brand.inkFaint }}
            >
              <CloseRoundedIcon fontSize="small" />
            </IconButton>
          </Box>

          <Typography sx={{ color: brand.inkMuted, fontSize: "0.85rem", mb: 2 }}>
            Nueva contraseña para {resetTarget?.nombre} ({resetTarget?.userName}). Mínimo 8 caracteres.
          </Typography>

          <TextField
            label="Nueva contraseña"
            type="password"
            fullWidth
            margin="dense"
            value={resetPassword}
            onChange={(e) => setResetPassword(e.target.value)}
            sx={glassFieldLight}
            autoFocus
          />

          <Box
            component="button"
            onClick={guardarReset}
            disabled={resetPassword.length < 8 || saving}
            sx={{ ...pillButtonSx, width: "100%", justifyContent: "center", mt: 3, py: 1.3, fontSize: "0.95rem" }}
          >
            {saving ? "Guardando..." : "Restablecer contraseña"}
          </Box>
        </Box>
      </Dialog>
    </Box>
  );
}
