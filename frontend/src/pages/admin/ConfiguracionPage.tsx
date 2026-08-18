import { useEffect, useState } from "react";
import { Box, IconButton, Table, TableBody, TableCell, TableHead, TableRow, TextField, Typography } from "@mui/material";
import AddRoundedIcon from "@mui/icons-material/AddRounded";
import CloseRoundedIcon from "@mui/icons-material/CloseRounded";
import DeleteRoundedIcon from "@mui/icons-material/DeleteRounded";
import EmojiEventsRoundedIcon from "@mui/icons-material/EmojiEventsRounded";
import CategoryRoundedIcon from "@mui/icons-material/CategoryRounded";
import { api } from "../../api/client";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { brand, glassFieldLight, glassTableSx, pillButtonSx, pillOutlineButtonSx } from "../../theme/brand";
import type { CategoriaGasto, Premio } from "../../types";

export function ConfiguracionPage() {
  return (
    <Box>
      <PageHeader title="Configuración" subtitle="Premios y categorías de gasto disponibles en todo el sistema" />
      <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
        <PremiosSection />
        <CategoriasGastoSection />
      </Box>
    </Box>
  );
}

function PremiosSection() {
  const [premios, setPremios] = useState<Premio[]>([]);
  const [nombre, setNombre] = useState("");
  const [denominacion, setDenominacion] = useState<number>(0);
  const [editandoId, setEditandoId] = useState<string | null>(null);
  const [editNombre, setEditNombre] = useState("");
  const [editDenominacion, setEditDenominacion] = useState<number>(0);
  const [saving, setSaving] = useState(false);

  const cargar = () => api.get<Premio[]>("/catalogos/premios").then((r) => setPremios(r.data));

  useEffect(() => {
    cargar();
  }, []);

  const crear = async () => {
    setSaving(true);
    try {
      await api.post("/catalogos/premios", { nombre, denominacion });
      setNombre("");
      setDenominacion(0);
      cargar();
    } finally {
      setSaving(false);
    }
  };

  const iniciarEdicion = (p: Premio) => {
    setEditandoId(p.id);
    setEditNombre(p.nombre);
    setEditDenominacion(p.denominacion);
  };

  const guardarEdicion = async (id: string) => {
    setSaving(true);
    try {
      await api.put(`/catalogos/premios/${id}`, { nombre: editNombre, denominacion: editDenominacion });
      setEditandoId(null);
      cargar();
    } finally {
      setSaving(false);
    }
  };

  const desactivar = async (id: string) => {
    await api.delete(`/catalogos/premios/${id}`);
    cargar();
  };

  return (
    <GlassCard sx={{ p: { xs: 2.5, sm: 3 } }}>
      <Box sx={{ display: "flex", alignItems: "center", gap: 1.2, mb: 2 }}>
        <Box
          sx={{
            width: 34,
            height: 34,
            borderRadius: "10px",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            background: `linear-gradient(135deg, ${brand.navySoft} 0%, ${brand.navy} 100%)`,
          }}
        >
          <EmojiEventsRoundedIcon sx={{ color: "#fff", fontSize: 18 }} />
        </Box>
        <Typography sx={{ fontWeight: 700, fontSize: "1.05rem", color: brand.ink }}>Premios</Typography>
      </Box>

      <Box sx={{ overflowX: "auto" }}>
        <Table size="small" sx={glassTableSx}>
          <TableHead>
            <TableRow>
              <TableCell>Nombre</TableCell>
              <TableCell align="right">Denominación</TableCell>
              <TableCell align="right"></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {premios.map((p) => (
              <TableRow key={p.id}>
                {editandoId === p.id ? (
                  <>
                    <TableCell>
                      <TextField
                        size="small"
                        value={editNombre}
                        onChange={(e) => setEditNombre(e.target.value)}
                        sx={{ width: 140, ...glassFieldLight }}
                      />
                    </TableCell>
                    <TableCell align="right">
                      <TextField
                        size="small"
                        type="number"
                        value={editDenominacion}
                        onChange={(e) => setEditDenominacion(Number(e.target.value))}
                        sx={{ width: 100, ...glassFieldLight }}
                      />
                    </TableCell>
                    <TableCell align="right">
                      <Box sx={{ display: "flex", gap: 0.5, justifyContent: "flex-end" }}>
                        <Box
                          component="button"
                          onClick={() => guardarEdicion(p.id)}
                          disabled={saving}
                          sx={{ ...pillOutlineButtonSx, py: 0.4, px: 1.2, fontSize: "0.72rem" }}
                        >
                          Guardar
                        </Box>
                        <IconButton size="small" onClick={() => setEditandoId(null)} sx={{ color: brand.inkFaint }}>
                          <CloseRoundedIcon fontSize="small" />
                        </IconButton>
                      </Box>
                    </TableCell>
                  </>
                ) : (
                  <>
                    <TableCell sx={{ fontWeight: 600, cursor: "pointer" }} onClick={() => iniciarEdicion(p)}>
                      {p.nombre}
                    </TableCell>
                    <TableCell align="right" sx={{ cursor: "pointer" }} onClick={() => iniciarEdicion(p)}>
                      ${p.denominacion.toLocaleString()}
                    </TableCell>
                    <TableCell align="right">
                      <IconButton size="small" onClick={() => desactivar(p.id)} sx={{ color: brand.inkFaint }}>
                        <DeleteRoundedIcon fontSize="small" />
                      </IconButton>
                    </TableCell>
                  </>
                )}
              </TableRow>
            ))}
            <TableRow>
              <TableCell>
                <TextField
                  size="small"
                  placeholder="Nombre (ej. $130)"
                  value={nombre}
                  onChange={(e) => setNombre(e.target.value)}
                  sx={{ width: 140, ...glassFieldLight }}
                />
              </TableCell>
              <TableCell align="right">
                <TextField
                  size="small"
                  type="number"
                  placeholder="0"
                  value={denominacion || ""}
                  onChange={(e) => setDenominacion(Number(e.target.value))}
                  sx={{ width: 100, ...glassFieldLight }}
                />
              </TableCell>
              <TableCell align="right">
                <Box
                  component="button"
                  onClick={crear}
                  disabled={!nombre || denominacion <= 0 || saving}
                  sx={{ ...pillButtonSx, py: 0.5, px: 1.4, fontSize: "0.75rem" }}
                >
                  <AddRoundedIcon sx={{ fontSize: 15 }} />
                  Agregar
                </Box>
              </TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </Box>
    </GlassCard>
  );
}

function CategoriasGastoSection() {
  const [categorias, setCategorias] = useState<CategoriaGasto[]>([]);
  const [nombre, setNombre] = useState("");
  const [editandoId, setEditandoId] = useState<string | null>(null);
  const [editNombre, setEditNombre] = useState("");
  const [saving, setSaving] = useState(false);

  const cargar = () => api.get<CategoriaGasto[]>("/catalogos/categorias-gasto").then((r) => setCategorias(r.data));

  useEffect(() => {
    cargar();
  }, []);

  const crear = async () => {
    setSaving(true);
    try {
      await api.post("/catalogos/categorias-gasto", { nombre });
      setNombre("");
      cargar();
    } finally {
      setSaving(false);
    }
  };

  const guardarEdicion = async (id: string) => {
    setSaving(true);
    try {
      await api.put(`/catalogos/categorias-gasto/${id}`, { nombre: editNombre });
      setEditandoId(null);
      cargar();
    } finally {
      setSaving(false);
    }
  };

  const desactivar = async (id: string) => {
    await api.delete(`/catalogos/categorias-gasto/${id}`);
    cargar();
  };

  return (
    <GlassCard sx={{ p: { xs: 2.5, sm: 3 } }}>
      <Box sx={{ display: "flex", alignItems: "center", gap: 1.2, mb: 2 }}>
        <Box
          sx={{
            width: 34,
            height: 34,
            borderRadius: "10px",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            background: `linear-gradient(135deg, ${brand.navySoft} 0%, ${brand.navy} 100%)`,
          }}
        >
          <CategoryRoundedIcon sx={{ color: "#fff", fontSize: 18 }} />
        </Box>
        <Typography sx={{ fontWeight: 700, fontSize: "1.05rem", color: brand.ink }}>Categorías de gasto</Typography>
      </Box>

      <Box sx={{ overflowX: "auto" }}>
        <Table size="small" sx={glassTableSx}>
          <TableHead>
            <TableRow>
              <TableCell>Nombre</TableCell>
              <TableCell align="right"></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {categorias.map((c) => (
              <TableRow key={c.id}>
                {editandoId === c.id ? (
                  <>
                    <TableCell>
                      <TextField
                        size="small"
                        value={editNombre}
                        onChange={(e) => setEditNombre(e.target.value)}
                        sx={{ width: 260, ...glassFieldLight }}
                      />
                    </TableCell>
                    <TableCell align="right">
                      <Box sx={{ display: "flex", gap: 0.5, justifyContent: "flex-end" }}>
                        <Box
                          component="button"
                          onClick={() => guardarEdicion(c.id)}
                          disabled={saving}
                          sx={{ ...pillOutlineButtonSx, py: 0.4, px: 1.2, fontSize: "0.72rem" }}
                        >
                          Guardar
                        </Box>
                        <IconButton size="small" onClick={() => setEditandoId(null)} sx={{ color: brand.inkFaint }}>
                          <CloseRoundedIcon fontSize="small" />
                        </IconButton>
                      </Box>
                    </TableCell>
                  </>
                ) : (
                  <>
                    <TableCell
                      sx={{ fontWeight: 600, cursor: "pointer" }}
                      onClick={() => {
                        setEditandoId(c.id);
                        setEditNombre(c.nombre);
                      }}
                    >
                      {c.nombre}
                    </TableCell>
                    <TableCell align="right">
                      <IconButton size="small" onClick={() => desactivar(c.id)} sx={{ color: brand.inkFaint }}>
                        <DeleteRoundedIcon fontSize="small" />
                      </IconButton>
                    </TableCell>
                  </>
                )}
              </TableRow>
            ))}
            <TableRow>
              <TableCell>
                <TextField
                  size="small"
                  placeholder="Nueva categoría (ej. Mantenimiento)"
                  value={nombre}
                  onChange={(e) => setNombre(e.target.value)}
                  sx={{ width: 260, ...glassFieldLight }}
                />
              </TableCell>
              <TableCell align="right">
                <Box
                  component="button"
                  onClick={crear}
                  disabled={!nombre || saving}
                  sx={{ ...pillButtonSx, py: 0.5, px: 1.4, fontSize: "0.75rem" }}
                >
                  <AddRoundedIcon sx={{ fontSize: 15 }} />
                  Agregar
                </Box>
              </TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </Box>
    </GlassCard>
  );
}
