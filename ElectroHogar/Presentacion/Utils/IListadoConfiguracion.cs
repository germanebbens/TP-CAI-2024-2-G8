using ElectroHogar.Negocio;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ElectroHogar.Presentacion.Utils
{
    public interface IListadoConfiguracion
    {
        string Titulo { get; }
        string CampoBusqueda { get; }
        List<string> CamposBusqueda { get; }
        List<ColumnaConfig> Columnas { get; }
        List<AccionConfig> Acciones { get; }
        string NombreIdentificador { get; }
        object Service { get; }
    }

    public class ColumnaConfig
    {
        public string Nombre { get; set; }
        public string Titulo { get; set; }
        public string PropiedadDatos { get; set; }
        public int Ancho { get; set; }
    }

    public class AccionConfig
    {
        public string Nombre { get; set; }
        public string Texto { get; set; }
        public bool RequiereConfirmacion { get; set; }
        public string MensajeConfirmacion { get; set; }
        public Action<DataGridViewRow> Accion { get; set; }
    }

    public class UsuariosListadoConfig : IListadoConfiguracion
    {
        private readonly Usuarios _service = new Usuarios();

        public string Titulo => "Usuarios Activos";
        public string CampoBusqueda => "ID o Username";
        public List<string> CamposBusqueda => new List<string> { "Id", "NombreUsuario" };
        public string NombreIdentificador => "Usuario";
        public object Service => _service;

        public List<ColumnaConfig> Columnas => new List<ColumnaConfig>
        {
            new ColumnaConfig
            {
                Nombre = "Id",
                Titulo = "ID",
                PropiedadDatos = "Id",
                Ancho = 100
            },
            // ... resto de columnas
        };

        public List<AccionConfig> Acciones => new List<AccionConfig>
        {
            new AccionConfig
            {
                Nombre = "Desactivar",
                Texto = "Desactivar",
                RequiereConfirmacion = true,
                MensajeConfirmacion = "¿Está seguro que desea deshabilitar al usuario {0}?",
                Accion = (row) => {
                    var userId = Guid.Parse(row.Cells["Id"].Value.ToString());
                    _service.DarBajaUsuario(userId);
                }
            }
        };

    }

    public class ProveedoresListadoConfig : IListadoConfiguracion
    {
        private readonly Proveedores _service = new Proveedores();

        public string Titulo => "Proveedores Activos";
        public string CampoBusqueda => "CUIT o Nombre";
        public List<string> CamposBusqueda => new List<string> { "Cuit", "NombreCompleto" };
        public string NombreIdentificador => "Proveedor";
        public object Service => _service;

        public List<ColumnaConfig> Columnas => new List<ColumnaConfig>
        {
            new ColumnaConfig
            {
                Nombre = "Id",
                Titulo = "ID",
                PropiedadDatos = "Id",
                Ancho = 100
            },
            // ... resto de columnas
        };

        public List<AccionConfig> Acciones => new List<AccionConfig>
        {
            new AccionConfig
            {
                Nombre = "Desactivar",
                Texto = "Desactivar",
                RequiereConfirmacion = true,
                MensajeConfirmacion = "¿Está seguro que desea deshabilitar al proveedor {0}?",
                Accion = (row) => {
                    var proveedorId = Guid.Parse(row.Cells["Id"].Value.ToString());
                    _service.DarBajaProveedor(proveedorId);
                }
            }
        };
    }

    public class ClientesListadoConfig// : IListadoConfiguracion
    {
       // private readonly Clientes _service = new Clientes();

        public string Titulo => "Clientes";
        public string CampoBusqueda => "DNI o Nombre";
        public string NombreIdentificador => "Cliente";
       // public object Service => _service;

        public List<ColumnaConfig> Columnas => new List<ColumnaConfig>
        {
            new ColumnaConfig
            {
                Nombre = "Id",
                Titulo = "ID",
                PropiedadDatos = "Id",
                Ancho = 100
            },
            // ... resto de columnas
        };

        public List<AccionConfig> Acciones => new List<AccionConfig>
        {
            new AccionConfig
            {
                Nombre = "Modificar",
                Texto = "Modificar",
                RequiereConfirmacion = false,
                //Accion = (sender, e) => /* Abrir formulario de modificación */
            },
            new AccionConfig
            {
                Nombre = "Desactivar",
                Texto = "Desactivar",
                RequiereConfirmacion = true,
                MensajeConfirmacion = "¿Está seguro que desea deshabilitar al cliente {0}?"
            }
        };
    }
}
