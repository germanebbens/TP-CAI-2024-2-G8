using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using ElectroHogar.Datos;
using ElectroHogar.Persistencia.Utils;
using ElectroHogar.Config;

namespace ElectroHogar.Persistencia
{
    public class LoginWS
    {
        private readonly string adminId;

        public LoginWS()
        {
            // Read values from App.Config
            adminId = ConfigHelper.GetValue("AdminId");
        }

        public String login(String username, String password)
        {
            Dictionary<String, String> datos = new Dictionary<String, String>();
            datos.Add("nombreUsuario", username);
            datos.Add("contraseña", password);

            var jsonData = JsonConvert.SerializeObject(datos);
            HttpResponseMessage response = WebHelper.Post("Usuario/Login", jsonData);
            String responseBody = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<String>(responseBody);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {responseBody}");
                throw new Exception(responseBody);
            }
        }

        public List<UsuarioWS> buscarDatosUsuario()
        {
            HttpResponseMessage response = WebHelper.Get("Usuario/TraerUsuariosActivos?id=" + adminId);
            if (response.IsSuccessStatusCode)
            {
                var contentStream = response.Content.ReadAsStringAsync().Result;
                List<UsuarioWS> listadoUsuarios = JsonConvert.DeserializeObject<List<UsuarioWS>>(contentStream);
                return listadoUsuarios;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                throw new Exception("Error al momento de buscar los usuarios");
            }
        }
    }
}