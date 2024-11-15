using System;

namespace ElectroHogar.Datos
{
    public enum CategoriaProducto
    {
        Audio = 1,
        Celulares = 2,
        ElectroHogar = 3,
        Informatica = 4,
        SmartTv = 5
    }

    public class ProveedorCategoria
    {
        public Guid IdProveedor { get; set; }
        public CategoriaProducto Categoria { get; set; }
    }

    public class ProveedorList
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Cuit { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
    public interface IProveedorBase
    {
        string Nombre { get; }
        string Apellido { get; }
        string Email { get; }
        string Cuit { get; }
    }

    public class AddProveedor : IProveedorBase
    {
        public Guid IdUsuario { get; set; }
        public string Nombre { get; set; } 
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Cuit { get; set; }
    }

    public class PatchProveedor : IProveedorBase
    {
        public Guid Id { get; set; }
        public Guid IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Cuit { get; set; }
    }
}