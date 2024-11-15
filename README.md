# Trabajo Pr�ctico - Construcci�n de Aplicaciones Inform�ticas
## Facultad de Ciencias Econ�micas - Universidad de Buenos Aires
### Informaci�n General

Asignatura: Construcci�n de Aplicaciones Inform�ticas
Cuatrimestre: 2do Cuatrimestre de 2024

### Contexto del Proyecto
La empresa EletroHogar SA, dedicada a la compra-venta de art�culos del hogar, ha lanzado una licitaci�n para desarrollar un software que permita administrar su stock y operaciones. La empresa de software ha sido la adjudicataria de este proyecto.
Actualmente, el proceso de gesti�n es burocr�tico y parcialmente manual, con dificultades para llevar un adecuado control del stock, las ventas y la informaci�n de clientes y proveedores.

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

- Registro y gesti�n de usuarios con perfiles de Administrador, Supervisor y Vendedor.
- Control de intentos de inicio de sesi�n y bloqueo de usuarios.
- Manejo de contrase�as con pol�ticas de seguridad.


#### Proveedores:

- Registro y gesti�n de proveedores.
- Categorizaci�n de productos por proveedor.


#### Productos:

- Registro y gesti�n de productos.
- Alertas de stock cr�tico para supervisores y administradores.


#### Ventas:

- Registro de ventas con aplicaci�n de descuentos.
- Devoluci�n de ventas.
- Generaci�n de remitos valorizados.


#### Clientes:

- Registro y modificaci�n de clientes.


#### Reportes:

- Reporte de productos con stock cr�tico.
- Reporte de ventas por vendedor.
- Reporte de productos m�s vendidos por categor�a.



## Tecnolog�as Utilizadas

- Framework: .NET Framework 4.8
- Lenguaje de Programaci�n: C#
- Base de Datos: Archivos JSON
- Controles de Windows Forms

### Estructura del Proyecto
El proyecto se divide en las siguientes capas:

- Datos: Clases que representan las entidades del sistema (Usuario, Proveedor, Producto, Venta, Cliente), en este nivel es donde ir�a la interacci�n con la base de datos.
- Negocio: L�gica de negocio para la gesti�n de usuarios, proveedores, productos, ventas y clientes.
- Persistencia: Clases que manejan la interacci�n con la base de datos (archivos JSON), y con el webservice (WS).
- Presentaci�n: Formularios y controles de la interfaz de usuario.

Las distintas capas tienen clases y m�todos de soporte, como validaciones y helpers de formularios.

### Instrucciones de Ejecuci�n

1. Aseg�rate de tener instalado el .NET Framework 4.8 o superior en tu sistema.
2. Clona el repositorio del proyecto.
3. Abre la soluci�n en Visual Studio.
4. Compila y ejecuta el proyecto.
