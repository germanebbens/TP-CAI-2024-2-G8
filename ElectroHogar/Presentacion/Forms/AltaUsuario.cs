using System;
using System.Drawing;
using System.Windows.Forms;
using ElectroHogar.Datos;
using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class AltaUsuarioForm : Form
    {
        private readonly NuevoUsuario _nuevoUsuarioService;
        private readonly Label lblEstado;
        private const int MARGEN = 50;
        private const int ANCHO_FORM = 600;

        public AltaUsuarioForm()
        {
            InitializeComponent();
            _nuevoUsuarioService = new NuevoUsuario();

            // Label de estado para mensajes
            lblEstado = FormHelper.CrearLabelEstado();
            lblEstado.Location = new Point(MARGEN, 400);
            this.Controls.Add(lblEstado);

            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Alta de Usuario";
            this.ClientSize = new Size(ANCHO_FORM, 500);

            // Panel superior
            var panelSuperior = FormHelper.CrearPanelSuperior("Alta de Usuario");
            this.Controls.Add(panelSuperior);

            int currentY = panelSuperior.Bottom + 20;

            // Datos básicos
            var txtNombre = CrearCampoTexto("Nombre:", ref currentY);
            var txtApellido = CrearCampoTexto("Apellido:", ref currentY);
            var txtDNI = CrearCampoTexto("DNI:", ref currentY);
            var txtDireccion = CrearCampoTexto("Dirección:", ref currentY);
            var txtTelefono = CrearCampoTexto("Teléfono:", ref currentY);
            var txtEmail = CrearCampoTexto("Email:", ref currentY);
            var dtpFechaNacimiento = CrearCampoFecha("Fecha de Nacimiento:", ref currentY);
            var txtUsername = CrearCampoTexto("Nombre de Usuario:", ref currentY);

            // Combo de perfiles
            var lblPerfil = FormHelper.CrearLabel("Perfil:");
            lblPerfil.Location = new Point(MARGEN, currentY);
            this.Controls.Add(lblPerfil);

            var cmbPerfil = new ComboBox
            {
                Location = new Point(MARGEN + 150, currentY),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPerfil.Items.AddRange(new object[] { "Vendedor", "Supervisor" });
            cmbPerfil.SelectedIndex = 0;
            this.Controls.Add(cmbPerfil);

            currentY += 40;

            // Botón guardar
            var btnGuardar = FormHelper.CrearBotonPrimario("Guardar Usuario", ANCHO_FORM - (MARGEN * 2));
            btnGuardar.Location = new Point(MARGEN, currentY);
            btnGuardar.Click += (s, e) => GuardarUsuario();
            this.Controls.Add(btnGuardar);

            // Funciones locales
            void GuardarUsuario()
            {
                try
                {
                    FormHelper.MostrarEstado(lblEstado, "Guardando usuario...", false);
                    btnGuardar.Enabled = false;

                    var nuevoUsuario = new AddUser
                    {
                        Nombre = txtNombre.Text,
                        Apellido = txtApellido.Text,
                        Dni = int.Parse(txtDNI.Text),
                        Direccion = txtDireccion.Text,
                        Telefono = txtTelefono.Text,
                        Email = txtEmail.Text,
                        FechaNacimiento = dtpFechaNacimiento.Value,
                        NombreUsuario = txtUsername.Text,
                        Host = cmbPerfil.SelectedIndex == 0 ? 1 : 2 // 1=Vendedor, 2=Supervisor
                    };

                    var usuarioCreado = _nuevoUsuarioService.RegistrarNuevoUsuario(nuevoUsuario);

                    MessageBox.Show(
                        $"Usuario creado exitosamente.\nContraseña temporal: {usuarioCreado.Contraseña}\n\nPor favor, guarde esta contraseña.",
                        "Usuario Creado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    this.Close();
                }
                catch (Exception ex)
                {
                    FormHelper.MostrarEstado(lblEstado, ex.Message, true);
                }
                finally
                {
                    btnGuardar.Enabled = true;
                }
            }
        }

        private TextBox CrearCampoTexto(string label, ref int currentY)
        {
            var lbl = FormHelper.CrearLabel(label);
            lbl.Location = new Point(MARGEN, currentY);
            this.Controls.Add(lbl);

            var txt = FormHelper.CrearTextBox(300);
            txt.Location = new Point(MARGEN + 150, currentY);
            this.Controls.Add(txt);

            currentY += 40;
            return txt;
        }

        private DateTimePicker CrearCampoFecha(string label, ref int currentY)
        {
            var lbl = FormHelper.CrearLabel(label);
            lbl.Location = new Point(MARGEN, currentY);
            this.Controls.Add(lbl);

            var dtp = new DateTimePicker
            {
                Location = new Point(MARGEN + 150, currentY),
                Width = 200,
                Format = DateTimePickerFormat.Short
            };
            this.Controls.Add(dtp);

            currentY += 40;
            return dtp;
        }
    }
}
