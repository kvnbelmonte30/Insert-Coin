import { useState } from "react";
import { Box, IconButton, InputAdornment, TextField, Typography } from "@mui/material";
import { keyframes } from "@emotion/react";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import { useNavigate } from "react-router-dom";
import { api } from "../api/client";
import { useAuthStore } from "../store/authStore";
import { PixelCoin } from "../components/PixelCoin";

const drift = keyframes`
  0%, 100% { transform: translate(0, 0) scale(1); }
  33% { transform: translate(3%, -4%) scale(1.08); }
  66% { transform: translate(-3%, 3%) scale(0.96); }
`;

export function CambiarContrasenaPage() {
  const nombre = useAuthStore((s) => s.nombre);
  const setDebeCambiarContrasena = useAuthStore((s) => s.setDebeCambiarContrasena);
  const isAdmin = useAuthStore((s) => s.isAdmin());
  const navigate = useNavigate();

  const [actual, setActual] = useState("");
  const [nueva, setNueva] = useState("");
  const [confirmar, setConfirmar] = useState("");
  const [mostrar, setMostrar] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const glassField = {
    "& .MuiOutlinedInput-root": {
      borderRadius: "14px",
      backgroundColor: "rgba(255,255,255,0.05)",
      backdropFilter: "blur(6px)",
      color: "#f4f6fb",
      transition: "all 0.2s ease",
      "& fieldset": { borderColor: "rgba(255,255,255,0.14)" },
      "&:hover fieldset": { borderColor: "rgba(255,255,255,0.28)" },
      "&.Mui-focused fieldset": { borderColor: "#f0b429", borderWidth: "1.5px" },
      "&.Mui-focused": { boxShadow: "0 0 0 4px rgba(240,180,41,0.12)" },
    },
    "& .MuiInputLabel-root": { color: "rgba(255,255,255,0.55)" },
    "& .MuiInputLabel-root.Mui-focused": { color: "#f0b429" },
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (nueva.length < 8) {
      setError("La nueva contraseña debe tener al menos 8 caracteres.");
      return;
    }
    if (nueva !== confirmar) {
      setError("Las contraseñas no coinciden.");
      return;
    }

    setLoading(true);
    try {
      await api.post("/auth/cambiar-contrasena", { contrasenaActual: actual, contrasenaNueva: nueva });
      setDebeCambiarContrasena(false);
      navigate(isAdmin ? "/admin/dashboard" : "/empleado/cuenta");
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        "No se pudo cambiar la contraseña. Verifica tu contraseña actual.";
      setError(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        position: "relative",
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        overflow: "hidden",
        px: 2,
        background: "radial-gradient(ellipse at 50% 0%, #14224a 0%, #0a1128 55%, #060a1a 100%)",
      }}
    >
      <Box
        sx={{
          position: "absolute",
          width: 480,
          height: 480,
          borderRadius: "50%",
          top: "-10%",
          left: "-8%",
          background: "radial-gradient(circle, rgba(240,180,41,0.35) 0%, rgba(240,180,41,0) 70%)",
          filter: "blur(10px)",
          animation: `${drift} 16s ease-in-out infinite`,
        }}
      />
      <Box
        sx={{
          position: "absolute",
          width: 420,
          height: 420,
          borderRadius: "50%",
          bottom: "-12%",
          right: "-6%",
          background: "radial-gradient(circle, rgba(90,120,255,0.35) 0%, rgba(90,120,255,0) 70%)",
          filter: "blur(10px)",
          animation: `${drift} 20s ease-in-out infinite reverse`,
        }}
      />

      <Box
        sx={{
          position: "relative",
          width: "100%",
          maxWidth: 380,
          borderRadius: "28px",
          border: "1px solid rgba(255,255,255,0.12)",
          background: "rgba(255,255,255,0.06)",
          backdropFilter: "blur(24px) saturate(140%)",
          WebkitBackdropFilter: "blur(24px) saturate(140%)",
          boxShadow: "0 8px 32px rgba(0,0,0,0.45), inset 0 1px 0 rgba(255,255,255,0.12)",
          px: { xs: 3.5, sm: 5 },
          py: 5,
        }}
      >
        <Box sx={{ display: "flex", flexDirection: "column", alignItems: "center", mb: 3 }}>
          <PixelCoin size={40} />
          <Typography
            sx={{
              mt: 2,
              fontWeight: 800,
              fontSize: "1.15rem",
              color: "#f4f6fb",
              textAlign: "center",
            }}
          >
            Cambia tu contraseña
          </Typography>
          <Typography sx={{ mt: 1, color: "rgba(255,255,255,0.5)", fontSize: "0.82rem", textAlign: "center" }}>
            {nombre}, por seguridad debes establecer una nueva contraseña antes de continuar.
          </Typography>
        </Box>

        <form onSubmit={handleSubmit}>
          <TextField
            label="Contraseña actual"
            type={mostrar ? "text" : "password"}
            fullWidth
            margin="normal"
            value={actual}
            onChange={(e) => setActual(e.target.value)}
            sx={glassField}
            autoFocus
          />
          <TextField
            label="Nueva contraseña"
            type={mostrar ? "text" : "password"}
            fullWidth
            margin="normal"
            value={nueva}
            onChange={(e) => setNueva(e.target.value)}
            sx={glassField}
          />
          <TextField
            label="Confirmar nueva contraseña"
            type={mostrar ? "text" : "password"}
            fullWidth
            margin="normal"
            value={confirmar}
            onChange={(e) => setConfirmar(e.target.value)}
            sx={glassField}
            slotProps={{
              input: {
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={() => setMostrar((v) => !v)} edge="end" sx={{ color: "rgba(255,255,255,0.4)" }} tabIndex={-1}>
                      {mostrar ? <VisibilityOff fontSize="small" /> : <Visibility fontSize="small" />}
                    </IconButton>
                  </InputAdornment>
                ),
              },
            }}
          />

          {error && (
            <Box
              sx={{
                mt: 2,
                px: 2,
                py: 1.2,
                borderRadius: "12px",
                border: "1px solid rgba(255,90,90,0.35)",
                background: "rgba(255,90,90,0.1)",
                color: "#ffb4b4",
                fontSize: "0.85rem",
              }}
            >
              {error}
            </Box>
          )}

          <Box
            component="button"
            type="submit"
            disabled={loading}
            sx={{
              mt: 3.5,
              width: "100%",
              border: "none",
              borderRadius: "14px",
              py: 1.4,
              cursor: loading ? "default" : "pointer",
              fontFamily: "inherit",
              fontWeight: 700,
              fontSize: "0.95rem",
              letterSpacing: "1.5px",
              textTransform: "uppercase",
              color: "#1a1206",
              background: "linear-gradient(135deg, #ffd873 0%, #f0b429 55%, #d99a1a 100%)",
              boxShadow: "0 4px 20px rgba(240,180,41,0.4), inset 0 1px 0 rgba(255,255,255,0.5)",
              opacity: loading ? 0.7 : 1,
            }}
          >
            {loading ? "Guardando..." : "Guardar y continuar"}
          </Box>
        </form>
      </Box>
    </Box>
  );
}
