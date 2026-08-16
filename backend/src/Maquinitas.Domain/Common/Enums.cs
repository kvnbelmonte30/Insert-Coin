namespace Maquinitas.Domain.Common;

public enum EstadoMaquina
{
    Funcional,
    Reportada,
    EnReparacion,
    Reparada,
    FueraDeServicio
}

public enum TipoDenominacion
{
    Bolsa,
    Moneda,
    Billete
}

public enum ConceptoMovimiento
{
    Bolsa,
    Moneda,
    Billete,
    Terminal,
    Transferencia,
    Premio,
    Gasto
}

public enum EstadoSemana
{
    Abierta,
    PendienteConfirmacion,
    Cerrada
}

public enum EstadoCierre
{
    Correcto,
    Revisar
}

public enum TipoNotificacion
{
    CuentaModificada,
    CierreConDiferencia,
    MaquinaReportada,
    CierreSemanalPendiente
}

public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Empleado = "Empleado";
}
