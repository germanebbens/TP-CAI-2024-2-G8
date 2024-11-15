using System;
using System.Collections.Generic;
using System.Linq;

namespace ElectroHogar.Datos
{
    public class VentaList
    {
        public Guid Id { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaAlta { get; set; }
        public EstadoVenta Estado { get; set; }
    }

    public class AddVenta
    {
        public Guid IdCliente { get; set; }
        public Guid IdUsuario { get; set; }
        public Guid IdProducto { get; set; }
        public int Cantidad { get; set; }
    }

    public enum EstadoVenta
    {
        Devolucion = 0,
        Finalizada = 1
    }

    public class VentaCompuesta
    {
        public Guid IdCliente { get; set; }
        public Guid IdUsuario {get; set; }
        public List<ItemVenta> Items { get; set; } = new List<ItemVenta>();
        public double Subtotal { get; set; }
        public List<string> Descuentos { get; set; } = new List<string>();
        public double TotalDescuentos { get; set; }
        public double MontoTotal { get; set; }

    }

    public class ItemVenta
    {
        public Guid IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public Categoria Categoria { get; set; }
        public double Subtotal => Cantidad * Precio;
    }

    public class VendedorReporte
    {
        public string Nombre { get; set; }
        public int CantidadVentas { get; set; }
        public decimal MontoTotal { get; set; }
    }
}