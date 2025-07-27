using ApiSAPBridge.Configuration.Services;
using ApiSAPBridge.Configuration.UI.Controls;
using ApiSAPBridge.Configuration.Utils;
using ApiSAPBridge.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ApiSAPBridge.Configuration.UI.Forms
{
    public partial class SqlConfigForm : Form
    {
        private readonly ILogger<SqlConfigForm> _logger;
        private readonly IConfigurationService _configurationService;
        private readonly IDatabaseTestService _databaseTestService;
        private readonly IDatabaseInitializerService _databaseInitializerService;
        private readonly ISqlServerVersionDetectorService _versionDetectorService;

        private TextBox _serverTextBox;
        private TextBox _databaseTextBox;
        private TextBox _usernameTextBox;
        private TextBox _passwordTextBox;
        private CheckBox _integratedSecurityCheckBox;
        private CheckBox _trustServerCertificateCheckBox;
        private NumericUpDown _timeoutNumericUpDown;
        private Button _detectVersionButton;
        private Button _testConnectionButton;
        private Button _createDatabaseButton;
        private Button _saveButton;
        private ConnectionTestPanel _connectionTestPanel;
        private Label _statusLabel;
        private Label _versionInfoLabel;

        public SqlConfigForm(
            ILogger<SqlConfigForm> logger,
            IConfigurationService configurationService,
            IDatabaseTestService databaseTestService,
            IDatabaseInitializerService databaseInitializerService,
            ISqlServerVersionDetectorService versionDetectorService)
        {
            _logger = logger;
            _configurationService = configurationService;
            _databaseTestService = databaseTestService;
            _databaseInitializerService = databaseInitializerService;
            _versionDetectorService = versionDetectorService;

            _logger.LogInformation("🚀 Iniciando SqlConfigForm con detección automática de versión");

            InitializeComponent();
            SetupControls();
            LoadConfiguration();
        }

        private void SetupControls()
        {
            Text = "Configuración SQL Server";
            Size = new Size(750, 700); // Aumentar tamaño para nuevos controles
            BackColor = Color.White;

            // Título
            var titleLabel = new Label
            {
                Text = "🗄️ Configuración de Base de Datos SQL Server",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Location = new Point(30, 20),
                Size = new Size(690, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Información importante
            var infoLabel = new Label
            {
                Text = "ℹ️ La configuración se optimiza automáticamente según la versión de SQL Server detectada.",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                Location = new Point(30, 60),
                Size = new Size(690, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Servidor
            var serverLabel = new Label
            {
                Text = "Servidor SQL:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(30, 100),
                Size = new Size(200, 20),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            _serverTextBox = new TextBox
            {
                Location = new Point(30, 125),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 9),
                Text = "ORION-LUIS"
            };

            // Botón detectar versión
            _detectVersionButton = new Button
            {
                Text = "🔍 Detectar Versión",
                Location = new Point(290, 125),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // Base de datos
            var databaseLabel = new Label
            {
                Text = "Base de Datos:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(430, 100),
                Size = new Size(200, 20),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            _databaseTextBox = new TextBox
            {
                Location = new Point(430, 125),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9),
                Text = "ApiSAP"
            };

            // Información de versión detectada
            _versionInfoLabel = new Label
            {
                Location = new Point(30, 160),
                Size = new Size(600, 40),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(108, 117, 125),
                Text = "💡 Haz clic en 'Detectar Versión' para optimizar la configuración automáticamente",
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Autenticación integrada
            _integratedSecurityCheckBox = new CheckBox
            {
                Text = "Usar Autenticación Integrada de Windows",
                Location = new Point(30, 210),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 9),
                Checked = false
            };

            // TrustServerCertificate
            _trustServerCertificateCheckBox = new CheckBox
            {
                Text = "TrustServerCertificate (requerido para SQL 2019+)",
                Location = new Point(350, 210),
                Size = new Size(350, 25),
                Font = new Font("Segoe UI", 9),
                Checked = false,
                ForeColor = Color.FromArgb(255, 193, 7) // Color amarillo para destacar
            };

            // Usuario
            var usernameLabel = new Label
            {
                Text = "Usuario:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(30, 250),
                Size = new Size(200, 20),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            _usernameTextBox = new TextBox
            {
                Location = new Point(30, 275),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9),
                Text = "ICGAdmin",
                Enabled = true
            };

            // Contraseña
            var passwordLabel = new Label
            {
                Text = "Contraseña:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(250, 250),
                Size = new Size(200, 20),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            _passwordTextBox = new TextBox
            {
                Location = new Point(250, 275),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9),
                UseSystemPasswordChar = true,
                Enabled = true
            };

            // Timeout
            var timeoutLabel = new Label
            {
                Text = "Timeout (segundos):",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(470, 250),
                Size = new Size(150, 20),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            _timeoutNumericUpDown = new NumericUpDown
            {
                Location = new Point(470, 275),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 9),
                Minimum = 5,
                Maximum = 300,
                Value = 30
            };

            // Botones principales
            _testConnectionButton = new Button
            {
                Text = "🔍 Probar Conexión",
                Location = new Point(30, 330),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            _createDatabaseButton = new Button
            {
                Text = "🚀 Crear Base de Datos",
                Location = new Point(200, 330),
                Size = new Size(160, 40),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };

            _saveButton = new Button
            {
                Text = "💾 Guardar",
                Location = new Point(380, 330),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // Panel de test de conexión
            _connectionTestPanel = new ConnectionTestPanel
            {
                Location = new Point(30, 390),
                Size = new Size(600, 100)
            };

            // Status label
            _statusLabel = new Label
            {
                Location = new Point(30, 510),
                Size = new Size(600, 60),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(73, 80, 87),
                Text = "",
                TextAlign = ContentAlignment.TopLeft
            };

            // Eventos
            _integratedSecurityCheckBox.CheckedChanged += IntegratedSecurity_CheckedChanged;
            _detectVersionButton.Click += DetectVersion_Click;
            _testConnectionButton.Click += TestConnection_Click;
            _createDatabaseButton.Click += CreateDatabase_Click;
            _saveButton.Click += Save_Click;

            // Agregar controles
            Controls.AddRange(new Control[]
            {
                titleLabel, infoLabel, serverLabel, _serverTextBox, _detectVersionButton,
                databaseLabel, _databaseTextBox, _versionInfoLabel,
                _integratedSecurityCheckBox, _trustServerCertificateCheckBox,
                usernameLabel, _usernameTextBox, passwordLabel, _passwordTextBox,
                timeoutLabel, _timeoutNumericUpDown,
                _testConnectionButton, _createDatabaseButton, _saveButton,
                _connectionTestPanel, _statusLabel
            });

            // Verificar estado inicial
            CheckDatabaseStatusAsync();
        }

        private async void DetectVersion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_serverTextBox.Text))
            {
                MessageBox.Show("Por favor ingresa el nombre del servidor primero", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _serverTextBox.Focus();
                return;
            }

            _detectVersionButton.Enabled = false;
            _detectVersionButton.Text = "🔄 Detectando...";

            try
            {
                _logger.LogInformation("🔍 Detectando versión de SQL Server: {Server}", _serverTextBox.Text);

                var versionInfo = await _versionDetectorService.DetectVersionAsync(
                    _serverTextBox.Text,
                    _integratedSecurityCheckBox.Checked,
                    _usernameTextBox.Text,
                    _passwordTextBox.Text
                );

                if (versionInfo.DetectionSuccessful)
                {
                    _logger.LogInformation("✅ Versión detectada: {Version}", versionInfo.GetFriendlyVersionName());

                    // Actualizar UI con información detectada
                    _versionInfoLabel.Text = $"✅ Detectado: {versionInfo.GetFriendlyVersionName()} " +
                                           $"({versionInfo.Edition}, {versionInfo.ProductLevel})";
                    _versionInfoLabel.ForeColor = Color.Green;

                    // Configurar automáticamente TrustServerCertificate
                    var shouldUseTrust = _versionDetectorService.ShouldUseTrustServerCertificate(versionInfo.Version);
                    _trustServerCertificateCheckBox.Checked = shouldUseTrust;

                    if (shouldUseTrust)
                    {
                        _trustServerCertificateCheckBox.ForeColor = Color.Green;
                        _trustServerCertificateCheckBox.Text = "✅ TrustServerCertificate (requerido para SQL 2019+)";
                    }
                    else
                    {
                        _trustServerCertificateCheckBox.ForeColor = Color.Gray;
                        _trustServerCertificateCheckBox.Text = "❌ TrustServerCertificate (no necesario para versiones antiguas)";
                    }

                    // Mostrar información adicional
                    var additionalInfo = $"📊 Información adicional:\n" +
                                       $"• Versión del producto: {versionInfo.ProductVersion}\n" +
                                       $"• TrustServerCertificate: {(versionInfo.RequiresTrustServerCertificate ? "Requerido" : "No necesario")}\n" +
                                       $"• Configuración optimizada automáticamente";

                    MessageBox.Show(additionalInfo, "Versión Detectada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _logger.LogWarning("⚠️ No se pudo detectar la versión: {Error}", versionInfo.ErrorMessage);

                    _versionInfoLabel.Text = $"⚠️ No se pudo detectar la versión: {versionInfo.ErrorMessage}";
                    _versionInfoLabel.ForeColor = Color.Orange;

                    // Configurar valores por defecto conservadores
                    _trustServerCertificateCheckBox.Checked = true; // Asumir que es necesario
                    _trustServerCertificateCheckBox.ForeColor = Color.Orange;
                    _trustServerCertificateCheckBox.Text = "⚠️ TrustServerCertificate (configurado por defecto)";

                    MessageBox.Show($"No se pudo detectar la versión automáticamente.\n\n" +
                                  $"Error: {versionInfo.ErrorMessage}\n\n" +
                                  $"Se han aplicado configuraciones por defecto.\n" +
                                  $"Puedes ajustar manualmente si es necesario.",
                                  "Detección de Versión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inesperado al detectar versión");

                _versionInfoLabel.Text = $"❌ Error al detectar versión: {ex.Message}";
                _versionInfoLabel.ForeColor = Color.Red;

                MessageBox.Show($"Error inesperado al detectar la versión:\n{ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _detectVersionButton.Enabled = true;
                _detectVersionButton.Text = "🔍 Detectar Versión";
            }
        }

        private async void CheckDatabaseStatusAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_serverTextBox.Text) || string.IsNullOrWhiteSpace(_databaseTextBox.Text))
                    return;

                var exists = await _databaseInitializerService.DatabaseExistsAsync(
                    _serverTextBox.Text,
                    _databaseTextBox.Text,
                    _integratedSecurityCheckBox.Checked,
                    _usernameTextBox.Text,
                    _passwordTextBox.Text
                );

                if (exists)
                {
                    _statusLabel.Text = "✅ Base de datos existe y está disponible";
                    _statusLabel.ForeColor = Color.Green;
                    _createDatabaseButton.Text = "🔄 Verificar/Actualizar BD";
                }
                else
                {
                    _statusLabel.Text = "⚠️ Base de datos no existe - usar botón 'Crear Base de Datos'";
                    _statusLabel.ForeColor = Color.Orange;
                    _createDatabaseButton.Text = "🚀 Crear Base de Datos";
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo verificar estado de la base de datos");
                _statusLabel.Text = "❓ No se pudo verificar el estado de la base de datos";
                _statusLabel.ForeColor = Color.Gray;
            }
        }

        private void IntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            bool useIntegrated = _integratedSecurityCheckBox.Checked;
            _usernameTextBox.Enabled = !useIntegrated;
            _passwordTextBox.Enabled = !useIntegrated;

            _logger.LogDebug("🔐 Autenticación integrada cambiada a: {UseIntegrated}", useIntegrated);

            if (useIntegrated)
            {
                _usernameTextBox.Clear();
                _passwordTextBox.Clear();
            }
            else
            {
                _usernameTextBox.Text = "ICGAdmin";
            }

            // Verificar estado de BD cuando cambie autenticación
            CheckDatabaseStatusAsync();
        }

        private async void TestConnection_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            _logger.LogInformation("🔧 Iniciando prueba de conexión inteligente...");

            _connectionTestPanel.SetTesting();
            _testConnectionButton.Enabled = false;
            _testConnectionButton.Text = "🔄 Probando...";

            try
            {
                // Usar el servicio mejorado que incluye detección automática
                var result = await _databaseTestService.TestConnectionWithAutoDetectionAsync(
                    _serverTextBox.Text,
                    _databaseTextBox.Text,
                    _integratedSecurityCheckBox.Checked ? null : _usernameTextBox.Text,
                    _integratedSecurityCheckBox.Checked ? null : _passwordTextBox.Text,
                    _integratedSecurityCheckBox.Checked
                );

                _connectionTestPanel.SetResult(result);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("✅ Prueba de conexión exitosa con detección automática");
                    CheckDatabaseStatusAsync();
                }
                else
                {
                    _logger.LogError("❌ Prueba de conexión falló: {Error}", result.ErrorDetails);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error inesperado en prueba de conexión");
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
                _testConnectionButton.Text = "🔍 Probar Conexión";
            }
        }

        private async void CreateDatabase_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            _logger.LogInformation("🚀 Iniciando creación/verificación de base de datos...");

            _createDatabaseButton.Enabled = false;
            _createDatabaseButton.Text = "🔄 Creando...";

            try
            {
                var connectionString = BuildOptimizedConnectionString();
                var result = await _databaseInitializerService.InitializeDatabaseAsync(connectionString);

                if (result.Success)
                {
                    var message = result.DatabaseExisted
                        ? "✅ Base de datos verificada correctamente"
                        : "🎉 Base de datos creada exitosamente";

                    MessageBox.Show($"{message}\n\n{result.Message}", "Éxito",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _logger.LogInformation("✅ Inicialización completada: {Message}", result.Message);
                    CheckDatabaseStatusAsync();
                }
                else
                {
                    var errorMsg = $"❌ Error al crear base de datos:\n\n{result.Message}";
                    if (!string.IsNullOrEmpty(result.ErrorDetails))
                    {
                        errorMsg += $"\n\nDetalles técnicos:\n{result.ErrorDetails}";
                    }

                    MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _logger.LogError("❌ Error en inicialización: {Error}", result.ErrorDetails);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error inesperado al crear base de datos");
                MessageBox.Show($"Error inesperado:\n{ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _createDatabaseButton.Enabled = true;
                _createDatabaseButton.Text = "🚀 Crear Base de Datos";
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
                    _logger.LogInformation("💾 Configuración SQL guardada exitosamente");
                }
                else
                {
                    MessageBox.Show($"Error al guardar: {result.Message}", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _logger.LogError("❌ Error al guardar configuración: {Error}", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error interno al guardar configuración SQL");
                MessageBox.Show($"Error interno: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildOptimizedConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = _serverTextBox.Text.Trim(),
                InitialCatalog = _databaseTextBox.Text.Trim(),
                ConnectTimeout = (int)_timeoutNumericUpDown.Value,
                MultipleActiveResultSets = true,
                ApplicationName = "ApiSAPBridge.Configuration",
                IntegratedSecurity = _integratedSecurityCheckBox.Checked,
                TrustServerCertificate = _trustServerCertificateCheckBox.Checked
            };

            if (!_integratedSecurityCheckBox.Checked)
            {
                builder.UserID = _usernameTextBox.Text.Trim();
                builder.Password = _passwordTextBox.Text;
            }

            return builder.ConnectionString;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(_serverTextBox.Text))
            {
                MessageBox.Show("El servidor es requerido", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _serverTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(_databaseTextBox.Text))
            {
                MessageBox.Show("El nombre de la base de datos es requerido", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _databaseTextBox.Focus();
                return false;
            }

            if (!_integratedSecurityCheckBox.Checked)
            {
                if (string.IsNullOrWhiteSpace(_usernameTextBox.Text))
                {
                    MessageBox.Show("El usuario es requerido cuando no se usa autenticación integrada",
                                  "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _usernameTextBox.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(_passwordTextBox.Text))
                {
                    MessageBox.Show("La contraseña es requerida cuando no se usa autenticación integrada",
                                  "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _passwordTextBox.Focus();
                    return false;
                }
            }

            return true;
        }

        private async void LoadConfiguration()
        {
            try
            {
                _logger.LogDebug("📥 Cargando configuración SQL existente...");
                var config = await _configurationService.GetSqlConfigurationAsync();

                if (config != null)
                {
                    _logger.LogInformation("✅ Configuración SQL cargada desde base de datos");
                    _serverTextBox.Text = config.Server ?? "ORION-LUIS";
                    _databaseTextBox.Text = config.Database ?? "ApiSAP";
                    _integratedSecurityCheckBox.Checked = config.UseIntegratedSecurity;
                    _usernameTextBox.Text = config.Username ?? "ICGAdmin";
                    _passwordTextBox.Text = config.Password ?? "";
                    _timeoutNumericUpDown.Value = config.ConnectionTimeout;

                    IntegratedSecurity_CheckedChanged(_integratedSecurityCheckBox, EventArgs.Empty);
                }
                else
                {
                    _logger.LogInformation("ℹ️ No se encontró configuración SQL, usando valores por defecto");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ No se pudo cargar configuración SQL, usando valores por defecto");
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(750, 700);
            Name = "SqlConfigForm";
            Text = "Configuración SQL Server";
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _logger.LogInformation("🏁 Cerrando SqlConfigForm");
            base.OnFormClosed(e);
        }
    }
}