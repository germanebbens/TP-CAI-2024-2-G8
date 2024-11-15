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

        public Proveedores()
        {
            _proveedoresWS = new ProveedoresWS();
        }

        public List<ProveedorList> ObtenerActivos()
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

        public void RegistrarProveedor(string nombre, string apellido, string email, string cuit)
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
                _proveedoresWS.AgregarProveedor(nuevoProveedor);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar proveedor: {ex.Message}");
            }
        }

        public void DarBajaProveedor(Guid idProveedor)
        {
            try
            {
                _proveedoresWS.BajaProveedor(idProveedor);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al dar de baja el proveedor: {ex.Message}");
            }
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

        // TODO: Implementar cuando esté la clase Productos
        public List<CategoriaProducto> ObtenerCategoriasProveedor(Guid idProveedor)
        {
            // Este método se implementará cuando tengamos la clase Productos
            // Deberá:
            // 1. Obtener todos los productos
            // 2. Filtrar por el proveedor
            // 3. Obtener las categorías únicas de esos productos
            throw new NotImplementedException("Pendiente de implementación con la clase Productos");
        }
    }
}