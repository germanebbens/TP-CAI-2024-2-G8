using System;
using System.Collections.Generic;
using System.Linq;
using ElectroHogar.Datos;
using ElectroHogar.Persistencia;

namespace ElectroHogar.Negocio
{
    public class Ventas
    {
        private readonly VentasWS _ventasWS;
        private readonly Productos _productosService;
        private readonly Clientes _clientesService;
        private const double DESCUENTO_ELECTRO_HOGAR = 0.05; // 5%
        private const double DESCUENTO_CLIENTE_NUEVO = 0.05; // 5%
        private const double MONTO_MINIMO_DESCUENTO = 100000;

        public Ventas()
        {
            _ventasWS = new VentasWS();
            _productosService = new Productos();
            _clientesService = new Clientes();
        }

        public void RegistrarVenta(VentaCompuesta venta)
        {
            try
            {
                venta.IdUsuario = Guid.Parse(_ventasWS.usuarioLogueadoId);
                ValidarVenta(venta);
                var idsVentas = new List<Guid>();

                AplicarDescuentos(venta);

                foreach (var item in venta.Items)
                {
                    var ventaIndividual = new AddVenta
                    {
                        IdCliente = venta.IdCliente,
                        IdUsuario = venta.IdUsuario,
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad
                    };

                    _ventasWS.AgregarVenta(ventaIndividual);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar la venta: {ex.Message}");
            }
        }

        private void ValidarVenta(VentaCompuesta venta)
        {
            if (venta.Items.Count == 0)
                throw new Exception("La venta debe contener al menos un item");

            if (venta.IdCliente == Guid.Empty)
                throw new Exception("El cliente es requerido");

            if (venta.IdUsuario == Guid.Empty)
                throw new Exception("El usuario es requerido");

            foreach (var item in venta.Items)
            {
                if (item.Cantidad <= 0)
                    throw new Exception($"La cantidad del producto {item.NombreProducto} debe ser mayor a cero");
            }
        }

        private void AplicarDescuentos(VentaCompuesta venta)
        {
            // Descuento por Electro Hogar
            var productosElectroHogar = venta.Items
                .Where(i => i.Categoria == Categoria.ElectroHogar)
                .ToList();

            double montoElectroHogar = productosElectroHogar.Sum(i => i.Subtotal);
        }

        public void DevolverVenta(Guid idVenta)
        {
            try
            {
                _ventasWS.DevolverVenta(idVenta);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al devolver la venta: {ex.Message}");
            }
        }

        public VentaList ObtenerVentaPorId(Guid idVenta)
        {
            try
            {
                return _ventasWS.ObtenerVenta(idVenta);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la venta: {ex.Message}");
            }
        }

        public List<VentaList> ObtenerVentasPorCliente(Guid idCliente)
        {
            try
            {
                return _ventasWS.ObtenerVentasPorCliente(idCliente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las ventas del cliente: {ex.Message}");
            }
        }
    }
}