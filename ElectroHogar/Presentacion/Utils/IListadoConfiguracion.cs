using ElectroHogar.Datos;
using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Forms;
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

    public class ClienteSeleccionListadoConfig : IListadoConfiguracion
    {
        private readonly Clientes _service = new Clientes();
        private readonly Action<ClienteList> _onSeleccion;

        public ClienteSeleccionListadoConfig(Action<ClienteList> onSeleccion)
        {
            _onSeleccion = onSeleccion;
        }

        public string Titulo => "Seleccionar Cliente";
        public string CampoBusqueda => "DNI o Nombre";
        public List<string> CamposBusqueda => new List<string> { "DNI", "Nombre" };
        public string NombreIdentificador => "Cliente";
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
                Nombre = "DNI",
                Titulo = "DNI",
                PropiedadDatos = "Dni",
                Ancho = 80
            },
            new ColumnaConfig
            {
                Nombre = "Direccion",
                Titulo = "Dirección",
                PropiedadDatos = "Direccion",
                Ancho = 150
            },
            new ColumnaConfig
            {
                Nombre = "Telefono",
                Titulo = "Teléfono",
                PropiedadDatos = "Telefono",
                Ancho = 100
            },
            new ColumnaConfig
            {
                Nombre = "Email",
                Titulo = "Email",
                PropiedadDatos = "Email",
                Ancho = 150
            }
        };

        public List<AccionConfig> Acciones => new List<AccionConfig>
        {
            new AccionConfig
            {
                Nombre = "Seleccionar",
                Texto = "Seleccionar",
                RequiereConfirmacion = false,
                Accion = (row) => {
                    var cliente = new ClienteList
                    {
                        Id = Guid.Parse(row.Cells["Id"].Value.ToString()),
                        Nombre = row.Cells["Nombre"].Value.ToString(),
                        Apellido = row.Cells["Apellido"].Value.ToString(),
                        Dni = int.Parse(row.Cells["DNI"].Value.ToString()),
                        Direccion = row.Cells["Direccion"].Value?.ToString(),
                        Telefono = row.Cells["Telefono"].Value?.ToString(),
                        Email = row.Cells["Email"].Value?.ToString()
                    };

                    _onSeleccion?.Invoke(cliente);
                    var form = row.DataGridView.FindForm();
                    if (form != null)
                    {
                        form.DialogResult = DialogResult.OK;
                        form.Close();
                    }
                }
            },
            new AccionConfig
            {
                Nombre = "Editar",
                Texto = "Editar",
                RequiereConfirmacion = false,
                Accion = (row) => {
                    var clienteId = Guid.Parse(row.Cells["Id"].Value.ToString());
                    var formEdicion = new ClienteEdicionForm(clienteId);
                    if (formEdicion.ShowDialog() == DialogResult.OK)
                    {
                        var clienteActualizado = _service.ObtenerClientePorId(clienteId);
                    
                        row.Cells["Direccion"].Value = clienteActualizado.Direccion;
                        row.Cells["Telefono"].Value = clienteActualizado.Telefono;
                        row.Cells["Email"].Value = clienteActualizado.Email;
                    
                        var gridView = row.DataGridView;
                        var items = (List<ClienteList>)gridView.Tag;
                        var index = items.FindIndex(c => c.Id == clienteId);
                        if (index >= 0)
                        {
                            items[index] = clienteActualizado;
                        }
                    }
                }
            }
        };
    }

    public class ProductoSeleccionListadoConfig : IListadoConfiguracion
    {
        private readonly Productos _service = new Productos();
        private readonly Action<ProductoList> _onSeleccion;

        public ProductoSeleccionListadoConfig(Action<ProductoList> onSeleccion)
        {
            _onSeleccion = onSeleccion;
        }

        public string Titulo => "Seleccionar Producto";
        public string CampoBusqueda => "Nombre";
        public List<string> CamposBusqueda => new List<string> { "Nombre" };
        public string NombreIdentificador => "Producto";
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
                Nombre = "Nombre",
                Titulo = "Nombre",
                PropiedadDatos = "Nombre",
                Ancho = 150
            },
            new ColumnaConfig
            {
                Nombre = "IdCategoria",
                Titulo = "Categoría",
                PropiedadDatos = "IdCategoria",
                Ancho = 100
            },
            new ColumnaConfig
            {
                Nombre = "Precio",
                Titulo = "Precio",
                PropiedadDatos = "Precio",
                Ancho = 100
            },
            new ColumnaConfig
            {
                Nombre = "Stock",
                Titulo = "Stock",
                PropiedadDatos = "Stock",
                Ancho = 100
            }
        };

        public List<AccionConfig> Acciones => new List<AccionConfig>
        {
            new AccionConfig
            {
                Nombre = "Seleccionar",
                Texto = "Seleccionar",
                RequiereConfirmacion = false,
                Accion = (row) => {
                    if (int.Parse(row.Cells["Stock"].Value.ToString()) <= 0)
                    {
                        MessageBox.Show("No hay stock disponible para este producto",
                            "Stock no disponible",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    var producto = new ProductoList
                    {
                        Id = Guid.Parse(row.Cells["Id"].Value.ToString()),
                        Nombre = row.Cells["Nombre"].Value.ToString(),
                        IdCategoria = (Categoria)row.Cells["IdCategoria"].Value,
                        Precio = double.Parse(row.Cells["Precio"].Value.ToString()),
                        Stock = int.Parse(row.Cells["Stock"].Value.ToString())
                    };

                    _onSeleccion?.Invoke(producto);
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
