using System;
using ElectroHogar.Datos;

namespace ElectroHogar.Negocio.Utils
{
    public class LoginResult
    {
        public bool Exito { get; private set; }
        public string Mensaje { get; private set; }
        public LoginErrorTipo? TipoError { get; private set; }
        public PerfilUsuario? Perfil { get; private set; }

        private LoginResult() { }

        public static LoginResult Exitoso(PerfilUsuario perfil, User usuario)
        {
            return new LoginResult
            {
                Exito = true,
                Perfil = perfil,
                Mensaje = string.Empty
            };
        }

        public static LoginResult RequiereCambioContrase�a()
        {
            return new LoginResult
            {
                Exito = false,
                Mensaje = "Debe cambiar su contrase�a temporal",
                TipoError = LoginErrorTipo.RequiereCambioContrase�a
            };
        }

        public static LoginResult Error(string mensaje, LoginErrorTipo tipo)
        {
            return new LoginResult
            {
                Exito = false,
                Mensaje = mensaje,
                TipoError = tipo
            };
        }

        public static LoginResult ErrorUsuarioBloqueado()
        {
            return Error("Usuario bloqueado por exceder el m�ximo de intentos",
                LoginErrorTipo.UsuarioBloqueado);
        }
    }

    public enum LoginErrorTipo
    {
        CredencialesInvalidas,
        UsuarioBloqueado,
        ErrorWebService,
        ErrorServidor,
        RequiereCambioContrase�a
    }
}