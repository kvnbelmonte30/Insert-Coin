import { Box, Typography } from "@mui/material";
import { brand } from "../../theme/brand";

interface Row {
  label: string;
  value: number;
  sublabel?: string;
}

export function BarRows({ rows, emptyLabel = "Sin datos todavía." }: { rows: Row[]; emptyLabel?: string }) {
  const max = Math.max(1, ...rows.map((r) => r.value));

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 1.5 }}>
      {rows.map((r) => (
        <Box key={r.label}>
          <Box sx={{ display: "flex", justifyContent: "space-between", mb: 0.5, gap: 1 }}>
            <Typography sx={{ fontSize: "0.82rem", color: brand.ink, fontWeight: 600, minWidth: 0 }} noWrap>
              {r.label}
            </Typography>
            <Typography sx={{ fontSize: "0.8rem", color: brand.inkMuted, whiteSpace: "nowrap" }}>
              {r.value}
              {r.sublabel ? ` ${r.sublabel}` : ""}
            </Typography>
          </Box>
          <Box sx={{ height: 8, borderRadius: "999px", background: "rgba(14,23,48,0.06)", overflow: "hidden" }}>
            <Box
              sx={{
                height: "100%",
                width: `${(r.value / max) * 100}%`,
                borderRadius: "999px",
                background: brand.goldGradient,
                transition: "width 0.4s ease",
              }}
            />
          </Box>
        </Box>
      ))}
      {rows.length === 0 && <Typography sx={{ color: brand.inkMuted, fontSize: "0.85rem" }}>{emptyLabel}</Typography>}
    </Box>
  );
}
