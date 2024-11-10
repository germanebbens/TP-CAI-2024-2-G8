using System;
using System.Collections.Generic;
using ElectroHogar.Persistencia.Utils;

namespace ElectroHogar.Persistencia
{
    public class ClavesTemporalesDB
    {
        private readonly DBHelper _dbHelper;

        public ClavesTemporalesDB()
        {
            _dbHelper = new DBHelper("claves_temporales");
        }

        public void GuardarClaveTemporal(string nombreUsuario, string claveTemporal)
        {
            _dbHelper.Insertar(nombreUsuario, claveTemporal);
        }

        public string ObtenerClaveTemporal(string nombreUsuario)
        {
            return _dbHelper.Buscar(nombreUsuario);
        }

        public void EliminarClaveTemporal(string nombreUsuario)
        {
            _dbHelper.Borrar(nombreUsuario);
        }
    }
}