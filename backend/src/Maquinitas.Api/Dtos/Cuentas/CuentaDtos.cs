namespace Maquinitas.Api.Dtos.Cuentas;

public class CuentaDetalleDto
{
    public Guid Id { get; set; }
    public Guid? DenominacionId { get; set; }
    public string? DenominacionNombre { get; set; }
    public Guid? PremioId { get; set; }
    public string? PremioNombre { get; set; }
    public int Cantidad { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public string Origen { get; set; } = string.Empty;
}

public class CuentaDto
{
    public Guid Id { get; set; }
    public Guid LocalId { get; set; }
    public Guid SemanaId { get; set; }
    public int SemanaNumero { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string CreadoPorNombre { get; set; } = string.Empty;
    public IList<CuentaDetalleDto> Detalles { get; set; } = new List<CuentaDetalleDto>();
    public decimal TotalAcumulado { get; set; }
}

public class LineaCuentaRequest
{
    public Guid? DenominacionId { get; set; }
    public Guid? PremioId { get; set; }
    public int Cantidad { get; set; }
}

public class CrearCuentaRequest
{
    public IList<LineaCuentaRequest> Lineas { get; set; } = new List<LineaCuentaRequest>();
}

public class ActualizarCantidadRequest
{
    public int Cantidad { get; set; }
}

public class CuentaModificacionDto
{
    public Guid Id { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public string Concepto { get; set; } = string.Empty;
    public string ValorAnterior { get; set; } = string.Empty;
    public string ValorNuevo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}
