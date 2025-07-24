using ApiSAPBridge.Configuration.Models;
using ApiSAPBridge.Configuration.Services;
using ApiSAPBridge.Configuration.UserControls;
using Serilog;

namespace ApiSAPBridge.Configuration.Forms
{
    public partial class MainForm : Form
    {
        private readonly IConfigurationService _configurationService;
        private readonly ILogger _logger;
        private SecurityConfiguration _securityConfig;
        private AppConfiguration _appConfig;

        // User Controls
        private SqlConfigurationControl _sqlConfigControl;
        private MethodsConfigurationControl _methodsConfigControl;
        private SwaggerConfigurationControl _swaggerConfigControl;

        public MainForm(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            _logger = Log.ForContext<MainForm>();

            InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                // Cargar configuraciones
                _securityConfig = await _configurationService.LoadSecurityConfigurationAsync();
                _appConfig = await _configurationService.LoadConfigurationAsync();

                // Inicializar controles de usuario
                InitializeUserControls();

                // Configurar tabs
                SetupTabs();

                // Mostrar información inicial
                UpdateStatusBar();

                _logger.Information("Aplicación de configuración iniciada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al inicializar la aplicación");
                MessageBox.Show($"Error al inicializar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeUserControls()
        {
            // SQL Configuration (siempre visible)
            _sqlConfigControl = new SqlConfigurationControl(_configurationService, _appConfig.SqlServer);

            // Methods Configuration (protegido)
            _methodsConfigControl = new MethodsConfigurationControl(_configurationService, _appConfig.SapAutomation);

            // Swagger Configuration (protegido)
            _swaggerConfigControl = new SwaggerConfigurationControl(_configurationService, _appConfig.Swagger);
        }

        private void SetupTabs()
        {
            tabControl.TabPages.Clear();

            // Tab 1: Configuración SQL (siempre visible)
            var sqlTab = new TabPage(ConfigurationConstants.TabNames.SQL_CONFIGURATION);
            sqlTab.Controls.Add(_sqlConfigControl);
            _sqlConfigControl.Dock = DockStyle.Fill;
            tabControl.TabPages.Add(sqlTab);

            // Tab 2: Métodos SAP (protegido - oculto inicialmente)
            var methodsTab = new TabPage(ConfigurationConstants.TabNames.METHODS_CONFIGURATION);
            methodsTab.Controls.Add(_methodsConfigControl);
            _methodsConfigControl.Dock = DockStyle.Fill;
            methodsTab.Tag = "protected"; // Marcar como protegido

            // Tab 3: Swagger (protegido - oculto inicialmente)
            var swaggerTab = new TabPage(ConfigurationConstants.TabNames.SWAGGER_CONFIGURATION);
            swaggerTab.Controls.Add(_swaggerConfigControl);
            _swaggerConfigControl.Dock = DockStyle.Fill;
            swaggerTab.Tag = "protected"; // Marcar como protegido

            // Agregar tabs protegidos solo si la configuración lo permite
            if (!_securityConfig.RequirePasswordForMethods)
            {
                tabControl.TabPages.Add(methodsTab);
            }

            if (!_securityConfig.RequirePasswordForSwagger)
            {
                tabControl.TabPages.Add(swaggerTab);
            }
        }

        private void UpdateStatusBar()
        {
            var configExists = _configurationService.ConfigurationExists();
            lblStatus.Text = configExists ? "Configuración cargada" : "Configuración por defecto";
            lblConfigPath.Text = $"Archivo: {_configurationService.GetConfigurationFilePath()}";
            lblVersion.Text = $"Versión: {ConfigurationConstants.APP_VERSION}";
        }

        private async void btnShowMethods_Click(object sender, EventArgs e)
        {
            await ShowProtectedTab(ConfigurationConstants.TabNames.METHODS_CONFIGURATION, "methods");
        }

        private async void btnShowSwagger_Click(object sender, EventArgs e)
        {
            await ShowProtectedTab(ConfigurationConstants.TabNames.SWAGGER_CONFIGURATION, "swagger");
        }

        private async Task ShowProtectedTab(string tabName, string purpose)
        {
            try
            {
                // Verificar si la tab ya está visible
                foreach (TabPage tab in tabControl.TabPages)
                {
                    if (tab.Text == tabName)
                    {
                        tabControl.SelectedTab = tab;
                        return;
                    }
                }

                // Mostrar formulario de autenticación
                using var loginForm = new LoginForm(_securityConfig, purpose);
                var result = loginForm.ShowDialog(this);

                if (result == DialogResult.OK && loginForm.IsAuthenticated)
                {
                    // Agregar la tab protegida
                    TabPage newTab;

                    if (tabName == ConfigurationConstants.TabNames.METHODS_CONFIGURATION)
                    {
                        newTab = new TabPage(tabName);
                        newTab.Controls.Add(_methodsConfigControl);
                        _methodsConfigControl.Dock = DockStyle.Fill;
                    }
                    else // Swagger
                    {
                        newTab = new TabPage(tabName);
                        newTab.Controls.Add(_swaggerConfigControl);
                        _swaggerConfigControl.Dock = DockStyle.Fill;
                    }

                    newTab.Tag = "protected";
                    tabControl.TabPages.Add(newTab);
                    tabControl.SelectedTab = newTab;

                    _logger.Information("Acceso concedido a tab protegida: {TabName}", tabName);
                }
                else
                {
                    _logger.Warning("Acceso denegado a tab protegida: {TabName}", tabName);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al mostrar tab protegida: {TabName}", tabName);
                MessageBox.Show($"Error al acceder a la configuración: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSaveAll_Click(object sender, EventArgs e)
        {
            try
            {
                // Recopilar configuraciones de todos los controles
                _appConfig.SqlServer = _sqlConfigControl.GetConfiguration();
                _appConfig.SapAutomation = _methodsConfigControl.GetConfiguration();
                _appConfig.Swagger = _swaggerConfigControl.GetConfiguration();

                // Guardar configuración
                await _configurationService.SaveConfigurationAsync(_appConfig);

                // Actualizar interfaz
                UpdateStatusBar();

                MessageBox.Show(ConfigurationConstants.Messages.CONFIG_SAVED, "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                _logger.Information("Configuración guardada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al guardar la configuración");
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("¿Está seguro que desea salir de la aplicación de configuración?",
                "Confirmar salida", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}