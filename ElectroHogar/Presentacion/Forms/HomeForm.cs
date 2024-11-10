using System.Drawing;
using System.Windows.Forms;
using ElectroHogar.Negocio;
using ElectroHogar.Negocio.Utils;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class HomeForm : Form
    {
        private readonly TipoPerfil _perfil;
        private readonly string _nombreUsuario;
        private const int MARGEN = 50;
        private const int ESPACIO_ENTRE_BOTONES = 20;

        public HomeForm(TipoPerfil perfil, string nombreUsuario)
        {
            InitializeComponent();
            _perfil = perfil;
            _nombreUsuario = nombreUsuario;
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            // Configuración básica del form
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = $"ElectroHogar - {_perfil}";
            this.Size = new Size(600, 500);

            // Panel superior con título
            var panelSuperior = FormHelper.CrearPanelSuperior("ElectroHogar");
            this.Controls.Add(panelSuperior);

            // Label de bienvenida
            var lblBienvenida = FormHelper.CrearLabel($"Bienvenido {_nombreUsuario} - {_perfil}");
            lblBienvenida.Location = new Point(MARGEN, panelSuperior.Bottom + 20);
            lblBienvenida.Font = new Font(lblBienvenida.Font, FontStyle.Bold);
            this.Controls.Add(lblBienvenida);

            // Crear botones de módulos
            CrearBotonesModulos(lblBienvenida.Bottom + 20);
        }

        private void CrearBotonesModulos(int startY)
        {
            var modulos = Perfiles.ObtenerModulos(_perfil);
            var currentY = startY;

            foreach (var modulo in modulos)
            {
                var btn = FormHelper.CrearBotonPrimario(modulo.Nombre, 500);
                btn.Location = new Point(MARGEN, currentY);
                btn.Click += (s, e) => AbrirModulo(modulo.FormularioDestino);
                this.Controls.Add(btn);

                currentY += btn.Height + ESPACIO_ENTRE_BOTONES;
            }

            // Botón de cerrar sesión al final
            var btnCerrarSesion = FormHelper.CrearBotonPrimario("Cerrar Sesión", 500);
            btnCerrarSesion.Location = new Point(MARGEN, currentY + ESPACIO_ENTRE_BOTONES);
            btnCerrarSesion.BackColor = Color.FromArgb(192, 0, 0); // Rojo
            btnCerrarSesion.Click += (s, e) => CerrarSesion();
            this.Controls.Add(btnCerrarSesion);
        }

        private void AbrirModulo(string nombreFormulario)
        {
            // Por ahora solo mostramos un mensaje
            MessageBox.Show($"Abriendo módulo: {nombreFormulario}", "Información",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Aquí iría la lógica para abrir cada formulario
            // Form formulario = null;
            // switch (nombreFormulario)
            // {
            //     case "VentasForm":
            //         formulario = new VentasForm();
            //         break;
            //     // ... etc
            // }
            // 
            // if (formulario != null)
            // {
            //     formulario.ShowDialog();
            // }
        }

        private void CerrarSesion()
        {
            if (MessageBox.Show("¿Está seguro que desea cerrar sesión?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
                Application.Restart();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }
    }
}