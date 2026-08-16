import { useEffect, useState } from "react";
import { Box, FormControl, InputLabel, MenuItem, Select, Table, TableBody, TableCell, TableHead, TableRow, Typography } from "@mui/material";
import { api } from "../../api/client";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { StatusPill } from "../../components/StatusPill";
import { brand, glassFieldLight, glassTableSx, pillButtonSx } from "../../theme/brand";
import type { CierreSemanal, Local } from "../../types";

function Stat({ label, value }: { label: string; value: string }) {
  return (
    <Box>
      <Typography sx={{ fontSize: "0.68rem", color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px" }}>
        {label}
      </Typography>
      <Typography sx={{ fontSize: "1.15rem", fontWeight: 800, color: brand.ink }}>{value}</Typography>
    </Box>
  );
}

export function CierresSemanalesPage() {
  const [locales, setLocales] = useState<Local[]>([]);
  const [localId, setLocalId] = useState("");
  const [cierres, setCierres] = useState<CierreSemanal[]>([]);

  useEffect(() => {
    api.get<Local[]>("/locales").then((r) => {
      setLocales(r.data);
      if (r.data.length > 0) setLocalId(r.data[0].id);
    });
  }, []);

  const cargar = async () => {
    if (!localId) return;
    const { data } = await api.get<CierreSemanal[]>(`/cierres-semanales/local/${localId}`);
    setCierres(data);
  };

  useEffect(() => {
    cargar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [localId]);

  const confirmar = async (id: string) => {
    await api.post(`/cierres-semanales/${id}/confirmar`);
    cargar();
  };

  return (
    <Box>
      <PageHeader
        title="Cierres semanales"
        subtitle={`${cierres.length} semana${cierres.length === 1 ? "" : "s"} registrada${cierres.length === 1 ? "" : "s"}`}
        action={
          <FormControl size="small" sx={{ minWidth: 200, ...glassFieldLight }}>
            <InputLabel>Local</InputLabel>
            <Select label="Local" value={localId} onChange={(e) => setLocalId(e.target.value)}>
              {locales.map((l) => (
                <MenuItem key={l.id} value={l.id}>
                  {l.nombre}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        }
      />

      <Box sx={{ display: "flex", flexDirection: "column", gap: 2.5 }}>
        {cierres.map((c) => (
          <GlassCard key={c.id} sx={{ p: { xs: 2.5, sm: 3 } }}>
            <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2, flexWrap: "wrap", gap: 1 }}>
              <Typography sx={{ fontWeight: 800, fontSize: "1.2rem", color: brand.ink }}>
                Semana #{c.semanaNumero}
              </Typography>
              <StatusPill label={c.confirmado ? "Confirmado" : "Pendiente"} tone={c.confirmado ? "success" : "warning"} />
            </Box>

            <Typography sx={{ color: brand.inkMuted, fontSize: "0.85rem", mb: 2 }}>
              Creado por {c.creadoPorNombre}
            </Typography>

            <Box sx={{ display: "flex", gap: 4, flexWrap: "wrap", mb: 2.5 }}>
              <Stat label="Total esperado" value={`$${c.totalEsperado.toLocaleString()}`} />
              <Stat label="Total reportado" value={`$${c.totalReportado.toLocaleString()}`} />
              <Box>
                <Typography sx={{ fontSize: "0.68rem", color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px" }}>
                  Diferencia
                </Typography>
                <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                  <Typography sx={{ fontSize: "1.15rem", fontWeight: 800, color: brand.ink }}>
                    ${c.diferencia.toLocaleString()}
                  </Typography>
                  <StatusPill label={c.estadoDiferencia} tone={c.estadoDiferencia === "Correcto" ? "success" : "warning"} />
                </Box>
              </Box>
            </Box>

            <Box sx={{ overflowX: "auto" }}>
              <Table size="small" sx={glassTableSx}>
                <TableHead>
                  <TableRow>
                    <TableCell>Concepto</TableCell>
                    <TableCell align="right">Cantidad</TableCell>
                    <TableCell align="right">Subtotal</TableCell>
                    <TableCell>Premio puesto</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {c.detalles.map((d, i) => (
                    <TableRow key={i}>
                      <TableCell>{d.denominacionNombre ?? d.premioNombre ?? d.concepto}</TableCell>
                      <TableCell align="right">{d.cantidad}</TableCell>
                      <TableCell align="right">${d.subtotal.toLocaleString()}</TableCell>
                      <TableCell>{d.esPremioPuesto ? <StatusPill label="Sí" tone="info" /> : ""}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </Box>

            {!c.confirmado && (
              <Box sx={{ mt: 2.5 }}>
                <Box component="button" onClick={() => confirmar(c.id)} sx={pillButtonSx}>
                  Confirmar cierre y abrir nueva semana
                </Box>
              </Box>
            )}
          </GlassCard>
        ))}

        {cierres.length === 0 && (
          <GlassCard sx={{ p: 5, textAlign: "center" }}>
            <Typography sx={{ color: brand.inkMuted }}>Sin cierres semanales registrados.</Typography>
          </GlassCard>
        )}
      </Box>
    </Box>
  );
}
