export type TipoDenominacion = "Bolsa" | "Moneda" | "Billete";
export type ConceptoMovimiento = "Bolsa" | "Moneda" | "Billete" | "Terminal" | "Transferencia" | "Premio" | "Gasto";
export type EstadoCierre = "Correcto" | "Revisar";

export interface CategoriaGasto {
  id: string;
  nombre: string;
  activo: boolean;
}

export interface LocalResumen {
  id: string;
  nombre: string;
}

export interface LoginResponse {
  token: string;
  expiraEn: string;
  usuarioId: string;
  nombre: string;
  userName: string;
  roles: string[];
  locales: LocalResumen[];
  debeCambiarContrasena: boolean;
}

export interface Local {
  id: string;
  nombre: string;
  direccion: string;
  activo: boolean;
  fechaCreacion: string;
  semanaActualNumero: number;
  semanaActualId: string | null;
}

export interface Usuario {
  id: string;
  userName: string;
  email: string;
  nombre: string;
  activo: boolean;
  roles: string[];
  localIds: string[];
  propietarioId: string | null;
  propietarioNombre: string | null;
}

export interface Denominacion {
  id: string;
  tipo: TipoDenominacion;
  valor: number;
  valorPorBolsa: number | null;
  activo: boolean;
}

export interface Premio {
  id: string;
  nombre: string;
  denominacion: number;
  activo: boolean;
}

export interface CuentaDetalle {
  id: string;
  denominacionId: string | null;
  denominacionNombre: string | null;
  premioId: string | null;
  premioNombre: string | null;
  cantidad: number;
  valorUnitario: number;
  subtotal: number;
  origen: string;
}

export interface Cuenta {
  id: string;
  localId: string;
  semanaId: string;
  semanaNumero: number;
  fechaCreacion: string;
  creadoPorNombre: string;
  detalles: CuentaDetalle[];
  totalAcumulado: number;
}

export interface Gasto {
  id: string;
  localId: string;
  descripcion: string;
  categoriaGastoId: string;
  categoriaGastoNombre: string;
  monto: number;
  fecha: string;
  empleadoNombre: string;
  tieneEvidencia: boolean;
  evidenciaUrls: string[];
  cierreDiarioId: string | null;
}

export interface CierreDiarioDetalle {
  concepto: ConceptoMovimiento;
  denominacionNombre: string | null;
  premioNombre: string | null;
  cantidad: number;
  valorUnitario: number;
  subtotal: number;
}

export interface CierreDiario {
  id: string;
  localId: string;
  fecha: string;
  empleadoNombre: string;
  detalles: CierreDiarioDetalle[];
  totalReportado: number;
  totalEsperado: number;
  diferencia: number;
  estado: EstadoCierre;
  fechaCreacion: string;
}

export type EstadoMaquina = "Funcional" | "Reportada" | "EnReparacion" | "Reparada" | "FueraDeServicio";

export interface Propietario {
  id: string;
  nombre: string;
  telefono: string | null;
  activo: boolean;
}

export interface TipoMaquina {
  id: string;
  nombre: string;
  activo: boolean;
}

export interface Maquina {
  id: string;
  nombre: string;
  tipoMaquinaId: string;
  tipoMaquinaNombre: string;
  localId: string;
  localNombre: string;
  propietarioId: string;
  propietarioNombre: string;
  marca: string | null;
  modelo: string | null;
  numeroSerie: string | null;
  estado: EstadoMaquina;
  fechaAlta: string;
  activo: boolean;
}

export interface ReporteAveria {
  id: string;
  localId: string;
  maquinaId: string;
  maquinaNombre: string;
  empleadoNombre: string;
  problema: string;
  descripcion: string;
  estadoAlReportar: EstadoMaquina;
  fecha: string;
  evidenciaUrls: string[];
}

export interface CorteMaquina {
  id: string;
  maquinaId: string;
  maquinaNombre: string;
  localId: string;
  localNombre: string;
  registradoPorNombre: string;
  fecha: string;
  comentario: string | null;
  total: number;
  fechaCreacion: string;
}

export interface MaquinaPremioConfig {
  id: string;
  premioId: string;
  premioNombre: string;
  premioDenominacion: number;
  cantidadAsignada: number;
}

export interface InventarioPremioDetalle {
  premioNombre: string;
  cantidadConfigurada: number;
  cantidadEncontrada: number;
  diferencia: number;
}

export interface InventarioPremio {
  id: string;
  maquinaId: string;
  usuarioNombre: string;
  fecha: string;
  detalles: InventarioPremioDetalle[];
}

export interface CierreSemanalDetalle {
  concepto: ConceptoMovimiento;
  denominacionNombre: string | null;
  premioNombre: string | null;
  esPremioPuesto: boolean;
  cantidad: number;
  valorUnitario: number;
  subtotal: number;
}

export interface CierreSemanal {
  id: string;
  semanaId: string;
  semanaNumero: number;
  localId: string;
  detalles: CierreSemanalDetalle[];
  totalReportado: number;
  totalEsperado: number;
  diferencia: number;
  estadoDiferencia: EstadoCierre;
  confirmado: boolean;
  creadoPorNombre: string;
  confirmadoPorNombre: string | null;
  fechaCreacion: string;
  fechaConfirmacion: string | null;
}

export type TipoNotificacion = "CuentaModificada" | "CierreConDiferencia" | "MaquinaReportada" | "CierreSemanalPendiente";

export interface Notificacion {
  id: string;
  localId: string | null;
  localNombre: string | null;
  tipo: TipoNotificacion;
  mensaje: string;
  referenciaId: string | null;
  leida: boolean;
  fechaCreacion: string;
}

export interface DashboardData {
  locales: number;
  maquinas: number;
  empleados: number;
  cierresCorrectos: number;
  cierresConDiferencia: number;
  maquinasAveriadas: number;
  maquinasEnReparacion: number;
  gastosPendientesEvidencia: number;
  totalAcumuladoSemanaActual: number;
  totalesPorLocal: { localId: string; localNombre: string; semanaNumero: number; totalAcumulado: number }[];
}

export interface PropietarioResumen {
  id: string;
  nombre: string;
  totalMaquinas: number;
}

export interface Conteo {
  etiqueta: string;
  cantidad: number;
}

export interface MaquinaResumen {
  id: string;
  nombre: string;
  tipoNombre: string;
  localNombre: string;
  estado: string;
  averiasUltimos30Dias: number;
}

export interface AveriaTendencia {
  periodo: string;
  inicioSemana: string;
  cantidad: number;
}

export interface PropietarioDashboardData {
  propietarioId: string;
  propietarioNombre: string;
  totalMaquinas: number;
  maquinasPorEstado: Conteo[];
  maquinasPorTipo: Conteo[];
  maquinas: MaquinaResumen[];
  averiasUltimos30Dias: number;
  averiasUltimos90Dias: number;
  averiasPorSemana: AveriaTendencia[];
  tiempoPromedioReparacionHoras: number | null;
}
