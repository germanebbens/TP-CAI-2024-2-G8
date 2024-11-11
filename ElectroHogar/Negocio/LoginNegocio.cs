using System;
using ElectroHogar.Persistencia;
using ElectroHogar.Negocio.Utils;
using ElectroHogar.Config;
using ElectroHogar.Datos;

namespace ElectroHogar.Negocio
{
    public class LoginNegocio
    {
        //this class is a singleton pattern! it will be instantiated only once during the session
        private static LoginNegocio _instance;
        private static readonly object _lock = new object();

        private readonly UsuariosWS _usuarioWS;
        private readonly LoginDB _loginDB;
        private string _usuarioLogueadoId;
        private readonly int _maxIntentos;

        private LoginNegocio()  // private constructor!
        {
            _usuarioWS = new UsuariosWS();
            _loginDB = new LoginDB();
            _maxIntentos = ConfigHelper.GetIntValueOrDefault("MaxIntentosLogin", 3);
        }

        public static LoginNegocio Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LoginNegocio();
                        }
                    }
                }
                return _instance;
            }
        }

        public static void Reset() // when the session is closed - logout
        {
            lock (_lock)
            {
                _instance = null;
            }
        }

        public LoginResult Login(string usuario, string password)
        {
            try
            {
                return RealizarLogin(usuario, password);
            }
            catch (Exception ex)
            {
                return LoginResult.Error($"Error en el servidor: {ex.Message}", LoginErrorTipo.ErrorServidor);
            }
        }

        private LoginResult RealizarLogin(string usuario, string password)
        {
            int intentos = ObtenerIntentosFallidos(usuario);
            if (intentos >= _maxIntentos)
            {
                return LoginResult.ErrorUsuarioBloqueado();
                // TODO: llamar al WS y poner usuario inactivo!
            }

            try
            {
                _usuarioLogueadoId = _usuarioWS.Login(usuario, password);
                ReiniciarIntentos(usuario);
                return ObtenerPerfilUsuario();
            }
            catch (Exception ex) when (ex.Message.Contains("incorrecto"))
            {
                RegistrarIntentoFallido(usuario, intentos);
                return LoginResult.Error(ex.Message, LoginErrorTipo.CredencialesInvalidas);
            }
        }

        private int ObtenerIntentosFallidos(string usuario)
        {
            try
            {
                return _loginDB.ObtenerIntentos(usuario);
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
                var nuevoValor = (intentosActuales + 1).ToString();

                if (intentosActuales == 0)
                    _loginDB.GuardarIntento(usuario);
                else
                    _loginDB.ActualizarIntento(usuario, nuevoValor);
            }
            catch (Exception ex)
            {
                // TODO: Implementar logging
                Console.WriteLine($"Error al registrar intento fallido: {ex.Message}");
            }
        }

        private void ReiniciarIntentos(string usuario)
        {
            try
            {
                _loginDB.ActualizarIntento(usuario, "0");
            }
            catch (Exception ex)
            {
                // TODO: Implementar logging
                Console.WriteLine($"Error al reiniciar intentos: {ex.Message}");
            }
        }

        private LoginResult ObtenerPerfilUsuario()
        {
            User usuarioActivo = _usuarioWS.BucarUsuarioPorId(_usuarioLogueadoId);

            if (usuarioActivo == null)
            {
                return LoginResult.Error(
                    "Error al obtener el perfil del usuario",
                    LoginErrorTipo.ErrorWebService
                );
            }

            // Checks if the profile (or 'host') is a valid value of PerfilUsuario
            if (Enum.IsDefined(typeof(PerfilUsuario), usuarioActivo.Host))
            {
                var perfil = (PerfilUsuario)usuarioActivo.Host;
                return LoginResult.Exitoso(perfil, usuarioActivo);
            }
            else
            {
                return LoginResult.Error(
                    $"Perfil no reconocido: {usuarioActivo.Host}",
                    LoginErrorTipo.ErrorWebService
                );
            }
        }

        public string ObtenerUsuarioLogueadoId() => _usuarioLogueadoId;
    }
}