using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ElectroHogar.Datos;
using ElectroHogar.Persistencia.Utils;

namespace ElectroHogar.Persistencia
{
    public class VentasWS : BaseWS
    {
        public readonly string adminId;

        public VentasWS() : base()
        {
            adminId = _adminId;
        }

        public void AgregarVenta(AddVenta venta)
        {
            var response = WebHelper.Post("Venta/AgregarVenta",
                JsonConvert.SerializeObject(venta));
            DeserializarRespuesta<object>(response);
        }

        public VentaList ObtenerVenta(Guid idVenta)
        {
            var response = WebHelper.Get($"Venta/GetVenta?id={idVenta}");
            return DeserializarRespuesta<VentaList>(response);
        }

        public List<VentaList> ObtenerVentasPorCliente(Guid idCliente)
        {
            var response = WebHelper.Get($"Venta/GetVentaByCliente?id={idCliente}");
            return DeserializarRespuesta<List<VentaList>>(response);
        }

        public void DevolverVenta(Guid idVenta)
        {
            var permisos = new { id = idVenta, idUsuario = _adminId };
            var response = WebHelper.Patch("Venta/DevolverVenta",
                JsonConvert.SerializeObject(permisos));
            DeserializarRespuesta<object>(response);
        }
    }
}