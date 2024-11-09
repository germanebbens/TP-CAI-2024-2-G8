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

        public LoginResult Login(string usuario, string password)
        {
            try
            {
                // 1. Verificar si el usuario está bloqueado
                int intentos = ObtenerIntentosFallidos(usuario);
                if (intentos >= MAX_INTENTOS)
                {
                    return new LoginResult(
                        "Usuario bloqueado por exceso de intentos. Contacte al administrador.",
                        false,
                        LoginErrorTipo.UsuarioBloqueado
                    );
                }

                try
                {
                    // 2. Intentar autenticación con el WebService
                    _usuarioLogueadoId = _loginWS.login(usuario, password);

                    // 3. Si llegamos aquí, el login fue exitoso - reiniciamos intentos
                    ReiniciarIntentos(usuario);

                    // 4. Obtener perfil del usuario
                    return ObtenerPerfilUsuario();
                }
                catch (Exception ex) when (ex.Message.Contains("Error al momento del Login"))
                {
                    // 5. Login fallido - registrar intento
                    RegistrarIntentoFallido(usuario, intentos);
                    return new LoginResult(
                        "Usuario o contraseña incorrectos",
                        false,
                        LoginErrorTipo.CredencialesInvalidas
                    );
                }
            }
            catch (Exception ex)
            {
                return new LoginResult(
                    $"Error en el servidor: {ex.Message}",
                    false,
                    LoginErrorTipo.ErrorServidor
                );
            }
        }

        private int ObtenerIntentosFallidos(string usuario)
        {
            try
            {
                return _loginDB.obtenerIntentos(usuario);
            }
            catch (Exception)
            {
                throw new Exception("Error al verificar intentos de login");
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
                // Log del error pero continuamos
                // TODO: Implementar sistema de logging
            }
        }

        private void ReiniciarIntentos(string usuario)
        {
            try
            {
                _loginDB.actualizarIntento(usuario, "0");
            }
            catch
            {
                // Log del error pero continuamos
                // TODO: Implementar sistema de logging
            }
        }

        private LoginResult ObtenerPerfilUsuario()
        {
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
            return new LoginResult(
                "Error al obtener el perfil del usuario",
                false,
                LoginErrorTipo.ErrorWebService
            );
        }

        public string ObtenerUsuarioLogueadoId()
        {
            return _usuarioLogueadoId;
        }
    }
}