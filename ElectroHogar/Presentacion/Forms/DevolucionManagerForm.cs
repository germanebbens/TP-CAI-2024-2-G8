using ElectroHogar.Datos;
using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class DevolucionManagerForm : Form
    {
        private readonly Ventas _ventasService;
        private readonly Label lblEstado;
        private readonly Panel panelCliente;
        private readonly Panel panelVentas;
        private ClienteList _clienteSeleccionado;
        private List<VentaList> _ventas;

        public DevolucionManagerForm()
        {
            InitializeComponent();
            _ventasService = new Ventas();
            lblEstado = FormHelper.CrearLabelEstado();
            panelCliente = CrearPanelCliente();
            panelVentas = CrearPanelVentas();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Devolución de Ventas";
            this.ClientSize = new Size(FormHelper.ANCHO_FORM + 20, 800);
            this.AutoScroll = true;

            var panelSuperior = FormHelper.CrearPanelSuperior("Devolución de Ventas");
            this.Controls.Add(panelSuperior);

            var btnVolver = FormHelper.CrearBotonPrimario("Volver", 100);
            btnVolver.Location = new Point(FormHelper.MARGEN, panelSuperior.Bottom + 10);
            btnVolver.Click += (s, e) => this.Close();

            lblEstado.Location = new Point(FormHelper.MARGEN, btnVolver.Bottom + 10);
            panelCliente.Location = new Point(0, lblEstado.Bottom + 10);
            panelVentas.Location = new Point(0, panelCliente.Bottom);

            this.Controls.AddRange(new Control[] {
                btnVolver,
                lblEstado,
                panelCliente,
                panelVentas
            });
        }

        private Panel CrearPanelCliente()
        {
            var panel = new Panel
            {
                Width = FormHelper.ANCHO_FORM,
                AutoSize = true,
                Padding = new Padding(FormHelper.MARGEN)
            };

            var panelDatos = new Panel
            {
                Name = "panelDatosCliente",
                Width = FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2),
                AutoSize = true
            };

            var btnSeleccionarCliente = FormHelper.CrearBotonPrimario("Seleccionar Cliente", 200);
            btnSeleccionarCliente.Location = new Point(FormHelper.MARGEN, 10);
            btnSeleccionarCliente.Click += (s, e) => SeleccionarCliente();

            var lblClienteInfo = new Label
            {
                Name = "lblClienteInfo",
                Text = "Ningún cliente seleccionado",
                Location = new Point(FormHelper.MARGEN, btnSeleccionarCliente.Bottom + 10),
                AutoSize = true,
                Font = new Font(FormHelper.FuenteNormal, FontStyle.Bold)
            };

            panelDatos.Controls.AddRange(new Control[] { btnSeleccionarCliente, lblClienteInfo });
            panel.Controls.Add(panelDatos);
            return panel;
        }

        private Panel CrearPanelVentas()
        {
            var panel = new Panel
            {
                Width = FormHelper.ANCHO_FORM,
                Height = 400,
                Padding = new Padding(FormHelper.MARGEN),
                Visible = false,
                Name = "panelVentas"
            };

            var titulo = FormHelper.CrearLabel("Ventas del Cliente");
            titulo.Font = new Font(titulo.Font, FontStyle.Bold);
            titulo.Location = new Point(FormHelper.MARGEN, 10);

            var dgvVentas = new DataGridView
            {
                Name = "dgvVentas",
                Location = new Point(FormHelper.MARGEN, titulo.Bottom + 10),
                Width = FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 3),
                Height = 300,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgvVentas.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn
                {
                    Name = "Id",
                    HeaderText = "ID",
                    DataPropertyName = "Id",
                    Width = 150
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "FechaAlta",
                    HeaderText = "Fecha",
                    DataPropertyName = "FechaAlta",
                    Width = 150
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Cantidad",
                    HeaderText = "Cantidad",
                    DataPropertyName = "Cantidad",
                    Width = 100
                },
                new DataGridViewButtonColumn
                {
                    Name = "Devolver",
                    HeaderText = "Acción",
                    Text = "Devolver",
                    UseColumnTextForButtonValue = true,
                    Width = 100
                }
            });

            dgvVentas.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgvVentas.Columns["Devolver"].Index)
                {
                    var row = dgvVentas.Rows[e.RowIndex];

                    if (row.Tag != null && row.Tag.ToString() == "Deshabilitado")
                        return;

                    if (MessageBox.Show("¿Está seguro que desea devolver la venta?", "Confirmar Devolución",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            var ventaId = Guid.Parse(row.Cells["Id"].Value.ToString());
                            _ventasService.DevolverVenta(ventaId);

                            row.Tag = "Deshabilitado";

                            var style = new DataGridViewCellStyle
                            {
                                BackColor = Color.LightGray,
                                ForeColor = Color.DarkGray,
                                SelectionBackColor = Color.LightGray,
                                SelectionForeColor = Color.DarkGray
                            };
                            row.DefaultCellStyle = style;

                            FormHelper.MostrarEstado(lblEstado, "Venta devuelta exitosamente", false);
                        }
                        catch (Exception ex)
                        {
                            FormHelper.MostrarEstado(lblEstado, ex.Message, true);
                        }
                    }
                }
            };

            panel.Controls.AddRange(new Control[] { titulo, dgvVentas });
            return panel;
        }

        private void SeleccionarCliente()
        {
            var config = new ClienteSeleccionListadoConfig(cliente =>
            {
                _clienteSeleccionado = cliente;
            });

            var formClientes = new BaseListForm(config);
            if (formClientes.ShowDialog() == DialogResult.OK)
            {
                ActualizarDatosCliente();
                CargarVentasCliente();
            }
        }

        private void ActualizarDatosCliente()
        {
            if (_clienteSeleccionado != null)
            {
                var lblClienteInfo = (Label)panelCliente.Controls["panelDatosCliente"].Controls["lblClienteInfo"];
                lblClienteInfo.Text = $"{_clienteSeleccionado.Nombre} {_clienteSeleccionado.Apellido} (DNI: {_clienteSeleccionado.Dni})";
            }
        }

        private void CargarVentasCliente()
        {
            try
            {
                _ventas = _ventasService.ObtenerVentasPorCliente(_clienteSeleccionado.Id);
                var dgvVentas = (DataGridView)panelVentas.Controls["dgvVentas"];
                dgvVentas.DataSource = _ventas;
                panelVentas.Visible = true;
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
        }
    }
}