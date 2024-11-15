using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElectroHogar.Datos;
using ElectroHogar.Persistencia;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Negocio
{
    public class Clientes
    {
        private readonly ClientesWS _clientesWS;

        public Clientes()
        {
            _clientesWS = new ClientesWS();
        }

        public List<ClienteList> ObtenerActivos()
        {
            try
            {
                var clientes = _clientesWS.ObtenerClientes();
                return clientes.Where(c => !c.FechaBaja.HasValue).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener clientes: {ex.Message}");
            }
        }

        public ClienteList ObtenerClientePorId(Guid idCliente)
        {
            try
            {
                return _clientesWS.ObtenerCliente(idCliente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener cliente: {ex.Message}");
            }
        }

        public void RegistrarCliente(string nombre, string apellido, int dni, string direccion,
            string telefono, string email, DateTime fechaNacimiento)
        {
            try
            {
                var nuevoCliente = new AddCliente
                {
                    IdUsuario = Guid.NewGuid(), // Este ID debería venir del usuario logueado
                    Nombre = nombre,
                    Apellido = apellido,
                    Dni = dni,
                    Direccion = direccion,
                    Telefono = telefono,
                    Email = email,
                    FechaNacimiento = fechaNacimiento,
                    Host = "Grupo X" // Según el TP, debe ser "Grupo X"
                };

                ValidarDatosBasicos(nuevoCliente);
                _clientesWS.AgregarCliente(nuevoCliente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar cliente: {ex.Message}");
            }
        }

        public void ModificarCliente(Guid idCliente, string direccion, string telefono, string email)
        {
            try
            {
                var cliente = ObtenerClientePorId(idCliente);
                if (cliente == null)
                    throw new Exception("Cliente no encontrado");

                if (cliente.FechaBaja.HasValue)
                    throw new Exception("No se puede modificar un cliente inactivo");

                var clienteModificado = new PatchCliente
                {
                    Id = idCliente,
                    Direccion = direccion,
                    Telefono = telefono,
                    Email = email
                };

                ValidarDatosModificacion(clienteModificado);
                _clientesWS.ModificarCliente(clienteModificado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al modificar cliente: {ex.Message}");
            }
        }

        public void DarBajaCliente(Guid idCliente)
        {
            try
            {
                var cliente = ObtenerClientePorId(idCliente);
                if (cliente == null)
                    throw new Exception("Cliente no encontrado");

                if (cliente.FechaBaja.HasValue)
                    throw new Exception("El cliente ya está dado de baja");

                _clientesWS.BajaCliente(idCliente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al dar de baja el cliente: {ex.Message}");
            }
        }

        private void ValidarDatosBasicos(AddCliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                throw new Exception("El nombre del cliente es requerido");

            if (string.IsNullOrWhiteSpace(cliente.Apellido))
                throw new Exception("El apellido del cliente es requerido");

            if (cliente.Dni <= 0)
                throw new Exception("El DNI del cliente es requerido y debe ser un número válido");

            if (string.IsNullOrWhiteSpace(cliente.Direccion))
                throw new Exception("La dirección del cliente es requerida");

            if (string.IsNullOrWhiteSpace(cliente.Telefono))
                throw new Exception("El teléfono del cliente es requerido");

            var (isValidEmail, mensajeEmail) = Validations.ValidarEmail(cliente.Email);
            if (!isValidEmail)
                throw new Exception(mensajeEmail);

            var (isValidTelefono, mensajeTelefono) = Validations.ValidarTelefono(cliente.Telefono);
            if (!isValidTelefono)
                throw new Exception(mensajeTelefono);

            var (isValidFecha, mensajeFecha) = Validations.ValidarFecha(cliente.FechaNacimiento);
            if (!isValidFecha)
                throw new Exception(mensajeFecha);
        }

        private void ValidarDatosModificacion(PatchCliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Direccion))
                throw new Exception("La dirección del cliente es requerida");

            if (string.IsNullOrWhiteSpace(cliente.Telefono))
                throw new Exception("El teléfono del cliente es requerido");

            var (isValidEmail, mensajeEmail) = Validations.ValidarEmail(cliente.Email);
            if (!isValidEmail)
                throw new Exception(mensajeEmail);

            var (isValidTelefono, mensajeTelefono) = Validations.ValidarTelefono(cliente.Telefono);
            if (!isValidTelefono)
                throw new Exception(mensajeTelefono);
        }

        public bool EsClienteNuevo(Guid idCliente)
        {
            try
            {
                Ventas ventas = new Ventas();
                List<VentaList> ventasCliente = ventas.ObtenerVentasPorCliente(idCliente);
                if (ventasCliente.Count == 0)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar si el cliente es nuevo: {ex.Message}");
            }
        }
    }
}