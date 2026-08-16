import type { ReactNode } from "react";
import { Box, type SxProps, type Theme } from "@mui/material";
import { brand } from "../theme/brand";

export function GlassCard({
  children,
  onClick,
  sx,
}: {
  children: ReactNode;
  onClick?: () => void;
  sx?: SxProps<Theme>;
}) {
  return (
    <Box
      onClick={onClick}
      sx={{
        position: "relative",
        borderRadius: "24px",
        background: brand.glassBg,
        backdropFilter: "blur(20px) saturate(160%)",
        WebkitBackdropFilter: "blur(20px) saturate(160%)",
        border: `1px solid ${brand.glassBorder}`,
        boxShadow: brand.glassShadow,
        cursor: onClick ? "pointer" : "default",
        transition: "transform 0.18s ease, box-shadow 0.18s ease",
        ...(onClick && {
          "&:hover": { transform: "translateY(-3px)", boxShadow: brand.glassShadowHover },
          "&:active": { transform: "translateY(-1px)" },
        }),
        ...sx,
      }}
    >
      {children}
    </Box>
  );
}
