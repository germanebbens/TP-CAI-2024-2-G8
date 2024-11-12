using ElectroHogar.Presentacion.Utils;
using System;
using System.Linq;

namespace ElectroHogar.Negocio.Utils
{
    public class UsuariosUtils
    {
        public string GenerarContraseniaTemporal()
        {
            // alowed characters
            const string mayusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string minusculas = "abcdefghijklmnopqrstuvwxyz";
            const string numeros = "0123456789";

            var random = new Random();
            int longitud = 15;

            // ensure that it has at least one capital letter and one number
            var password = new char[longitud];
            password[0] = mayusculas[random.Next(mayusculas.Length)];
            password[1] = numeros[random.Next(numeros.Length)];

            // remaining random characters
            string caracteresPermitidos = mayusculas + minusculas + numeros;
            for (int i = 2; i < longitud; i++)
            {
                password[i] = caracteresPermitidos[random.Next(caracteresPermitidos.Length)];
            }

            // mix characters
            for (int i = 0; i < password.Length; i++)
            {
                int j = random.Next(i, password.Length);
                var temp = password[i];
                password[i] = password[j];
                password[j] = temp;
            }

            return new string(password);
        }

        public static (bool isValid, string message) ValidarCambioContraseña(string contraseñaActual, string contraseñaNueva)
        {
            // Validar que la nueva contraseña cumpla con todos los requisitos
            var (isValid, message) = Validations.ValidarPassword(contraseñaNueva);
            if (!isValid)
                return (false, message);

            // Validar que la nueva contraseña sea diferente a la actual
            if (contraseñaActual == contraseñaNueva)
                return (false, "La nueva contraseña debe ser diferente a la actual");

            return (true, string.Empty);
        }

        public static (bool isValid, string message) ValidarExpiracionContraseña(DateTime fechaAlta)
        {
            var diasTranscurridos = (DateTime.Today - fechaAlta).TotalDays;
            return (diasTranscurridos >= 30,
                diasTranscurridos >= 30 ? string.Empty : "La contraseña aún no ha expirado");
        }
    }
}
