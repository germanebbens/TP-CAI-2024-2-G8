using System.Drawing;
using System.Windows.Forms;
using ElectroHogar.Datos;
using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class HomeForm : Form
    {
        private readonly PerfilUsuario _perfil;
        private readonly string _nombreUsuario;
        private const int MARGEN = 50;
        private const int ESPACIO_ENTRE_BOTONES = 20;
        private readonly int ANCHO_FORM;

        public HomeForm(PerfilUsuario perfil, string nombreUsuario)
        {
            InitializeComponent();
            ANCHO_FORM = FormHelper.ANCHO_FORM;
            _perfil = perfil;
            _nombreUsuario = nombreUsuario;
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = $"ElectroHogar - {_perfil}";
            this.ClientSize = new Size(ANCHO_FORM, 0); // initial height 0

            var panelSuperior = FormHelper.CrearPanelSuperior("ElectroHogar");
            this.Controls.Add(panelSuperior);

            var lblBienvenida = FormHelper.CrearLabel($"Bienvenido {_nombreUsuario} - {_perfil}");
            lblBienvenida.Location = new Point(MARGEN, panelSuperior.Bottom + 20);
            lblBienvenida.Font = new Font(lblBienvenida.Font, FontStyle.Bold);
            this.Controls.Add(lblBienvenida);

            CrearBotonesModulos(lblBienvenida.Bottom + 20);
        }

        private void CrearBotonesModulos(int startY)
        {
            var modulos = Perfiles.ObtenerModulos(_perfil);
            var currentY = startY;

            foreach (var modulo in modulos)
            {
                var btn = FormHelper.CrearBotonPrimario(modulo.Nombre, ANCHO_FORM - (MARGEN * 2));
                btn.Location = new Point(MARGEN, currentY);
                btn.Click += (s, e) => AbrirModulo(modulo.FormularioDestino);
                this.Controls.Add(btn);
                currentY += btn.Height + ESPACIO_ENTRE_BOTONES;
            }

            var btnCerrarSesion = FormHelper.CrearBotonPrimario("Cerrar Sesión", ANCHO_FORM - (MARGEN * 2));
            btnCerrarSesion.Location = new Point(MARGEN, currentY + ESPACIO_ENTRE_BOTONES);
            btnCerrarSesion.BackColor = Color.FromArgb(192, 0, 0);
            btnCerrarSesion.Click += (s, e) => CerrarSesion();
            this.Controls.Add(btnCerrarSesion);

            this.ClientSize = new Size(
                ANCHO_FORM,
                btnCerrarSesion.Bottom + MARGEN
            );
        }

        private void AbrirModulo(string nombreFormulario)
        {
            Form formulario = null;
            switch (nombreFormulario)
            {
                case "UsuariosForm":
                    formulario = new UserManagerForm();
                    break;
                case "ProveedoresForm":
                    formulario = new ProveedorManagerForm();
                    break;
                case "ProductosForm":
                    formulario = new ProductoManagerForm();
                    break;
                case "VentasForm":
                    formulario = new VentasManagerForm();
                    break;
                case "DevolucionesForm":
                    formulario = new DevolucionManagerForm();
                    break;
                case "StockReportForm":
                    formulario = new StockCriticoForm();
                    break;
            }

            if (formulario != null)
            {
                this.Hide();
                formulario.FormClosed += (s, e) => {
                    this.Show();
                };
                formulario.ShowDialog();
            }
            else
            {
                MessageBox.Show($"abriendo módulo: {nombreFormulario}", "información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CerrarSesion()
        {
            if (MessageBox.Show("¿Está seguro que desea cerrar sesión?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var loginForm = new LoginForm();
                LoginNegocio.Reset(); //reset singleton class -> login negocio
                ApplicationManager.Instance.ShowNewForm(loginForm);
                this.Close();
            }
        }

    }
}