using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroHogar.Presentacion.Utils
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Perfil { get; set; }
    }
}
