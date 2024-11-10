using System;
using System.Linq;
using System.Text.RegularExpressions;
using ElectroHogar.Datos;
using ElectroHogar.Negocio.Utils;
using ElectroHogar.Persistencia;

namespace ElectroHogar.Negocio
{
    internal class NuevoUsuario
    {
        private readonly UsuariosWS _nuevoUsuarioService;
        private readonly ClavesTemporalesDB _clavesTemporalesDB;
        private readonly LoginDB _loginDB;

        public NuevoUsuario()
        {
            _nuevoUsuarioService = new UsuariosWS();
            _clavesTemporalesDB = new ClavesTemporalesDB();
            _loginDB = new LoginDB();
        }

        public void RegistrarNuevoUsuario(AddUsuario usuario)
        {
            ValidarUsuario(usuario);

            var claveTemporal = GenerarContraseniaTemporal();
            usuario.Contraseña = claveTemporal;

            _nuevoUsuarioService.AgregarUsuario(usuario);

            // Guardar la clave temporal en la base de datos local
            _clavesTemporalesDB.GuardarClaveTemporal(usuario.NombreUsuario, claveTemporal);

            // Cambiar estado a "INACTIVO" hasta el primer login
            InactivarUsuario(usuario.NombreUsuario);
        }

        private void ValidarUsuario(AddUsuario usuario)
        {
            // Validar que el nombre de usuario cumpla con las reglas
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new Exception("El nombre del usuario es requerido");

            if (string.IsNullOrWhiteSpace(usuario.Apellido))
                throw new Exception("El apellido del usuario es requerido");

            if (usuario.Dni <= 0)
                throw new Exception("El DNI del usuario no es válido");

            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
                throw new Exception("El nombre de usuario es requerido");

            if (usuario.NombreUsuario.Length < 8 || usuario.NombreUsuario.Length > 15)
                throw new Exception("El nombre de usuario debe tener entre 8 y 15 caracteres");

            if (usuario.NombreUsuario.Contains(usuario.Nombre) || usuario.NombreUsuario.Contains(usuario.Apellido))
                throw new Exception("El nombre de usuario no puede contener el nombre o apellido del usuario");

            if (string.IsNullOrWhiteSpace(usuario.Contraseña))
                throw new Exception("La contraseña es requerida");

            if (usuario.Contraseña.Length < 8 || usuario.Contraseña.Length > 15)
                throw new Exception("La contraseña debe tener entre 8 y 15 caracteres");

            if (!Regex.IsMatch(usuario.Contraseña, @"^(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,15}$"))
                throw new Exception("La contraseña debe contener al menos una letra mayúscula y un número");
        }

        private string GenerarContraseniaTemporal()
        {
            return "CAI20232";
        }

        private void InactivarUsuario(string nombreUsuario)
        {
            // Cambiar el estado del usuario en la base de datos local
        }
    }
}
