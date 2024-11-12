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

        public static LoginResult RequiereCambioContraseña()
        {
            return new LoginResult
            {
                Exito = false,
                Mensaje = "Debe cambiar su contraseña temporal",
                TipoError = LoginErrorTipo.RequiereCambioContraseña
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
            return Error("Usuario bloqueado por exceder el máximo de intentos",
                LoginErrorTipo.UsuarioBloqueado);
        }
    }

    public enum LoginErrorTipo
    {
        CredencialesInvalidas,
        UsuarioBloqueado,
        ErrorWebService,
        ErrorServidor,
        RequiereCambioContraseña
    }
}