using Newtonsoft.Json;
using System;
using ElectroHogar.Config;
using ElectroHogar.Datos;
using ElectroHogar.Persistencia.Utils;

namespace ElectroHogar.Persistencia
{
    internal class NuevoUsuarioWS
    {
        private readonly string _adminId;

        public NuevoUsuarioWS()
        {
            _adminId = ConfigHelper.GetValue("AdminId");
        }

        public void AgregarUsuario(NuevoUsuarioDef usuario)
        {
            var jsonData = JsonConvert.SerializeObject(usuario);
            var response = WebHelper.Post("Usuario/AgregarUsuario", jsonData);

            if (response.IsSuccessStatusCode)
            {
                // Usuario agregado con éxito
            }
            else
            {
                // Manejar error al agregar usuario
                throw new Exception($"Error al agregar usuario: {response.ReasonPhrase}");
            }
        }
    }
}
