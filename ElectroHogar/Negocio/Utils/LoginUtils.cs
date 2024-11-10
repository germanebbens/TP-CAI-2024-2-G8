using System;

namespace ElectroHogar.Negocio.Utils
{
    public enum TipoPerfil
    {
        Vendedor = 1,
        Supervisor = 2,
        Administrador = 3
    }

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
        public LoginErrorTipo? TipoError { get; }
        public TipoPerfil? Perfil { get; }

        private LoginResult(string mensaje, bool exito, TipoPerfil? perfil = null, LoginErrorTipo? tipoError = null)
        {
            Mensaje = mensaje;
            Exito = exito;
            Perfil = perfil;
            TipoError = tipoError;
        }

        public static LoginResult Exitoso(TipoPerfil perfil) =>
            new LoginResult(perfil.ToString(), true, perfil);

        public static LoginResult Error(string mensaje, LoginErrorTipo tipo) =>
            new LoginResult(mensaje, false, tipoError: tipo);

        public static LoginResult ErrorUsuarioBloqueado() =>
            Error("Usuario bloqueado por exceso de intentos. Contacte al administrador.", LoginErrorTipo.UsuarioBloqueado);
    }
}