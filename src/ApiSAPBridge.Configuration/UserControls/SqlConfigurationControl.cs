using ApiSAPBridge.Configuration.Models;
using ApiSAPBridge.Configuration.Services;
using System.ComponentModel;
using System.Data.SqlClient;
using Serilog;

namespace ApiSAPBridge.Configuration.UserControls
{
    public partial class SqlConfigurationControl : UserControl
    {
        private readonly IConfigurationService _configurationService;
        private SqlServerConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly Timer _validationTimer;
        private bool _isUpdatingControls = false;

        public SqlConfigurationControl(IConfigurationService configurationService, SqlServerConfiguration configuration)
        {
            _configurationService = configurationService;
            _configuration = configuration ?? new SqlServerConfiguration();
            _logger = Log.ForContext<SqlConfigurationControl>();

            InitializeComponent();

            // Timer para validaciones con delay
            _validationTimer = new Timer();
            _validationTimer.Interval = 500; // 500ms delay
            _validationTimer.Tick += ValidationTimer_Tick;

            LoadConfiguration();
            AttachEventHandlers();
        }

        private void LoadConfiguration()
        {
            _isUpdatingControls = true;

            try
            {
                // Cargar configuración en controles
                txtServer.Text = _configuration.Server;
                txtDatabase.Text = _configuration.Database;
                txtUsername.Text = _configuration.Username ?? "";
                txtPassword.Text = _configuration.Password ?? "";
                chkWindowsAuth.Checked = _configuration.UseWindowsAuthentication;
                numConnectionTimeout.Value = _configuration.ConnectionTimeout;
                chkTrustServerCert.Checked = _configuration.TrustServerCertificate;

                // Actualizar estado de controles
                UpdateAuthenticationControls();
                UpdateConnectionString();
                ValidateConfiguration();

                _logger.Information("Configuración SQL cargada en controles");
            }
            finally
            {
                _isUpdatingControls = false;
            }
        }

        private void AttachEventHandlers()
        {
            // Eventos de cambio para validación automática
            txtServer.TextChanged += Control_Changed;
            txtDatabase.TextChanged += Control_Changed;
            txtUsername.TextChanged += Control_Changed;
            txtPassword.TextChanged += Control_Changed;
            chkWindowsAuth.CheckedChanged += WindowsAuth_CheckedChanged;
            numConnectionTimeout.ValueChanged += Control_Changed;
            chkTrustServerCert.CheckedChanged += Control_Changed;

            // Eventos de validación en tiempo real
            txtServer.Leave += ValidateServer;
            txtDatabase.Leave += ValidateDatabase;
            txtUsername.Leave += ValidateUsername;
            txtPassword.Leave += ValidatePassword;
        }

        private void Control_Changed(object sender, EventArgs e)
        {
            if (_isUpdatingControls) return;

            // Restart timer para validación con delay
            _validationTimer.Stop();
            _validationTimer.Start();

            UpdateConnectionString();
        }

        private void ValidationTimer_Tick(object sender, EventArgs e)
        {
            _validationTimer.Stop();
            ValidateConfiguration();
        }

        private void WindowsAuth_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAuthenticationControls();
            Control_Changed(sender, e);
        }

        private void UpdateAuthenticationControls()
        {
            bool isWindowsAuth = chkWindowsAuth.Checked;

            txtUsername.Enabled = !isWindowsAuth;
            txtPassword.Enabled = !isWindowsAuth;
            lblUsername.Enabled = !isWindowsAuth;
            lblPassword.Enabled = !isWindowsAuth;

            if (isWindowsAuth)
            {
                txtUsername.BackColor = SystemColors.Control;
                txtPassword.BackColor = SystemColors.Control;
            }
            else
            {
                txtUsername.BackColor = SystemColors.Window;
                txtPassword.BackColor = SystemColors.Window;
            }
        }

        private void UpdateConnectionString()
        {
            try
            {
                var tempConfig = GetConfiguration();
                txtConnectionString.Text = tempConfig.GetConnectionString();
                txtConnectionString.BackColor = SystemColors.Window;
            }
            catch (Exception ex)
            {
                txtConnectionString.Text = $"Error: {ex.Message}";
                txtConnectionString.BackColor = Color.LightCoral;
            }
        }

        private void ValidateConfiguration()
        {
            var errors = new List<string>();

            // Validar servidor
            if (string.IsNullOrWhiteSpace(txtServer.Text))
            {
                errors.Add("El servidor es requerido");
                SetControlError(txtServer, "Campo requerido");
            }
            else
            {
                ClearControlError(txtServer);
            }

            // Validar base de datos
            if (string.IsNullOrWhiteSpace(txtDatabase.Text))
            {
                errors.Add("La base de datos es requerida");
                SetControlError(txtDatabase, "Campo requerido");
            }
            else
            {
                ClearControlError(txtDatabase);
            }

            // Validar credenciales SQL Server
            if (!chkWindowsAuth.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    errors.Add("El usuario es requerido para autenticación SQL Server");
                    SetControlError(txtUsername, "Campo requerido");
                }
                else
                {
                    ClearControlError(txtUsername);
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    errors.Add("La contraseña es requerida para autenticación SQL Server");
                    SetControlError(txtPassword, "Campo requerido");
                }
                else
                {
                    ClearControlError(txtPassword);
                }
            }

            // Actualizar estado de validación
            bool isValid = !errors.Any();
            btnTestConnection.Enabled = isValid;

            if (isValid)
            {
                lblValidationStatus.Text = "✅ Configuración válida";
                lblValidationStatus.ForeColor = Color.Green;
            }
            else
            {
                lblValidationStatus.Text = $"❌ {errors.Count} error(es) encontrado(s)";
                lblValidationStatus.ForeColor = Color.Red;
                toolTip.SetToolTip(lblValidationStatus, string.Join("\n", errors));
            }
        }

        private void SetControlError(Control control, string message)
        {
            control.BackColor = Color.LightCoral;
            toolTip.SetToolTip(control, message);
        }

        private void ClearControlError(Control control)
        {
            control.BackColor = SystemColors.Window;
            toolTip.SetToolTip(control, "");
        }

        #region Validaciones específicas

        private void ValidateServer(object sender, EventArgs e)
        {
            var text = txtServer.Text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                SetControlError(txtServer, "El servidor es requerido");
                return;
            }

            // Validaciones adicionales del formato del servidor
            if (text.Contains(" ") && !text.Contains("\\"))
            {
                SetControlError(txtServer, "Formato de servidor inválido");
                return;
            }

            ClearControlError(txtServer);
        }

        private void ValidateDatabase(object sender, EventArgs e)
        {
            var text = txtDatabase.Text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                SetControlError(txtDatabase, "La base de datos es requerida");
                return;
            }

            // Validar caracteres no permitidos
            char[] invalidChars = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            if (text.IndexOfAny(invalidChars) >= 0)
            {
                SetControlError(txtDatabase, "Nombre de base de datos contiene caracteres inválidos");
                return;
            }

            ClearControlError(txtDatabase);
        }

        private void ValidateUsername(object sender, EventArgs e)
        {
            if (chkWindowsAuth.Checked) return;

            var text = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                SetControlError(txtUsername, "El usuario es requerido");
                return;
            }

            ClearControlError(txtUsername);
        }

        private void ValidatePassword(object sender, EventArgs e)
        {
            if (chkWindowsAuth.Checked) return;

            var text = txtPassword.Text;

            if (string.IsNullOrEmpty(text))
            {
                SetControlError(txtPassword, "La contraseña es requerida");
                return;
            }

            ClearControlError(txtPassword);
        }

        #endregion

        #region Eventos de botones

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            await TestConnection();
        }

        private async Task TestConnection()
        {
            btnTestConnection.Enabled = false;
            lblConnectionStatus.Text = "🔄 Probando conexión...";
            lblConnectionStatus.ForeColor = Color.Blue;
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Visible = true;

            try
            {
                var testConfig = GetConfiguration();
                _logger.Information("Iniciando prueba de conexión a {Server}/{Database}", testConfig.Server, testConfig.Database);

                var isConnected = await _configurationService.TestSqlConnectionAsync(testConfig);

                if (isConnected)
                {
                    lblConnectionStatus.Text = "✅ Conexión exitosa";
                    lblConnectionStatus.ForeColor = Color.Green;

                    MessageBox.Show(
                        $"Conexión exitosa a:\nServidor: {testConfig.Server}\nBase de datos: {testConfig.Database}",
                        "Prueba de Conexión",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    _logger.Information("Conexión SQL exitosa");
                }
                else
                {
                    lblConnectionStatus.Text = "❌ Error de conexión";
                    lblConnectionStatus.ForeColor = Color.Red;

                    MessageBox.Show(
                        "No se pudo conectar a la base de datos.\nVerifique la configuración e intente nuevamente.",
                        "Error de Conexión",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    _logger.Warning("Fallo en prueba de conexión SQL");
                }
            }
            catch (Exception ex)
            {
                lblConnectionStatus.Text = "❌ Error de conexión";
                lblConnectionStatus.ForeColor = Color.Red;

                MessageBox.Show(
                    $"Error al probar la conexión:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                _logger.Error(ex, "Error en prueba de conexión SQL");
            }
            finally
            {
                btnTestConnection.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private void btnResetDefaults_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "¿Está seguro que desea restaurar los valores por defecto?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _configuration = new SqlServerConfiguration
                {
                    Server = ConfigurationConstants.DefaultValues.DEFAULT_SERVER,
                    Database = ConfigurationConstants.DefaultValues.DEFAULT_DATABASE,
                    UseWindowsAuthentication = true,
                    ConnectionTimeout = ConfigurationConstants.DefaultValues.DEFAULT_CONNECTION_TIMEOUT,
                    TrustServerCertificate = true
                };

                LoadConfiguration();
                _logger.Information("Configuración SQL restaurada a valores por defecto");
            }
        }

        private void btnCopyConnectionString_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtConnectionString.Text);

                // Mostrar feedback temporal
                var originalText = btnCopyConnectionString.Text;
                btnCopyConnectionString.Text = "¡Copiado!";
                btnCopyConnectionString.BackColor = Color.LightGreen;

                Timer resetTimer = new Timer();
                resetTimer.Interval = 2000;
                resetTimer.Tick += (s, args) =>
                {
                    btnCopyConnectionString.Text = originalText;
                    btnCopyConnectionString.BackColor = SystemColors.Control;
                    resetTimer.Stop();
                    resetTimer.Dispose();
                };
                resetTimer.Start();

                _logger.Information("Cadena de conexión copiada al portapapeles");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al copiar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.Error(ex, "Error al copiar cadena de conexión");
            }
        }

        #endregion

        public SqlServerConfiguration GetConfiguration()
        {
            return new SqlServerConfiguration
            {
                Server = txtServer.Text.Trim(),
                Database = txtDatabase.Text.Trim(),
                Username = chkWindowsAuth.Checked ? null : txtUsername.Text.Trim(),
                Password = chkWindowsAuth.Checked ? null : txtPassword.Text,
                UseWindowsAuthentication = chkWindowsAuth.Checked,
                ConnectionTimeout = (int)numConnectionTimeout.Value,
                TrustServerCertificate = chkTrustServerCert.Checked
            };
        }

        public void UpdateConfiguration(SqlServerConfiguration configuration)
        {
            _configuration = configuration;
            LoadConfiguration();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _validationTimer?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}