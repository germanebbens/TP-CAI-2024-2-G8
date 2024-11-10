using System;

namespace ElectroHogar.Datos
{
    public enum PerfilUsuario
    {
        Vendedor = 1,
        Supervisor = 2,
        Administrador = 3
    }
    public class UsuarioList
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Dni { get; set; }
        public string NombreUsuario { get; set; }
        public int Host { get; set; }
    }
    public class AddUsuario
    {
        public Guid IdUsuario { get; set; }
        public int Host { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Dni { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; }
    }
    public class PatchUsuario
    {
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; }
        public string ContraseñaNueva { get; set; }
    }
    public class Permisos
    {
        public Guid Id { get; set; }
        public Guid IdUsuario { get; set; }
    }
}
