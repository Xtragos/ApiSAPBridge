using ApiSAPBridge.Configuration.Models;
using ApiSAPBridge.Configuration.Services;

namespace ApiSAPBridge.Configuration.UserControls
{
    public partial class SwaggerConfigurationControl : UserControl
    {
        private readonly IConfigurationService _configurationService;
        private SwaggerConfiguration _configuration;

        public SwaggerConfigurationControl(IConfigurationService configurationService, SwaggerConfiguration configuration)
        {
            _configurationService = configurationService;
            _configuration = configuration;

            InitializeComponent();
            LoadConfiguration();
            InitializeDefaultEndpoints();
        }

        private void LoadConfiguration()
        {
            chkEnableSwagger.Checked = _configuration.EnableSwagger;
            chkEnableSwaggerUI.Checked = _configuration.EnableSwaggerUI;

            LoadAllowedMethods();
            LoadHiddenEndpoints();
        }

        private void InitializeDefaultEndpoints()
        {
            if (!_configuration.HiddenEndpoints.Any())
            {
                _configuration.HiddenEndpoints = new List<SwaggerEndpointConfiguration>();
            }

            // Lista de todos los endpoints disponibles
            var allEndpoints = new List<SwaggerEndpointConfiguration>
            {
                new() { Controller = "Departamentos", Action = "CreateDepartamentos", Method = "POST", IsHidden = false },
                new() { Controller = "Departamentos", Action = "GetDepartamentos", Method = "GET", IsHidden = false },
                new() { Controller = "Secciones", Action = "CreateSecciones", Method = "POST", IsHidden = false },
                new() { Controller = "Secciones", Action = "GetSecciones", Method = "GET", IsHidden = false },
                new() { Controller = "Familias", Action = "CreateFamilias", Method = "POST", IsHidden = false },
                new() { Controller = "Familias", Action = "GetFamilias", Method = "GET", IsHidden = false },
                new() { Controller = "Clientes", Action = "CreateClientes", Method = "POST", IsHidden = false },
                new() { Controller = "Clientes", Action = "GetClientes", Method = "GET", IsHidden = false },
                new() { Controller = "Vendedores", Action = "CreateVendedores", Method = "POST", IsHidden = false },
                new() { Controller = "Vendedores", Action = "GetVendedores", Method = "GET", IsHidden = false },
                new() { Controller = "Impuestos", Action = "CreateImpuestos", Method = "POST", IsHidden = false },
                new() { Controller = "Impuestos", Action = "GetImpuestos", Method = "GET", IsHidden = false },
                new() { Controller = "FormasPago", Action = "CreateFormasPago", Method = "POST", IsHidden = false },
                new() { Controller = "FormasPago", Action = "GetFormasPago", Method = "GET", IsHidden = false },
                new() { Controller = "Tarifas", Action = "CreateTarifas", Method = "POST", IsHidden = false },
                new() { Controller = "Tarifas", Action = "GetTarifas", Method = "GET", IsHidden = false },
                new() { Controller = "Articulos", Action = "CreateArticulos", Method = "POST", IsHidden = false },
                new() { Controller = "Articulos", Action = "GetArticulos", Method = "GET", IsHidden = false },
                new() { Controller = "ArticuloLineas", Action = "CreateArticuloLineas", Method = "POST", IsHidden = false },
                new() { Controller = "ArticuloLineas", Action = "GetArticuloLineas", Method = "GET", IsHidden = false },
                new() { Controller = "Precios", Action = "CreatePrecios", Method = "POST", IsHidden = false },
                new() { Controller = "Precios", Action = "GetPrecios", Method = "GET", IsHidden = false },
                new() { Controller = "Facturas", Action = "CreateFacturas", Method = "POST", IsHidden = true },
                new() { Controller = "Facturas", Action = "GetFacturas", Method = "GET", IsHidden = false }
            };

            // Agregar endpoints faltantes
            foreach (var endpoint in allEndpoints)
            {
                if (!_configuration.HiddenEndpoints.Any(h =>
                    h.Controller == endpoint.Controller &&
                    h.Action == endpoint.Action &&
                    h.Method == endpoint.Method))
                {
                    _configuration.HiddenEndpoints.Add(endpoint);
                }
            }
        }

        private void LoadAllowedMethods()
        {
            chklstMethods.Items.Clear();
            var allMethods = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" };

            foreach (var method in allMethods)
            {
                var isChecked = _configuration.AllowedMethods.Contains(method);
                chklstMethods.Items.Add(method, isChecked);
            }
        }

        private void LoadHiddenEndpoints()
        {
            dgvEndpoints.Rows.Clear();

            foreach (var endpoint in _configuration.HiddenEndpoints.OrderBy(e => e.Controller).ThenBy(e => e.Action))
            {
                var row = dgvEndpoints.Rows.Add();
                row.Cells["colController"].Value = endpoint.Controller;
                row.Cells["colAction"].Value = endpoint.Action;
                row.Cells["colMethod"].Value = endpoint.Method;
                row.Cells["colVisible"].Value = !endpoint.IsHidden;
                row.Tag = endpoint;
            }
        }

        private void chkEnableSwagger_CheckedChanged(object sender, EventArgs e)
        {
            bool isEnabled = chkEnableSwagger.Checked;
            chkEnableSwaggerUI.Enabled = isEnabled;
            chklstMethods.Enabled = isEnabled;
            dgvEndpoints.Enabled = isEnabled;
            btnHideSelected.Enabled = isEnabled;
            btnShowSelected.Enabled = isEnabled;
            btnHideAll.Enabled = isEnabled;
            btnShowAll.Enabled = isEnabled;
            btnFilterByMethod.Enabled = isEnabled;
        }

        private void chklstMethods_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Usar BeginInvoke para ejecutar después de que se actualice la UI
            this.BeginInvoke((MethodInvoker)delegate
            {
                UpdateAllowedMethods();
            });
        }

        private void UpdateAllowedMethods()
        {
            _configuration.AllowedMethods.Clear();
            foreach (var item in chklstMethods.CheckedItems)
            {
                _configuration.AllowedMethods.Add(item.ToString());
            }
        }

        private void btnHideSelected_Click(object sender, EventArgs e)
        {
            SetSelectedEndpointsVisibility(false);
        }

        private void btnShowSelected_Click(object sender, EventArgs e)
        {
            SetSelectedEndpointsVisibility(true);
        }

        private void SetSelectedEndpointsVisibility(bool isVisible)
        {
            foreach (DataGridViewRow row in dgvEndpoints.SelectedRows)
            {
                if (row.Tag is SwaggerEndpointConfiguration endpoint)
                {
                    endpoint.IsHidden = !isVisible;
                    row.Cells["colVisible"].Value = isVisible;
                }
            }
        }

        private void btnHideAll_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Está seguro que desea ocultar todos los endpoints en Swagger?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                SetAllEndpointsVisibility(false);
            }
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            SetAllEndpointsVisibility(true);
        }

        private void SetAllEndpointsVisibility(bool isVisible)
        {
            foreach (var endpoint in _configuration.HiddenEndpoints)
            {
                endpoint.IsHidden = !isVisible;
            }
            LoadHiddenEndpoints();
        }

        private void btnFilterByMethod_Click(object sender, EventArgs e)
        {
            using var form = new MethodFilterForm(_configuration.AllowedMethods);
            var result = form.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var selectedMethods = form.GetSelectedMethods();
                SetEndpointVisibilityByMethods(selectedMethods);
            }
        }

        private void SetEndpointVisibilityByMethods(List<string> methods)
        {
            foreach (var endpoint in _configuration.HiddenEndpoints)
            {
                endpoint.IsHidden = !methods.Contains(endpoint.Method);
            }
            LoadHiddenEndpoints();
        }

        private void dgvEndpoints_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvEndpoints.Columns["colVisible"].Index)
            {
                var row = dgvEndpoints.Rows[e.RowIndex];
                if (row.Tag is SwaggerEndpointConfiguration endpoint)
                {
                    endpoint.IsHidden = !Convert.ToBoolean(row.Cells["colVisible"].Value);
                }
            }
        }

        private void btnPreviewSwagger_Click(object sender, EventArgs e)
        {
            var visibleEndpoints = _configuration.HiddenEndpoints.Where(e => !e.IsHidden).ToList();
            var hiddenEndpoints = _configuration.HiddenEndpoints.Where(e => e.IsHidden).ToList();

            var preview = $"CONFIGURACIÓN SWAGGER\n" +
                         $"====================\n\n" +
                         $"Swagger Habilitado: {(_configuration.EnableSwagger ? "SÍ" : "NO")}\n" +
                         $"Swagger UI Habilitado: {(_configuration.EnableSwaggerUI ? "SÍ" : "NO")}\n\n" +
                         $"Métodos Permitidos: {string.Join(", ", _configuration.AllowedMethods)}\n\n" +
                         $"Endpoints Visibles: {visibleEndpoints.Count}\n" +
                         $"Endpoints Ocultos: {hiddenEndpoints.Count}\n\n" +
                         $"ENDPOINTS OCULTOS:\n" +
                         $"==================\n";

            foreach (var endpoint in hiddenEndpoints.OrderBy(e => e.Controller))
            {
                preview += $"- {endpoint.Method} {endpoint.Controller}.{endpoint.Action}\n";
            }

            using var form = new PreviewForm("Vista Previa Swagger", preview);
            form.ShowDialog(this);
        }

        public SwaggerConfiguration GetConfiguration()
        {
            _configuration.EnableSwagger = chkEnableSwagger.Checked;
            _configuration.EnableSwaggerUI = chkEnableSwaggerUI.Checked;

            UpdateAllowedMethods();

            return _configuration;
        }
    }

    // Formularios auxiliares
    public partial class MethodFilterForm : Form
    {
        private CheckedListBox chklstMethods;
        private Button btnOK;
        private Button btnCancel;

        public MethodFilterForm(List<string> allowedMethods)
        {
            InitializeComponent();
            LoadMethods(allowedMethods);
        }

        public List<string> GetSelectedMethods()
        {
            return chklstMethods.CheckedItems.Cast<string>().ToList();
        }

        private void LoadMethods(List<string> allowedMethods)
        {
            foreach (var method in allowedMethods)
            {
                chklstMethods.Items.Add(method, true);
            }
        }

        private void InitializeComponent()
        {
            this.chklstMethods = new CheckedListBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();

            this.Text = "Filtrar por Métodos";
            this.Size = new Size(300, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            this.chklstMethods.Location = new Point(20, 20);
            this.chklstMethods.Size = new Size(240, 300);
            this.Controls.Add(this.chklstMethods);

            this.btnOK.Text = "Aplicar";
            this.btnOK.Location = new Point(100, 340);
            this.btnOK.DialogResult = DialogResult.OK;
            this.Controls.Add(this.btnOK);

            this.btnCancel.Text = "Cancelar";
            this.btnCancel.Location = new Point(185, 340);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(this.btnCancel);
        }
    }

    public partial class PreviewForm : Form
    {
        public PreviewForm(string title, string content)
        {
            InitializeComponent();
            this.Text = title;
            textBox.Text = content;
        }

        private void InitializeComponent()
        {
            this.textBox = new TextBox();
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            this.textBox.Multiline = true;
            this.textBox.ReadOnly = true;
            this.textBox.ScrollBars = ScrollBars.Both;
            this.textBox.Dock = DockStyle.Fill;
            this.textBox.Font = new Font("Consolas", 9);
            this.Controls.Add(this.textBox);
        }

        private TextBox textBox;
    }
}