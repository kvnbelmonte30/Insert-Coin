import { useEffect, useState } from "react";
import { Badge, Box, IconButton, Menu, Typography } from "@mui/material";
import NotificationsRoundedIcon from "@mui/icons-material/NotificationsRounded";
import { api } from "../api/client";
import type { Notificacion } from "../types";

function tiempoRelativo(fecha: string): string {
  const diffMs = Date.now() - new Date(fecha).getTime();
  const minutos = Math.floor(diffMs / 60000);
  if (minutos < 1) return "justo ahora";
  if (minutos < 60) return `hace ${minutos} min`;
  const horas = Math.floor(minutos / 60);
  if (horas < 24) return `hace ${horas} h`;
  const dias = Math.floor(horas / 24);
  return `hace ${dias} d`;
}

export function NotificationBell({ dark = true }: { dark?: boolean }) {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [notificaciones, setNotificaciones] = useState<Notificacion[]>([]);
  const [noLeidas, setNoLeidas] = useState(0);

  const cargarConteo = () => {
    api.get<number>("/notificaciones/no-leidas-count").then((r) => setNoLeidas(r.data));
  };

  useEffect(() => {
    cargarConteo();
    const interval = setInterval(cargarConteo, 60000);
    return () => clearInterval(interval);
  }, []);

  const abrir = (e: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(e.currentTarget);
    api.get<Notificacion[]>("/notificaciones").then((r) => setNotificaciones(r.data));
  };

  const cerrar = () => setAnchorEl(null);

  const marcarTodasLeidas = async () => {
    await api.put("/notificaciones/leer-todas");
    setNotificaciones((prev) => prev.map((n) => ({ ...n, leida: true })));
    setNoLeidas(0);
  };

  const marcarLeida = async (n: Notificacion) => {
    if (n.leida) return;
    await api.put(`/notificaciones/${n.id}/leer`);
    setNotificaciones((prev) => prev.map((x) => (x.id === n.id ? { ...x, leida: true } : x)));
    setNoLeidas((c) => Math.max(0, c - 1));
  };

  const color = dark ? "#f4f6fb" : "rgba(14,23,48,0.7)";

  return (
    <>
      <IconButton onClick={abrir} size="small" sx={{ color }}>
        <Badge
          badgeContent={noLeidas}
          max={9}
          sx={{ "& .MuiBadge-badge": { background: "#f0b429", color: "#1a1206", fontWeight: 700 } }}
        >
          <NotificationsRoundedIcon fontSize="small" />
        </Badge>
      </IconButton>

      <Menu
        anchorEl={anchorEl}
        open={!!anchorEl}
        onClose={cerrar}
        anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
        transformOrigin={{ vertical: "top", horizontal: "right" }}
        slotProps={{
          paper: {
            sx: {
              mt: 1,
              width: 340,
              maxHeight: 420,
              borderRadius: "18px",
              background: "linear-gradient(160deg, rgba(20,34,74,0.97) 0%, rgba(8,13,32,0.98) 100%)",
              backdropFilter: "blur(20px) saturate(160%)",
              border: "1px solid rgba(255,255,255,0.1)",
              boxShadow: "0 16px 40px rgba(10,17,40,0.4)",
              color: "#fff",
            },
          },
        }}
      >
        <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", px: 2, py: 1.5 }}>
          <Typography sx={{ fontWeight: 700, fontSize: "0.9rem" }}>Notificaciones</Typography>
          {noLeidas > 0 && (
            <Box
              component="button"
              onClick={marcarTodasLeidas}
              sx={{
                border: "none",
                background: "none",
                color: "#f0b429",
                fontSize: "0.72rem",
                fontWeight: 600,
                cursor: "pointer",
                fontFamily: "inherit",
              }}
            >
              Marcar todas como leídas
            </Box>
          )}
        </Box>
        <Box sx={{ height: "1px", background: "rgba(255,255,255,0.08)" }} />

        <Box sx={{ maxHeight: 340, overflowY: "auto" }}>
          {notificaciones.length === 0 && (
            <Typography sx={{ color: "rgba(255,255,255,0.5)", fontSize: "0.82rem", textAlign: "center", py: 4 }}>
              Sin notificaciones todavía.
            </Typography>
          )}
          {notificaciones.map((n) => (
            <Box
              key={n.id}
              onClick={() => marcarLeida(n)}
              sx={{
                px: 2,
                py: 1.4,
                cursor: n.leida ? "default" : "pointer",
                borderBottom: "1px solid rgba(255,255,255,0.06)",
                background: n.leida ? "transparent" : "rgba(240,180,41,0.08)",
                display: "flex",
                gap: 1,
                alignItems: "flex-start",
              }}
            >
              {!n.leida && (
                <Box sx={{ width: 7, height: 7, borderRadius: "50%", background: "#f0b429", mt: 0.6, flexShrink: 0 }} />
              )}
              <Box sx={{ minWidth: 0 }}>
                <Typography sx={{ fontSize: "0.82rem", color: "#f4f6fb", lineHeight: 1.4 }}>{n.mensaje}</Typography>
                <Typography sx={{ fontSize: "0.7rem", color: "rgba(255,255,255,0.45)", mt: 0.4 }}>
                  {tiempoRelativo(n.fechaCreacion)}
                </Typography>
              </Box>
            </Box>
          ))}
        </Box>
      </Menu>
    </>
  );
}
