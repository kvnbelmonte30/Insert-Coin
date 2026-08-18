import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Box, Table, TableBody, TableCell, TableHead, TableRow, TextField, Typography } from "@mui/material";
import ArrowBackRoundedIcon from "@mui/icons-material/ArrowBackRounded";
import InfoRoundedIcon from "@mui/icons-material/InfoRounded";
import { api } from "../../api/client";
import { GlassCard } from "../../components/GlassCard";
import { StatusPill } from "../../components/StatusPill";
import { brand, glassFieldLight, glassTableSx, pillButtonSx, pillOutlineButtonSx } from "../../theme/brand";
import { etiquetaConcepto, ordenarDetalles } from "../../utils/cuenta";
import type { Cuenta, Denominacion, Local, Premio, CierreDiario } from "../../types";

export function LocalDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [local, setLocal] = useState<Local | null>(null);
  const [cuenta, setCuenta] = useState<Cuenta | null>(null);
  const [cuentaNoExiste, setCuentaNoExiste] = useState(false);
  const [denominaciones, setDenominaciones] = useState<Denominacion[]>([]);
  const [premios, setPremios] = useState<Premio[]>([]);
  const [cantidades, setCantidades] = useState<Record<string, number>>({});
  const [cierres, setCierres] = useState<CierreDiario[]>([]);
  const [desde, setDesde] = useState("");
  const [hasta, setHasta] = useState("");

  const cargar = async () => {
    if (!id) return;
    try {
      const [localRes, denomRes, premioRes] = await Promise.all([
        api.get<Local>(`/locales/${id}`),
        api.get<Denominacion[]>("/catalogos/denominaciones"),
        api.get<Premio[]>("/catalogos/premios"),
      ]);
      setLocal(localRes.data);
      setDenominaciones(denomRes.data);
      setPremios(premioRes.data);
    } catch (err) {
      console.error("Error cargando el detalle del local", err);
      return;
    }

    try {
      const cuentaRes = await api.get<Cuenta>(`/cuentas/local/${id}/actual`);
      setCuenta(cuentaRes.data);
      setCuentaNoExiste(false);
    } catch {
      setCuenta(null);
      setCuentaNoExiste(true);
    }
  };

  useEffect(() => {
    cargar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  useEffect(() => {
    if (!id) return;
    api
      .get<CierreDiario[]>(`/cierres-diarios/local/${id}`, { params: { desde: desde || undefined, hasta: hasta || undefined } })
      .then((r) => setCierres(r.data));
  }, [id, desde, hasta]);

  const crearCuenta = async () => {
    const lineas = [
      ...denominaciones.map((d) => ({ denominacionId: d.id, cantidad: cantidades[d.id] ?? 0 })),
      ...premios.map((p) => ({ premioId: p.id, cantidad: cantidades[p.id] ?? 0 })),
    ].filter((l) => l.cantidad > 0);

    await api.post(`/cuentas/local/${id}`, { lineas });
    cargar();
  };

  const actualizarLinea = async (detalleId: string, cantidad: number) => {
    await api.put(`/cuentas/lineas/${detalleId}`, { cantidad });
    cargar();
  };

  if (!local) return null;

  return (
    <Box>
      <Box
        component="button"
        onClick={() => navigate("/admin/locales")}
        sx={{ ...pillOutlineButtonSx, mb: 2.5, py: 0.6, px: 1.4, fontSize: "0.78rem" }}
      >
        <ArrowBackRoundedIcon sx={{ fontSize: 16 }} />
        Locales
      </Box>

      <Typography sx={{ fontSize: { xs: "1.6rem", sm: "1.9rem" }, fontWeight: 800, color: brand.ink, letterSpacing: "-0.5px" }}>
        {local.nombre}
      </Typography>
      <Typography sx={{ color: brand.inkMuted, fontSize: "0.9rem", mt: 0.3, mb: 3 }}>
        {local.direccion} · Semana actual #{local.semanaActualNumero}
      </Typography>

      <GlassCard sx={{ p: { xs: 2.5, sm: 3 }, mb: 3 }}>
        <Typography sx={{ fontWeight: 700, fontSize: "1.05rem", color: brand.ink, mb: 2 }}>
          Cuenta de la semana
        </Typography>

        {cuentaNoExiste && (
          <Box>
            <Box
              sx={{
                display: "flex",
                alignItems: "flex-start",
                gap: 1,
                mb: 2.5,
                p: 1.8,
                borderRadius: "14px",
                background: "rgba(33,88,176,0.08)",
                color: "#2158b0",
              }}
            >
              <InfoRoundedIcon fontSize="small" sx={{ mt: 0.1 }} />
              <Typography sx={{ fontSize: "0.85rem" }}>
                Este local aún no tiene una cuenta configurada para la semana actual. Captura las cantidades iniciales.
              </Typography>
            </Box>

            <Typography sx={{ fontSize: "0.75rem", fontWeight: 700, color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px", mb: 1 }}>
              Bolsas / Monedas / Billetes
            </Typography>
            <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1.2, mb: 2.5 }}>
              {denominaciones.map((d) => (
                <TextField
                  key={d.id}
                  label={`${d.tipo} $${d.valor}`}
                  type="number"
                  size="small"
                  sx={{ width: 150, ...glassFieldLight }}
                  value={cantidades[d.id] ?? ""}
                  onChange={(e) => setCantidades({ ...cantidades, [d.id]: Number(e.target.value) })}
                />
              ))}
            </Box>

            <Typography sx={{ fontSize: "0.75rem", fontWeight: 700, color: brand.inkFaint, textTransform: "uppercase", letterSpacing: "0.4px", mb: 1 }}>
              Premios
            </Typography>
            <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1.2, mb: 3 }}>
              {premios.map((p) => (
                <TextField
                  key={p.id}
                  label={p.nombre}
                  type="number"
                  size="small"
                  sx={{ width: 150, ...glassFieldLight }}
                  value={cantidades[p.id] ?? ""}
                  onChange={(e) => setCantidades({ ...cantidades, [p.id]: Number(e.target.value) })}
                />
              ))}
            </Box>

            <Box component="button" onClick={crearCuenta} sx={pillButtonSx}>
              Guardar cuenta inicial
            </Box>
          </Box>
        )}

        {cuenta && (
          <Box sx={{ overflowX: "auto" }}>
            <Table size="small" sx={glassTableSx}>
              <TableHead>
                <TableRow>
                  <TableCell>Concepto</TableCell>
                  <TableCell align="right">Cantidad</TableCell>
                  <TableCell align="right">Valor unitario</TableCell>
                  <TableCell align="right">Subtotal</TableCell>
                  <TableCell></TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {ordenarDetalles(cuenta.detalles).map((d) => (
                  <FilaEditable key={d.id} detalle={d} onGuardar={actualizarLinea} />
                ))}
                <TableRow>
                  <TableCell colSpan={3} sx={{ fontWeight: 800, color: brand.ink }}>
                    Total acumulado
                  </TableCell>
                  <TableCell align="right" sx={{ fontWeight: 800, color: brand.goldDark, fontSize: "1rem" }}>
                    ${cuenta.totalAcumulado.toLocaleString()}
                  </TableCell>
                  <TableCell></TableCell>
                </TableRow>
              </TableBody>
            </Table>
          </Box>
        )}
      </GlassCard>

      <GlassCard sx={{ p: { xs: 2.5, sm: 3 } }}>
        <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2, flexWrap: "wrap", gap: 1.5 }}>
          <Typography sx={{ fontWeight: 700, fontSize: "1.05rem", color: brand.ink }}>
            Cierres diarios
          </Typography>
          <Box sx={{ display: "flex", gap: 1.2 }}>
            <TextField
              label="Desde"
              type="date"
              size="small"
              slotProps={{ inputLabel: { shrink: true } }}
              sx={{ width: 155, ...glassFieldLight }}
              value={desde}
              onChange={(e) => setDesde(e.target.value)}
            />
            <TextField
              label="Hasta"
              type="date"
              size="small"
              slotProps={{ inputLabel: { shrink: true } }}
              sx={{ width: 155, ...glassFieldLight }}
              value={hasta}
              onChange={(e) => setHasta(e.target.value)}
            />
          </Box>
        </Box>
        <Box sx={{ overflowX: "auto" }}>
          <Table size="small" sx={glassTableSx}>
            <TableHead>
              <TableRow>
                <TableCell>Fecha</TableCell>
                <TableCell>Empleado</TableCell>
                <TableCell align="right">Reportado</TableCell>
                <TableCell align="right">Esperado</TableCell>
                <TableCell align="right">Diferencia</TableCell>
                <TableCell>Estado</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {cierres.map((c) => (
                <TableRow key={c.id}>
                  <TableCell sx={{ color: brand.inkMuted, whiteSpace: "nowrap" }}>{c.fecha}</TableCell>
                  <TableCell sx={{ fontWeight: 600 }}>{c.empleadoNombre}</TableCell>
                  <TableCell align="right">${c.totalReportado.toLocaleString()}</TableCell>
                  <TableCell align="right">${c.totalEsperado.toLocaleString()}</TableCell>
                  <TableCell align="right">${c.diferencia.toLocaleString()}</TableCell>
                  <TableCell>
                    <StatusPill label={c.estado} tone={c.estado === "Correcto" ? "success" : "warning"} />
                  </TableCell>
                </TableRow>
              ))}
              {cierres.length === 0 && (
                <TableRow>
                  <TableCell colSpan={6} sx={{ textAlign: "center", color: brand.inkMuted, py: 4 }}>
                    Sin cierres registrados todavía.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </Box>
      </GlassCard>
    </Box>
  );
}

function FilaEditable({
  detalle,
  onGuardar,
}: {
  detalle: Cuenta["detalles"][number];
  onGuardar: (id: string, cantidad: number) => void;
}) {
  const [valor, setValor] = useState(detalle.cantidad);
  const modificado = valor !== detalle.cantidad;

  return (
    <TableRow>
      <TableCell sx={{ fontWeight: 600 }}>{etiquetaConcepto(detalle)}</TableCell>
      <TableCell align="right">
        <TextField
          type="number"
          size="small"
          value={valor}
          onChange={(e) => setValor(Number(e.target.value))}
          sx={{ width: 90, ...glassFieldLight }}
        />
      </TableCell>
      <TableCell align="right">${detalle.valorUnitario.toLocaleString()}</TableCell>
      <TableCell align="right">${detalle.subtotal.toLocaleString()}</TableCell>
      <TableCell align="right">
        {modificado && (
          <Box
            component="button"
            onClick={() => onGuardar(detalle.id, valor)}
            sx={{ ...pillOutlineButtonSx, py: 0.5, px: 1.4, fontSize: "0.75rem" }}
          >
            Guardar
          </Box>
        )}
      </TableCell>
    </TableRow>
  );
}
