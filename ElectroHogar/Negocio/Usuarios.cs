using System;
using System.Collections.Generic;
using System.Linq;
using ElectroHogar.Datos;
using ElectroHogar.Negocio.Utils;
using ElectroHogar.Persistencia;
using ElectroHogar.Persistencia.Utils;
using ElectroHogar.Presentacion.Utils;
using Newtonsoft.Json;

namespace ElectroHogar.Negocio
{
    internal class Usuarios
    {
        private readonly UsuariosWS _usuarioWS;
        private readonly ClavesTemporalesDB _clavesTemporalesDB;
        private readonly LoginDB _loginDB;
        private readonly UsuariosUtils _utils;

        public Usuarios()
        {
            _usuarioWS = new UsuariosWS();
            _clavesTemporalesDB = new ClavesTemporalesDB();
            _loginDB = new LoginDB();
            _utils = new UsuariosUtils();
        }

        public AddUser RegistrarNuevoUsuario(AddUser datosUsuario)
        {
            ValidarDatosBasicos(datosUsuario);
            ValidarNombreUsuario(datosUsuario);
            ValidarPerfil(datosUsuario.Host);

            var nuevoUsuario = new AddUser
            {
                IdUsuario = Guid.Parse(_usuarioWS.adminId),
                Host = datosUsuario.Host,
                Nombre = datosUsuario.Nombre,
                Apellido = datosUsuario.Apellido,
                Dni = datosUsuario.Dni,
                Direccion = datosUsuario.Direccion,
                Telefono = datosUsuario.Telefono?.Trim(),
                Email = datosUsuario.Email?.Trim(),
                FechaNacimiento = datosUsuario.FechaNacimiento,
                NombreUsuario = datosUsuario.NombreUsuario,
                Contraseña = _utils.GenerarContraseniaTemporal()
            };

            try
            {
                // Register user, save OTP and deactivate user
                _usuarioWS.AgregarUsuario(nuevoUsuario);
                var usuarioActivo = BuscarUsuarioPorUsername(nuevoUsuario.NombreUsuario);

                _clavesTemporalesDB.GuardarClaveTemporal(
                    nuevoUsuario.NombreUsuario,
                    nuevoUsuario.Contraseña,
                    usuarioActivo.Id.ToString()
                );

                // DarBajaUsuario(usuarioActivo.Id);
                return nuevoUsuario;
            }
            catch (Exception ex)
            {
                // TODO: loggear error
                throw new Exception($"Error al crear el usuario: {ex.Message}");
            }
        }
        
        public User BuscarUsuarioPorUsername(string username)
        {
            return _usuarioWS.BucarUsuarioPorUsername(username);
        }

        public void DarBajaUsuario(Guid? userId = null, string username = null)
        {
            try
            {
                Guid usuarioId;
                if (userId.HasValue)
                {
                    usuarioId = userId.Value;
                }
                else if (!string.IsNullOrEmpty(username))
                {
                    var usuario = BuscarUsuarioPorUsername(username);
                    usuarioId = usuario.Id;
                }
                else
                {
                    throw new ArgumentException("Debe proporcionar el ID de usuario o el nombre de usuario para dar de baja al usuario.");
                }

                _usuarioWS.BajaUsuario(usuarioId);
            }
            catch (Exception ex)
            {
                // TODO: loggear error
                throw new Exception($"Error al dar de baja al usuario: {ex.Message}", ex);
            }
        }

        public void ActivarUsuario(Guid? userId = null, string username = null)
        {
            try
            {
                Guid usuarioId;
                if (userId.HasValue)
                {
                    usuarioId = userId.Value;
                }
                else if (!string.IsNullOrEmpty(username))
                {
                    var usuario = BuscarUsuarioPorUsername(username);
                    usuarioId = usuario.Id;
                }
                else
                {
                    throw new ArgumentException("Debe proporcionar el ID de usuario o el nombre de usuario para dar reactivar al usuario.");
                }

                _usuarioWS.ReactivarUsuario(usuarioId);
            }
            catch (Exception ex)
            {
                // TODO: loggear error
                throw new Exception($"Error al reactivar usuario: {ex.Message}", ex);
            }
        }

        private void ValidarDatosBasicos(AddUser usuario)
        {
            // Required fields
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new Exception("El nombre del usuario es requerido");
            if (string.IsNullOrWhiteSpace(usuario.Apellido))
                throw new Exception("El apellido del usuario es requerido");
            if (string.IsNullOrWhiteSpace(usuario.Direccion))
                throw new Exception("La dirección del usuario es requerida");
            if (string.IsNullOrWhiteSpace(usuario.Telefono))
                throw new Exception("El teléfono del usuario es requerido");
            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new Exception("El email del usuario es requerido");

            // DNI
            if (usuario.Dni <= 0)
                throw new Exception("El DNI del usuario no es válido");
            if (usuario.Dni.ToString().Length != 8)
                throw new Exception("El DNI debe tener 8 dígitos");

            // Format Validations
            if (!Validations.ValidarEmail(usuario.Email).isValid)
                throw new Exception("El formato del email no es válido");
            if (!Validations.ValidarTelefono(usuario.Telefono).isValid)
                throw new Exception("El formato del teléfono no es válido");
            var (fechaValida, mensaje) = Validations.ValidarFecha(usuario.FechaNacimiento);
            if (!fechaValida)
                throw new Exception(mensaje);

            // age validation
            var edad = DateTime.Today.Year - usuario.FechaNacimiento.Year;
            if (edad > 150)
                throw new Exception("El usuario debe tener menos de 150 años.");
        }

        private void ValidarNombreUsuario(AddUser usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
                throw new Exception("El nombre de usuario es requerido");

            if (usuario.NombreUsuario.Length < Validations.MIN_LENGTH_USERNAME ||
                usuario.NombreUsuario.Length > Validations.MAX_LENGTH_USERNAME)
                throw new Exception($"El nombre de usuario debe tener entre {Validations.MIN_LENGTH_USERNAME} y {Validations.MAX_LENGTH_USERNAME} caracteres");

            // Validate that doesn't contain first or last name (case insensitive)
            if (usuario.NombreUsuario.ToLower().Contains(usuario.Nombre.ToLower()) ||
                usuario.NombreUsuario.ToLower().Contains(usuario.Apellido.ToLower()))
                throw new Exception("El nombre de usuario no puede contener el nombre o apellido del usuario");
        }

        private void ValidarPerfil(int perfil)
        {
            // Only allow to create salespeople and supervisors
            if (perfil != (int)PerfilUsuario.Vendedor && perfil != (int)PerfilUsuario.Supervisor)
                throw new Exception("Solo se pueden crear usuarios con perfil Vendedor o Supervisor");
        }

        public List<User> ObtenerActivos()
        {
            try
            {
                return _usuarioWS.BuscarUsuariosActivos()
                    .Select(u => {
                        u.Perfil = ((PerfilUsuario)u.Host).ToString();
                        return u;
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                // TODO: loggear error
                throw new Exception($"Error al obtener usuarios activos: {ex.Message}");
            }
        }

        public void CambiarContraseña(PatchUser user)
        {
            try
            {
                _usuarioWS.CambiarContraseña(user);
            }
            catch (Exception ex)
            {
                // TODO: Implementar logging
                throw new Exception($"Error al cambiar la contraseña del usuario: {ex.Message}", ex);
            }
        }

        public User BucarUsuarioPorId(string id)
        {
            try
            {
                return _usuarioWS.BucarUsuarioPorId(id);
            }
            catch (Exception ex)
            {
                // TODO: Implementar logging
                throw new Exception($"Error buscar usuario por ID: {ex.Message}", ex);
            }
        }

        public string Login(string username, string password)
        {
            try
            {
                return _usuarioWS.Login(username, password);
            }
            catch (Exception ex)
            {
                // TODO: Implementar logging
                throw new Exception($"Error al realizar el login: {ex.Message}", ex);
            }

        }
    }
}
