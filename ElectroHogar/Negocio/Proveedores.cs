using System;
using System.Collections.Generic;
using System.Linq;
using ElectroHogar.Datos;
using ElectroHogar.Persistencia;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Negocio
{
    public class Proveedores
    {
        private readonly ProveedoresWS _proveedoresWS;
        private readonly Dictionary<Guid, List<CategoriaProducto>> _categoriasPorProveedor;

        public Proveedores()
        {
            _proveedoresWS = new ProveedoresWS();
            _categoriasPorProveedor = new Dictionary<Guid, List<CategoriaProducto>>();
        }

        public List<ProveedorList> ObtenerProveedoresActivos()
        {
            try
            {
                var proveedores = _proveedoresWS.ObtenerProveedores();
                return proveedores.Where(p => !p.FechaBaja.HasValue).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener proveedores: {ex.Message}");
            }
        }

        public void RegistrarProveedor(string nombre, string apellido, string email, string cuit, List<CategoriaProducto> categorias)
        {
            try
            {
                var nuevoProveedor = new AddProveedor
                {
                    IdUsuario = Guid.Parse(_proveedoresWS.AdminId),
                    Nombre = nombre,
                    Apellido = apellido,
                    Email = email,
                    Cuit = cuit
                };

                ValidarDatosBasicos(nuevoProveedor);
                ValidarCategorias(categorias);

                _proveedoresWS.AgregarProveedor(nuevoProveedor);

                // Guardamos las categorías del proveedor nuevo
                var proveedores = _proveedoresWS.ObtenerProveedores();
                var proveedorCreado = proveedores.FirstOrDefault(p =>
                    p.Cuit == nuevoProveedor.Cuit &&
                    !p.FechaBaja.HasValue);

                if (proveedorCreado != null)
                {
                    _categoriasPorProveedor[proveedorCreado.Id] = categorias;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar proveedor: {ex.Message}");
            }
        }

        public void ModificarProveedor(Guid idProveedor, string nombre, string apellido, string email, string cuit, List<CategoriaProducto> categorias)
        {
            try
            {
                // Verificar que el proveedor existe y está activo
                var proveedorActual = ObtenerProveedoresActivos()
                    .FirstOrDefault(p => p.Id == idProveedor);

                if (proveedorActual == null)
                    throw new Exception("El proveedor no existe o está inactivo");

                var proveedorModificado = new PatchProveedor
                {
                    Id = idProveedor,
                    IdUsuario = Guid.Parse(_proveedoresWS.AdminId),
                    Nombre = nombre,
                    Apellido = apellido,
                    Email = email,
                    Cuit = cuit
                };

                ValidarDatosBasicos(proveedorModificado);
                ValidarCategorias(categorias);

                _proveedoresWS.ModificarProveedor(proveedorModificado);

                // Actualizar categorías
                _categoriasPorProveedor[idProveedor] = categorias;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al modificar proveedor: {ex.Message}");
            }
        }

        public void DarBajaProveedor(Guid idProveedor)
        {
            try
            {
                _proveedoresWS.BajaProveedor(idProveedor);
                _categoriasPorProveedor.Remove(idProveedor);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al dar de baja el proveedor: {ex.Message}");
            }
        }

        public List<CategoriaProducto> ObtenerCategoriasProveedor(Guid idProveedor)
        {
            return _categoriasPorProveedor.TryGetValue(idProveedor, out var categorias)
                ? categorias
                : new List<CategoriaProducto>();
        }

        public ProveedorList ObtenerProveedorPorId(Guid idProveedor)
        {
            try
            {
                return _proveedoresWS.ObtenerProveedores()
                    .FirstOrDefault(p => p.Id == idProveedor && !p.FechaBaja.HasValue);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener proveedor: {ex.Message}");
            }
        }

        private void ValidarDatosBasicos(IProveedorBase proveedor)
        {
            if (string.IsNullOrWhiteSpace(proveedor.Nombre))
                throw new Exception("El nombre del proveedor es requerido");

            if (string.IsNullOrWhiteSpace(proveedor.Apellido))
                throw new Exception("El apellido del proveedor es requerido");

            if (string.IsNullOrWhiteSpace(proveedor.Email))
                throw new Exception("El email del proveedor es requerido");

            if (string.IsNullOrWhiteSpace(proveedor.Cuit))
                throw new Exception("El CUIT del proveedor es requerido");

            if (!Validations.ValidarEmail(proveedor.Email).isValid)
                throw new Exception("El formato del email no es válido");

            if (!proveedor.Cuit.All(char.IsDigit) || proveedor.Cuit.Length != 11)
                throw new Exception("El CUIT debe contener 11 dígitos numéricos");
        }

        private void ValidarCategorias(List<CategoriaProducto> categorias)
        {
            if (categorias == null || !categorias.Any())
                throw new Exception("Debe seleccionar al menos una categoría");

            if (categorias.Count != categorias.Distinct().Count())
                throw new Exception("No puede seleccionar la misma categoría más de una vez");
        }

        public static List<CategoriaProducto> ObtenerTodasLasCategorias()
        {
            return Enum.GetValues(typeof(CategoriaProducto))
                .Cast<CategoriaProducto>()
                .ToList();
        }

        public static string ObtenerDescripcionCategoria(CategoriaProducto categoria)
        {
            return categoria.ToString().Replace("_", " ");
        }
    }
}