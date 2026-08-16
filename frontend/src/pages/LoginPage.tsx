import { useState } from "react";
import { Box, IconButton, InputAdornment, TextField, Typography } from "@mui/material";
import { keyframes } from "@emotion/react";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import { useNavigate } from "react-router-dom";
import { api } from "../api/client";
import { useAuthStore } from "../store/authStore";
import { PixelCoin } from "../components/PixelCoin";
import type { LoginResponse } from "../types";

const drift = keyframes`
  0%, 100% { transform: translate(0, 0) scale(1); }
  33% { transform: translate(3%, -4%) scale(1.08); }
  66% { transform: translate(-3%, 3%) scale(0.96); }
`;

export function LoginPage() {
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const setSession = useAuthStore((s) => s.setSession);
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const { data } = await api.post<LoginResponse>("/auth/login", { userName, password });
      setSession(data);
      navigate("/");
    } catch {
      setError("Usuario o contraseña incorrectos.");
    } finally {
      setLoading(false);
    }
  };

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
    "& .MuiInputBase-input": { caretColor: "#f0b429" },
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
      {/* liquid glass background blobs */}
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
          position: "absolute",
          width: 320,
          height: 320,
          borderRadius: "50%",
          top: "35%",
          right: "18%",
          background: "radial-gradient(circle, rgba(199,90,255,0.22) 0%, rgba(199,90,255,0) 70%)",
          filter: "blur(10px)",
          animation: `${drift} 24s ease-in-out infinite`,
        }}
      />

      {/* faint pixel-grid texture */}
      <Box
        sx={{
          position: "absolute",
          inset: 0,
          opacity: 0.05,
          backgroundImage:
            "linear-gradient(rgba(255,255,255,0.6) 1px, transparent 1px), linear-gradient(90deg, rgba(255,255,255,0.6) 1px, transparent 1px)",
          backgroundSize: "28px 28px",
        }}
      />

      {/* glass card */}
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
        <Box sx={{ display: "flex", flexDirection: "column", alignItems: "center", mb: 4 }}>
          <PixelCoin size={44} />
          <Typography
            sx={{
              mt: 2,
              fontFamily: "'Press Start 2P', monospace",
              fontSize: { xs: "1.05rem", sm: "1.2rem" },
              color: "#f4f6fb",
              letterSpacing: "1px",
              textAlign: "center",
              lineHeight: 1.6,
              textShadow: "0 0 18px rgba(240,180,41,0.35)",
            }}
          >
            INSERT
            <br />
            COIN
          </Typography>
          <Typography
            sx={{
              mt: 2,
              color: "rgba(255,255,255,0.5)",
              fontSize: "0.75rem",
              letterSpacing: "3px",
              textTransform: "uppercase",
            }}
          >
            Sistema de Maquinitas
          </Typography>
        </Box>

        <form onSubmit={handleSubmit}>
          <TextField
            label="Usuario"
            fullWidth
            margin="normal"
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            autoFocus
            sx={glassField}
          />
          <TextField
            label="Contraseña"
            type={showPassword ? "text" : "password"}
            fullWidth
            margin="normal"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            sx={glassField}
            slotProps={{
              input: {
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() => setShowPassword((v) => !v)}
                      edge="end"
                      sx={{ color: "rgba(255,255,255,0.4)" }}
                      tabIndex={-1}
                    >
                      {showPassword ? <VisibilityOff fontSize="small" /> : <Visibility fontSize="small" />}
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
              transition: "transform 0.15s ease, box-shadow 0.15s ease, opacity 0.15s ease",
              opacity: loading ? 0.7 : 1,
              "&:hover": loading
                ? {}
                : {
                    transform: "translateY(-1px)",
                    boxShadow: "0 6px 26px rgba(240,180,41,0.55), inset 0 1px 0 rgba(255,255,255,0.5)",
                  },
              "&:active": loading ? {} : { transform: "translateY(0)" },
            }}
          >
            {loading ? "Ingresando..." : "Ingresar"}
          </Box>
        </form>
      </Box>
    </Box>
  );
}
