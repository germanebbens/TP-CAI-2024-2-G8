using System;
using System.Collections.Generic;
using System.Linq;

using ElectroHogar.Datos;
using ElectroHogar.Persistencia;

namespace ElectroHogar.Negocio
{
    public class Productos
    {
        private readonly ProductosWS _productosWS;
        private readonly Proveedores _proveedoresService;
        private const double PORCENTAJE_STOCK_CRITICO = 0.25; // 25%
        private const int STOCK_MAXIMO_NORMAL = 100;

        public Productos()
        {
            _productosWS = new ProductosWS();
            _proveedoresService = new Proveedores();
        }

        public List<ProductoList> ObtenerActivos()
        {
            try
            {
                var productos = _productosWS.ObtenerProductos();
                return productos.Where(p => !p.FechaBaja.HasValue).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener productos: {ex.Message}");
            }
        }

        public List<ProductoList> ObtenerProductosPorCategoria(Categoria categoria)
        {
            try
            {
                return _productosWS.ObtenerProductosPorCategoria(categoria)
                    .Where(p => !p.FechaBaja.HasValue)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener productos por categoría: {ex.Message}");
            }
        }

        public void RegistrarProducto(Guid idProveedor, Categoria categoria, string nombre, double precio, int stock)
        {
            try
            {
                var proveedor = _proveedoresService.ObtenerProveedorPorId(idProveedor);
                if (proveedor == null)
                    throw new Exception("El proveedor especificado no existe");

                if (proveedor.FechaBaja.HasValue)
                    throw new Exception("No se puede registrar un producto con un proveedor inactivo");

                var nuevoProducto = new AddProducto
                {
                    IdUsuario = Guid.Parse(_productosWS.adminId),
                    IdProveedor = idProveedor,
                    IdCategoria = categoria,
                    Nombre = nombre,
                    Precio = precio,
                    Stock = stock
                };

                ValidarDatosBasicos(nuevoProducto);
                _productosWS.AgregarProducto(nuevoProducto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar producto: {ex.Message}");
            }
        }

        public void ModificarProducto(Guid idProducto, double precio, int stock)
        {
            try
            {
                var productoModificado = new PatchProducto
                {
                    Id = idProducto,
                    IdUsuario = Guid.Parse(_productosWS.adminId),
                    Precio = precio,
                    Stock = stock
                };

                ValidarModificacion(productoModificado);
                _productosWS.ModificarProducto(productoModificado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al modificar producto: {ex.Message}");
            }
        }

        public void DarBajaProducto(Guid idProducto)
        {
            try
            {
                _productosWS.BajaProducto(idProducto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al dar de baja el producto: {ex.Message}");
            }
        }

        public List<ProductoList> ObtenerProductosStockCritico()
        {
            try
            {
                var productos = ObtenerActivos();
                return productos
                    .Where(p => EsStockCritico(p.Stock))
                    .OrderBy(p => p.IdCategoria)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener productos con stock crítico: {ex.Message}");
            }
        }

        public bool ExisteAlertaStockCritico()
        {
            try
            {
                var productos = ObtenerProductosStockCritico();
                return productos.Any();
            }
            catch
            {
                return false;
            }
        }

        private void ValidarDatosBasicos(AddProducto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.Nombre))
                throw new Exception("El nombre del producto es requerido");

            if (producto.Precio <= 0)
                throw new Exception("El precio debe ser mayor a cero");

            if (producto.Stock < 0)
                throw new Exception("El stock no puede ser negativo");
        }

        private void ValidarModificacion(PatchProducto producto)
        {
            if (producto.Precio <= 0)
                throw new Exception("El precio debe ser mayor a cero");

            if (producto.Stock < 0)
                throw new Exception("El stock no puede ser negativo");
        }

        private bool EsStockCritico(int stockActual)
        {
            // Se asume que X es el stock máximo normal para un producto
            // Esto podría ajustarse según la lógica de negocio 
            return stockActual <= (STOCK_MAXIMO_NORMAL * PORCENTAJE_STOCK_CRITICO);
        }

        public List<ProductoMasVendido> ObtenerProductosMasVendidos()
        {
            var random = new Random();
            var productos = ObtenerActivos()
                .Take(20)
                .Select(p => new ProductoMasVendido
                {
                    Nombre = p.Nombre,
                    CantidadVentas = random.Next(50, 201),
                    PrecioUnitario = (decimal)p.Precio,
                    MontoTotal = (decimal)(p.Precio * random.Next(50, 201))
                })
                .OrderByDescending(p => p.CantidadVentas)
                .ToList();

            return productos;
        }
    }
}