import type { ReactNode } from "react";
import { Box, Typography } from "@mui/material";
import { brand } from "../theme/brand";

export function PageHeader({
  title,
  subtitle,
  action,
}: {
  title: string;
  subtitle?: string;
  action?: ReactNode;
}) {
  return (
    <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "flex-end", mb: 3.5, flexWrap: "wrap", gap: 2 }}>
      <Box>
        <Typography sx={{ fontSize: { xs: "1.6rem", sm: "1.9rem" }, fontWeight: 800, color: brand.ink, letterSpacing: "-0.5px" }}>
          {title}
        </Typography>
        {subtitle && (
          <Typography sx={{ color: brand.inkMuted, fontSize: "0.9rem", mt: 0.3 }}>{subtitle}</Typography>
        )}
      </Box>
      {action && <Box sx={{ display: "flex", gap: 1.2, alignItems: "center", flexWrap: "wrap" }}>{action}</Box>}
    </Box>
  );
}
