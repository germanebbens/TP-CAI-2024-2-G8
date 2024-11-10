using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ElectroHogar.Datos;
using ElectroHogar.Persistencia.Utils;
using ElectroHogar.Config;

namespace ElectroHogar.Persistencia
{
    public class UsuariosWS : BaseWS
    {

        public UsuariosWS(string usuarioLogueadoId = null) : base()
        {
        }

        public string Login(string username, string password)
        {
            var datos = new Dictionary<string, string>
                {
                    { "nombreUsuario", username },
                    { "contraseña", password }
                };

            var response = WebHelper.Post("Usuario/Login", JsonConvert.SerializeObject(datos));
            return DeserializarRespuesta<string>(response);
        }

        public List<UsuarioList> BuscarUsuariosActivos()
        {
            var response = WebHelper.Get($"Usuario/TraerUsuariosActivos?id={_adminId}");
            return DeserializarRespuesta<List<UsuarioList>>(response);
        }

        public void AgregarUsuario(AddUsuario usuario)
        {
            var response = WebHelper.Post("Usuario/AgregarUsuario", JsonConvert.SerializeObject(usuario));
            DeserializarRespuesta<object>(response);
        }

        public void CambiarContraseña(PatchUsuario cambioPassword)
        {
            var response = WebHelper.Patch("Usuario/CambiarContraseña", JsonConvert.SerializeObject(cambioPassword));
            DeserializarRespuesta<object>(response);
        }

        public void BajaUsuario(Guid idUsuario)
        {
            var permisos = new { id = idUsuario, idUsuario = _adminId };
            var response = WebHelper.DeleteWithBody("Usuario/BajaUsuario", JsonConvert.SerializeObject(permisos));
            DeserializarRespuesta<object>(response);
        }

        public void ReactivarUsuario(Guid idUsuario)
        {
            var permisos = new { id = idUsuario, idUsuario = _adminId };
            var response = WebHelper.Patch("Usuario/ReactivarUsuario", JsonConvert.SerializeObject(permisos));
            DeserializarRespuesta<object>(response);
        }
    }
}