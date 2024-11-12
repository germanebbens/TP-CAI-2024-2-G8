using ElectroHogar.Datos;
using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class ActiveUsersForm : Form
    {
        private readonly Usuarios _usuarioService;
        private readonly Label lblEstado;
        private TextBox txtBusqueda;
        private DataGridView dgvUsuarios;
        private List<UserViewModel> _todosUsuarios;

        public ActiveUsersForm()
        {
            InitializeComponent();
            _usuarioService = new Usuarios();
            lblEstado = FormHelper.CrearLabelEstado();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Usuarios Activos";
            this.ClientSize = new Size(FormHelper.ANCHO_FORM, 600);
            this.AutoScroll = true;

            var panelSuperior = FormHelper.CrearPanelSuperior("Usuarios Activos");
            this.Controls.Add(panelSuperior);

            var panelBusqueda = new Panel
            {
                Width = FormHelper.ANCHO_FORM,
                Height = 80,
                Location = new Point(0, panelSuperior.Bottom + 10)
            };

            var lblBuscar = FormHelper.CrearLabel("Buscar por ID o Username:");
            lblBuscar.Location = new Point(FormHelper.MARGEN, 10);

            txtBusqueda = FormHelper.CrearTextBoxBusqueda();
            txtBusqueda.Location = new Point(FormHelper.MARGEN, lblBuscar.Bottom + 5);
            txtBusqueda.TextChanged += (s, e) => {
                if (txtBusqueda.Text != "Escriba para filtrar...")
                    FiltrarUsuarios();
            };

            panelBusqueda.Controls.AddRange(new Control[] { lblBuscar, txtBusqueda });

            // DataGridView
            dgvUsuarios = new DataGridView
            {
                Location = new Point(FormHelper.MARGEN, panelBusqueda.Bottom + 10),
                Width = FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2),
                Height = 400,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgvUsuarios.Columns.AddRange(new DataGridViewColumn[]
            {
            new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Width = 100
            },
            new DataGridViewTextBoxColumn
            {
                Name = "NombreUsuario",
                HeaderText = "Username",
                DataPropertyName = "NombreUsuario",
                Width = 150
            },
            new DataGridViewTextBoxColumn
            {
                Name = "NombreCompleto",
                HeaderText = "Nombre Completo",
                DataPropertyName = "NombreCompleto",
                Width = 200
            },
            new DataGridViewTextBoxColumn
            {
                Name = "Perfil",
                HeaderText = "Perfil",
                DataPropertyName = "Perfil",
                Width = 100
            },
            new DataGridViewButtonColumn
            {
                Name = "Desactivar",
                HeaderText = "Acción",
                Text = "Desactivar",
                UseColumnTextForButtonValue = true,
                Width = 100
            }
            });

            dgvUsuarios.CellClick += DgvUsuarios_CellClick;

            var btnVolver = FormHelper.CrearBotonPrimario("Volver", 100);
            btnVolver.Location = new Point(FormHelper.MARGEN, dgvUsuarios.Bottom + 10);
            btnVolver.Click += (s, e) => this.Close();

            lblEstado.Location = new Point(FormHelper.MARGEN, btnVolver.Bottom + 10);

            this.Controls.AddRange(new Control[] {
            panelSuperior,
            panelBusqueda,
            dgvUsuarios,
            btnVolver,
            lblEstado
        });

            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            try
            {
                var usuarios = _usuarioService.BuscarUsuariosActivos();
                _todosUsuarios = usuarios.Select(u => new UserViewModel
                {
                    Id = u.Id,
                    NombreUsuario = u.NombreUsuario,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    Perfil = ((PerfilUsuario)u.Host).ToString()
                }).ToList();

                dgvUsuarios.DataSource = _todosUsuarios;
                dgvUsuarios.Tag = _todosUsuarios; // Guardar lista original para filtrado
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
        }

        private void FiltrarUsuarios()
        {
            if (dgvUsuarios.Tag == null) return;

            var filtro = txtBusqueda.Text.ToLower();
            var usuariosFiltrados = ((List<UserViewModel>)dgvUsuarios.Tag)
                .Where(u =>
                    u.Id.ToString().ToLower().Contains(filtro) ||
                    u.NombreUsuario.ToLower().Contains(filtro))
                .ToList();

            dgvUsuarios.DataSource = usuariosFiltrados;
        }
        
        private void DgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvUsuarios.Columns["Desactivar"].Index && e.RowIndex >= 0)
            {
                var row = dgvUsuarios.Rows[e.RowIndex];
                var buttonCell = (DataGridViewButtonCell)row.Cells["Desactivar"];

                if (buttonCell.Value?.ToString() == "Desactivado")
                    return;

                var userId = Guid.Parse(row.Cells["Id"].Value.ToString());
                var username = row.Cells["NombreUsuario"].Value.ToString();

                if (MessageBox.Show(
                    $"¿Está seguro que desea deshabilitar al usuario {username}? Esta acción no se puede deshacer.",
                    "Confirmar Deshabilitación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        _usuarioService.DarBajaUsuario(userId);

                        buttonCell.Value = "Desactivado";
                        buttonCell.Style.BackColor = Color.LightGray;
                        buttonCell.Style.ForeColor = Color.DarkGray;

                        row.DefaultCellStyle.BackColor = Color.LightGray;
                        row.DefaultCellStyle.ForeColor = Color.DarkGray;

                        MessageBox.Show(
                            $"El usuario {username} ha sido desactivado exitosamente.",
                            "Usuario Desactivado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        FormHelper.MostrarEstado(lblEstado, "Usuario deshabilitado exitosamente", false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error al desactivar el usuario: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        FormHelper.MostrarEstado(lblEstado, ex.Message, true);
                    }
                }
            }
        }
    }
}
