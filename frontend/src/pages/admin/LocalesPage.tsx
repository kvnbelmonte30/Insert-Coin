import { useEffect, useState } from "react";
import { Box, Dialog, Grid, IconButton, TextField, Typography } from "@mui/material";
import AddRoundedIcon from "@mui/icons-material/AddRounded";
import CloseRoundedIcon from "@mui/icons-material/CloseRounded";
import StorefrontRoundedIcon from "@mui/icons-material/StorefrontRounded";
import PlaceRoundedIcon from "@mui/icons-material/PlaceRounded";
import ChevronRightRoundedIcon from "@mui/icons-material/ChevronRightRounded";
import { useNavigate } from "react-router-dom";
import { api } from "../../api/client";
import { GlassCard } from "../../components/GlassCard";
import { brand, glassFieldLight, pillButtonSx } from "../../theme/brand";
import type { Local } from "../../types";

export function LocalesPage() {
  const [locales, setLocales] = useState<Local[]>([]);
  const [open, setOpen] = useState(false);
  const [nombre, setNombre] = useState("");
  const [direccion, setDireccion] = useState("");
  const [saving, setSaving] = useState(false);
  const navigate = useNavigate();

  const cargar = () => api.get<Local[]>("/locales").then((r) => setLocales(r.data));

  useEffect(() => {
    cargar();
  }, []);

  const crear = async () => {
    setSaving(true);
    try {
      await api.post("/locales", { nombre, direccion });
      setOpen(false);
      setNombre("");
      setDireccion("");
      cargar();
    } finally {
      setSaving(false);
    }
  };

  const activos = locales.filter((l) => l.activo).length;

  return (
    <Box>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-end", mb: 3.5, flexWrap: "wrap", gap: 2 }}>
        <Box>
          <Typography sx={{ fontSize: { xs: "1.6rem", sm: "1.9rem" }, fontWeight: 800, color: brand.ink, letterSpacing: "-0.5px" }}>
            Locales
          </Typography>
          <Typography sx={{ color: brand.inkMuted, fontSize: "0.9rem", mt: 0.3 }}>
            {locales.length === 0 ? "Aún no hay locales" : `${locales.length} local${locales.length === 1 ? "" : "es"} · ${activos} activo${activos === 1 ? "" : "s"}`}
          </Typography>
        </Box>
        <Box component="button" onClick={() => setOpen(true)} sx={pillButtonSx}>
          <AddRoundedIcon fontSize="small" />
          Nuevo local
        </Box>
      </Box>

      {locales.length === 0 ? (
        <GlassCard sx={{ p: 6, textAlign: "center" }}>
          <StorefrontRoundedIcon sx={{ fontSize: 42, color: brand.inkFaint, mb: 1.5 }} />
          <Typography sx={{ color: brand.inkMuted, fontSize: "0.95rem" }}>
            Crea tu primer local para empezar a configurar cuentas y máquinas.
          </Typography>
        </GlassCard>
      ) : (
        <Grid container spacing={2.5}>
          {locales.map((local) => (
            <Grid size={{ xs: 12, sm: 6, md: 4 }} key={local.id}>
              <GlassCard onClick={() => navigate(`/admin/locales/${local.id}`)} sx={{ p: 2.75, height: "100%", display: "flex", flexDirection: "column" }}>
                <Box sx={{ display: "flex", alignItems: "flex-start", justifyContent: "space-between", mb: 1.5 }}>
                  <Box
                    sx={{
                      width: 44,
                      height: 44,
                      borderRadius: "14px",
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      background: `linear-gradient(135deg, ${brand.navySoft} 0%, ${brand.navy} 100%)`,
                      boxShadow: "0 4px 12px rgba(10,17,40,0.25)",
                    }}
                  >
                    <StorefrontRoundedIcon sx={{ color: "#fff", fontSize: 22 }} />
                  </Box>
                  <Box
                    sx={{
                      px: 1.2,
                      py: 0.35,
                      borderRadius: "999px",
                      fontSize: "0.68rem",
                      fontWeight: 700,
                      letterSpacing: "0.4px",
                      textTransform: "uppercase",
                      color: local.activo ? "#1b7a4d" : brand.inkFaint,
                      background: local.activo ? "rgba(27,122,77,0.12)" : "rgba(14,23,48,0.06)",
                    }}
                  >
                    {local.activo ? "Activo" : "Inactivo"}
                  </Box>
                </Box>

                <Typography sx={{ fontWeight: 700, fontSize: "1.05rem", color: brand.ink, mb: 0.4 }}>
                  {local.nombre}
                </Typography>

                <Box sx={{ display: "flex", alignItems: "center", gap: 0.6, color: brand.inkMuted, mb: "auto" }}>
                  <PlaceRoundedIcon sx={{ fontSize: 16 }} />
                  <Typography sx={{ fontSize: "0.85rem" }}>{local.direccion || "Sin dirección"}</Typography>
                </Box>

                <Box
                  sx={{
                    mt: 2,
                    pt: 1.5,
                    borderTop: "1px solid rgba(14,23,48,0.08)",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "space-between",
                  }}
                >
                  <Box>
                    <Typography sx={{ fontSize: "0.68rem", color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px" }}>
                      Semana actual
                    </Typography>
                    <Typography sx={{ fontSize: "1.15rem", fontWeight: 800, color: brand.goldDark, lineHeight: 1.2 }}>
                      #{local.semanaActualNumero || "—"}
                    </Typography>
                  </Box>
                  <ChevronRightRoundedIcon sx={{ color: brand.inkFaint }} />
                </Box>
              </GlassCard>
            </Grid>
          ))}
        </Grid>
      )}

      <Dialog
        open={open}
        onClose={() => setOpen(false)}
        fullWidth
        maxWidth="xs"
        slotProps={{
          paper: { sx: { borderRadius: "28px", p: 0.5 } },
          backdrop: { sx: { backdropFilter: "blur(4px)", backgroundColor: "rgba(10,17,40,0.35)" } },
        }}
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
                <StorefrontRoundedIcon sx={{ color: "#fff", fontSize: 20 }} />
              </Box>
              <Typography sx={{ fontWeight: 800, fontSize: "1.15rem", color: brand.ink }}>Nuevo local</Typography>
            </Box>
            <IconButton size="small" onClick={() => setOpen(false)} sx={{ color: brand.inkFaint }}>
              <CloseRoundedIcon fontSize="small" />
            </IconButton>
          </Box>

          <TextField
            label="Nombre"
            fullWidth
            margin="dense"
            value={nombre}
            onChange={(e) => setNombre(e.target.value)}
            sx={glassFieldLight}
            autoFocus
          />
          <TextField
            label="Dirección"
            fullWidth
            margin="dense"
            value={direccion}
            onChange={(e) => setDireccion(e.target.value)}
            sx={glassFieldLight}
          />

          <Box
            component="button"
            onClick={crear}
            disabled={!nombre || saving}
            sx={{ ...pillButtonSx, width: "100%", justifyContent: "center", mt: 3, py: 1.3, fontSize: "0.95rem" }}
          >
            {saving ? "Creando..." : "Crear local"}
          </Box>
        </Box>
      </Dialog>
    </Box>
  );
}
