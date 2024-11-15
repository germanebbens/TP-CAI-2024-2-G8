using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ElectroHogar.Datos;
using ElectroHogar.Persistencia.Utils;

namespace ElectroHogar.Persistencia
{
    public class ProductosWS : BaseWS
    { 
        public readonly string adminId;
        public ProductosWS() : base()
        {
            adminId = _adminId;
        }

        public List<ProductoList> ObtenerProductos()
        {
            var response = WebHelper.Get("Producto/TraerProductos");
            return DeserializarRespuesta<List<ProductoList>>(response);
        }

        public List<ProductoList> ObtenerProductosPorCategoria(Categoria categoria)
        {
            var response = WebHelper.Get($"Producto/TraerProductosPorCategoria?catnum={(int)categoria}");
            return DeserializarRespuesta<List<ProductoList>>(response);
        }

        public void AgregarProducto(AddProducto producto)
        {
            var response = WebHelper.Post("Producto/AgregarProducto",
                JsonConvert.SerializeObject(producto));
            DeserializarRespuesta<object>(response);
        }

        public void ModificarProducto(PatchProducto producto)
        {
            var response = WebHelper.Patch("Producto/ModificarProducto",
                JsonConvert.SerializeObject(producto));
            DeserializarRespuesta<object>(response);
        }

        public void BajaProducto(Guid idProducto)
        {
            var permisos = new { id = idProducto, idUsuario = _adminId };
            var response = WebHelper.DeleteWithBody("Producto/BajaProducto",
                JsonConvert.SerializeObject(permisos));
            DeserializarRespuesta<object>(response);
        }

        public void ReactivarProducto(Guid idProducto)
        {
            var permisos = new { id = idProducto, idUsuario = _adminId };
            var response = WebHelper.Patch("Producto/ReactivarProducto",
                JsonConvert.SerializeObject(permisos));
            DeserializarRespuesta<object>(response);
        }
    }
}