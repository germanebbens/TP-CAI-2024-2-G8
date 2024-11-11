using System;
using System.Drawing;
using System.Windows.Forms;
using ElectroHogar.Config;
using ElectroHogar.Datos;
using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class UserManagerForm : Form
    {
        private readonly NuevoUsuario _usuarioService;
        private readonly Label lblEstado;
        private readonly Panel panelBusqueda;
        private readonly Panel panelResultado;
        private readonly Panel panelAltaUsuario;
        private readonly TextBox txtBusqueda;
        private readonly Label lblResultado;
        private bool acordeonAbierto = false;

        public UserManagerForm()
        {
            InitializeComponent();
            _usuarioService = new NuevoUsuario();
            lblEstado = FormHelper.CrearLabelEstado();

            // Inicializar paneles
            panelBusqueda = FormHelper.CrearPanelBusqueda(
                "Buscar por Nombre de Usuario:",
                (s, e) => RealizarBusqueda(),
                out txtBusqueda
            );

            panelResultado = FormHelper.CrearPanelResultadoBusqueda(
                (s, e) => DeshabilitarUsuario(),
                out lblResultado
            );

            panelAltaUsuario = CrearPanelAltaUsuario();

            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Gestión de Usuarios";
            this.ClientSize = new Size(FormHelper.ANCHO_FORM, 600);
            this.AutoScroll = true;

            var panelSuperior = FormHelper.CrearPanelSuperior("Gestión de Usuarios");
            this.Controls.Add(panelSuperior);

            var btnVolver = FormHelper.CrearBotonPrimario("Volver", 100);
            btnVolver.Location = new Point(50, panelSuperior.Bottom + 20);
            btnVolver.Click += (s, e) => {
                this.Close();
                new HomeForm(PerfilUsuario.Administrador, "admin").Show();
            };

            panelBusqueda.Location = new Point(0, btnVolver.Bottom + 20);
            panelResultado.Location = new Point(0, panelBusqueda.Bottom);
            lblEstado.Location = new Point(50, panelResultado.Bottom + 20);
            panelAltaUsuario.Location = new Point(0, lblEstado.Bottom + 20);

            this.Controls.AddRange(new Control[] {
                btnVolver,
                panelBusqueda,
                panelResultado,
                lblEstado,
                panelAltaUsuario
            });
        }
        
        private void LimpiarBusqueda()
        {
            // Limpiar controles de búsqueda
            txtBusqueda.Text = string.Empty;
            panelResultado.Visible = false;
            FormHelper.MostrarEstado(lblEstado, "", false);
        }

        private Panel CrearPanelAltaUsuario()
        {
            var panel = new Panel
            {
                Width = FormHelper.ANCHO_FORM,
                AutoSize = true,
                Padding = new Padding(FormHelper.MARGEN)
            };

            // Header del acordeón
            var btnAcordeon = FormHelper.CrearBotonPrimario("+ Agregar Nuevo Usuario", FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2));
            btnAcordeon.Location = new Point(FormHelper.MARGEN, 10);
            btnAcordeon.Click += (s, e) => ToggleAcordeon();

            // Contenido del acordeón (inicialmente oculto)
            var panelContenido = new Panel
            {
                Width = FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2),
                Location = new Point(FormHelper.MARGEN, btnAcordeon.Bottom + 10),
                AutoSize = true,
                Visible = false
            };

            int currentY = 10;

            var txtNombre = FormHelper.CrearCampoTexto("Nombre:", "txtNombre", ref currentY, panelContenido);
            var txtApellido = FormHelper.CrearCampoTexto("Apellido:", "txtApellido", ref currentY, panelContenido);
            var txtDNI = FormHelper.CrearCampoTexto("DNI:", "txtDNI", ref currentY, panelContenido);
            var txtDireccion = FormHelper.CrearCampoTexto("Dirección:", "txtDireccion", ref currentY, panelContenido);
            var txtTelefono = FormHelper.CrearCampoTexto("Teléfono:", "txtTelefono", ref currentY, panelContenido);
            var txtEmail = FormHelper.CrearCampoTexto("Email:", "txtEmail", ref currentY, panelContenido);
            var dtpFechaNacimiento = FormHelper.CrearCampoFecha("Fecha de Nacimiento:", "dtpFechaNacimiento", ref currentY, panelContenido);
            var txtUsername = FormHelper.CrearCampoTexto("Nombre de Usuario:", "txtUsername", ref currentY, panelContenido);

            // Combo de perfiles con nombre asignado
            var cmbPerfil = FormHelper.CrearComboPerfil("Perfil:", "cmbPerfil", ref currentY, panelContenido);

            // Botón guardar
            var btnGuardar = FormHelper.CrearBotonPrimario("Guardar Usuario", FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 3));
            btnGuardar.Location = new Point(FormHelper.MARGEN, currentY + 20);
            btnGuardar.Click += (s, e) => GuardarUsuario();
            panelContenido.Controls.Add(btnGuardar);

            panel.Controls.AddRange(new Control[] {
                btnAcordeon,
                panelContenido
            });

            return panel;
        }

        private void ToggleAcordeon()
        {
            acordeonAbierto = !acordeonAbierto;
            panelAltaUsuario.Controls[1].Visible = acordeonAbierto;  // panelContenido
            var btnAcordeon = (Button)panelAltaUsuario.Controls[0];
            btnAcordeon.Text = acordeonAbierto ? "- Cerrar" : "+ Agregar Nuevo Usuario";
        }

        private void RealizarBusqueda()
        {
            try
            {
                var nombreUsuario = txtBusqueda.Text.Trim();
                if (string.IsNullOrEmpty(nombreUsuario))
                {
                    FormHelper.MostrarEstado(lblEstado, "Ingrese un nombre de usuario para buscar", true);
                    return;
                }

                User usuario = _usuarioService.BuscarUsuarioPorUsername(nombreUsuario);

                if (usuario != null)
                {
                    panelResultado.Visible = true;
                    var lblResultado = (Label)panelResultado.Controls[0];
                    lblResultado.Text = $"Usuario: {usuario.NombreUsuario}\nPerfil: {(PerfilUsuario)usuario.Host}";

                    var btnDeshabilitar = (Button)panelResultado.Controls[1];
                    btnDeshabilitar.Visible = true;
                    btnDeshabilitar.Tag = usuario.Id.ToString();  // save user_id to deactivate it                }
                }
                else
                {
                    FormHelper.MostrarEstado(lblEstado, "Usuario no encontrado", true);
                    panelResultado.Visible = false;
                }
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
                panelResultado.Visible = false;
            }
        }

        private void DeshabilitarUsuario()
        {
            var btnDeshabilitar = (Button)panelResultado.Controls[1];
            Guid userId = new Guid((String)btnDeshabilitar.Tag);

            if (MessageBox.Show(
                $"¿Está seguro que desea deshabilitar al usuario? Esta acción no se puede deshacer.",
                "Confirmar Deshabilitación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    _usuarioService.DarBajaUsuario(userId);
                    FormHelper.MostrarEstado(lblEstado, "Usuario deshabilitado exitosamente", false);
                    panelResultado.Visible = false;
                    LimpiarBusqueda();
                }
                catch (Exception ex)
                {
                    FormHelper.MostrarEstado(lblEstado, ex.Message, true);
                }
            }
        }

        private void GuardarUsuario()
        {
            try
            {
                // Obtener referencias a los controles del formulario
                var panelContenido = (Panel)panelAltaUsuario.Controls[1]; // Panel contenido del acordeón

                // Buscar los controles por índice (según el orden en que fueron agregados)
                var txtNombre = (TextBox)panelContenido.Controls["txtNombre"];
                var txtApellido = (TextBox)panelContenido.Controls["txtApellido"];
                var txtDNI = (TextBox)panelContenido.Controls["txtDNI"];
                var txtDireccion = (TextBox)panelContenido.Controls["txtDireccion"];
                var txtTelefono = (TextBox)panelContenido.Controls["txtTelefono"];
                var txtEmail = (TextBox)panelContenido.Controls["txtEmail"];
                var dtpFechaNacimiento = (DateTimePicker)panelContenido.Controls["dtpFechaNacimiento"];
                var txtUsername = (TextBox)panelContenido.Controls["txtUsername"];
                var cmbPerfil = (ComboBox)panelContenido.Controls["cmbPerfil"];

                // Validar que todos los campos requeridos estén completos
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtDNI.Text) ||
                    string.IsNullOrWhiteSpace(txtDireccion.Text) ||
                    string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtUsername.Text) ||
                    cmbPerfil.SelectedItem == null)
                {
                    throw new Exception("Todos los campos son requeridos");
                }

                // Validar que el DNI sea un número válido
                if (!int.TryParse(txtDNI.Text, out int dni))
                {
                    throw new Exception("El DNI debe ser un número válido");
                }

                // Crear objeto AddUser con los datos del formulario
                var nuevoUsuario = new AddUser
                {
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Dni = dni,
                    Direccion = txtDireccion.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    FechaNacimiento = dtpFechaNacimiento.Value,
                    NombreUsuario = txtUsername.Text.Trim(),
                    Host = (int)cmbPerfil.SelectedValue // Asumiendo que el ComboBox tiene configurado el ValueMember
                };

                _usuarioService.RegistrarNuevoUsuario(nuevoUsuario);
                FormHelper.MostrarEstado(lblEstado, "Usuario creado exitosamente. La contraseña temporal ha sido guardada.", false);
                ToggleAcordeon();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
        }

        private void LimpiarFormulario()
        {
            var panelContenido = (Panel)panelAltaUsuario.Controls[1];

            foreach (Control control in panelContenido.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Text = string.Empty;
                }
                else if (control is DateTimePicker dateTimePicker)
                {
                    dateTimePicker.Value = DateTime.Today;
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.SelectedIndex = -1;
                }
            }
        }

        private void BtnVolver_Click(object sender, EventArgs e)
        {
            // Cierra el UserManagerForm y muestra el HomeForm
            this.Close();
            var homeForm = new HomeForm(PerfilUsuario.Administrador, "admin");
            homeForm.Show();
        }
    }
}