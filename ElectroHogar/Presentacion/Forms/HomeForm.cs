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
        private const int ANCHO_FORM = 600;

        public HomeForm(TipoPerfil perfil, string nombreUsuario)
        {
            InitializeComponent();
            _perfil = perfil;
            _nombreUsuario = nombreUsuario;
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = $"ElectroHogar - {_perfil}";
            this.ClientSize = new Size(ANCHO_FORM, 0);  // Alto inicial 0, se ajustará automáticamente

            // Panel superior con título
            var panelSuperior = FormHelper.CrearPanelSuperior("ElectroHogar");
            this.Controls.Add(panelSuperior);

            // Label de bienvenida
            var lblBienvenida = FormHelper.CrearLabel($"Bienvenido {_nombreUsuario} - {_perfil}");
            lblBienvenida.Location = new Point(MARGEN, panelSuperior.Bottom + 20);
            lblBienvenida.Font = new Font(lblBienvenida.Font, FontStyle.Bold);
            this.Controls.Add(lblBienvenida);

            // Crear botones y ajustar altura del form
            CrearBotonesModulos(lblBienvenida.Bottom + 20);
        }

        private void CrearBotonesModulos(int startY)
        {
            var modulos = Perfiles.ObtenerModulos(_perfil);
            var currentY = startY;

            // Crear botones de módulos
            foreach (var modulo in modulos)
            {
                var btn = FormHelper.CrearBotonPrimario(modulo.Nombre, ANCHO_FORM - (MARGEN * 2));
                btn.Location = new Point(MARGEN, currentY);
                btn.Click += (s, e) => AbrirModulo(modulo.FormularioDestino);
                this.Controls.Add(btn);
                currentY += btn.Height + ESPACIO_ENTRE_BOTONES;
            }

            // Botón de cerrar sesión
            var btnCerrarSesion = FormHelper.CrearBotonPrimario("Cerrar Sesión", ANCHO_FORM - (MARGEN * 2));
            btnCerrarSesion.Location = new Point(MARGEN, currentY + ESPACIO_ENTRE_BOTONES);
            btnCerrarSesion.BackColor = Color.FromArgb(192, 0, 0);
            btnCerrarSesion.Click += (s, e) => CerrarSesion();
            this.Controls.Add(btnCerrarSesion);

            // Ajustar altura del formulario automáticamente
            this.ClientSize = new Size(
                ANCHO_FORM,
                btnCerrarSesion.Bottom + MARGEN  // Alto dinámico basado en el último botón
            );
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
                var loginForm = new LoginForm();
                this.Hide();
                loginForm.ShowDialog();
                this.Close();
            }
        }

    }
}