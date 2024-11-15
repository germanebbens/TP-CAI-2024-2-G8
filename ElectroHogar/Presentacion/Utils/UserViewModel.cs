using System;
using System.Collections.Generic;

namespace ElectroHogar.Presentacion.Utils
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Perfil { get; set; }
    }

    public class ProveedorViewModel
    {
        public Guid Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Cuit { get; set; }
        public List<string> Categorias { get; set; }
        public string CategoriasDisplay => string.Join(", ", Categorias);
    }

}
