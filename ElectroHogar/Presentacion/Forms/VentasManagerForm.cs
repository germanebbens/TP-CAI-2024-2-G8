using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ElectroHogar.Datos;
using ElectroHogar.Negocio;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Presentacion.Forms
{
    public partial class VentasManagerForm : Form
    {
        private readonly Ventas _ventasService;
        private readonly Label lblEstado;
        private readonly Panel panelCliente;
        private readonly Panel panelProductos;
        private readonly Panel panelResumen;
        private ClienteList _clienteSeleccionado;
        private List<ItemVenta> _items = new List<ItemVenta>();

        public VentasManagerForm()
        {
            InitializeComponent();
            _ventasService = new Ventas();
            lblEstado = FormHelper.CrearLabelEstado();
            panelCliente = CrearPanelCliente();
            panelProductos = CrearPanelProductos();
            panelResumen = CrearPanelResumen();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            FormHelper.ConfigurarFormularioBase(this);
            this.Text = "ElectroHogar - Nueva Venta";
            this.ClientSize = new Size(FormHelper.ANCHO_FORM + 20, 800);
            this.AutoScroll = true;

            var panelSuperior = FormHelper.CrearPanelSuperior("Nueva Venta");
            this.Controls.Add(panelSuperior);

            var btnVolver = FormHelper.CrearBotonPrimario("Volver", 100);
            btnVolver.Location = new Point(FormHelper.MARGEN, panelSuperior.Bottom + 10);
            btnVolver.Click += (s, e) => this.Close();

            lblEstado.Location = new Point(FormHelper.MARGEN, btnVolver.Bottom + 10);
            panelCliente.Location = new Point(0, lblEstado.Bottom + 10);
            panelProductos.Location = new Point(0, panelCliente.Bottom + 40);
            panelResumen.Location = new Point(0, panelProductos.Bottom + 10);

            this.Controls.AddRange(new Control[] {
                btnVolver,
                lblEstado,
                panelCliente,
                panelProductos,
                panelResumen
            });
        }

        private Panel CrearPanelCliente()
        {
            var panel = new Panel
            {
                Name = "panelCliente",
                Width = FormHelper.ANCHO_FORM,
                AutoSize = true,
                Padding = new Padding(FormHelper.MARGEN)
            };

            // Título
            var lblTitulo = FormHelper.CrearLabel("Datos del Cliente");
            lblTitulo.Font = new Font(lblTitulo.Font, FontStyle.Bold);
            lblTitulo.Location = new Point(FormHelper.MARGEN, 10);
            lblTitulo.MaximumSize = new Size(150, 0);

            // Botón seleccionar cliente
            var btnSeleccionarCliente = FormHelper.CrearBotonPrimario("Seleccionar Cliente", 200);
            btnSeleccionarCliente.Location = new Point(300, 10);
            btnSeleccionarCliente.Click += (s, e) => SeleccionarCliente();

            // Panel de datos del cliente
            var panelDatos = new Panel
            {
                Name = "panelDatosCliente",
                Width = FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2),
                Location = new Point(FormHelper.MARGEN, btnSeleccionarCliente.Bottom + 10),
                AutoSize = true
            };
            int currentY = 10;

            // Labels para mostrar datos (readonly)
            var lblClienteInfo = new Label
            {
                Name = "lblClienteInfo",
                Text = "Ningún cliente seleccionado",
                Location = new Point(FormHelper.MARGEN, currentY),
                AutoSize = true,
                Font = new Font(FormHelper.FuenteNormal, FontStyle.Bold)
            };
            currentY = lblClienteInfo.Bottom + 10;

            var lblDireccion = new Label
            {
                Name = "lblDireccion",
                Text = "",
                Location = new Point(FormHelper.MARGEN, currentY),
                AutoSize = true
            };
            currentY = lblDireccion.Bottom + 5;

            var lblTelefono = new Label
            {
                Name = "lblTelefono",
                Text = "",
                Location = new Point(FormHelper.MARGEN, currentY),
                AutoSize = true
            };
            currentY = lblTelefono.Bottom + 5;

            var lblEmail = new Label
            {
                Name = "lblEmail",
                Text = "",
                Location = new Point(FormHelper.MARGEN, currentY),
                AutoSize = true
            };

            panelDatos.Controls.AddRange(new Control[] {
                lblClienteInfo,
                lblDireccion,
                lblTelefono,
                lblEmail
            });
            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(btnSeleccionarCliente);
            panel.Controls.Add(panelDatos);
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
            }
        }

        private void ActualizarDatosCliente()
        {
            var panelDatos = (Panel)panelCliente.Controls["panelDatosCliente"];

            if (_clienteSeleccionado != null)
            {
                var lblClienteInfo = (Label)panelDatos.Controls["lblClienteInfo"];
                var lblDireccion = (Label)panelDatos.Controls["lblDireccion"];
                var lblTelefono = (Label)panelDatos.Controls["lblTelefono"];
                var lblEmail = (Label)panelDatos.Controls["lblEmail"];

                lblClienteInfo.Text = $"{_clienteSeleccionado.Nombre} {_clienteSeleccionado.Apellido} (DNI: {_clienteSeleccionado.Dni})";
                lblDireccion.Text = $"Dirección: {_clienteSeleccionado.Direccion}";
                lblTelefono.Text = $"Teléfono: {_clienteSeleccionado.Telefono}";
                lblEmail.Text = $"Email: {_clienteSeleccionado.Email}";

            }
        }

        private Panel CrearPanelProductos()
        {
            var panel = new Panel
            {
                Width = FormHelper.ANCHO_FORM,
                Height = 350
            };

            // Título
            var lblTitulo = FormHelper.CrearLabel("Productos");
            lblTitulo.Font = new Font(lblTitulo.Font, FontStyle.Bold);
            lblTitulo.Location = new Point(20, 10);

            // Botón
            var btnAgregarProducto = FormHelper.CrearBotonPrimario("+ Agregar Producto", 200);
            btnAgregarProducto.Name = "btnAgregarProducto";
            btnAgregarProducto.Location = new Point(20, lblTitulo.Bottom + 10);
            btnAgregarProducto.Click += (s, e) => SeleccionarProducto();

            // Grid
            var dgvProductos = new DataGridView
            {
                Name = "dgvProductos",
                Location = new Point(20, btnAgregarProducto.Bottom + 10),
                Width = panel.Width - 40,
                Height = 200,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgvProductos.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn
                {
                    Name = "Nombre",
                    HeaderText = "Producto",
                    DataPropertyName = "NombreProducto",
                    Width = 200
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Cantidad",
                    HeaderText = "Cantidad",
                    DataPropertyName = "Cantidad",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Precio",
                    HeaderText = "Precio",
                    DataPropertyName = "Precio",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Subtotal",
                    HeaderText = "Subtotal",
                    DataPropertyName = "Subtotal",
                    Width = 100
                },
                new DataGridViewButtonColumn
                {
                    Name = "Quitar",
                    HeaderText = "Acción",
                    Text = "Quitar",
                    UseColumnTextForButtonValue = true,
                    Width = 70
                }
            });

            dgvProductos.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgvProductos.Columns["Quitar"].Index)
                {
                    _items.RemoveAt(e.RowIndex);
                    ActualizarGridProductos();
                    ActualizarResumen();
                }
            };

            panel.Controls.AddRange(new Control[] { lblTitulo, btnAgregarProducto, dgvProductos });
            return panel;
        }

        private Panel CrearPanelResumen()
        {
            var panel = new Panel
            {
                Width = FormHelper.ANCHO_FORM,
                AutoSize = true,
                Padding = new Padding(FormHelper.MARGEN)
            };

            // Lista de descuentos
            var lblDescuentos = FormHelper.CrearLabel("Descuentos Aplicables:");
            lblDescuentos.Location = new Point(FormHelper.MARGEN, 10); // Cambiamos esto
            lblDescuentos.Font = new Font(lblDescuentos.Font, FontStyle.Bold);

            var lstDescuentos = new ListBox
            {
                Name = "lstDescuentos",
                Location = new Point(FormHelper.MARGEN, lblDescuentos.Bottom + 5),
                Width = FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2),
                Height = 60
            };

            // Total
            var lblTotal = new Label
            {
                Name = "lblTotal",
                Text = "Total: $0,00",
                Location = new Point(FormHelper.MARGEN, lstDescuentos.Bottom + 10),
                AutoSize = true,
                Font = new Font(lblDescuentos.Font, FontStyle.Bold)
            };

            // Botón confirmar venta
            var btnConfirmar = FormHelper.CrearBotonPrimario("Confirmar Venta", FormHelper.ANCHO_FORM - (FormHelper.MARGEN * 2));
            btnConfirmar.Location = new Point(FormHelper.MARGEN, lblTotal.Bottom + 20);
            btnConfirmar.Click += (s, e) => ConfirmarVenta();

            panel.Controls.AddRange(new Control[] { lblDescuentos, lstDescuentos, lblTotal, btnConfirmar });
            return panel;
        }

        private void SeleccionarProducto()
        {
            if (_items.Count >= 10)
            {
                MessageBox.Show("No se pueden agregar más de 10 productos", "Límite alcanzado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var config = new ProductoSeleccionListadoConfig(producto => {
                if (producto != null)
                {
                    var cantidadForm = new Form
                    {
                        Text = "Cantidad",
                        Size = new Size(300, 150),
                        StartPosition = FormStartPosition.CenterScreen,
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        MaximizeBox = false,
                        MinimizeBox = false
                    };

                    var numCantidad = new NumericUpDown
                    {
                        Location = new Point(20, 20),
                        Width = 250,
                        Minimum = 1,
                        Maximum = producto.Stock,
                        Value = 1
                    };

                    var btnAceptar = new Button
                    {
                        Text = "Aceptar",
                        Location = new Point(20, 60),
                        DialogResult = DialogResult.OK
                    };

                    cantidadForm.Controls.AddRange(new Control[] { numCantidad, btnAceptar });

                    if (cantidadForm.ShowDialog() == DialogResult.OK)
                    {
                        var item = new ItemVenta
                        {
                            IdProducto = producto.Id,
                            NombreProducto = producto.Nombre,
                            Cantidad = (int)numCantidad.Value,
                            Precio = producto.Precio,
                            Categoria = producto.IdCategoria
                        };

                        _items.Add(item);
                        ActualizarGridProductos();
                        ActualizarResumen();
                    }
                }
            });

            var formProductos = new BaseListForm(config);
            formProductos.ShowDialog();
        }
        
        private void ActualizarGridProductos()
        {
            var dgvProductos = (DataGridView)panelProductos.Controls["dgvProductos"];
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = _items;

            // Actualizar visibilidad del botón Agregar Producto
            var btnAgregarProducto = (Button)panelProductos.Controls["btnAgregarProducto"];
            btnAgregarProducto.Visible = _items.Count < 10;
        }

        private void ActualizarResumen()
        {
            var lstDescuentos = (ListBox)panelResumen.Controls["lstDescuentos"];
            var lblTotal = (Label)panelResumen.Controls["lblTotal"];

            // Calcular descuentos aplicables
            lstDescuentos.Items.Clear();

            // Descuento Electro Hogar
            var montoElectroHogar = _items
                .Where(i => i.Categoria == Categoria.ElectroHogar)
                .Sum(i => i.Subtotal);

            if (montoElectroHogar > 100000)
            {
                lstDescuentos.Items.Add("5% Descuento Electro Hogar");
            }

            // Descuento Cliente Nuevo
            if (_clienteSeleccionado != null)
            {
                var clientesService = new Clientes();
                if (clientesService.EsClienteNuevo(_clienteSeleccionado.Id))
                {
                    lstDescuentos.Items.Add("5% Descuento Cliente Nuevo");
                }
            }

            // Calcular total
            double total = _items.Sum(i => i.Subtotal);
            lblTotal.Text = $"Total: ${total:N2}";
        }

        private void ConfirmarVenta()
        {
            try
            {
                if (_clienteSeleccionado == null)
                    throw new Exception("Debe seleccionar un cliente");

                if (_items.Count == 0)
                    throw new Exception("Debe agregar al menos un producto");

                if (MessageBox.Show("¿Confirma la venta?", "Confirmar Venta",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                var venta = new VentaCompuesta
                {
                    IdCliente = _clienteSeleccionado.Id,
                    Items = _items
                };

                _ventasService.RegistrarVenta(venta);
                FormHelper.MostrarEstado(lblEstado, "Venta registrada exitosamente", false);
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                FormHelper.MostrarEstado(lblEstado, ex.Message, true);
            }
        }

        private void LimpiarFormulario()
        {
            _clienteSeleccionado = null;
            _items.Clear();

            // Actualizar los labels del cliente en lugar de ocultar el panel
            var panelDatos = (Panel)panelCliente.Controls["panelDatosCliente"];
            var lblClienteInfo = (Label)panelDatos.Controls["lblClienteInfo"];
            var lblDireccion = (Label)panelDatos.Controls["lblDireccion"];
            var lblTelefono = (Label)panelDatos.Controls["lblTelefono"];
            var lblEmail = (Label)panelDatos.Controls["lblEmail"];
            var btnSeleccionar = panelDatos.Controls.OfType<Button>().First();

            lblClienteInfo.Text = "Ningún cliente seleccionado";
            lblDireccion.Text = "";
            lblTelefono.Text = "";
            lblEmail.Text = "";
            btnSeleccionar.Text = "Seleccionar Cliente";

            ActualizarGridProductos();
            ActualizarResumen();
        }
    }
}
