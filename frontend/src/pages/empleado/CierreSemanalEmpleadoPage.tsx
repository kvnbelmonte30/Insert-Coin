import { useEffect, useState } from "react";
import { Box, FormControl, InputLabel, MenuItem, Select, TextField, Typography } from "@mui/material";
import InfoRoundedIcon from "@mui/icons-material/InfoRounded";
import CheckCircleRoundedIcon from "@mui/icons-material/CheckCircleRounded";
import { api } from "../../api/client";
import { useAuthStore } from "../../store/authStore";
import { GlassCard } from "../../components/GlassCard";
import { PageHeader } from "../../components/PageHeader";
import { StatusPill } from "../../components/StatusPill";
import { brand, glassFieldLight, pillButtonSx } from "../../theme/brand";
import type { CierreSemanal, Denominacion, Local, Premio } from "../../types";

function SectionLabel({ children }: { children: React.ReactNode }) {
  return (
    <Typography sx={{ fontSize: "0.75rem", fontWeight: 700, color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px", mb: 1, mt: 2.5 }}>
      {children}
    </Typography>
  );
}

export function CierreSemanalEmpleadoPage() {
  const locales = useAuthStore((s) => s.locales);
  const [localId, setLocalId] = useState(locales[0]?.id ?? "");
  const [local, setLocal] = useState<Local | null>(null);
  const [cierreActual, setCierreActual] = useState<CierreSemanal | null>(null);
  const [denominaciones, setDenominaciones] = useState<Denominacion[]>([]);
  const [premios, setPremios] = useState<Premio[]>([]);
  const [cantidades, setCantidades] = useState<Record<string, number>>({});
  const [puestos, setPuestos] = useState<Record<string, number>>({});
  const [terminal, setTerminal] = useState(0);
  const [transferencia, setTransferencia] = useState(0);

  const cargar = async () => {
    if (!localId) return;
    const [localRes, denomRes, premioRes] = await Promise.all([
      api.get<Local>(`/locales/${localId}`),
      api.get<Denominacion[]>("/catalogos/denominaciones"),
      api.get<Premio[]>("/catalogos/premios"),
    ]);
    setLocal(localRes.data);
    setDenominaciones(denomRes.data);
    setPremios(premioRes.data);

    if (localRes.data.semanaActualId) {
      const cierres = await api.get<CierreSemanal[]>(`/cierres-semanales/local/${localId}`);
      const actual = cierres.data.find((c) => c.semanaId === localRes.data.semanaActualId);
      setCierreActual(actual ?? null);
    }
  };

  useEffect(() => {
    cargar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [localId]);

  const construirLineas = () => [
    ...denominaciones.filter((d) => (cantidades[d.id] ?? 0) > 0).map((d) => ({
      concepto: d.tipo,
      denominacionId: d.id,
      cantidad: cantidades[d.id],
    })),
    ...premios.filter((p) => (cantidades[p.id] ?? 0) > 0).map((p) => ({
      concepto: "Premio",
      premioId: p.id,
      cantidad: cantidades[p.id],
    })),
    ...premios.filter((p) => (puestos[p.id] ?? 0) > 0).map((p) => ({
      concepto: "Premio",
      premioId: p.id,
      cantidad: puestos[p.id],
      esPremioPuesto: true,
    })),
    ...(terminal > 0 ? [{ concepto: "Terminal", cantidad: 1, monto: terminal }] : []),
    ...(transferencia > 0 ? [{ concepto: "Transferencia", cantidad: 1, monto: transferencia }] : []),
  ];

  const registrar = async () => {
    if (!local?.semanaActualId) return;
    const { data } = await api.post<CierreSemanal>(`/cierres-semanales/semana/${local.semanaActualId}`, {
      lineas: construirLineas(),
    });
    setCierreActual(data);
  };

  const corregir = async () => {
    if (!cierreActual) return;
    const { data } = await api.put<CierreSemanal>(`/cierres-semanales/${cierreActual.id}`, {
      lineas: construirLineas(),
    });
    setCierreActual(data);
  };

  return (
    <Box>
      <PageHeader
        title="Cierre semanal"
        subtitle={local ? `Semana actual #${local.semanaActualNumero}` : undefined}
        action={
          locales.length > 1 ? (
            <FormControl size="small" sx={{ minWidth: 190, ...glassFieldLight }}>
              <InputLabel>Local</InputLabel>
              <Select label="Local" value={localId} onChange={(e) => setLocalId(e.target.value)}>
                {locales.map((l) => (
                  <MenuItem key={l.id} value={l.id}>
                    {l.nombre}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          ) : undefined
        }
      />

      {cierreActual?.confirmado && (
        <Box sx={{ display: "flex", alignItems: "flex-start", gap: 1, mb: 3, p: 1.8, borderRadius: "14px", background: "rgba(27,122,77,0.12)", color: "#1b7a4d" }}>
          <CheckCircleRoundedIcon fontSize="small" sx={{ mt: 0.1 }} />
          <Typography sx={{ fontSize: "0.85rem" }}>
            El cierre de la semana #{cierreActual.semanaNumero} ya fue confirmado por el administrador.
          </Typography>
        </Box>
      )}

      {cierreActual && !cierreActual.confirmado && (
        <Box sx={{ display: "flex", alignItems: "flex-start", gap: 1, mb: 3, p: 1.8, borderRadius: "14px", background: "rgba(240,180,41,0.16)", color: "#b06a00" }}>
          <InfoRoundedIcon fontSize="small" sx={{ mt: 0.1 }} />
          <Typography sx={{ fontSize: "0.85rem" }}>
            Ya registraste el cierre de esta semana (pendiente de confirmación). Puedes corregirlo mientras el administrador no lo confirme.
          </Typography>
        </Box>
      )}

      {(!cierreActual || !cierreActual.confirmado) && (
        <GlassCard sx={{ p: { xs: 2.5, sm: 3 }, mb: 3 }}>
          <SectionLabel>Bolsas / Monedas / Billetes contados</SectionLabel>
          <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1.2 }}>
            {denominaciones.map((d) => (
              <TextField
                key={d.id}
                label={`${d.tipo} $${d.valor}`}
                type="number"
                size="small"
                sx={{ width: 145, ...glassFieldLight }}
                value={cantidades[d.id] ?? ""}
                onChange={(e) => setCantidades({ ...cantidades, [d.id]: Number(e.target.value) })}
              />
            ))}
          </Box>

          <SectionLabel>Otros</SectionLabel>
          <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1.2 }}>
            <TextField
              label="Terminal"
              type="number"
              size="small"
              sx={{ width: 145, ...glassFieldLight }}
              value={terminal || ""}
              onChange={(e) => setTerminal(Number(e.target.value))}
            />
            <TextField
              label="Transferencias"
              type="number"
              size="small"
              sx={{ width: 145, ...glassFieldLight }}
              value={transferencia || ""}
              onChange={(e) => setTransferencia(Number(e.target.value))}
            />
          </Box>

          <SectionLabel>Premios encontrados</SectionLabel>
          <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1.2 }}>
            {premios.map((p) => (
              <TextField
                key={p.id}
                label={p.nombre}
                type="number"
                size="small"
                sx={{ width: 145, ...glassFieldLight }}
                value={cantidades[p.id] ?? ""}
                onChange={(e) => setCantidades({ ...cantidades, [p.id]: Number(e.target.value) })}
              />
            ))}
          </Box>

          <SectionLabel>Premios puestos durante la semana</SectionLabel>
          <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1.2 }}>
            {premios.map((p) => (
              <TextField
                key={p.id}
                label={p.nombre}
                type="number"
                size="small"
                sx={{ width: 145, ...glassFieldLight }}
                value={puestos[p.id] ?? ""}
                onChange={(e) => setPuestos({ ...puestos, [p.id]: Number(e.target.value) })}
              />
            ))}
          </Box>

          <Box sx={{ mt: 3 }}>
            {cierreActual ? (
              <Box component="button" onClick={corregir} sx={pillButtonSx}>
                Guardar corrección
              </Box>
            ) : (
              <Box component="button" onClick={registrar} disabled={!local?.semanaActualId} sx={pillButtonSx}>
                Registrar cierre semanal
              </Box>
            )}
          </Box>
        </GlassCard>
      )}

      {cierreActual && (
        <GlassCard sx={{ p: { xs: 2.5, sm: 3 } }}>
          <Typography sx={{ fontWeight: 700, fontSize: "1.05rem", color: brand.ink, mb: 2 }}>Resultado</Typography>
          <Box sx={{ display: "flex", gap: 4, flexWrap: "wrap" }}>
            <Box>
              <Typography sx={{ fontSize: "0.68rem", color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px" }}>
                Total esperado
              </Typography>
              <Typography sx={{ fontSize: "1.15rem", fontWeight: 800, color: brand.ink }}>
                ${cierreActual.totalEsperado.toLocaleString()}
              </Typography>
            </Box>
            <Box>
              <Typography sx={{ fontSize: "0.68rem", color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px" }}>
                Total reportado
              </Typography>
              <Typography sx={{ fontSize: "1.15rem", fontWeight: 800, color: brand.ink }}>
                ${cierreActual.totalReportado.toLocaleString()}
              </Typography>
            </Box>
            <Box>
              <Typography sx={{ fontSize: "0.68rem", color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px" }}>
                Diferencia
              </Typography>
              <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                <Typography sx={{ fontSize: "1.15rem", fontWeight: 800, color: brand.ink }}>
                  ${cierreActual.diferencia.toLocaleString()}
                </Typography>
                <StatusPill label={cierreActual.estadoDiferencia} tone={cierreActual.estadoDiferencia === "Correcto" ? "success" : "warning"} />
              </Box>
            </Box>
          </Box>
        </GlassCard>
      )}
    </Box>
  );
}
