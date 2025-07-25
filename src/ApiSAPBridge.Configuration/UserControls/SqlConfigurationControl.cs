using ApiSAPBridge.Configuration.Models;
using ApiSAPBridge.Configuration.Services;
using System.Data.SqlClient;

namespace ApiSAPBridge.Configuration.UserControls
{
    public partial class SqlConfigurationControl : UserControl
    {
        private readonly IConfigurationService _configurationService;
        private SqlServerConfiguration _configuration;

        public SqlConfigurationControl(IConfigurationService configurationService, SqlServerConfiguration configuration)
        {
            _configurationService = configurationService;
            _configuration = configuration;

            InitializeComponent();
            LoadConfiguration();
            SetupValidations();
        }

        private void LoadConfiguration()
        {
            // Cargar configuración en controles
            txtServer.Text = _configuration.Server;
            txtDatabase.Text = _configuration.Database;
            txtUsername.Text = _configuration.Username ?? "";
            txtPassword.Text = _configuration.Password ?? "";
            chkWindowsAuth.Checked = _configuration.UseWindowsAuthentication;
            numConnectionTimeout.Value = _configuration.ConnectionTimeout;
            chkTrustServerCertificate.Checked = _configuration.TrustServerCertificate;

            // Actualizar estado de controles
            UpdateAuthenticationControls();
        }

        private void SetupValidations()
        {
            // Validaciones en tiempo real
            txtServer.TextChanged += ValidateForm;
            txtDatabase.TextChanged += ValidateForm;
            txtUsername.TextChanged += ValidateForm;
            txtPassword.TextChanged += ValidateForm;
            chkWindowsAuth.CheckedChanged += OnAuthenticationModeChanged;
        }

        private void OnAuthenticationModeChanged(object sender, EventArgs e)
        {
            UpdateAuthenticationControls();
            ValidateForm(sender, e);
        }

        private void UpdateAuthenticationControls()
        {
            bool useWindowsAuth = chkWindowsAuth.Checked;

            txtUsername.Enabled = !useWindowsAuth;
            txtPassword.Enabled = !useWindowsAuth;
            lblUsername.Enabled = !useWindowsAuth;
            lblPassword.Enabled = !useWindowsAuth;

            if (useWindowsAuth)
            {
                txtUsername.Text = "";
                txtPassword.Text = "";
            }
        }

        private void ValidateForm(object sender, EventArgs e)
        {
            bool isValid = true;
            var errors = new List<string>();

            // Validar servidor
            if (string.IsNullOrWhiteSpace(txtServer.Text))
            {
                errors.Add("El servidor es requerido");
                isValid = false;
            }

            // Validar base de datos
            if (string.IsNullOrWhiteSpace(txtDatabase.Text))
            {
                errors.Add("La base de datos es requerida");
                isValid = false;
            }

            // Validar autenticación SQL
            if (!chkWindowsAuth.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    errors.Add("El usuario es requerido para autenticación SQL");
                    isValid = false;
                }
            }

            // Actualizar interfaz
            btnTestConnection.Enabled = isValid;
            lblValidationMessage.Text = isValid ? "✅ Configuración válida" : $"❌ {string.Join(", ", errors)}";
            lblValidationMessage.ForeColor = isValid ? Color.Green : Color.Red;
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                btnTestConnection.Enabled = false;
                btnTestConnection.Text = "Probando...";
                lblConnectionStatus.Text = "🔄 Probando conexión...";
                lblConnectionStatus.ForeColor = Color.Blue;

                // Crear configuración temporal
                var tempConfig = GetConfiguration();

                // Probar conexión
                var success = await _configurationService.TestSqlConnectionAsync(tempConfig);

                if (success)
                {
                    lblConnectionStatus.Text = "✅ Conexión exitosa";
                    lblConnectionStatus.ForeColor = Color.Green;

                    MessageBox.Show(ConfigurationConstants.Messages.CONNECTION_SUCCESS,
                        "Prueba de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblConnectionStatus.Text = "❌ Error de conexión";
                    lblConnectionStatus.ForeColor = Color.Red;

                    MessageBox.Show(ConfigurationConstants.Messages.CONNECTION_FAILED,
                        "Prueba de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblConnectionStatus.Text = "❌ Error de conexión";
                lblConnectionStatus.ForeColor = Color.Red;

                MessageBox.Show($"Error al probar la conexión: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTestConnection.Enabled = true;
                btnTestConnection.Text = "Probar Conexión";
            }
        }

        private void btnResetDefaults_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Está seguro que desea restaurar los valores por defecto?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _configuration = new SqlServerConfiguration();
                LoadConfiguration();
            }
        }

        private void btnGenerateConnectionString_Click(object sender, EventArgs e)
        {
            try
            {
                var config = GetConfiguration();
                var connectionString = config.GetConnectionString();

                using var form = new Form();
                var textBox = new TextBox();

                form.Text = "Cadena de Conexión Generada";
                form.Size = new Size(600, 200);
                form.StartPosition = FormStartPosition.CenterParent;

                textBox.Text = connectionString;
                textBox.Multiline = true;
                textBox.ReadOnly = true;
                textBox.Dock = DockStyle.Fill;
                textBox.ScrollBars = ScrollBars.Both;

                form.Controls.Add(textBox);
                form.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar cadena de conexión: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public SqlServerConfiguration GetConfiguration()
        {
            _configuration.Server = txtServer.Text.Trim();
            _configuration.Database = txtDatabase.Text.Trim();
            _configuration.Username = chkWindowsAuth.Checked ? null : txtUsername.Text.Trim();
            _configuration.Password = chkWindowsAuth.Checked ? null : txtPassword.Text;
            _configuration.UseWindowsAuthentication = chkWindowsAuth.Checked;
            _configuration.ConnectionTimeout = (int)numConnectionTimeout.Value;
            _configuration.TrustServerCertificate = chkTrustServerCertificate.Checked;

            return _configuration;
        }
    }
}