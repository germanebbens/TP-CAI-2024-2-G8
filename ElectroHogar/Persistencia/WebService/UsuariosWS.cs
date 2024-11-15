using ElectroHogar.Datos;
using ElectroHogar.Persistencia.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElectroHogar.Persistencia
{
    public class UsuariosWS : BaseWS
    {
        public readonly string adminId;
        public UsuariosWS() : base()
        {
            adminId = _adminId;
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

        public List<User> BuscarUsuariosActivos()
        {
            var response = WebHelper.Get($"Usuario/TraerUsuariosActivos?id={_adminId}");
            return DeserializarRespuesta<List<User>>(response);
        }

        public User BucarUsuarioPorId(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));
            return BuscarUsuariosActivos()
                .FirstOrDefault(u => u.Id.ToString().Equals(id));
        }

        public User BucarUsuarioPorUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

            return BuscarUsuariosActivos()
                .FirstOrDefault(u => u.NombreUsuario.Equals(username));
        }

        public void AgregarUsuario(AddUser usuario)
        {
            var response = WebHelper.Post("Usuario/AgregarUsuario", JsonConvert.SerializeObject(usuario));
            DeserializarRespuesta<object>(response);
        }

        public void CambiarContraseña(PatchUser cambioPassword)
        {
            var response = WebHelper.Patch("Usuario/CambiarContraseña", JsonConvert.SerializeObject(cambioPassword));
            DeserializarRespuesta<object>(response);
        }

        public void BajaUsuario(Guid IdUsuario)
        {
            var permisos = new { id = IdUsuario.ToString(), idUsuario = _adminId };
            var response = WebHelper.DeleteWithBody("Usuario/BajaUsuario", JsonConvert.SerializeObject(permisos));
            DeserializarRespuesta<object>(response);
        }

        public void ReactivarUsuario(Guid idUsuario)
        {
            var permisos = new { id = idUsuario.ToString(), idUsuario = _adminId };
            var response = WebHelper.Patch("Usuario/ReactivarUsuario", JsonConvert.SerializeObject(permisos));
            DeserializarRespuesta<object>(response);
        }
    }
}