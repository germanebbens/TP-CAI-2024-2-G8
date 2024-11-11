using System;

namespace ElectroHogar.Negocio.Utils
{
    public class UserUtils
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
    }
}
