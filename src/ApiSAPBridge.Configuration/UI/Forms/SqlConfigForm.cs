using System.Drawing;
using System.Windows.Forms;
using ApiSAPBridge.Configuration.Models;
using ApiSAPBridge.Configuration.Services;
using ApiSAPBridge.Configuration.UI.Controls;
using ApiSAPBridge.Configuration.Utils;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Configuration.UI.Forms
{
    public partial class SqlConfigForm : Form
    {
        private readonly ILogger<SqlConfigForm> _logger;
        private readonly IConfigurationService _configurationService;
        private readonly IDatabaseTestService _databaseTestService;

        private TextBox _serverTextBox;
        private TextBox _databaseTextBox;
        private TextBox _usernameTextBox;
        private TextBox _passwordTextBox;
        private CheckBox _integratedSecurityCheckBox;
        private NumericUpDown _timeoutNumericUpDown;
        private Button _testConnectionButton;
        private Button _saveButton;
        private ConnectionTestPanel _connectionTestPanel;

        public SqlConfigForm(
            ILogger<SqlConfigForm> logger,
            IConfigurationService configurationService,
            IDatabaseTestService databaseTestService)
        {
            _logger = logger;
            _configurationService = configurationService;
            _databaseTestService = databaseTestService;

            InitializeComponent();
            SetupControls();
            LoadConfiguration();
        }

        private void SetupControls()
        {
            Text = "Configuración SQL Server";
            Size = new Size(700, 500);
            BackColor = Color.White;

            // Título
            var titleLabel = new Label
            {
                Text = "🗄️ Configuración de Base de Datos SQL Server",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Location = new Point(30, 20),
                Size = new Size(640, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Servidor
            var serverLabel = new Label
            {
                Text = "Servidor SQL:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(30, 80),
                Size = new Size(200, 20),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            _serverTextBox = new TextBox
            {
                Location = new Point(30, 105),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 9),
                Text = "localhost\\SQLEXPRESS"
            };

            // Base de datos
            var databaseLabel = new Label
            {
                Text = "Base de Datos:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(350, 80),
                Size = new Size(200, 20),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            _databaseTextBox = new TextBox
            {
                Location = new Point(350, 105),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9),
                Text = "ApiSAP"
            };

            // Autenticación integrada
            _integratedSecurityCheckBox = new CheckBox
            {
                Text = "Usar Autenticación Integrada de Windows",
                Location = new Point(30, 150),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 9),
                Checked = true
            };

            // Usuario
            var usernameLabel = new Label
            {
                Text = "Usuario:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(30, 190),
                Size = new Size(200, 20),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            _usernameTextBox = new TextBox
            {
                Location = new Point(30, 215),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9),
                Enabled = false
            };

            // Contraseña
            var passwordLabel = new Label
            {
                Text = "Contraseña:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(250, 190),
                Size = new Size(200, 20),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            _passwordTextBox = new TextBox
            {
                Location = new Point(250, 215),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9),
                UseSystemPasswordChar = true,
                Enabled = false
            };

            // Timeout
            var timeoutLabel = new Label
            {
                Text = "Timeout (segundos):",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(470, 190),
                Size = new Size(150, 20),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            _timeoutNumericUpDown = new NumericUpDown
            {
                Location = new Point(470, 215),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 9),
                Minimum = 5,
                Maximum = 300,
                Value = 30
            };

            // Botones
            _testConnectionButton = new Button
            {
                Text = "🔍 Probar Conexión",
                Location = new Point(30, 270),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            _saveButton = new Button
            {
                Text = "💾 Guardar",
                Location = new Point(200, 270),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // Panel de test de conexión
            _connectionTestPanel = new ConnectionTestPanel
            {
                Location = new Point(30, 330),
                Size = new Size(520, 80)
            };

            // Eventos
            _integratedSecurityCheckBox.CheckedChanged += IntegratedSecurity_CheckedChanged;
            _testConnectionButton.Click += TestConnection_Click;
            _saveButton.Click += Save_Click;

            // Agregar controles
            Controls.AddRange(new Control[]
            {
                titleLabel, serverLabel, _serverTextBox, databaseLabel, _databaseTextBox,
                _integratedSecurityCheckBox, usernameLabel, _usernameTextBox,
                passwordLabel, _passwordTextBox, timeoutLabel, _timeoutNumericUpDown,
                _testConnectionButton, _saveButton, _connectionTestPanel
            });
        }

        private void IntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            bool useIntegrated = _integratedSecurityCheckBox.Checked;
            _usernameTextBox.Enabled = !useIntegrated;
            _passwordTextBox.Enabled = !useIntegrated;

            if (useIntegrated)
            {
                _usernameTextBox.Clear();
                _passwordTextBox.Clear();
            }
        }

        private async void TestConnection_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            _connectionTestPanel.SetTesting();
            _testConnectionButton.Enabled = false;

            try
            {
                var result = await _databaseTestService.TestConnectionAsync(
                    _serverTextBox.Text,
                    _databaseTextBox.Text,
                    _integratedSecurityCheckBox.Checked ? null : _usernameTextBox.Text,
                    _integratedSecurityCheckBox.Checked ? null : _passwordTextBox.Text,
                    _integratedSecurityCheckBox.Checked
                );

                _connectionTestPanel.SetResult(result);
                _logger.LogInformation("Test de conexión: {Success}", result.IsSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en test de conexión");
                _connectionTestPanel.SetResult(new Models.DTOs.ConnectionTestResult
                {
                    IsSuccess = false,
                    Message = "Error interno",
                    ErrorDetails = ex.Message
                });
            }
            finally
            {
                _testConnectionButton.Enabled = true;
            }
        }

        private async void Save_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                var config = new SqlConfiguration
                {
                    Server = _serverTextBox.Text,
                    Database = _databaseTextBox.Text,
                    UseIntegratedSecurity = _integratedSecurityCheckBox.Checked,
                    Username = _integratedSecurityCheckBox.Checked ? null : _usernameTextBox.Text,
                    Password = _integratedSecurityCheckBox.Checked ? null : _passwordTextBox.Text,
                    ConnectionTimeout = (int)_timeoutNumericUpDown.Value
                };

                var result = await _configurationService.SaveSqlConfigurationAsync(config);

                if (result.IsSuccess)
                {
                    MessageBox.Show("Configuración guardada correctamente", "Éxito",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _logger.LogInformation("Configuración SQL guardada");
                }
                else
                {
                    MessageBox.Show($"Error al guardar: {result.Message}", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar configuración SQL");
                MessageBox.Show($"Error interno: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(_serverTextBox.Text))
            {
                MessageBox.Show("El servidor es requerido", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!ValidationHelper.IsValidSqlServer(_serverTextBox.Text))
            {
                MessageBox.Show("Formato de servidor inválido", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!ValidationHelper.IsValidDatabaseName(_databaseTextBox.Text))
            {
                MessageBox.Show("Nombre de base de datos inválido", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!_integratedSecurityCheckBox.Checked)
            {
                if (string.IsNullOrWhiteSpace(_usernameTextBox.Text))
                {
                    MessageBox.Show("El usuario es requerido cuando no se usa autenticación integrada",
                                  "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private async void LoadConfiguration()
        {
            try
            {
                var config = await _configurationService.GetSqlConfigurationAsync();
                if (config != null)
                {
                    _serverTextBox.Text = config.Server;
                    _databaseTextBox.Text = config.Database;
                    _integratedSecurityCheckBox.Checked = config.UseIntegratedSecurity;
                    _usernameTextBox.Text = config.Username ?? "";
                    _passwordTextBox.Text = config.Password ?? "";
                    _timeoutNumericUpDown.Value = config.ConnectionTimeout;

                    _logger.LogInformation("Configuración SQL cargada");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar configuración SQL");
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 500);
            Name = "SqlConfigForm";
            ResumeLayout(false);
        }
    }
}
