using System;
using System.Collections.Generic;

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
        public Guid IdUsuario { get; set; }
        public List<ItemVenta> Items { get; set; } = new List<ItemVenta>();
        public double MontoTotal => CalcularMontoTotal();
        public List<string> Descuentos { get; set; } = new List<string>();

        private double CalcularMontoTotal()
        {
            double total = 0;
            foreach (var item in Items)
            {
                total += item.Cantidad * item.Precio;
            }
            // TODO: APLICAR los descuentos
            return total;
        }
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
}