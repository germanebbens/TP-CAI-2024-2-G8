using ElectroHogar.Datos;

namespace ElectroHogar.Negocio
{
    public static class Perfiles
    {
        public class ModuloMenu
        {
            public string Nombre { get; }
            public string FormularioDestino { get; }

            public ModuloMenu(string nombre, string formularioDestino)
            {
                Nombre = nombre;
                FormularioDestino = formularioDestino;
            }
        }

        public static ModuloMenu[] ObtenerModulos(PerfilUsuario perfil)
        {
            switch (perfil)
            {
                case PerfilUsuario.Administrador:
                    return new[]
                    {
                        new ModuloMenu("Gestión de Usuarios", "UsuariosForm"),
                        new ModuloMenu("Gestión de Proveedores", "ProveedoresForm"),
                        new ModuloMenu("Gestión de Productos", "ProductosForm"),
                        new ModuloMenu("Reportes de Stock", "StockReportForm"),
                        new ModuloMenu("Reportes de Ventas", "VentasReportForm"),
                        new ModuloMenu("Reportes de Productos", "ProductosReportForm")
                    };

                case PerfilUsuario.Supervisor:
                    return new[]
                    {
                        new ModuloMenu("Gestión de Productos", "ProductosForm"),
                        new ModuloMenu("Devoluciones", "DevolucionesForm"),
                        new ModuloMenu("Reportes de Stock", "StockReportForm"),
                        new ModuloMenu("Reportes de Ventas", "VentasReportForm"),
                        new ModuloMenu("Reportes de Productos", "ProductosReportForm")
                    };

                case PerfilUsuario.Vendedor:
                    return new[]
                    {
                        new ModuloMenu("Ventas", "VentasForm"),
                        new ModuloMenu("Reportes de Ventas", "VentasReportForm")
                    };

                default:
                    return new ModuloMenu[] { };
            }
        }
    }
}