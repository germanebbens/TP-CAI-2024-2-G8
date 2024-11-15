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
        private readonly ClavesTemporalesDB _clavesTemporalesDB;
        private readonly Usuarios _usuarioService;
        private readonly LoginDB _loginDB;
        public string _usuarioLogueadoId;
        private readonly int _maxIntentos;

        private LoginNegocio()  // private constructor!
        {
            _usuarioService = new Usuarios();
            _loginDB = new LoginDB();
            _clavesTemporalesDB = new ClavesTemporalesDB();
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
            }

            try
            {
                var (claveTemporal, userId) = _clavesTemporalesDB.ObtenerClaveTemporal(usuario);

                if (!string.IsNullOrEmpty(claveTemporal) && claveTemporal == password && !string.IsNullOrEmpty(userId))
                {
                    return LoginResult.RequiereCambioContraseña();
                }

                _usuarioLogueadoId = _usuarioService.Login(usuario, password);
                ReiniciarIntentos(usuario);
                return ObtenerPerfilUsuario();
            }
            catch (Exception ex) when (ex.Message.Contains("incorrecto"))
            {
                RegistrarIntentoFallido(usuario, intentos);
                return LoginResult.Error(ex.Message, LoginErrorTipo.CredencialesInvalidas);
            }
        }

        private LoginResult ObtenerPerfilUsuario()
        {
            User usuarioActivo = _usuarioService.BucarUsuarioPorId(_usuarioLogueadoId);

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
        
        public string ObtenerUsuarioLogueadoId() => _usuarioLogueadoId;

        public bool CambiarContraseña(string username, string passwordActual, string passwordNueva)
        {
            try
            {
                var patchUser = new PatchUser
                {
                    NombreUsuario = username,
                    Contraseña = passwordActual,
                    ContraseñaNueva = passwordNueva
                };

                var (claveTemporal, userId) = _clavesTemporalesDB.ObtenerClaveTemporal(username);

                _usuarioService.ActivarUsuario(username: username);
                _usuarioService.CambiarContraseña(patchUser);

                return true;
            }
            catch (Exception ex)
            {
                _usuarioService.DarBajaUsuario(username: username);
                throw new Exception($"Error al cambiar la contraseña: {ex.Message}");
            }
        }
    }
}