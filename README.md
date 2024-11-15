# Trabajo Práctico - Construcción de Aplicaciones Informáticas
## Facultad de Ciencias Económicas - Universidad de Buenos Aires

### Información General

**Asignatura**: Construcción de Aplicaciones Informáticas  
**Cuatrimestre**: 2do Cuatrimestre de 2024

### Contexto del Proyecto
La empresa EletroHogar SA, dedicada a la compra-venta de artículos del hogar, ha lanzado una licitación para desarrollar un software que permita administrar su stock y operaciones. La empresa de software ha sido la adjudicataria de este proyecto.  
Actualmente, el proceso de gestión es burocrático y parcialmente manual, con dificultades para llevar un adecuado control del stock, las ventas y la información de clientes y proveedores.

### Diagramas

#### Caso de uso Login
![Alt text](caso_uso_login.png?raw=true "Caso de uso Login")

#### Diagrama de clases
![Alt text](diagrama_clases.png?raw=true "Diagrama de clases")

#### Diagrama de secuencia
![Alt text](diagrama_secuencia.png?raw=true "Diagrama de secuencia")

### Requerimientos Funcionales
El proyecto busca automatizar y modernizar los procesos clave de la empresa, incluyendo:

#### Usuarios:

- Registro y gestión de usuarios con perfiles de Administrador, Supervisor y Vendedor.
- Control de intentos de inicio de sesión y bloqueo de usuarios.
- Manejo de contraseñas con políticas de seguridad.

#### Proveedores:

- Registro y gestión de proveedores.
- Categorización de productos por proveedor.

#### Productos:

- Registro y gestión de productos.
- Alertas de stock crítico para supervisores y administradores.

#### Ventas:

- Registro de ventas con aplicación de descuentos.
- Devolución de ventas.
- Generación de remitos valorizados.

#### Clientes:

- Registro y modificación de clientes.

#### Reportes:

- Reporte de productos con stock crítico.
- Reporte de ventas por vendedor.
- Reporte de productos más vendidos por categoría.

### Tecnologías Utilizadas

- **Framework**: .NET Framework 4.8
- **Lenguaje de Programación**: C#
- **Base de Datos**: Archivos JSON
- **Controles**: Windows Forms

### Estructura del Proyecto
El proyecto se divide en las siguientes capas:

- **Datos**: Clases que representan las entidades del sistema (Usuario, Proveedor, Producto, Venta, Cliente), en este nivel es donde iría la interacción con la base de datos.
- **Negocio**: Lógica de negocio para la gestión de usuarios, proveedores, productos, ventas y clientes.
- **Persistencia**: Clases que manejan la interacción con la base de datos (archivos JSON), y con el webservice (WS).
- **Presentación**: Formularios y controles de la interfaz de usuario.

Las distintas capas tienen clases y métodos de soporte, como validaciones y helpers de formularios.

### Instrucciones de Ejecución

1. Asegúrate de tener instalado el .NET Framework 4.8 o superior en tu sistema.
2. Clona el repositorio del proyecto.
3. Abre la solución en Visual Studio.
4. Compila y ejecuta el proyecto.
