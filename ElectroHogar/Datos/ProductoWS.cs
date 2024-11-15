// Datos/ProductoDef.cs
using System;

namespace ElectroHogar.Datos
{
    public enum Categoria
    {
        Audio = 1,
        Celulares = 2,
        ElectroHogar = 3,
        Informatica = 4,
        SmartTV = 5
    }

    public class ProductoList
    {
        public Guid Id { get; set; }
        public Categoria IdCategoria { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
        public double Precio { get; set; }
        public int Stock { get; set; }
    }

    public class AddProducto
    {
        public Categoria IdCategoria { get; set; }
        public Guid IdUsuario { get; set; }
        public Guid IdProveedor { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public int Stock { get; set; }
    }

    public class PatchProducto
    {
        public Guid Id { get; set; }
        public Guid IdUsuario { get; set; }
        public double Precio { get; set; }
        public int Stock { get; set; }
    }

    public class ProductoMasVendido
    {
        public string Nombre { get; set; }
        public int CantidadVentas { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal MontoTotal { get; set; }
    }
}