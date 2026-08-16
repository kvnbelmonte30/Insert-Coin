import { Box } from "@mui/material";
import { keyframes } from "@emotion/react";

const flicker = keyframes`
  0%, 100% { opacity: 1; }
  92% { opacity: 1; }
  93% { opacity: 0.6; }
  94% { opacity: 1; }
  96% { opacity: 0.7; }
  97% { opacity: 1; }
`;

const ROWS = [
  "00111100",
  "01222210",
  "12223210",
  "12232210",
  "12222210",
  "12222210",
  "01222210",
  "00111100",
];

const PALETTE: Record<string, string> = {
  "1": "#7a4e0a",
  "2": "#f0b429",
  "3": "#ffe28a",
};

export function PixelCoin({ size = 40, animate = true }: { size?: number; animate?: boolean }) {
  return (
    <Box
      component="svg"
      viewBox="0 0 8 8"
      sx={{
        width: size,
        height: size,
        flexShrink: 0,
        filter: "drop-shadow(0 0 10px rgba(240,180,41,0.65))",
        animation: animate ? `${flicker} 3.2s ease-in-out infinite` : "none",
      }}
    >
      {ROWS.map((row, y) =>
        row.split("").map((cell, x) =>
          cell === "0" ? null : (
            <rect key={`${x}-${y}`} x={x} y={y} width={1} height={1} fill={PALETTE[cell]} />
          )
        )
      )}
    </Box>
  );
}
