import { Box, Typography } from "@mui/material";
import { brand } from "../../theme/brand";

interface Segment {
  label: string;
  value: number;
  color: string;
}

export function DonutChart({
  segments,
  centerLabel,
  centerValue,
  size = 160,
}: {
  segments: Segment[];
  centerLabel: string;
  centerValue: string | number;
  size?: number;
}) {
  const total = segments.reduce((s, x) => s + x.value, 0);
  const radius = size / 2;
  const stroke = size * 0.16;
  const r = radius - stroke / 2;
  const circumference = 2 * Math.PI * r;

  let offset = 0;
  const arcs = segments
    .filter((s) => s.value > 0)
    .map((s) => {
      const fraction = total === 0 ? 0 : s.value / total;
      const dash = fraction * circumference;
      const arc = (
        <circle
          key={s.label}
          cx={radius}
          cy={radius}
          r={r}
          fill="none"
          stroke={s.color}
          strokeWidth={stroke}
          strokeDasharray={`${dash} ${circumference - dash}`}
          strokeDashoffset={-offset}
          transform={`rotate(-90 ${radius} ${radius})`}
        />
      );
      offset += dash;
      return arc;
    });

  return (
    <Box sx={{ display: "flex", flexDirection: "column", alignItems: "center", gap: 1.5 }}>
      <Box sx={{ position: "relative", width: size, height: size }}>
        <svg width={size} height={size}>
          <circle cx={radius} cy={radius} r={r} fill="none" stroke="rgba(14,23,48,0.06)" strokeWidth={stroke} />
          {total > 0 ? arcs : null}
        </svg>
        <Box
          sx={{
            position: "absolute",
            inset: 0,
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Typography sx={{ fontSize: "1.5rem", fontWeight: 800, color: brand.ink, lineHeight: 1 }}>
            {centerValue}
          </Typography>
          <Typography
            sx={{ fontSize: "0.62rem", color: brand.inkMuted, textTransform: "uppercase", letterSpacing: "0.4px", mt: 0.4 }}
          >
            {centerLabel}
          </Typography>
        </Box>
      </Box>
      <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1.2, justifyContent: "center" }}>
        {segments.map((s) => (
          <Box key={s.label} sx={{ display: "flex", alignItems: "center", gap: 0.6 }}>
            <Box sx={{ width: 8, height: 8, borderRadius: "50%", background: s.color, flexShrink: 0 }} />
            <Typography sx={{ fontSize: "0.72rem", color: brand.inkMuted, whiteSpace: "nowrap" }}>
              {s.label} ({s.value})
            </Typography>
          </Box>
        ))}
        {segments.length === 0 && (
          <Typography sx={{ fontSize: "0.8rem", color: brand.inkMuted }}>Sin datos todavía.</Typography>
        )}
      </Box>
    </Box>
  );
}
