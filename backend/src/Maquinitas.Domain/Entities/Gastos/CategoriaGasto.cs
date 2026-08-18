using Maquinitas.Domain.Common;

namespace Maquinitas.Domain.Entities.Gastos;

public class CategoriaGasto : EntityBase
{
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
}
