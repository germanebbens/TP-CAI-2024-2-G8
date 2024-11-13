using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class GenericListForm : Form
    {
        private readonly IListadoConfiguracion _config;
        private readonly Label lblEstado;
        private TextBox txtBusqueda;
        private DataGridView dgvItems;
        private dynamic _todosItems;

        public GenericListForm(IListadoConfiguracion config)
        {
            InitializeComponent();
            _config = config;
            lblEstado = FormHelper.CrearLabelEstado();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = $"ElectroHogar - {_config.Titulo}";
            this.ClientSize = new Size(FormHelper.ANCHO_FORM + 30, 630);
            this.AutoScroll = true;

            var panelSuperior = FormHelper.CrearPanelSuperior(_config.Titulo);
            this.Controls.Add(panelSuperior);

            // Panel de búsqueda
            var panelBusqueda = new Panel
            {
                Width = FormHelper.ANCHO_FORM,
                Height = 80,
                Location = new Point(0, panelSuperior.Bottom + 10)
            };

            var lblBuscar = FormHelper.CrearLabel($"Buscar por {_config.CampoBusqueda}:");
            lblBuscar.Location = new Point(FormHelper.MARGEN, 10);

            txtBusqueda = FormHelper.CrearTextBoxBusqueda();
            txtBusqueda.Location = new Point(FormHelper.MARGEN, lblBuscar.Bottom + 5);
            txtBusqueda.TextChanged += (s, e) => {
                if (txtBusqueda.Text != "Escriba para filtrar...")
                    FiltrarItems();
            };

            panelBusqueda.Controls.AddRange(new Control[] { lblBuscar, txtBusqueda });

            // DataGridView
            dgvItems = new DataGridView
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

            ConfigurarColumnas();

            // Solo agregamos el evento si hay acciones configuradas
            if (_config.Acciones.Any())
            {
                dgvItems.CellClick += DgvItems_CellClick;
            }

            var btnVolver = FormHelper.CrearBotonPrimario("Volver", 100);
            btnVolver.Location = new Point(FormHelper.MARGEN, dgvItems.Bottom + 10);
            btnVolver.Click += (s, e) => this.Close();

            lblEstado.Location = new Point(FormHelper.MARGEN, btnVolver.Bottom + 10);

            this.Controls.AddRange(new Control[] {
            panelSuperior,
            panelBusqueda,
            dgvItems,
            btnVolver,
            lblEstado
        });

            CargarItems();
        }

        private void ConfigurarColumnas()
        {
            // Primero las columnas de datos
            var columnas = _config.Columnas.Select(c => new DataGridViewTextBoxColumn
            {
                Name = c.Nombre,
                HeaderText = c.Titulo,
                DataPropertyName = c.PropiedadDatos,
                Width = c.Ancho
            }).ToList<DataGridViewColumn>();

            // Luego las columnas de acciones
            columnas.AddRange(_config.Acciones.Select(a => new DataGridViewButtonColumn
            {
                Name = a.Nombre,
                HeaderText = "Acción",
                Text = a.Texto,
                UseColumnTextForButtonValue = true,
                Width = 85
            }));

            dgvItems.Columns.AddRange(columnas.ToArray());
        }

        private void DgvItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvItems.Rows[e.RowIndex];
            var columnName = dgvItems.Columns[e.ColumnIndex].Name;
            var accion = _config.Acciones.FirstOrDefault(a => a.Nombre == columnName);

            if (accion == null) return;

            var buttonCell = row.Cells[columnName] as DataGridViewButtonCell;
            if (buttonCell?.Value?.ToString() == "Desactivado") return;

            // Si la acción requiere confirmación
            if (accion.RequiereConfirmacion)
            {
                var itemId = Guid.Parse(row.Cells["Id"].Value.ToString());
                var nombreItem = row.Cells[_config.Columnas[1].Nombre].Value.ToString();

                var mensaje = string.Format(accion.MensajeConfirmacion, nombreItem);
                if (MessageBox.Show(mensaje, "Confirmar Acción",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }
            }

            try
            {
                // Ejecutar la acción
                accion.Accion?.Invoke(row);

                // Si es una acción de desactivar, actualizamos la UI
                if (accion.Nombre == "Desactivar")
                {
                    buttonCell.Value = "Desactivado";
                    buttonCell.Style.BackColor = Color.LightGray;
                    buttonCell.Style.ForeColor = Color.DarkGray;
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                    row.DefaultCellStyle.ForeColor = Color.DarkGray;

                    FormHelper.MostrarEstado(lblEstado, $"{_config.NombreIdentificador} deshabilitado exitosamente", false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al ejecutar la acción: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
        }

        private void CargarItems()
        {
            try
            {
                // Asumimos que el servicio tiene un método para obtener items
                var method = _config.Service.GetType().GetMethod("ObtenerActivos");
                if (method != null)
                {
                    _todosItems = method.Invoke(_config.Service, null);
                    dgvItems.DataSource = _todosItems;
                    dgvItems.Tag = _todosItems;
                }
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
        }

        private void FiltrarItems()
        {
            if (dgvItems.Tag == null) return;

            var filtro = txtBusqueda.Text.ToLower();
            var items = (IEnumerable<dynamic>)dgvItems.Tag;

            var itemsFiltrados = items.Where(item =>
                _config.CamposBusqueda.Any(campo =>
                    item.GetType()
                        .GetProperty(campo)
                        ?.GetValue(item)
                        ?.ToString()
                        .ToLower()
                        .Contains(filtro)
                    ?? false
                )
            ).ToList();

            dgvItems.DataSource = itemsFiltrados;
        }
    }
}
