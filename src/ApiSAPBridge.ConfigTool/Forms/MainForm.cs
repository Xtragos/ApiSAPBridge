using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ApiSAPBridge.ConfigTool.Models;
using ApiSAPBridge.ConfigTool.Services;
using ApiSAPBridge.ConfigTool.Utils;

namespace ApiSAPBridge.ConfigTool.Forms
{
    public partial class MainForm : Form
    {
        private readonly ConfigurationService _configService;
        private readonly AuthenticationService _authService;
        private ConfigurationModel _config;

        private TabControl mainTabControl;
        private SqlConfigPanel sqlConfigPanel;
        private MethodsConfigPanel methodsConfigPanel;
        private SwaggerConfigPanel swaggerConfigPanel;

        private bool _isMethodsUnlocked = false;
        private bool _isSwaggerUnlocked = false;

        public MainForm()
        {
            _configService = new ConfigurationService();
            _authService = new AuthenticationService();
            InitializeComponent();
            LoadConfiguration();
        }

        private void InitializeComponent()
        {
            this.Text = "Configurador API SAP Bridge";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);
            this.Icon = Properties.Resources.AppIcon; // Agregar icono

            // Tab Control Principal
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Appearance = TabAppearance.Normal
            };

            // Pestaña SQL Configuration
            var sqlTab = new TabPage("🗄️ Configuración SQL")
            {
                BackColor = Color.White,
                UseVisualStyleBackColor = true
            };

            sqlConfigPanel = new SqlConfigPanel();
            sqlConfigPanel.Dock = DockStyle.Fill;
            sqlTab.Controls.Add(sqlConfigPanel);
            mainTabControl.TabPages.Add(sqlTab);

            // Pestaña Methods (Protegida)
            var methodsTab = new TabPage("⚙️ Métodos (🔒)")
            {
                BackColor = Color.LightGray,
                UseVisualStyleBackColor = false
            };

            methodsConfigPanel = new MethodsConfigPanel();
            methodsConfigPanel.Dock = DockStyle.Fill;
            methodsConfigPanel.Enabled = false;
            methodsTab.Controls.Add(methodsConfigPanel);
            mainTabControl.TabPages.Add(methodsTab);

            // Pestaña Swagger (Protegida)
            var swaggerTab = new TabPage("📋 Swagger (🔒)")
            {
                BackColor = Color.LightGray,
                UseVisualStyleBackColor = false
            };

            swaggerConfigPanel = new SwaggerConfigPanel();
            swaggerConfigPanel.Dock = DockStyle.Fill;
            swaggerConfigPanel.Enabled = false;
            swaggerTab.Controls.Add(swaggerConfigPanel);
            mainTabControl.TabPages.Add(swaggerTab);

            // Eventos
            mainTabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            // Panel de botones
            var buttonPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            var saveButton = new Button
            {
                Text = "💾 Guardar Configuración",
                Size = new Size(180, 35),
                Location = new Point(20, 12),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            saveButton.Click += SaveButton_Click;

            var loadButton = new Button
            {
                Text = "📂 Cargar Configuración",
                Size = new Size(180, 35),
                Location = new Point(220, 12),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            loadButton.Click += LoadButton_Click;

            var testButton = new Button
            {
                Text = "🔍 Probar Conexión SQL",
                Size = new Size(180, 35),
                Location = new Point(420, 12),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            testButton.Click += TestButton_Click;

            var aboutButton = new Button
            {
                Text = "ℹ️ Acerca de",
                Size = new Size(120, 35),
                Location = new Point(620, 12),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F)
            };
            aboutButton.Click += AboutButton_Click;

            buttonPanel.Controls.AddRange(new Control[] { saveButton, loadButton, testButton, aboutButton });

            // Layout principal
            var mainPanel = new Panel { Dock = DockStyle.Fill };
            mainPanel.Controls.Add(mainTabControl);

            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedIndex = mainTabControl.SelectedIndex;

            // Verificar acceso a pestañas protegidas
            if (selectedIndex == 1 && !_isMethodsUnlocked) // Pestaña Methods
            {
                if (ShowAuthenticationDialog("Métodos"))
                {
                    UnlockMethodsTab();
                }
                else
                {
                    mainTabControl.SelectedIndex = 0; // Volver a SQL
                }
            }
            else if (selectedIndex == 2 && !_isSwaggerUnlocked) // Pestaña Swagger  
            {
                if (ShowAuthenticationDialog("Swagger"))
                {
                    UnlockSwaggerTab();
                }
                else
                {
                    mainTabControl.SelectedIndex = 0; // Volver a SQL
                }
            }
        }

        private bool ShowAuthenticationDialog(string section)
        {
            using (var authForm = new AuthForm(section))
            {
                return authForm.ShowDialog() == DialogResult.OK;
            }
        }

        private void UnlockMethodsTab()
        {
            _isMethodsUnlocked = true;
            var methodsTab = mainTabControl.TabPages[1];
            methodsTab.Text = "⚙️ Métodos";
            methodsTab.BackColor = Color.White;
            methodsConfigPanel.Enabled = true;
        }

        private void UnlockSwaggerTab()
        {
            _isSwaggerUnlocked = true;
            var swaggerTab = mainTabControl.TabPages[2];
            swaggerTab.Text = "📋 Swagger";
            swaggerTab.BackColor = Color.White;
            swaggerConfigPanel.Enabled = true;
        }

        private void LoadConfiguration()
        {
            try
            {
                _config = _configService.LoadConfiguration();

                // Cargar datos en paneles
                sqlConfigPanel.LoadConfig(_config.SqlConfig);
                methodsConfigPanel.LoadConfig(_config.MethodsConfig);
                swaggerConfigPanel.LoadConfig(_config.SwaggerConfig);

                this.Text = $"Configurador API SAP Bridge - Última actualización: {_config.LastUpdated:dd/MM/yyyy HH:mm}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar configuración: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Recopilar configuración de todos los paneles
                _config.SqlConfig = sqlConfigPanel.GetConfig();
                _config.MethodsConfig = methodsConfigPanel.GetConfig();
                _config.SwaggerConfig = swaggerConfigPanel.GetConfig();
                _config.LastUpdated = DateTime.Now;

                _configService.SaveConfiguration(_config);

                MessageBox.Show("Configuración guardada exitosamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Text = $"Configurador API SAP Bridge - Última actualización: {_config.LastUpdated:dd/MM/yyyy HH:mm}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar configuración: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Seleccionar archivo de configuración",
                Filter = "Archivos JSON (*.json)|*.json|Todos los archivos (*.*)|*.*",
                DefaultExt = "json"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _config = _configService.LoadConfigurationFromFile(dialog.FileName);
                    LoadConfiguration();

                    MessageBox.Show("Configuración cargada exitosamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar configuración: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void TestButton_Click(object sender, EventArgs e)
        {
            var sqlConfig = sqlConfigPanel.GetConfig();
            var testService = new SqlTestService();

            var progressForm = new ProgressForm("Probando conexión SQL...");
            progressForm.Show();

            try
            {
                var result = await testService.TestConnectionAsync(sqlConfig);
                progressForm.Close();

                if (result.Success)
                {
                    MessageBox.Show($"✅ Conexión exitosa!\n\nDetalles:\n{result.Details}",
                        "Prueba de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"❌ Error de conexión:\n\n{result.ErrorMessage}",
                        "Prueba de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                progressForm.Close();
                MessageBox.Show($"Error durante la prueba: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            var aboutText = $@"
🏭 Configurador API SAP Bridge
Versión: {_config?.Version ?? "1.0.0"}

📋 Características:
- Configuración visual de SQL Server
- Control de métodos automatizados
- Gestión de visibilidad Swagger
- Seguridad con pestañas protegidas

👨‍💻 Desarrollado para:
Integración empresarial con SAP

📅 Última actualización:
{_config?.LastUpdated:dd/MM/yyyy HH:mm:ss}
";

            MessageBox.Show(aboutText, "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
