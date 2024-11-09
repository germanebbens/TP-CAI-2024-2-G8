using Datos;
using Persistencia;
using Persistencia.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class LoginNegocio
    {
        public String login(String usuario, String password) {
            String perfilLogin = "";

            // Paso 1: Registrar intentos
            LoginDB loginDB = new LoginDB();
            int cantidadIntentos = loginDB.obtenerIntentos(usuario);

            if (cantidadIntentos == 0)
            {
                loginDB.guardarIntento(usuario);
            }
            else if(cantidadIntentos <= 3)
            {
                int valorActualizado = cantidadIntentos + 1;
                loginDB.actualizarIntento(usuario, valorActualizado.ToString());
            }
            else
            {
                // circuito de bloqueado de usuario
            }

            // Paso 2: Llamar login
            LoginWS loginWS = new LoginWS();   
            String idUsuario = loginWS.login(usuario, password);

            // Paso 2.1: Verificar primer login
            verificarPrimerLogin(usuario);

            // Paso 2.2: Credenciales invalidas
           
            // Paso 2.3: Credenciales validas

            // Pase 3: Obtener perfil del usuario logueado
            List<UsuarioWS> usuariosActivos = loginWS.buscarDatosUsuario();

            // Paseo 3.1: Verificar que el usuario este activo

            // Paso 3.2: Buscar el id en el listado de Usuarios activos
            int perfilUsuarioLogueado = 0;

            foreach(UsuarioWS usuarioActivo in usuariosActivos)
            {
                if (usuarioActivo.Id.Equals(idUsuario))
                {
                    perfilUsuarioLogueado = usuarioActivo.Perfil;
                }
            }

            // Paso 4: Mandar al formulario que corresponde
            if(perfilUsuarioLogueado == 3)
            {
                perfilLogin = "Administrador";
            } else if (perfilUsuarioLogueado == 2)
            {
                perfilLogin = "Supervisor";
            }
            else
            {
                perfilLogin = "Vendedor";
            }

            return perfilLogin;
        }

        private void verificarPrimerLogin(string usuario)
        {
            // A desarrollar por X
        }
    }
}
