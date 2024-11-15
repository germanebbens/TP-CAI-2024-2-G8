using ElectroHogar.Datos;
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
        public bool Editable { get; set; }
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
            new ColumnaConfig
            {
                Nombre = "NombreUsuario",
                Titulo = "NombreUsuario",
                PropiedadDatos = "NombreUsuario",
                Ancho = 90
            },
            new ColumnaConfig
            {
                Nombre = "Nombre",
                Titulo = "Nombre Completo",
                PropiedadDatos = "Nombre",
                Ancho = 150
            },
            new ColumnaConfig
            {
                Nombre = "Perfil",
                Titulo = "Perfil",
                PropiedadDatos = "Perfil",
                Ancho = 80
            }
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
        public List<string> CamposBusqueda => new List<string> { "Cuit", "Nombre" };
        public string NombreIdentificador => "Proveedor";
        public object Service => _service;

        public List<ColumnaConfig> Columnas => new List<ColumnaConfig>
        {
            new ColumnaConfig
            {
                Nombre = "Id",
                Titulo = "Id",
                PropiedadDatos = "Id",
                Ancho = 100
            },
            new ColumnaConfig
            {
                Nombre = "Nombre",
                Titulo = "Nombre",
                PropiedadDatos = "Nombre",
                Ancho = 100
            },
            new ColumnaConfig
            {
                Nombre = "Apellido",
                Titulo = "Apellido",
                PropiedadDatos = "Apellido",
                Ancho = 100
            },
            new ColumnaConfig
            {
                Nombre = "CUIT",
                Titulo = "CUIT",
                PropiedadDatos = "CUIT",
                Ancho = 100
            },
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

    public class ProveedoresSeleccionListadoConfig : IListadoConfiguracion
    {
        private readonly Proveedores _service = new Proveedores();
        private readonly Action<ProveedorList> _onSeleccion;

        public ProveedoresSeleccionListadoConfig(Action<ProveedorList> onSeleccion)
        {
            _onSeleccion = onSeleccion;
        }

        public string Titulo => "Seleccionar Proveedor";
        public string CampoBusqueda => "CUIT o Nombre";
        public List<string> CamposBusqueda => new List<string> { "Cuit", "Nombre" };
        public string NombreIdentificador => "Proveedor";
        public object Service => _service;

        public List<ColumnaConfig> Columnas => new List<ColumnaConfig>
        {
            new ColumnaConfig
            {
                Nombre = "Id",
                Titulo = "Id",
                PropiedadDatos = "Id",
                Ancho = 100
            },
            new ColumnaConfig
            {
                Nombre = "Nombre",
                Titulo = "Nombre",
                PropiedadDatos = "Nombre",
                Ancho = 100
            },
            new ColumnaConfig
            {
                Nombre = "CUIT",
                Titulo = "CUIT",
                PropiedadDatos = "CUIT",
                Ancho = 100
            },
        };

        public List<AccionConfig> Acciones => new List<AccionConfig>
        {
            new AccionConfig
            {
                Nombre = "Seleccionar",
                Texto = "Seleccionar",
                RequiereConfirmacion = false,
                Accion = (row) => {
                    var proveedorId = Guid.Parse(row.Cells["Id"].Value.ToString());
                    var proveedor = _service.ObtenerProveedorPorId(proveedorId);
                    _onSeleccion?.Invoke(proveedor);
                
                    // Obtener el formulario que contiene la fila y cerrarlo
                    var form = row.DataGridView.FindForm();
                    if (form != null)
                    {
                        form.DialogResult = DialogResult.OK;
                        form.Close();
                    }
                }
            }
        };
    }
}
