using ApiSAPBridge.Configuration.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using ApiSAPBridge.Configuration.Models;
using ApiSAPBridge.Configuration.Services;
using Microsoft.Extensions.Logging;
using System.Windows.Forms.Design;
using static System.Net.Mime.MediaTypeNames;

namespace ApiSAPBridge.Configuration.Forms
{
    public partial class MainForm : UserControl
    {
        private readonly ConfigurationService _configService;
        private readonly SqlConnectionService _sqlService;
        private readonly ApiService _apiService;
        private readonly ILogger<MainForm> _logger;

        private TabControl _mainTabControl;
        private Button _saveButton;
        private Button _testButton;
        private Button _exportButton;
        private Button _importButton;
        private Label _statusLabel;
        private ProgressBar _progressBar;

        // Controles de pestañas
        private SqlConfigurationControl _sqlControl;
        private MethodsConfigurationControl _methodsControl;
        private SwaggerConfigurationControl _swaggerControl;

        // Estado de autenticación
        private bool _isMethodsUnlocked = false;
        private bool _isSwaggerUnlocked = false;

        public MainForm(
            ConfigurationService configService,
            SqlConnectionService sqlService,
            ApiService apiService,
            ILogger<MainForm> logger)
        {
            _configService = configService;
            _sqlService = sqlService;
            _apiService = apiService;
            _logger = logger;

            InitializeComponent();
            LoadConfiguration();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Configuración del formulario principal
            Text = "ApiSAPBridge - Herramienta de Configuración";
            Size = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(800, 600);
            Icon = SystemIcons.Application;

            // Panel superior con botones
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            _saveButton = new Button
            {
                Text = "💾 Guardar",
                Size = new Size(100, 35),
                Location = new Point(10, 12),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _saveButton.FlatAppearance.BorderSize = 0;
            _saveButton.Click += SaveButton_Click;

            _testButton = new Button
            {
                Text = "🔍 Probar",
                Size = new Size(100, 35),
                Location = new Point(120, 12),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _testButton.FlatAppearance.BorderSize = 0;
            _testButton.Click += TestButton_Click;

            _exportButton = new Button
            {
                Text = "📤 Exportar",
                Size = new Size(100, 35),
                Location = new Point(230, 12),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _exportButton.FlatAppearance.BorderSize = 0;
            _exportButton.Click += ExportButton_Click;

            _importButton = new Button
            {
                Text = "📥 Importar",
                Size = new Size(100, 35),
                Location = new Point(340, 12),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _importButton.FlatAppearance.BorderSize = 0;
            _importButton.Click += ImportButton_Click;

            topPanel.Controls.AddRange(new Control[] {
                _saveButton, _testButton, _exportButton, _importButton
            });

            // Panel inferior con estado
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            _statusLabel = new Label
            {
                Text = "Listo",
                Dock = DockStyle.Left,
                AutoSize = false,
                Width = 400,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            _progressBar = new ProgressBar
            {
                Dock = DockStyle.Right,
                Width = 200,
                Style = ProgressBarStyle.Continuous,
                Visible = false
            };

            bottomPanel.Controls.AddRange(new Control[] { _statusLabel, _progressBar });

            // Control de pestañas principal
            _mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F)
            };

            // Pestaña SQL (siempre visible)
            var sqlTab = new TabPage("🗄️ Configuración SQL")
            {
                BackColor = Color.White
            };
            _sqlControl = new SqlConfigurationControl(_configService, _sqlService);
            _sqlControl.Dock = DockStyle.Fill;
            sqlTab.Controls.Add(_sqlControl);

            // Pestaña Métodos (protegida)
            var methodsTab = new TabPage("⚙️ Métodos")
            {
                BackColor = Color.White
            };
            _methodsControl = new MethodsConfigurationControl(_configService);
            _methodsControl.Dock = DockStyle.Fill;
            methodsTab.Controls.Add(_methodsControl);

            // Pestaña Swagger (protegida)
            var swaggerTab = new TabPage("📚 Swagger")
            {
                BackColor = Color.White
            };
            _swaggerControl = new SwaggerConfigurationControl(_configService);
            _swaggerControl.Dock = DockStyle.Fill;
            swaggerTab.Controls.Add(_swaggerControl);

            _mainTabControl.TabPages.AddRange(new TabPage[] { sqlTab, methodsTab, swaggerTab });
            _mainTabControl.Selecting += MainTabControl_Selecting;

            // Agregar controles al formulario
            Controls.AddRange(new Control[] { _mainTabControl, topPanel, bottomPanel });

            ResumeLayout(false);
        }

        private void LoadConfiguration()
        {
            try
            {
                var config = _configService.Configuration;
                _sqlControl.LoadConfiguration(config.SqlConfig);
                _methodsControl.LoadConfiguration(config.MethodConfig);
                _swaggerControl.LoadConfiguration(config.SwaggerConfig);

                UpdateStatus("Configuración cargada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar configuración");
                MessageBox.Show($"Error al cargar configuración: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == null) return;

            // Verificar acceso a pestañas protegidas
            if (e.TabPage.Text.Contains("Métodos") && !_isMethodsUnlocked)
            {
                e.Cancel = true;
                if (ShowLoginDialog("Acceso a Configuración de Métodos"))
                {
                    _isMethodsUnlocked = true;
                    _mainTabControl.SelectedTab = e.TabPage;
                }
            }
            else if (e.TabPage.Text.Contains("Swagger") && !_isSwaggerUnlocked)
            {
                e.Cancel = true;
                if (ShowLoginDialog("Acceso a Configuración de Swagger"))
                {
                    _isSwaggerUnlocked = true;
                    _mainTabControl.SelectedTab = e.TabPage;
                }
            }
        }

        private bool ShowLoginDialog(string title)
        {
            using var loginForm = new LoginForm(_configService)
            {
                Text = title
            };

            return loginForm.ShowDialog(this) == DialogResult.OK;
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                ShowProgress("Guardando configuración...");

                var config = _configService.Configuration;
                config.SqlConfig = _sqlControl.GetConfiguration();

                if (_isMethodsUnlocked)
                    config.MethodConfig = _methodsControl.GetConfiguration();

                if (_isSwaggerUnlocked)
                    config.SwaggerConfig = _swaggerControl.GetConfiguration();

                _configService.SaveConfiguration(config);

                UpdateStatus("Configuración guardada exitosamente");
                MessageBox.Show("Configuración guardada exitosamente",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar configuración");
                MessageBox.Show($"Error al guardar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                HideProgress();
            }
        }

        private async void TestButton_Click(object sender, EventArgs e)
        {
            try
            {
                ShowProgress("Probando configuración...");

                var sqlConfig = _sqlControl.GetConfiguration();
                var (success, message) = await _sqlService.TestConnectionAsync(sqlConfig);

                if (success)
                {
                    UpdateStatus("Conexión SQL exitosa");
                    MessageBox.Show("✅ " + message, "Prueba Exitosa",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    UpdateStatus("Error en conexión SQL");
                    MessageBox.Show("❌ " + message, "Error de Conexión",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al probar configuración");
                MessageBox.Show($"Error en prueba: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                HideProgress();
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                using var saveDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    Title = "Exportar Configuración",
                    FileName = $"ApiSAPBridge_Config_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    _configService.ExportConfiguration(saveDialog.FileName);
                    UpdateStatus($"Configuración exportada: {Path.GetFileName(saveDialog.FileName)}");
                    MessageBox.Show("Configuración exportada exitosamente",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar configuración");
                MessageBox.Show($"Error al exportar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            try
            {
                using var openDialog = new OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    Title = "Importar Configuración"
                };

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    var result = MessageBox.Show(
                        "¿Está seguro de importar esta configuración? Se sobrescribirá la configuración actual.",
                        "Confirmar Importación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        _configService.ImportConfiguration(openDialog.FileName);
                        LoadConfiguration();

                        UpdateStatus($"Configuración importada: {Path.GetFileName(openDialog.FileName)}");
                        MessageBox.Show("Configuración importada exitosamente",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al importar configuración");
                MessageBox.Show($"Error al importar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatus(string status)
        {
            _statusLabel.Text = status;
        }

        private void ShowProgress(string status)
        {
            UpdateStatus(status);
            _progressBar.Visible = true;
            _progressBar.Style = ProgressBarStyle.Marquee;
        }

        private void HideProgress()
        {
            _progressBar.Visible = false;
            _progressBar.Style = ProgressBarStyle.Continuous;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                "¿Está seguro de cerrar la aplicación?",
                "Confirmar Cierre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }

            base.OnFormClosing(e);
        }
    }
}