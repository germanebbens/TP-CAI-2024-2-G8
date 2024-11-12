using ElectroHogar.Persistencia.Utils;
using Newtonsoft.Json;

public class ClavesTemporalesDB
{
    private readonly DBHelper _dbHelper;

    public ClavesTemporalesDB()
    {
        _dbHelper = new DBHelper("claves_temporales");
    }

    public void GuardarClaveTemporal(string nombreUsuario, string claveTemporal, string userId)
    {
        var datosClave = new
        {
            Clave = claveTemporal,
            UserId = userId
        }; 

        string datosJson = JsonConvert.SerializeObject(datosClave);
        _dbHelper.Insertar(nombreUsuario, datosJson);
    }

    public (string Clave, string UserId) ObtenerClaveTemporal(string nombreUsuario)
    {
        string datosJson = _dbHelper.Buscar(nombreUsuario);

        if (string.IsNullOrEmpty(datosJson))
            return (null, null);

        try
        {
            var datos = JsonConvert.DeserializeObject<dynamic>(datosJson);
            return (datos.Clave.ToString(), datos.UserId.ToString());
        }
        catch
        {
            return (null, null);
        }
    }

    public void EliminarClaveTemporal(string nombreUsuario)
    {
        _dbHelper.Borrar(nombreUsuario);
    }
}