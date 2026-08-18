import { Box } from "@mui/material";
import { brand } from "../../theme/brand";

interface Point {
  label: string;
  value: number;
}

export function TrendChart({ points, height = 140 }: { points: Point[]; height?: number }) {
  const width = Math.max(280, points.length * 46);
  const max = Math.max(1, ...points.map((p) => p.value));
  const stepX = points.length > 1 ? width / (points.length - 1) : 0;
  const padY = 18;
  const usableH = height - padY * 2;

  const coords = points.map((p, i) => ({
    x: points.length === 1 ? width / 2 : i * stepX,
    y: padY + usableH - (p.value / max) * usableH,
  }));

  const linePath = coords.map((c, i) => `${i === 0 ? "M" : "L"} ${c.x} ${c.y}`).join(" ");
  const areaPath =
    coords.length > 0
      ? `${linePath} L ${coords[coords.length - 1].x} ${height - padY} L ${coords[0].x} ${height - padY} Z`
      : "";

  return (
    <Box sx={{ overflowX: "auto" }}>
      <svg width={width} height={height} style={{ display: "block" }}>
        <defs>
          <linearGradient id="trendFill" x1="0" y1="0" x2="0" y2="1">
            <stop offset="0%" stopColor={brand.gold} stopOpacity="0.35" />
            <stop offset="100%" stopColor={brand.gold} stopOpacity="0" />
          </linearGradient>
        </defs>
        {points.length > 0 && <path d={areaPath} fill="url(#trendFill)" />}
        {points.length > 0 && (
          <path d={linePath} fill="none" stroke={brand.goldDark} strokeWidth={2.5} strokeLinecap="round" strokeLinejoin="round" />
        )}
        {coords.map((c, i) => (
          <circle key={points[i].label} cx={c.x} cy={c.y} r={3.5} fill={brand.goldDark} />
        ))}
        {points.map((p, i) => (
          <text key={p.label} x={coords[i].x} y={height - 2} fontSize="9" textAnchor="middle" fill={brand.inkFaint}>
            {p.label}
          </text>
        ))}
      </svg>
    </Box>
  );
}
