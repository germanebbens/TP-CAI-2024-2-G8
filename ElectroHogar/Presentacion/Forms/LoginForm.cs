using System;
using System.Windows.Forms;
using ElectroHogar.Negocio;
using ElectroHogar.Negocio.Utils;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class LoginForm : Form
    {
        private readonly LoginNegocio _loginNegocio;
        private readonly Label lblEstado;

        public LoginForm()
        {
            InitializeComponent();
            _loginNegocio = new LoginNegocio();

            // Inicializar label de estado
            lblEstado = FormHelper.CrearLabelEstado();
            lblEstado.Location = new System.Drawing.Point(50, 280);
            this.Controls.Add(lblEstado);

            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            // Configuración básica del form
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Login";

            // Configuración de controles
            FormHelper.ConfigurarTextBoxPassword(txtPassword);
            txtUsuario.MaxLength = Validations.MAX_LENGTH_USERNAME;

            // Eventos de navegación
            FormHelper.ConfigurarEnterTab(txtUsuario, txtPassword);
            FormHelper.ConfigurarEnterTab(txtPassword, button1);

            // Efectos visuales
            FormHelper.AgregarEfectoFoco(txtUsuario);
            FormHelper.AgregarEfectoFoco(txtPassword);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lblEstado.Visible = false;

            try
            {
                if (!ValidarFormulario()) return;

                FormHelper.MostrarCargando(button1, true, "INICIAR SESIÓN", "INICIANDO SESIÓN...");
                DeshabilitarControles(true);

                var resultado = _loginNegocio.Login(txtUsuario.Text, txtPassword.Text);

                if (resultado.Exito)
                {
                    ManejarLoginExitoso(resultado);
                }
                else
                {
                    ManejarLoginFallido(resultado);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                FormHelper.MostrarEstado(lblEstado, "Error al intentar conectar con el servidor", true);
            }
            finally
            {
                FormHelper.MostrarCargando(button1, false, "INICIAR SESIÓN", "INICIANDO SESIÓN...");
                DeshabilitarControles(false);
            }
        }

        private bool ValidarFormulario()
        {
            var (usuarioValido, mensajeUsuario) = Validations.ValidarUsuario(txtUsuario.Text);
            if (!usuarioValido)
            {
                FormHelper.MostrarEstado(lblEstado, mensajeUsuario, true);
                txtUsuario.Focus();
                return false;
            }

            var (passwordValido, mensajePassword) = Validations.ValidarPassword(txtPassword.Text);
            if (!passwordValido)
            {
                FormHelper.MostrarEstado(lblEstado, mensajePassword, true);
                txtPassword.Focus();
                return false;
            }

            return true;
        }

        private void DeshabilitarControles(bool deshabilitar)
        {
            txtUsuario.Enabled = !deshabilitar;
            txtPassword.Enabled = !deshabilitar;
            button1.Enabled = !deshabilitar;
        }

        private void ManejarLoginExitoso(LoginResult resultado)
        {
            FormHelper.MostrarEstado(lblEstado, $"Bienvenido! Iniciando sesión como {resultado.Perfil}...");
            System.Threading.Thread.Sleep(1000);

            var homeForm = new HomeForm(
                resultado.Perfil.Value,
                txtUsuario.Text
            );

            this.Hide();
            homeForm.ShowDialog();
            this.Close();
        }

        private void ManejarLoginFallido(LoginResult resultado)
        {
            FormHelper.MostrarEstado(lblEstado, resultado.Mensaje, true);
            txtPassword.Clear();
            txtPassword.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtUsuario.Focus();
        }
    }
}