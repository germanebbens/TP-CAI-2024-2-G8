using System;
using System.Drawing;
using System.Windows.Forms;
using ElectroHogar.Datos;
using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class UserManagerForm : Form
    {
        private readonly Usuarios _usuarioService;
        private readonly Label lblEstado;
        private readonly Panel panelAltaUsuario;
        private bool acordeonAbierto = false;

        public UserManagerForm()
        {
            InitializeComponent();
            _usuarioService = new Usuarios();
            lblEstado = FormHelper.CrearLabelEstado();
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

            var btnUsuariosActivos = FormHelper.CrearBotonPrimario("Ver Usuarios Activos");
            btnUsuariosActivos.Location = new Point(FormHelper.MARGEN, panelSuperior.Bottom + 20);
            btnUsuariosActivos.Click += (s, e) => {
                var configUsuarios = new UsuariosListadoConfig();
                var formProveedores = new BaseListForm(configUsuarios);
                formProveedores.ShowDialog();
            };

            var btnVolver = FormHelper.CrearBotonPrimario("Volver", 100);
            btnVolver.Location = new Point(btnUsuariosActivos.Right + 20, panelSuperior.Bottom + 20);
            btnVolver.Click += (s, e) => this.Close();

            lblEstado.Location = new Point(FormHelper.MARGEN, btnUsuariosActivos.Bottom + 10);
            panelAltaUsuario.Location = new Point(0, lblEstado.Bottom + 10);

            this.Controls.AddRange(new Control[] {
                btnVolver,
                btnUsuariosActivos,
                lblEstado,
                panelAltaUsuario
            });
        }

        private Panel CrearPanelAltaUsuario()
        {
            var panel = new Panel
            {
                Width = FormHelper.ANCHO_FORM,
                AutoSize = true,
                Padding = new Padding(FormHelper.MARGEN)
            };

            var btnAcordeon = FormHelper.CrearBotonPrimario("+ Agregar Nuevo Usuario", FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2));
            btnAcordeon.Location = new Point(FormHelper.MARGEN, 10);
            btnAcordeon.Click += (s, e) => ToggleAcordeon();

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
            var cmbPerfil = FormHelper.CrearComboPerfil("Perfil:", "cmbPerfil", ref currentY, panelContenido);

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
            panelAltaUsuario.Controls[1].Visible = acordeonAbierto;
            var btnAcordeon = (Button)panelAltaUsuario.Controls[0];
            btnAcordeon.Text = acordeonAbierto ? "- Cerrar" : "+ Agregar Nuevo Usuario";
        }

        private void GuardarUsuario()
        {
            try
            {
                var panelContenido = (Panel)panelAltaUsuario.Controls[1];
                var txtNombre = (TextBox)panelContenido.Controls["txtNombre"];
                var txtApellido = (TextBox)panelContenido.Controls["txtApellido"];
                var txtDNI = (TextBox)panelContenido.Controls["txtDNI"];
                var txtDireccion = (TextBox)panelContenido.Controls["txtDireccion"];
                var txtTelefono = (TextBox)panelContenido.Controls["txtTelefono"];
                var txtEmail = (TextBox)panelContenido.Controls["txtEmail"];
                var dtpFechaNacimiento = (DateTimePicker)panelContenido.Controls["dtpFechaNacimiento"];
                var txtUsername = (TextBox)panelContenido.Controls["txtUsername"];
                var cmbPerfil = (ComboBox)panelContenido.Controls["cmbPerfil"];

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

                if (!int.TryParse(txtDNI.Text, out int dni))
                {
                    throw new Exception("El DNI debe ser un número válido");
                }

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
                    Host = (int)cmbPerfil.SelectedValue
                };

                AddUser newUser = _usuarioService.RegistrarNuevoUsuario(nuevoUsuario);
                FormHelper.MostrarContraseñaTemporal(newUser.Contraseña);
                ToggleAcordeon();
                LimpiarFormulario();
                FormHelper.MostrarEstado(lblEstado, "Usuario creado exitosamente.", false);
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
    }
}