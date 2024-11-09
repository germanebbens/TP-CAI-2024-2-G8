using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class UsuarioWS
    {
        Guid _id;
        String _nombre;
        String _apellido;
        int _dni;
        String _nombreUsuario;
        int _perfil;

        public UsuarioWS(Guid id, string nombre, string apellido, int dni, string nombreUsuario, int perfil)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Dni = dni;
            NombreUsuario = nombreUsuario;
            Perfil = perfil;
        }

        public Guid Id { get => _id; set => _id = value; }
        public string Nombre { get => _nombre; set => _nombre = value; }
        public string Apellido { get => _apellido; set => _apellido = value; }
        public int Dni { get => _dni; set => _dni = value; }
        public string NombreUsuario { get => _nombreUsuario; set => _nombreUsuario = value; }
        public int Perfil { get => _perfil; set => _perfil = value; }
    }
}
