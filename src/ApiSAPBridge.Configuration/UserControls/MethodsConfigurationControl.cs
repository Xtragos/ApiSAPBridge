using ApiSAPBridge.Configuration.Models;
using ApiSAPBridge.Configuration.Services;

namespace ApiSAPBridge.Configuration.UserControls
{
    public partial class MethodsConfigurationControl : UserControl
    {
        private readonly IConfigurationService _configurationService;
        private SapAutomationConfiguration _configuration;

        public MethodsConfigurationControl(IConfigurationService configurationService, SapAutomationConfiguration configuration)
        {
            _configurationService = configurationService;
            _configuration = configuration;

            InitializeComponent();
            LoadConfiguration();
            InitializeDefaultEndpoints();
        }

        private void LoadConfiguration()
        {
            chkEnableSync.Checked = _configuration.EnableAutomaticSync;
            numSyncInterval.Value = _configuration.SyncIntervalMinutes;
            lblLastSync.Text = _configuration.LastSyncTime == default
                ? "Nunca ejecutado"
                : _configuration.LastSyncTime.ToString("dd/MM/yyyy HH:mm:ss");

            LoadEndpoints();
        }

        private void InitializeDefaultEndpoints()
        {
            if (!_configuration.Endpoints.Any())
            {
                _configuration.Endpoints = new List<SapEndpointConfiguration>
                {
                    new() { Name = "Departamentos", Endpoint = "/api/departamentos", Method = "POST", IsEnabled = true, Priority = 1 },
                    new() { Name = "Secciones", Endpoint = "/api/secciones", Method = "POST", IsEnabled = true, Priority = 2 },
                    new() { Name = "Familias", Endpoint = "/api/familias", Method = "POST", IsEnabled = true, Priority = 3 },
                    new() { Name = "Clientes", Endpoint = "/api/clientes", Method = "POST", IsEnabled = true, Priority = 4 },
                    new() { Name = "Vendedores", Endpoint = "/api/vendedores", Method = "POST", IsEnabled = true, Priority = 5 },
                    new() { Name = "Impuestos", Endpoint = "/api/impuestos", Method = "POST", IsEnabled = true, Priority = 6 },
                    new() { Name = "Formas de Pago", Endpoint = "/api/formaspago", Method = "POST", IsEnabled = true, Priority = 7 },
                    new() { Name = "Tarifas", Endpoint = "/api/tarifas", Method = "POST", IsEnabled = true, Priority = 8 },
                    new() { Name = "Artículos", Endpoint = "/api/articulos", Method = "POST", IsEnabled = true, Priority = 9 },
                    new() { Name = "Artículos Completos", Endpoint = "/api/articulos/completo", Method = "POST", IsEnabled = false, Priority = 10 },
                    new() { Name = "Líneas de Artículos", Endpoint = "/api/articulolineas", Method = "POST", IsEnabled = true, Priority = 11 },
                    new() { Name = "Precios", Endpoint = "/api/precios", Method = "POST", IsEnabled = true, Priority = 12 },
                    new() { Name = "Facturas", Endpoint = "/api/facturas", Method = "POST", IsEnabled = false, Priority = 13 }
                };
            }
        }

        private void LoadEndpoints()
        {
            dgvEndpoints.Rows.Clear();

            foreach (var endpoint in _configuration.Endpoints.OrderBy(e => e.Priority))
            {
                var row = dgvEndpoints.Rows.Add();
                row.Cells["colEnabled"].Value = endpoint.IsEnabled;
                row.Cells["colName"].Value = endpoint.Name;
                row.Cells["colEndpoint"].Value = endpoint.Endpoint;
                row.Cells["colMethod"].Value = endpoint.Method;
                row.Cells["colPriority"].Value = endpoint.Priority;
                row.Cells["colLastExecuted"].Value = endpoint.LastExecuted == default
                    ? "Nunca"
                    : endpoint.LastExecuted.ToString("dd/MM HH:mm");
                row.Cells["colRequiresAuth"].Value = endpoint.RequiresAuthentication;
                row.Tag = endpoint;
            }
        }

        private void chkEnableSync_CheckedChanged(object sender, EventArgs e)
        {
            bool isEnabled = chkEnableSync.Checked;
            numSyncInterval.Enabled = isEnabled;
            lblSyncInterval.Enabled = isEnabled;
            dgvEndpoints.Enabled = isEnabled;
            btnTestEndpoint.Enabled = isEnabled;
            btnExecuteNow.Enabled = isEnabled;
        }

        private void btnAddEndpoint_Click(object sender, EventArgs e)
        {
            using var form = new EndpointEditForm();
            var result = form.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var newEndpoint = form.GetEndpoint();
                newEndpoint.Priority = _configuration.Endpoints.Count + 1;
                _configuration.Endpoints.Add(newEndpoint);
                LoadEndpoints();
            }
        }

        private void btnEditEndpoint_Click(object sender, EventArgs e)
        {
            if (dgvEndpoints.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un endpoint para editar.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedRow = dgvEndpoints.SelectedRows[0];
            var endpoint = (SapEndpointConfiguration)selectedRow.Tag;

            using var form = new EndpointEditForm(endpoint);
            var result = form.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var updatedEndpoint = form.GetEndpoint();
                var index = _configuration.Endpoints.IndexOf(endpoint);
                _configuration.Endpoints[index] = updatedEndpoint;
                LoadEndpoints();
            }
        }

        private void btnDeleteEndpoint_Click(object sender, EventArgs e)
        {
            if (dgvEndpoints.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un endpoint para eliminar.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("¿Está seguro que desea eliminar el endpoint seleccionado?",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var selectedRow = dgvEndpoints.SelectedRows[0];
                var endpoint = (SapEndpointConfiguration)selectedRow.Tag;
                _configuration.Endpoints.Remove(endpoint);
                LoadEndpoints();
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            MoveEndpoint(-1);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            MoveEndpoint(1);
        }

        private void MoveEndpoint(int direction)
        {
            if (dgvEndpoints.SelectedRows.Count == 0) return;

            var selectedRow = dgvEndpoints.SelectedRows[0];
            var endpoint = (SapEndpointConfiguration)selectedRow.Tag;
            var currentIndex = _configuration.Endpoints.IndexOf(endpoint);
            var newIndex = currentIndex + direction;

            if (newIndex >= 0 && newIndex < _configuration.Endpoints.Count)
            {
                _configuration.Endpoints.RemoveAt(currentIndex);
                _configuration.Endpoints.Insert(newIndex, endpoint);

                // Actualizar prioridades
                for (int i = 0; i < _configuration.Endpoints.Count; i++)
                {
                    _configuration.Endpoints[i].Priority = i + 1;
                }

                LoadEndpoints();
                dgvEndpoints.Rows[newIndex].Selected = true;
            }
        }

        private void btnTestEndpoint_Click(object sender, EventArgs e)
        {
            if (dgvEndpoints.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un endpoint para probar.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedRow = dgvEndpoints.SelectedRows[0];
            var endpoint = (SapEndpointConfiguration)selectedRow.Tag;

            MessageBox.Show($"Probando endpoint: {endpoint.Name}\nURL: {endpoint.Endpoint}\nMétodo: {endpoint.Method}\n\n" +
                           "Nota: Esta es una simulación. En producción se haría una llamada HTTP real.",
                           "Prueba de Endpoint", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExecuteNow_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Desea ejecutar la sincronización SAP ahora?",
                "Ejecutar Sincronización", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _configuration.LastSyncTime = DateTime.Now;
                lblLastSync.Text = _configuration.LastSyncTime.ToString("dd/MM/yyyy HH:mm:ss");

                MessageBox.Show("Sincronización ejecutada correctamente.\n\n" +
                               "Nota: Esta es una simulación. En producción se ejecutarían los endpoints reales.",
                               "Sincronización Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvEndpoints_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvEndpoints.Rows[e.RowIndex].Tag is SapEndpointConfiguration endpoint)
            {
                var row = dgvEndpoints.Rows[e.RowIndex];

                endpoint.IsEnabled = Convert.ToBoolean(row.Cells["colEnabled"].Value);
                endpoint.RequiresAuthentication = Convert.ToBoolean(row.Cells["colRequiresAuth"].Value);
            }
        }

        public SapAutomationConfiguration GetConfiguration()
        {
            _configuration.EnableAutomaticSync = chkEnableSync.Checked;
            _configuration.SyncIntervalMinutes = (int)numSyncInterval.Value;

            return _configuration;
        }
    }

    // Formulario auxiliar para editar endpoints
    public partial class EndpointEditForm : Form
    {
        private SapEndpointConfiguration _endpoint;

        public EndpointEditForm(SapEndpointConfiguration endpoint = null)
        {
            _endpoint = endpoint ?? new SapEndpointConfiguration();
            InitializeComponent();
            LoadEndpoint();
        }

        public SapEndpointConfiguration GetEndpoint()
        {
            _endpoint.Name = txtName.Text;
            _endpoint.Endpoint = txtEndpoint.Text;
            _endpoint.Method = cmbMethod.Text;
            _endpoint.IsEnabled = chkEnabled.Checked;
            _endpoint.RequiresAuthentication = chkRequiresAuth.Checked;

            return _endpoint;
        }

        private void LoadEndpoint()
        {
            txtName.Text = _endpoint.Name;
            txtEndpoint.Text = _endpoint.Endpoint;
            cmbMethod.Text = _endpoint.Method;
            chkEnabled.Checked = _endpoint.IsEnabled;
            chkRequiresAuth.Checked = _endpoint.RequiresAuthentication;
        }

        private void InitializeComponent()
        {
            this.txtName = new TextBox();
            this.txtEndpoint = new TextBox();
            this.cmbMethod = new ComboBox();
            this.chkEnabled = new CheckBox();
            this.chkRequiresAuth = new CheckBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();

            this.Text = "Editar Endpoint";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Layout simple
            var y = 20;
            this.Controls.Add(new Label { Text = "Nombre:", Location = new Point(20, y), Size = new Size(80, 20) });
            this.txtName.Location = new Point(100, y);
            this.txtName.Size = new Size(250, 20);
            this.Controls.Add(this.txtName);

            y += 30;
            this.Controls.Add(new Label { Text = "Endpoint:", Location = new Point(20, y), Size = new Size(80, 20) });
            this.txtEndpoint.Location = new Point(100, y);
            this.txtEndpoint.Size = new Size(250, 20);
            this.Controls.Add(this.txtEndpoint);

            y += 30;
            this.Controls.Add(new Label { Text = "Método:", Location = new Point(20, y), Size = new Size(80, 20) });
            this.cmbMethod.Location = new Point(100, y);
            this.cmbMethod.Size = new Size(100, 20);
            this.cmbMethod.Items.AddRange(new[] { "GET", "POST", "PUT", "DELETE" });
            this.cmbMethod.Text = "POST";
            this.Controls.Add(this.cmbMethod);

            y += 30;
            this.chkEnabled.Text = "Habilitado";
            this.chkEnabled.Location = new Point(100, y);
            this.chkEnabled.Checked = true;
            this.Controls.Add(this.chkEnabled);

            y += 25;
            this.chkRequiresAuth.Text = "Requiere Autenticación";
            this.chkRequiresAuth.Location = new Point(100, y);
            this.chkRequiresAuth.Checked = true;
            this.Controls.Add(this.chkRequiresAuth);

            y += 40;
            this.btnOK.Text = "Aceptar";
            this.btnOK.Location = new Point(200, y);
            this.btnOK.DialogResult = DialogResult.OK;
            this.Controls.Add(this.btnOK);

            this.btnCancel.Text = "Cancelar";
            this.btnCancel.Location = new Point(290, y);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(this.btnCancel);
        }

        private TextBox txtName;
        private TextBox txtEndpoint;
        private ComboBox cmbMethod;
        private CheckBox chkEnabled;
        private CheckBox chkRequiresAuth;
        private Button btnOK;
        private Button btnCancel;
    }
}