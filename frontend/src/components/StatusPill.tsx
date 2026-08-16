import { Box } from "@mui/material";

type Tone = "success" | "warning" | "error" | "neutral" | "info";

const TONES: Record<Tone, { fg: string; bg: string }> = {
  success: { fg: "#1b7a4d", bg: "rgba(27,122,77,0.12)" },
  warning: { fg: "#b06a00", bg: "rgba(240,180,41,0.18)" },
  error: { fg: "#c02b3c", bg: "rgba(192,43,60,0.12)" },
  info: { fg: "#2158b0", bg: "rgba(33,88,176,0.1)" },
  neutral: { fg: "rgba(14,23,48,0.5)", bg: "rgba(14,23,48,0.06)" },
};

export function StatusPill({ label, tone = "neutral" }: { label: string; tone?: Tone }) {
  const c = TONES[tone];
  return (
    <Box
      component="span"
      sx={{
        display: "inline-flex",
        alignItems: "center",
        px: 1.2,
        py: 0.35,
        borderRadius: "999px",
        fontSize: "0.68rem",
        fontWeight: 700,
        letterSpacing: "0.4px",
        textTransform: "uppercase",
        color: c.fg,
        background: c.bg,
        whiteSpace: "nowrap",
      }}
    >
      {label}
    </Box>
  );
}
