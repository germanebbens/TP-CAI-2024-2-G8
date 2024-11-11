using System;
using ElectroHogar.Datos;

namespace ElectroHogar.Negocio.Utils
{
    public enum LoginErrorTipo
    {
        CredencialesInvalidas,
        UsuarioBloqueado,
        ErrorServidor,
        ErrorWebService,
        ErrorBaseDatos
    }

    public class LoginResult
    {
        public string Mensaje { get; }
        public bool Exito { get; }
        public User Usuario { get; }
        public LoginErrorTipo? TipoError { get; }
        public PerfilUsuario? Perfil { get; }

        private LoginResult(string mensaje, bool exito, User usuarioActivo = null, PerfilUsuario? perfil = null, LoginErrorTipo? tipoError = null)
        {
            Mensaje = mensaje;
            Exito = exito;
            Perfil = perfil;
            TipoError = tipoError;
            Usuario = usuarioActivo;
        }

        public static LoginResult Exitoso(PerfilUsuario perfil, User usuarioActivo) =>
            new LoginResult(perfil.ToString(), true, usuarioActivo, perfil);

        public static LoginResult Error(string mensaje, LoginErrorTipo tipo) =>
            new LoginResult(mensaje, false, tipoError: tipo);

        public static LoginResult ErrorUsuarioBloqueado() =>
            Error("Usuario bloqueado por exceso de intentos.", LoginErrorTipo.UsuarioBloqueado);
    }
}