using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TiendaApp.Models;
using TiendaApp.Controllers;

namespace TiendaApp.Views
{
    public partial class frmNuevaVenta : Form
    {
        private Venta nuevaVenta;
        private VentaController controller;
        
        // Variables temporales para el artículo seleccionado antes de agregarlo a la tabla
        private int articuloSeleccionadoId = 0;
        private string articuloSeleccionadoNombre = "";
        private decimal articuloSeleccionadoPrecio = 0;

        // Declaración de controles de UI
        private Label lblTitulo;
        private TextBox txtClienteNombre;
        private Button btnBuscarCliente;
        private TextBox txtArticuloNombre;
        private TextBox txtPrecio;
        private TextBox txtCantidad;
        private Button btnBuscarArticulo;
        private Button btnAgregar;
        private DataGridView dgvDetalles;
        private TextBox txtTotal;
        private Button btnGuardar;
        private Button btnCancelar;

        public frmNuevaVenta()
        {
            InitializeComponent();
            nuevaVenta = new Venta();
            controller = new VentaController();

            // Configuración UX (Criterio A): Permitir cerrar con la tecla ESC
            this.KeyPreview = true;
            this.KeyDown += FrmNuevaVenta_KeyDown;
        }

        private void FrmNuevaVenta_Load(object sender, EventArgs e)
        {
            // Configuración UX (Criterio A): Foco automático al abrir la ventana
            btnBuscarCliente.Focus();
        }

        // --- EVENTOS DE INTERACCIÓN ---

        private void BtnBuscarCliente_Click(object sender, EventArgs e)
        {
            // Aquí el Desarrollador C se conecta con el trabajo del Desarrollador A
            /* using (frmBusquedaCliente buscador = new frmBusquedaCliente())
            {
                if (buscador.ShowDialog() == DialogResult.OK)
                {
                    nuevaVenta.ClienteId = buscador.ClienteSeleccionado.Id;
                    txtClienteNombre.Text = buscador.ClienteSeleccionado.Nombre;
                    btnBuscarArticulo.Focus(); // UX: Pasar foco al siguiente paso lógico
                }
            }
            */
            
            // Simulación temporal para que puedan probar la UI antes de integrar:
            nuevaVenta.ClienteId = 1;
            txtClienteNombre.Text = "Cliente de Prueba S.A.";
            btnBuscarArticulo.Focus();
        }

        private void BtnBuscarArticulo_Click(object sender, EventArgs e)
        {
            // Aquí se conectan con el trabajo del Desarrollador B
            /*
            using (frmBusquedaArticulo buscador = new frmBusquedaArticulo())
            {
                if (buscador.ShowDialog() == DialogResult.OK)
                {
                    articuloSeleccionadoId = buscador.ArticuloSeleccionado.Id;
                    articuloSeleccionadoNombre = buscador.ArticuloSeleccionado.Nombre;
                    articuloSeleccionadoPrecio = buscador.ArticuloSeleccionado.Precio;
                    
                    txtArticuloNombre.Text = articuloSeleccionadoNombre;
                    txtPrecio.Text = articuloSeleccionadoPrecio.ToString("C");
                    txtCantidad.Focus(); // UX: Pasar foco a la cantidad
                }
            }
            */

            // Simulación temporal:
            articuloSeleccionadoId = 1;
            articuloSeleccionadoNombre = "Teclado Mecánico";
            articuloSeleccionadoPrecio = 450.50m;
            txtArticuloNombre.Text = articuloSeleccionadoNombre;
            txtPrecio.Text = articuloSeleccionadoPrecio.ToString("C");
            txtCantidad.Focus();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (articuloSeleccionadoId == 0 || string.IsNullOrEmpty(txtCantidad.Text))
            {
                MessageBox.Show("Seleccione un artículo e ingrese una cantidad válida.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int cantidad = int.Parse(txtCantidad.Text);
            decimal subtotal = cantidad * articuloSeleccionadoPrecio;

            // Agregar al modelo
            DetalleVenta detalle = new DetalleVenta
            {
                ArticuloId = articuloSeleccionadoId,
                ArticuloNombre = articuloSeleccionadoNombre,
                Cantidad = cantidad,
                Subtotal = subtotal
            };
            nuevaVenta.Detalles.Add(detalle);

            // Actualizar Total
            nuevaVenta.Total += subtotal;
            txtTotal.Text = nuevaVenta.Total.ToString("C");

            // Actualizar DataGridView
            ActualizarGrid();

            // Limpiar campos temporales para el siguiente artículo
            articuloSeleccionadoId = 0;
            txtArticuloNombre.Clear();
            txtPrecio.Clear();
            txtCantidad.Clear();
            btnBuscarArticulo.Focus();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (nuevaVenta.ClienteId == 0 || nuevaVenta.Detalles.Count == 0)
            {
                MessageBox.Show("Debe seleccionar un cliente y agregar al menos un artículo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                controller.RegistrarVenta(nuevaVenta);
                MessageBox.Show("Venta registrada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Cierra el formulario tras venta exitosa
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            // UX Nivel A: Validación en tiempo real (solo acepta números y tecla de borrar)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void FrmNuevaVenta_KeyDown(object sender, KeyEventArgs e)
        {
            // UX Nivel A: Cerrar ventana secundaria con ESC
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void ActualizarGrid()
        {
            dgvDetalles.DataSource = null;
            dgvDetalles.DataSource = nuevaVenta.Detalles;
            
            // Ocultar columnas técnicas que el usuario no necesita ver
            if (dgvDetalles.Columns["Id"] != null) dgvDetalles.Columns["Id"].Visible = false;
            if (dgvDetalles.Columns["VentaId"] != null) dgvDetalles.Columns["VentaId"].Visible = false;
            if (dgvDetalles.Columns["ArticuloId"] != null) dgvDetalles.Columns["ArticuloId"].Visible = false;
        }

        // --- CÓDIGO DE DISEÑO (Reemplaza al diseñador visual de VS Studio) ---
        private void InitializeComponent()
        {
            this.lblTitulo = new Label();
            this.txtClienteNombre = new TextBox();
            this.btnBuscarCliente = new Button();
            this.txtArticuloNombre = new TextBox();
            this.txtPrecio = new TextBox();
            this.txtCantidad = new TextBox();
            this.btnBuscarArticulo = new Button();
            this.btnAgregar = new Button();
            this.dgvDetalles = new DataGridView();
            this.txtTotal = new TextBox();
            this.btnGuardar = new Button();
            this.btnCancelar = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalles)).BeginInit();
            this.SuspendLayout();

            // Configuración general del Formulario
            this.Text = "BarrioTec - Nueva Venta";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += FrmNuevaVenta_Load;

            // lblTitulo
            this.lblTitulo.Text = "Registro de Transacción";
            this.lblTitulo.Font = new Font("Arial", 14, FontStyle.Bold);
            this.lblTitulo.Location = new Point(20, 20);
            this.lblTitulo.AutoSize = true;

            // txtClienteNombre (Criterio A: ReadOnly para evitar errores manuales)
            this.txtClienteNombre.Location = new Point(20, 60);
            this.txtClienteNombre.Size = new Size(300, 25);
            this.txtClienteNombre.ReadOnly = true;
            this.txtClienteNombre.PlaceholderText = "Cliente seleccionado...";
            this.txtClienteNombre.TabIndex = 99; // Fuera del orden de tabulación

            // btnBuscarCliente (Criterio A: TabIndex = 1)
            this.btnBuscarCliente.Text = "1. Buscar Cliente";
            this.btnBuscarCliente.Location = new Point(330, 58);
            this.btnBuscarCliente.Size = new Size(120, 28);
            this.btnBuscarCliente.TabIndex = 1;
            this.btnBuscarCliente.Click += BtnBuscarCliente_Click;

            // txtArticuloNombre (ReadOnly)
            this.txtArticuloNombre.Location = new Point(20, 100);
            this.txtArticuloNombre.Size = new Size(200, 25);
            this.txtArticuloNombre.ReadOnly = true;
            this.txtArticuloNombre.PlaceholderText = "Artículo...";

            // txtPrecio (ReadOnly)
            this.txtPrecio.Location = new Point(230, 100);
            this.txtPrecio.Size = new Size(80, 25);
            this.txtPrecio.ReadOnly = true;
            this.txtPrecio.PlaceholderText = "Precio";

            // txtCantidad (Criterio A: TabIndex = 3, Solo números)
            this.txtCantidad.Location = new Point(320, 100);
            this.txtCantidad.Size = new Size(60, 25);
            this.txtCantidad.PlaceholderText = "Cant.";
            this.txtCantidad.TabIndex = 3;
            this.txtCantidad.KeyPress += TxtCantidad_KeyPress;

            // btnBuscarArticulo (Criterio A: TabIndex = 2)
            this.btnBuscarArticulo.Text = "2. Buscar Art.";
            this.btnBuscarArticulo.Location = new Point(390, 98);
            this.btnBuscarArticulo.Size = new Size(90, 28);
            this.btnBuscarArticulo.TabIndex = 2;
            this.btnBuscarArticulo.Click += BtnBuscarArticulo_Click;

            // btnAgregar (Criterio A: TabIndex = 4)
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.Location = new Point(490, 98);
            this.btnAgregar.Size = new Size(70, 28);
            this.btnAgregar.TabIndex = 4;
            this.btnAgregar.Click += BtnAgregar_Click;

            // dgvDetalles (Criterio A: Anchor para redimensionar)
            this.dgvDetalles.Location = new Point(20, 140);
            this.dgvDetalles.Size = new Size(540, 220);
            this.dgvDetalles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvDetalles.AllowUserToAddRows = false;
            this.dgvDetalles.ReadOnly = true;
            this.dgvDetalles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // txtTotal (ReadOnly)
            this.txtTotal.Location = new Point(440, 370);
            this.txtTotal.Size = new Size(120, 25);
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Font = new Font("Arial", 12, FontStyle.Bold);
            this.txtTotal.Text = "$0.00";
            this.txtTotal.TextAlign = HorizontalAlignment.Right;
            this.txtTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            // btnGuardar (Criterio A: TabIndex = 5)
            this.btnGuardar.Text = "Procesar Venta";
            this.btnGuardar.Location = new Point(440, 410);
            this.btnGuardar.Size = new Size(120, 35);
            this.btnGuardar.TabIndex = 5;
            this.btnGuardar.BackColor = Color.LightGreen;
            this.btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnGuardar.Click += BtnGuardar_Click;

            // btnCancelar
            this.btnCancelar.Text = "Cancelar (ESC)";
            this.btnCancelar.Location = new Point(310, 410);
            this.btnCancelar.Size = new Size(120, 35);
            this.btnCancelar.TabIndex = 6;
            this.btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancelar.Click += (s, e) => this.Close();

            // Agregar controles al formulario
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.txtClienteNombre);
            this.Controls.Add(this.btnBuscarCliente);
            this.Controls.Add(this.txtArticuloNombre);
            this.Controls.Add(this.txtPrecio);
            this.Controls.Add(this.txtCantidad);
            this.Controls.Add(this.btnBuscarArticulo);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.dgvDetalles);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);

            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}