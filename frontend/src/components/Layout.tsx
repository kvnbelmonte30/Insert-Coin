import type { ReactNode } from "react";
import { useState } from "react";
import { Box, Drawer, IconButton, Typography } from "@mui/material";
import MenuRoundedIcon from "@mui/icons-material/MenuRounded";
import CloseRoundedIcon from "@mui/icons-material/CloseRounded";
import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";
import SpaceDashboardRoundedIcon from "@mui/icons-material/SpaceDashboardRounded";
import StorefrontRoundedIcon from "@mui/icons-material/StorefrontRounded";
import CasinoRoundedIcon from "@mui/icons-material/CasinoRounded";
import ReportProblemRoundedIcon from "@mui/icons-material/ReportProblemRounded";
import ReceiptLongRoundedIcon from "@mui/icons-material/ReceiptLongRounded";
import EventRepeatRoundedIcon from "@mui/icons-material/EventRepeatRounded";
import GroupRoundedIcon from "@mui/icons-material/GroupRounded";
import AccountBalanceWalletRoundedIcon from "@mui/icons-material/AccountBalanceWalletRounded";
import WavesRoundedIcon from "@mui/icons-material/WavesRounded";
import { useLocation, useNavigate } from "react-router-dom";
import { useAuthStore } from "../store/authStore";
import { PixelCoin } from "./PixelCoin";

interface NavItem {
  value: string;
  label: string;
  icon: React.ReactElement;
}

const ADMIN_NAV: NavItem[] = [
  { value: "dashboard", label: "Dashboard", icon: <SpaceDashboardRoundedIcon fontSize="small" /> },
  { value: "locales", label: "Locales", icon: <StorefrontRoundedIcon fontSize="small" /> },
  { value: "maquinas", label: "Máquinas", icon: <CasinoRoundedIcon fontSize="small" /> },
  { value: "averias", label: "Averías", icon: <ReportProblemRoundedIcon fontSize="small" /> },
  { value: "gastos", label: "Gastos", icon: <ReceiptLongRoundedIcon fontSize="small" /> },
  { value: "cierres-semanales", label: "Cierres semanales", icon: <EventRepeatRoundedIcon fontSize="small" /> },
  { value: "usuarios", label: "Usuarios", icon: <GroupRoundedIcon fontSize="small" /> },
];

const EMPLEADO_NAV: NavItem[] = [
  { value: "cuenta", label: "Mi cuenta", icon: <AccountBalanceWalletRoundedIcon fontSize="small" /> },
  { value: "averias", label: "Averías", icon: <ReportProblemRoundedIcon fontSize="small" /> },
  { value: "cascadas", label: "Cascadas", icon: <WavesRoundedIcon fontSize="small" /> },
  { value: "cierre-semanal", label: "Cierre semanal", icon: <EventRepeatRoundedIcon fontSize="small" /> },
];

const glass = {
  background: "linear-gradient(135deg, rgba(20,34,74,0.75) 0%, rgba(10,17,40,0.8) 100%)",
  backdropFilter: "blur(20px) saturate(160%)",
  WebkitBackdropFilter: "blur(20px) saturate(160%)",
  border: "1px solid rgba(255,255,255,0.1)",
};

export function Layout({ children }: { children: ReactNode }) {
  const navigate = useNavigate();
  const location = useLocation();
  const { nombre, roles, logout } = useAuthStore();
  const [drawerOpen, setDrawerOpen] = useState(false);

  const isAdmin = location.pathname.startsWith("/admin");
  const basePath = isAdmin ? "/admin" : "/empleado";
  const navItems = isAdmin ? ADMIN_NAV : EMPLEADO_NAV;
  const activeValue = navItems.find((item) => location.pathname.startsWith(`${basePath}/${item.value}`))?.value;

  const goTo = (value: string) => {
    navigate(`${basePath}/${value}`);
    setDrawerOpen(false);
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const initials = (nombre ?? "?")
    .split(" ")
    .map((p) => p[0])
    .slice(0, 2)
    .join("")
    .toUpperCase();

  return (
    <Box sx={{ minHeight: "100vh", bgcolor: "#f2f4fa" }}>
      {/* floating glass nav bar */}
      <Box
        sx={{
          position: "sticky",
          top: { xs: 10, sm: 16 },
          zIndex: 1100,
          mx: { xs: 1.5, sm: 3 },
          mb: 3,
        }}
      >
        <Box
          sx={{
            ...glass,
            borderRadius: "20px",
            boxShadow: "0 8px 28px rgba(10,17,40,0.28)",
            px: { xs: 1.5, sm: 2.5 },
            py: 1,
            display: "flex",
            alignItems: "center",
            gap: 1.5,
          }}
        >
          <PixelCoin size={26} />
          <Typography
            sx={{
              fontFamily: "'Press Start 2P', monospace",
              fontSize: "0.62rem",
              color: "#f4f6fb",
              letterSpacing: "0.5px",
              display: { xs: "none", sm: "block" },
              whiteSpace: "nowrap",
            }}
          >
            INSERT COIN
          </Typography>

          {/* desktop pill nav */}
          <Box sx={{ flexGrow: 1, display: { xs: "none", md: "flex" }, gap: 0.5, justifyContent: "center" }}>
            {navItems.map((item) => {
              const active = item.value === activeValue;
              return (
                <Box
                  key={item.value}
                  component="button"
                  onClick={() => goTo(item.value)}
                  sx={{
                    display: "flex",
                    alignItems: "center",
                    gap: 0.7,
                    border: "none",
                    borderRadius: "999px",
                    px: 1.6,
                    py: 0.7,
                    cursor: "pointer",
                    fontFamily: "inherit",
                    fontSize: "0.78rem",
                    fontWeight: 600,
                    whiteSpace: "nowrap",
                    color: active ? "#1a1206" : "rgba(255,255,255,0.75)",
                    background: active
                      ? "linear-gradient(135deg, #ffd873 0%, #f0b429 100%)"
                      : "transparent",
                    boxShadow: active ? "0 2px 12px rgba(240,180,41,0.45)" : "none",
                    transition: "all 0.15s ease",
                    "&:hover": {
                      background: active ? undefined : "rgba(255,255,255,0.08)",
                      color: active ? "#1a1206" : "#fff",
                    },
                  }}
                >
                  {item.icon}
                  {item.label}
                </Box>
              );
            })}
          </Box>

          <Box sx={{ flexGrow: { xs: 1, md: 0 } }} />

          {/* desktop user + logout */}
          <Box sx={{ display: { xs: "none", md: "flex" }, alignItems: "center", gap: 1.2 }}>
            <Box
              sx={{
                px: 1.2,
                py: 0.4,
                borderRadius: "999px",
                border: "1px solid rgba(240,180,41,0.4)",
                color: "#f0b429",
                fontSize: "0.7rem",
                fontWeight: 700,
                letterSpacing: "0.5px",
                textTransform: "uppercase",
              }}
            >
              {roles[0]}
            </Box>
            <Typography sx={{ color: "rgba(255,255,255,0.85)", fontSize: "0.85rem", whiteSpace: "nowrap" }}>
              {nombre}
            </Typography>
            <IconButton onClick={handleLogout} size="small" sx={{ color: "rgba(255,255,255,0.7)" }}>
              <LogoutRoundedIcon fontSize="small" />
            </IconButton>
          </Box>

          {/* mobile hamburger */}
          <IconButton
            onClick={() => setDrawerOpen(true)}
            sx={{ display: { xs: "flex", md: "none" }, color: "#f4f6fb" }}
          >
            <MenuRoundedIcon />
          </IconButton>
        </Box>
      </Box>

      {/* mobile glass drawer */}
      <Drawer
        anchor="right"
        open={drawerOpen}
        onClose={() => setDrawerOpen(false)}
        slotProps={{
          paper: {
            sx: {
              width: 290,
              ...glass,
              background: "linear-gradient(160deg, rgba(20,34,74,0.96) 0%, rgba(8,13,32,0.98) 100%)",
              borderLeft: "1px solid rgba(255,255,255,0.1)",
              display: "flex",
              flexDirection: "column",
            },
          },
        }}
      >
        <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", px: 2.5, pt: 3, pb: 1 }}>
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            <PixelCoin size={26} />
            <Typography
              sx={{
                fontFamily: "'Press Start 2P', monospace",
                fontSize: "0.6rem",
                color: "#f4f6fb",
              }}
            >
              INSERT COIN
            </Typography>
          </Box>
          <IconButton onClick={() => setDrawerOpen(false)} sx={{ color: "rgba(255,255,255,0.7)" }}>
            <CloseRoundedIcon fontSize="small" />
          </IconButton>
        </Box>

        <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, px: 2.5, py: 2 }}>
          <Box
            sx={{
              width: 40,
              height: 40,
              borderRadius: "50%",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              fontWeight: 700,
              fontSize: "0.85rem",
              color: "#1a1206",
              background: "linear-gradient(135deg, #ffd873 0%, #f0b429 100%)",
              flexShrink: 0,
            }}
          >
            {initials}
          </Box>
          <Box sx={{ minWidth: 0 }}>
            <Typography sx={{ color: "#fff", fontSize: "0.9rem", fontWeight: 600, lineHeight: 1.2 }} noWrap>
              {nombre}
            </Typography>
            <Typography sx={{ color: "#f0b429", fontSize: "0.7rem", letterSpacing: "0.5px", textTransform: "uppercase" }}>
              {roles[0]}
            </Typography>
          </Box>
        </Box>

        <Box sx={{ height: "1px", background: "rgba(255,255,255,0.1)", mx: 2.5, mb: 1 }} />

        <Box sx={{ flexGrow: 1, overflowY: "auto", py: 1 }}>
          {navItems.map((item) => {
            const active = item.value === activeValue;
            return (
              <Box
                key={item.value}
                component="button"
                onClick={() => goTo(item.value)}
                sx={{
                  width: "calc(100% - 32px)",
                  mx: 2,
                  mb: 0.5,
                  display: "flex",
                  alignItems: "center",
                  gap: 1.5,
                  border: "none",
                  borderRadius: "14px",
                  px: 1.8,
                  py: 1.3,
                  cursor: "pointer",
                  fontFamily: "inherit",
                  fontSize: "0.9rem",
                  fontWeight: 600,
                  textAlign: "left",
                  color: active ? "#1a1206" : "rgba(255,255,255,0.8)",
                  background: active
                    ? "linear-gradient(135deg, #ffd873 0%, #f0b429 100%)"
                    : "rgba(255,255,255,0.04)",
                  boxShadow: active ? "0 2px 14px rgba(240,180,41,0.4)" : "none",
                }}
              >
                {item.icon}
                {item.label}
              </Box>
            );
          })}
        </Box>

        <Box sx={{ p: 2.5 }}>
          <Box
            component="button"
            onClick={handleLogout}
            sx={{
              width: "100%",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              gap: 1,
              border: "1px solid rgba(255,255,255,0.15)",
              borderRadius: "14px",
              py: 1.3,
              cursor: "pointer",
              fontFamily: "inherit",
              fontSize: "0.85rem",
              fontWeight: 600,
              color: "rgba(255,255,255,0.85)",
              background: "rgba(255,255,255,0.05)",
            }}
          >
            <LogoutRoundedIcon fontSize="small" />
            Salir
          </Box>
        </Box>
      </Drawer>

      <Box sx={{ px: { xs: 2, sm: 3 }, pb: 4, maxWidth: 1200, mx: "auto" }}>{children}</Box>
    </Box>
  );
}
