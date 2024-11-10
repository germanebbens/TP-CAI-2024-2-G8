using System;
using System.Collections.Generic;
using ElectroHogar.Persistencia.Utils;

namespace ElectroHogar.Persistencia
{
    public class LoginDB
    {
        private readonly DBHelper _dbHelper;

        public LoginDB()
        {
            _dbHelper = new DBHelper("intentos_login");
        }

        public void guardarIntento(string username)
        {
            _dbHelper.Insertar(username, "1");
        }

        public void actualizarIntento(string key, string newValue)
        {
            _dbHelper.Modificar(key, newValue);
        }

        public int obtenerIntentos(string username)
        {
            var valor = _dbHelper.Buscar(username);
            if (valor == null)
            {
                return 0;
            }
            return int.Parse(valor);
        }

        public string obtenerArray(string key)
        {
            return _dbHelper.Buscar(key);
        }

        public void modificarDatos(string key, string newValue)
        {
            _dbHelper.Modificar(key, newValue);
        }

        public void guardarArray(List<string> datos)
        {
            string correosElectronicos = string.Join(",", datos);
            _dbHelper.Insertar("correosElectronicos", correosElectronicos);
        }
    }
}