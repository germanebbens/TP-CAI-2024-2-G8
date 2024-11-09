using System;
using System.Text.RegularExpressions;

namespace Presentacion.Utils
{
    public static class Validations
    {
        // Constantes de validación
        public const int MIN_LENGTH_USERNAME = 8;
        public const int MAX_LENGTH_USERNAME = 15;
        public const int MIN_LENGTH_PASSWORD = 8;
        public const int MAX_LENGTH_PASSWORD = 15;
        public const int MAX_INTENTOS_LOGIN = 3;

        public static class ValidationMessages
        {
            public const string USUARIO_REQUERIDO = "El usuario es requerido";
            public const string PASSWORD_REQUERIDO = "La contraseña es requerida";
            public const string USUARIO_LENGTH = "El usuario debe tener entre 8 y 15 caracteres";
            public const string PASSWORD_LENGTH = "La contraseña debe tener entre 8 y 15 caracteres";
            public const string USUARIO_INVALIDO = "El usuario no puede contener caracteres especiales";
            public const string EMAIL_INVALIDO = "El email no tiene un formato válido";
        }

        public static (bool isValid, string message) ValidarUsuario(string usuario)
        {
            if (string.IsNullOrEmpty(usuario?.Trim()))
                return (false, ValidationMessages.USUARIO_REQUERIDO);

            if (usuario.Length < MIN_LENGTH_USERNAME || usuario.Length > MAX_LENGTH_USERNAME)
                return (false, ValidationMessages.USUARIO_LENGTH);

            // Validar que solo contenga letras, números y guiones
            if (!Regex.IsMatch(usuario, @"^[a-zA-Z0-9_-]+$"))
                return (false, ValidationMessages.USUARIO_INVALIDO);

            return (true, string.Empty);
        }

        public static (bool isValid, string message) ValidarPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return (false, ValidationMessages.PASSWORD_REQUERIDO);

            if (password.Length < MIN_LENGTH_PASSWORD || password.Length > MAX_LENGTH_PASSWORD)
                return (false, ValidationMessages.PASSWORD_LENGTH);

            return (true, string.Empty);
        }

        public static (bool isValid, string message) ValidarEmail(string email)
        {
            if (string.IsNullOrEmpty(email?.Trim()))
                return (false, "El email es requerido");

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return (addr.Address == email, string.Empty);
            }
            catch
            {
                return (false, ValidationMessages.EMAIL_INVALIDO);
            }
        }

        public static (bool isValid, string message) ValidarTelefono(string telefono)
        {
            if (string.IsNullOrEmpty(telefono?.Trim()))
                return (false, "El teléfono es requerido");

            // Permite números, guiones y espacios
            if (!Regex.IsMatch(telefono, @"^[\d\s-]+$"))
                return (false, "El teléfono solo puede contener números, espacios y guiones");

            return (true, string.Empty);
        }

        public static (bool isValid, string message) ValidarFecha(DateTime fecha)
        {
            if (fecha > DateTime.Now)
                return (false, "La fecha no puede ser futura");

            if (fecha.Year < 1900)
                return (false, "La fecha no es válida");

            return (true, string.Empty);
        }
    }
}