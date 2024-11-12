using System;
using System.Drawing;
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
            _loginNegocio = LoginNegocio.Instance;

            // Inicializar label de estado
            lblEstado = FormHelper.CrearLabelEstado();
            this.Controls.Add(lblEstado);

            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Login";

            FormHelper.ConfigurarTextBoxPassword(txtPassword);
            txtUsuario.MaxLength = Validations.MAX_LENGTH_USERNAME;

            FormHelper.ConfigurarEnterTab(txtUsuario, txtPassword);
            FormHelper.ConfigurarEnterTab(txtPassword, button1);

            FormHelper.AgregarEfectoFoco(txtUsuario);
            FormHelper.AgregarEfectoFoco(txtPassword);

            CambiarContraseñaBtn();
        }

        private void IniciarSesionBtn(object sender, EventArgs e)
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

        private void CambiarContraseñaBtn()
        {
            var btnCambiarPass = new Button
            {
                Text = "CAMBIAR CONTRASEÑA",
                Location = new System.Drawing.Point(button1.Left, button1.Bottom + 10),
                Size = button1.Size,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(FormHelper.FuenteNormal, FontStyle.Bold),
                ForeColor = FormHelper.ColorPrimario
            };

            btnCambiarPass.Click += (s, e) =>
            {
                var username = txtUsuario.Text.Trim();
                if (string.IsNullOrEmpty(username))
                {
                    FormHelper.MostrarEstado(lblEstado, "Debe ingresar un nombre de usuario", true);
                    return;
                }

                var cambioForm = new CambiarPasswordForm(username);
                cambioForm.ShowDialog();
            };

            this.Controls.Add(btnCambiarPass);

            // Ajustar la posición del label de estado
            lblEstado.Location = new System.Drawing.Point(
                lblEstado.Location.X,
                btnCambiarPass.Bottom + 10
            );
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
            // Si el login falló por requerir cambio de contraseña temporal
            if (resultado.TipoError == LoginErrorTipo.RequiereCambioContraseña)
            {
                var cambioForm = new CambiarPasswordForm(txtUsuario.Text, true);
                if (cambioForm.ShowDialog() != DialogResult.OK)
                {
                    FormHelper.MostrarEstado(lblEstado, "Debe cambiar su contraseña temporal antes de continuar", true);
                    return;
                }
                // Después de cambiar la contraseña, limpiar y solicitar nuevo login
                txtPassword.Clear();
                txtPassword.Focus();
                FormHelper.MostrarEstado(lblEstado, "Por favor, inicie sesión con su nueva contraseña", false);
                return;
            }

            if (!resultado.Exito)
            {
                ManejarLoginFallido(resultado);
                return;
            }

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