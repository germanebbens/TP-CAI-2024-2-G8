using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ElectroHogar.Datos;
using ElectroHogar.Persistencia.Utils;

namespace ElectroHogar.Persistencia
{
    public class ClientesWS : BaseWS
    {
        public ClientesWS() : base()
        {
        }

        public ClienteList ObtenerCliente(Guid idCliente)
        {
            var response = WebHelper.Get($"Cliente/GetCliente?id={idCliente}");
            return DeserializarRespuesta<ClienteList>(response);
        }

        public List<ClienteList> ObtenerClientes()
        {
            var response = WebHelper.Get("Cliente/GetClientes");
            return DeserializarRespuesta<List<ClienteList>>(response);
        }

        public void AgregarCliente(AddCliente cliente)
        {
            var response = WebHelper.Post("Cliente/AgregarCliente",
                JsonConvert.SerializeObject(cliente));
            DeserializarRespuesta<object>(response);
        }

        public void ModificarCliente(PatchCliente cliente)
        {
            var response = WebHelper.Patch("Cliente/PatchCliente",
                JsonConvert.SerializeObject(cliente));
            DeserializarRespuesta<object>(response);
        }

        public void BajaCliente(Guid idCliente)
        {
            var response = WebHelper.Delete($"Cliente/BajaCliente?id={idCliente}");
            DeserializarRespuesta<object>(response);
        }

        public void ReactivarCliente(Guid idCliente)
        {
            var response = WebHelper.Patch($"Cliente/ReactivarCliente?id={idCliente}", "");
            DeserializarRespuesta<object>(response);
        }
    }
}