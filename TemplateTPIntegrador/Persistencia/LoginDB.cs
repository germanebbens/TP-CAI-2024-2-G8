using Persistencia.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia
{
    public class LoginDB
    {
        public void guardarIntento(String username)
        {
            DBHelper dbHelper = new DBHelper("intentos_login");
            dbHelper.Insertar(username, "1");
        }

        public void actualizarIntento(String key, String newValue)
        {
            DBHelper dbHelper = new DBHelper("intentos_login");
            dbHelper.Modificar(key, newValue);
        }

        public int obtenerIntentos(String username)
        {
            DBHelper dbHelper = new DBHelper("intentos_login");
            String valor = dbHelper.Buscar(username);
            
            if(valor == null)
            {
                valor = "0";
            }

            return int.Parse(valor);

        }

        public String obtenerArray(String key)
        {
            DBHelper dbHelper = new DBHelper("intentos_login");
            String valor = dbHelper.Buscar(key);
            return valor;
        }

        public void modificarDatos(String key, String newValue)
        {
            DBHelper dbHelper = new DBHelper("intentos_login");
            dbHelper.Modificar(key, newValue);
        }

        public void guardarArray(List<String> datos)
        {
            DBHelper dbHelper = new DBHelper("intentos_login");
            String correosElectronicos = "";
            foreach(String correo in datos)
            {
                correosElectronicos = correosElectronicos + "," + correo;
            }
            dbHelper.Insertar("correosElectronicos", correosElectronicos);

        }
    }
}
