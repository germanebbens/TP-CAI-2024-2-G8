using System;
using ElectroHogar.Persistencia;
using ElectroHogar.Negocio.Utils;
using ElectroHogar.Config;

namespace ElectroHogar.Negocio
{
    public class LoginNegocio
    {
        private readonly LoginWS _loginWS;
        private readonly LoginDB _loginDB;
        private string _usuarioLogueadoId;
        private readonly int _maxIntentos;

        public LoginNegocio()
        {
            _loginWS = new LoginWS();
            _loginDB = new LoginDB();
            _maxIntentos = ConfigHelper.GetIntValueOrDefault("MaxIntentosLogin", 3);
        }

        public LoginResult Login(string usuario, string password)
        {
            try
            {
                return RealizarLogin(usuario, password);
            }
            catch (Exception ex)
            {
                // Aquí podríamos agregar logging
                return LoginResult.Error($"Error en el servidor: {ex.Message}", LoginErrorTipo.ErrorServidor);
            }
        }

        private LoginResult RealizarLogin(string usuario, string password)
        {
            // 1. Verificar si el usuario está bloqueado
            int intentos = ObtenerIntentosFallidos(usuario);
            if (intentos >= _maxIntentos)
            {
                return LoginResult.ErrorUsuarioBloqueado();
                // TODO: llamar al WS y poner usuario inactivo!
            }

            try
            {
                // 2. Intentar autenticación
                _usuarioLogueadoId = _loginWS.login(usuario, password);

                // 3. Login exitoso - reiniciar intentos
                ReiniciarIntentos(usuario);

                // 4. Obtener y retornar perfil
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
                var nuevoValor = (intentosActuales + 1).ToString();

                if (intentosActuales == 0)
                    _loginDB.guardarIntento(usuario);
                else
                    _loginDB.actualizarIntento(usuario, nuevoValor);
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
                _loginDB.actualizarIntento(usuario, "0");
            }
            catch (Exception ex)
            {
                // TODO: Implementar logging
                Console.WriteLine($"Error al reiniciar intentos: {ex.Message}");
            }
        }

        private LoginResult ObtenerPerfilUsuario()
        {
            var usuarios = _loginWS.buscarDatosUsuario();
            var usuarioActivo = usuarios.Find(u => u.Id.ToString() == _usuarioLogueadoId);

            if (usuarioActivo == null)
            {
                return LoginResult.Error(
                    "Error al obtener el perfil del usuario",
                    LoginErrorTipo.ErrorWebService
                );
            }

            // Verifica si el perfil es un valor válido de TipoPerfil
            if (Enum.IsDefined(typeof(TipoPerfil), usuarioActivo.Host))
            {
                var perfil = (TipoPerfil)usuarioActivo.Host;
                return LoginResult.Exitoso(perfil);
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