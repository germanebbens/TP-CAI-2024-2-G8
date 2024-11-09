using Persistencia;
using System;

namespace Negocio.Controllers
{
    public class LoginNegocio
    {
        private readonly LoginWS _loginWS;
        private readonly LoginDB _loginDB;
        private string _usuarioLogueadoId;

        private const int MAX_INTENTOS = 3;
        private const int MIN_LENGTH_USERNAME = 8;
        private const int MAX_LENGTH = 15;

        public LoginNegocio()
        {
            _loginWS = new LoginWS();
            _loginDB = new LoginDB();
        }

        public class LoginResult
        {
            public string Mensaje { get; set; }
            public bool Exito { get; set; }
            public LoginErrorTipo? TipoError { get; set; }

            public LoginResult(string mensaje, bool exito, LoginErrorTipo? tipoError = null)
            {
                Mensaje = mensaje;
                Exito = exito;
                TipoError = tipoError;
            }
        }

        public enum LoginErrorTipo
        {
            CredencialesInvalidas,
            UsuarioBloqueado,
            ErrorServidor,
            ErrorWebService,
            ErrorBaseDatos
        }

        private bool ValidarUsuario(string usuario)
        {
            if (string.IsNullOrEmpty(usuario)) return false;
            if (usuario.Length < MIN_LENGTH_USERNAME || usuario.Length > MAX_LENGTH) return false;
            return true;
        }

        private bool ValidarPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            if (password.Length > MAX_LENGTH) return false;
            return true;
        }

        public LoginResult Login(string usuario, string password)
        {
            try
            {
                if (!ValidarUsuario(usuario))
                    return new LoginResult("El usuario debe tener entre 8 y 15 caracteres", false, LoginErrorTipo.CredencialesInvalidas);

                if (!ValidarPassword(password))
                    return new LoginResult("La contraseña no cumple con los requisitos", false, LoginErrorTipo.CredencialesInvalidas);

                // 1. Verificar intentos
                int intentos;
                try
                {
                    intentos = _loginDB.obtenerIntentos(usuario);
                }
                catch (Exception)
                {
                    return new LoginResult("Error al verificar intentos de login", false, LoginErrorTipo.ErrorBaseDatos);
                }

                if (intentos >= MAX_INTENTOS)
                {
                    return new LoginResult("Usuario bloqueado por exceso de intentos", false, LoginErrorTipo.UsuarioBloqueado);
                }

                try
                {
                    // 2. Intentar login
                    _usuarioLogueadoId = _loginWS.login(usuario, password);

                    // 3. Login exitoso - reiniciar intentos
                    _loginDB.actualizarIntento(usuario, "0");

                    // 4. Obtener perfil
                    var usuarios = _loginWS.buscarDatosUsuario();
                    foreach (var usuarioActivo in usuarios)
                    {
                        if (usuarioActivo.Id.Equals(_usuarioLogueadoId))
                        {
                            string perfil;
                            if (usuarioActivo.Perfil == 3)
                                perfil = "Administrador";
                            else if (usuarioActivo.Perfil == 2)
                                perfil = "Supervisor";
                            else
                                perfil = "Vendedor";

                            return new LoginResult(perfil, true);
                        }
                    }

                    return new LoginResult("Error al obtener el perfil", false, LoginErrorTipo.ErrorWebService);
                }
                catch (Exception ex) when (ex.Message.Contains("Error al momento del Login"))
                {
                    RegistrarIntentoFallido(usuario, intentos);
                    return new LoginResult("Usuario o contraseña incorrectos", false, LoginErrorTipo.CredencialesInvalidas);
                }
            }
            catch (Exception ex)
            {
                return new LoginResult($"Error en el servidor: {ex.Message}", false, LoginErrorTipo.ErrorServidor);
            }
        }

        private void RegistrarIntentoFallido(string usuario, int intentosActuales)
        {
            try
            {
                if (intentosActuales == 0)
                    _loginDB.guardarIntento(usuario);
                else
                    _loginDB.actualizarIntento(usuario, (intentosActuales + 1).ToString());
            }
            catch
            {
                // Solo logueamos el error, no queremos que falle el login si falla el registro de intentos
            }
        }

        public string ObtenerUsuarioLogueadoId()
        {
            return _usuarioLogueadoId;
        }
    }
}