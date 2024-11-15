using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ElectroHogar.Config;

namespace ElectroHogar.Persistencia.Utils
{
    public class DBHelper
    {
        private readonly string _filePath;

        public DBHelper(string dataBaseName)
        {
            var rutaBase = Path.GetDirectoryName(typeof(DBHelper).Assembly.Location);
            var rutaArchivos = ConfigHelper.GetValue("RutaArchivos");
            _filePath = Path.Combine(rutaBase, rutaArchivos, $"{dataBaseName}.json");

            // Crear el archivo si no existe
            if (!File.Exists(_filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
                File.WriteAllText(_filePath, "{}");
            }
        }

        public void Insertar(string key, string value)
        {
            var datos = ObtenerDatos();
            datos[key] = value;
            GuardarDatos(datos);
        }

        public void Modificar(string key, string newValue)
        {
            var datos = ObtenerDatos();
            datos[key] = newValue;
            GuardarDatos(datos);
        }

        public string Buscar(string key)
        {
            var datos = ObtenerDatos();
            if (datos.TryGetValue(key, out var valor))
            {
                return valor;
            }
            return null;
        }

        public void Borrar(string key)
        {
            var datos = ObtenerDatos();
            datos.Remove(key);
            GuardarDatos(datos);
        }

        public List<string> Listar()
        {
            var datos = ObtenerDatos();
            return new List<string>(datos.Keys);
        }

        private Dictionary<string, string> ObtenerDatos()
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        private void GuardarDatos(Dictionary<string, string> datos)
        {
            var json = JsonConvert.SerializeObject(datos, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}