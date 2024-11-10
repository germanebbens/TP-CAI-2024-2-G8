using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ElectroHogar.Datos;
using ElectroHogar.Persistencia.Utils;

namespace ElectroHogar.Persistencia
{
    public class ProveedoresWS : BaseWS
    {
        public ProveedoresWS() : base()
        {
        }

        public List<ProveedorList> ObtenerProveedores()
        {
            var response = WebHelper.Get("Proveedor/TraerProveedores");
            return DeserializarRespuesta<List<ProveedorList>>(response);
        }

        public void AgregarProveedor(AddProveedor proveedor)
        {
            var response = WebHelper.Post("Proveedor/AgregarProveedor",
                JsonConvert.SerializeObject(proveedor));
            DeserializarRespuesta<object>(response);
        }

        public void ModificarProveedor(PatchProveedor proveedor)
        {
            var response = WebHelper.Patch("Proveedor/ModificarProveedor",
                JsonConvert.SerializeObject(proveedor));
            DeserializarRespuesta<object>(response);
        }

        public void BajaProveedor(Guid idProveedor)
        {
            var permisos = new { id = idProveedor, idUsuario = _adminId };
            var response = WebHelper.DeleteWithBody("Proveedor/BajaProveedor",
                JsonConvert.SerializeObject(permisos));
            DeserializarRespuesta<object>(response);
        }

        public void ReactivarProveedor(Guid idProveedor)
        {
            var permisos = new { id = idProveedor, idUsuario = _adminId };
            var response = WebHelper.Patch("Proveedor/ReactivarProveedor",
                JsonConvert.SerializeObject(permisos));
            DeserializarRespuesta<object>(response);
        }
    }
}