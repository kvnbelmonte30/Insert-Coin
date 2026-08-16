export const brand = {
  navy: "#0a1128",
  navyDeep: "#060a1a",
  navyMid: "#14224a",
  navySoft: "#1e3a5f",
  gold: "#f0b429",
  goldLight: "#ffd873",
  goldDark: "#d99a1a",
  goldGradient: "linear-gradient(135deg, #ffd873 0%, #f0b429 55%, #d99a1a 100%)",
  ink: "#0e1730",
  inkMuted: "rgba(14,23,48,0.55)",
  inkFaint: "rgba(14,23,48,0.38)",
  page: "#f2f4fa",
  glassBg: "rgba(255,255,255,0.72)",
  glassBorder: "rgba(255,255,255,0.6)",
  glassShadow: "0 10px 30px rgba(10,17,40,0.08), 0 1px 2px rgba(10,17,40,0.04)",
  glassShadowHover: "0 16px 40px rgba(10,17,40,0.14), 0 2px 4px rgba(10,17,40,0.06)",
};

/** Text field styling for forms sitting on a light/glass background (dark text, gold focus). */
export const glassFieldLight = {
  "& .MuiOutlinedInput-root": {
    borderRadius: "14px",
    backgroundColor: "rgba(255,255,255,0.6)",
    transition: "all 0.2s ease",
    "& fieldset": { borderColor: "rgba(10,17,40,0.12)" },
    "&:hover fieldset": { borderColor: "rgba(10,17,40,0.22)" },
    "&.Mui-focused fieldset": { borderColor: brand.gold, borderWidth: "1.5px" },
    "&.Mui-focused": { boxShadow: `0 0 0 4px rgba(240,180,41,0.14)` },
  },
  "& .MuiInputLabel-root.Mui-focused": { color: brand.goldDark },
};

export const pillButtonSx = {
  border: "none",
  borderRadius: "999px",
  cursor: "pointer",
  fontFamily: "inherit",
  fontWeight: 700,
  fontSize: "0.85rem",
  letterSpacing: "0.3px",
  color: "#1a1206",
  background: brand.goldGradient,
  boxShadow: "0 4px 16px rgba(240,180,41,0.38), inset 0 1px 0 rgba(255,255,255,0.5)",
  transition: "transform 0.15s ease, box-shadow 0.15s ease, opacity 0.15s ease",
  display: "inline-flex",
  alignItems: "center",
  gap: 0.7,
  px: 2.2,
  py: 1,
  "&:hover": {
    transform: "translateY(-1px)",
    boxShadow: "0 6px 20px rgba(240,180,41,0.5), inset 0 1px 0 rgba(255,255,255,0.5)",
  },
  "&:active": { transform: "translateY(0)" },
  "&:disabled": { opacity: 0.55, cursor: "default", transform: "none" },
};

export const pillOutlineButtonSx = {
  border: `1px solid rgba(14,23,48,0.16)`,
  borderRadius: "999px",
  cursor: "pointer",
  fontFamily: "inherit",
  fontWeight: 600,
  fontSize: "0.85rem",
  letterSpacing: "0.2px",
  color: brand.ink,
  background: "rgba(255,255,255,0.5)",
  transition: "all 0.15s ease",
  display: "inline-flex",
  alignItems: "center",
  gap: 0.7,
  px: 2,
  py: 0.9,
  "&:hover": { background: "rgba(255,255,255,0.85)", borderColor: "rgba(14,23,48,0.28)" },
  "&:disabled": { opacity: 0.5, cursor: "default" },
};

export const dialogPaperSx = { borderRadius: "28px", p: 0.5 };
export const dialogBackdropSx = { backdropFilter: "blur(4px)", backgroundColor: "rgba(10,17,40,0.35)" };

/** Consistent look for MUI <Table> sitting inside a GlassCard: quiet header, soft row separators, hover. */
export const glassTableSx = {
  "& .MuiTableHead-root .MuiTableCell-root": {
    border: "none",
    fontSize: "0.7rem",
    fontWeight: 700,
    letterSpacing: "0.4px",
    textTransform: "uppercase",
    color: brand.inkFaint,
    paddingBottom: "10px",
  },
  "& .MuiTableBody-root .MuiTableRow-root": {
    transition: "background 0.15s ease",
  },
  "& .MuiTableBody-root .MuiTableRow-root:hover": {
    background: "rgba(14,23,48,0.03)",
  },
  "& .MuiTableCell-root": {
    borderBottom: "1px solid rgba(14,23,48,0.06)",
    color: brand.ink,
    fontSize: "0.85rem",
  },
  "& .MuiTableBody-root .MuiTableRow-root:last-of-type .MuiTableCell-root": {
    borderBottom: "none",
  },
};
