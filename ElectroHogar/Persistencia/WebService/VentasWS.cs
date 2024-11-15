using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ElectroHogar.Datos;
using ElectroHogar.Persistencia.Utils;
using ElectroHogar.Negocio;

namespace ElectroHogar.Persistencia
{
    public class VentasWS : BaseWS
    {
        private readonly LoginNegocio _loginNegocio;
        public readonly string usuarioLogueadoId;

        public VentasWS() : base()
        {
            _loginNegocio = LoginNegocio.Instance;
            usuarioLogueadoId = _loginNegocio._usuarioLogueadoId;
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
            var permisos = new { id = idVenta, idUsuario = usuarioLogueadoId };
            var response = WebHelper.Patch("Venta/DevolverVenta",
                JsonConvert.SerializeObject(permisos));
            DeserializarRespuesta<object>(response);
        }
    }
}