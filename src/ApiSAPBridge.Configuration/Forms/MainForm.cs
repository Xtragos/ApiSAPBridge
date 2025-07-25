using System;
using System.Drawing;
using System.Windows.Forms;
using ApiSAPBridge.Configuration.Services;
using ApiSAPBridge.Configuration.UserControls;
using ApiSAPBridge.Configuration.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ApiSAPBridge.Configuration.Forms
{
    public partial class MainForm : Form
    {
        #region Fields

        private readonly IConfigurationService _configurationService;
        private SqlConfigurationControl _sqlConfigControl;

        #endregion

        #region Constructor

        public MainForm(IConfigurationService configurationService)
        {
            InitializeComponent();
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

            InitializeUserControls();
            SetupTabControl();

            Log.Information("MainForm inicializado correctamente");
        }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Inicializa los UserControls necesarios
        /// </summary>
        private void InitializeUserControls()
        {
            try
            {
                // Crear el control de configuración SQL con inyección de dependencias
                _sqlConfigControl = new SqlConfigurationControl(_configurationService);
                _sqlConfigControl.Dock = DockStyle.Fill;

                Log.Information("UserControls inicializados correctamente");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al inicializar UserControls");
                MessageBox.Show($"Error al inicializar controles: {ex.Message}",
                    "Error de Inicialización", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Configura el TabControl con las pestañas necesarias
        /// </summary>
        private void SetupTabControl()
        {
            try
            {
                tabControl.TabPages.Clear();

                // 1. Pestaña de Configuración SQL (SIEMPRE VISIBLE)
                var sqlTab = new TabPage("🔌 Configuración SQL")
                {
                    Name = "sqlTab",
                    BackColor = Color.White,
                    Padding = new Padding(10)
                };

                // Agregar el SqlConfigurationControl a la pestaña
                if (_sqlConfigControl != null)
                {
                    sqlTab.Controls.Add(_sqlConfigControl);
                }

                tabControl.TabPages.Add(sqlTab);

                // 2. Pestaña de Métodos SAP (PROTEGIDA)
                var methodsTab = new TabPage("⚙️ Métodos SAP")
                {
                    Name = "methodsTab",
                    BackColor = Color.White,
                    Padding = new Padding(10)
                };

                var methodsLabel = new Label
                {
                    Text = "🔒 Esta pestaña requiere autenticación\n\n" +
                           "Funcionalidad pendiente de implementación en Fase 4:\n" +
                           "• Configuración de intervalos de sincronización\n" +
                           "• Habilitar/deshabilitar endpoints específicos\n" +
                           "• Control de prioridades de ejecución\n" +
                           "• Estado de última ejecución por endpoint",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    ForeColor = Color.Gray
                };
                methodsTab.Controls.Add(methodsLabel);

                tabControl.TabPages.Add(methodsTab);

                // 3. Pestaña de Swagger (PROTEGIDA)
                var swaggerTab = new TabPage("📋 Swagger")
                {
                    Name = "swaggerTab",
                    BackColor = Color.White,
                    Padding = new Padding(10)
                };

                var swaggerLabel = new Label
                {
                    Text = "🔒 Esta pestaña requiere autenticación\n\n" +
                           "Funcionalidad pendiente de implementación en Fase 5:\n" +
                           "• Control de visibilidad de endpoints\n" +
                           "• Filtro por métodos HTTP (GET, POST, PUT, DELETE)\n" +
                           "• Configuración global de Swagger ON/OFF\n" +
                           "• Preview de endpoints visibles",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    ForeColor = Color.Gray
                };
                swaggerTab.Controls.Add(swaggerLabel);

                tabControl.TabPages.Add(swaggerTab);

                // Establecer la pestaña SQL como activa por defecto
                tabControl.SelectedTab = sqlTab;

                Log.Information("TabControl configurado con {TabCount} pestañas", tabControl.TabPages.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al configurar TabControl");
                MessageBox.Show($"Error al configurar pestañas: {ex.Message}",
                    "Error de Configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Configuration Methods

        /// <summary>
        /// Obtiene la configuración SQL actual
        /// </summary>
        public void SaveSqlConfiguration()
        {
            try
            {
                if (_sqlConfigControl != null)
                {
                    var config = _sqlConfigControl.GetConfiguration();
                    Log.Information("Configuración SQL obtenida: Servidor={Server}, DB={Database}",
                        config.Server, config.Database);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener configuración SQL");
                throw; // Re-lanzar para manejo en el llamador
            }
        }

        /// <summary>
        /// Actualiza el mensaje en la barra de estado
        /// </summary>
        private void UpdateStatusBar(string message)
        {
            try
            {
                if (statusLabel != null)
                {
                    statusLabel.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
                    statusStrip.Refresh();
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error al actualizar StatusBar");
            }
        }

        /// <summary>
        /// Muestra u oculta la barra de progreso en el status
        /// </summary>
        private void SetStatusProgress(bool visible, string message = "")
        {
            try
            {
                statusProgressBar.Visible = visible;
                if (!string.IsNullOrEmpty(message))
                {
                    UpdateStatusBar(message);
                }
                statusStrip.Refresh();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error al actualizar progreso en StatusBar");
            }
        }

        #endregion

        #region ToolStrip Event Handlers

        /// <summary>
        /// Evento del botón de configuración SQL
        /// </summary>
        private void btnSqlConfig_Click(object sender, EventArgs e)
        {
            try
            {
                // Cambiar a la pestaña SQL
                var sqlTab = tabControl.TabPages["sqlTab"];
                if (sqlTab != null)
                {
                    tabControl.SelectedTab = sqlTab;
                    UpdateStatusBar("Configuración SQL activa");
                    Log.Information("Usuario navegó a configuración SQL");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al navegar a configuración SQL");
                MessageBox.Show($"Error al cambiar a configuración SQL: {ex.Message}",
                    "Error de Navegación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento del botón de Métodos SAP (protegido)
        /// </summary>
        private void btnMethodsConfig_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(
                    "Esta funcionalidad estará disponible en la Fase 4.\n\n" +
                    "Características pendientes:\n" +
                    "• Autenticación por contraseña\n" +
                    "• Configuración de intervalos de sincronización\n" +
                    "• Control de endpoints SAP\n" +
                    "• Gestión de prioridades de ejecución",
                    "Funcionalidad en Desarrollo - Fase 4",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Log.Information("Usuario intentó acceder a configuración de métodos (pendiente Fase 4)");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en btnMethodsConfig_Click");
            }
        }

        /// <summary>
        /// Evento del botón de configuración Swagger (protegido)
        /// </summary>
        private void btnSwaggerConfig_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(
                    "Esta funcionalidad estará disponible en la Fase 5.\n\n" +
                    "Características pendientes:\n" +
                    "• Autenticación por contraseña\n" +
                    "• Control de visibilidad de endpoints\n" +
                    "• Filtros por métodos HTTP\n" +
                    "• Configuración global de Swagger",
                    "Funcionalidad en Desarrollo - Fase 5",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Log.Information("Usuario intentó acceder a configuración de Swagger (pendiente Fase 5)");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en btnSwaggerConfig_Click");
            }
        }

        /// <summary>
        /// Evento del botón Guardar
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Determinar qué pestaña está activa y guardar su configuración
                var activeTab = tabControl.SelectedTab;

                if (activeTab?.Name == "sqlTab")
                {
                    // Validar configuración SQL
                    string errorMessage = string.Empty; // CORRECCIÓN CS0165: Inicializar variable

                    if (_sqlConfigControl?.ValidateConfiguration(out errorMessage) == true)
                    {
                        SetStatusProgress(true, "Guardando configuración SQL...");

                        SaveSqlConfiguration();

                        SetStatusProgress(false);
                        UpdateStatusBar("Configuración SQL guardada correctamente");

                        MessageBox.Show("Configuración SQL guardada correctamente",
                            "Configuración Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        UpdateStatusBar("Error de validación en configuración SQL");
                        MessageBox.Show($"Error de validación:\n\n{errorMessage}",
                            "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("La pestaña activa no tiene configuración guardable disponible.",
                        "Sin Configuración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Log.Information("Proceso de guardado completado para pestaña: {TabName}", activeTab?.Name);
            }
            catch (Exception ex)
            {
                SetStatusProgress(false);
                Log.Error(ex, "Error al guardar configuración");
                MessageBox.Show($"Error al guardar configuración:\n\n{ex.Message}",
                    "Error de Guardado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento del botón de Ayuda
        /// </summary>
        private void btnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                var helpMessage = @"ApiSAPBridge Configuration Tool v1.0

🔌 CONFIGURACIÓN SQL:
- Configure la conexión a SQL Server
- Seleccione autenticación Windows o SQL
- Pruebe la conectividad antes de guardar
- Ajuste timeout y certificados SSL

⚙️ MÉTODOS SAP: (Requiere autenticación - Fase 4)
- Configure intervalos de sincronización
- Habilite/deshabilite endpoints específicos
- Controle prioridades de ejecución
- Monitoree estado de sincronización

📋 SWAGGER: (Requiere autenticación - Fase 5)  
- Controle visibilidad de endpoints
- Configure métodos HTTP visibles
- Gestione documentación de API
- Configure acceso público/privado

💡 CONSEJOS:
- Use 'Probar Conexión' antes de guardar
- Los cambios se guardan automáticamente
- Consulte los logs para diagnósticos

Para más información, consulte la documentación del proyecto ApiSAPBridge.";

                MessageBox.Show(helpMessage, "Ayuda - ApiSAPBridge Configuration",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Log.Information("Usuario accedió a la ayuda del sistema");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en btnHelp_Click");
            }
        }

        #endregion

        #region TabControl Event Handlers

        /// <summary>
        /// Evento cuando cambia la pestaña activa
        /// </summary>
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selectedTab = tabControl.SelectedTab;

                if (selectedTab != null)
                {
                    string tabName = selectedTab.Name;
                    string tabText = selectedTab.Text;

                    UpdateStatusBar($"Pestaña activa: {tabText}");
                    Log.Information("Usuario cambió a pestaña: {TabName}", tabName);

                    // Actualizar título de la ventana
                    this.Text = $"ApiSAPBridge Configuration - {tabText}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en tabControl_SelectedIndexChanged");
            }
        }

        #endregion

        #region Menu Event Handlers

        /// <summary>
        /// Evento para el menú Archivo > Nueva Configuración
        /// </summary>
        private void newConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "¿Está seguro de que desea crear una nueva configuración?\n\n" +
                    "Esto restablecerá todos los valores a sus predeterminados.",
                    "Nueva Configuración",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Restablecer configuración SQL a valores predeterminados
                    _sqlConfigControl?.SetConfiguration(new SqlServerConfiguration
                    {
                        Server = "localhost\\SQLEXPRESS",
                        Database = "ApiSAP",
                        UseWindowsAuthentication = true,
                        ConnectionTimeout = 30,
                        TrustServerCertificate = true
                    });

                    // Cambiar a la pestaña SQL
                    var sqlTab = tabControl.TabPages["sqlTab"];
                    if (sqlTab != null)
                    {
                        tabControl.SelectedTab = sqlTab;
                    }

                    UpdateStatusBar("Nueva configuración creada");
                    Log.Information("Usuario creó nueva configuración");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en newConfigToolStripMenuItem_Click");
                MessageBox.Show($"Error al crear nueva configuración:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento para el menú Archivo > Salir
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en exitToolStripMenuItem_Click");
            }
        }

        /// <summary>
        /// Evento para el menú Ayuda > Acerca de
        /// </summary>
        private void btnAbout_Click(object sender, EventArgs e)
        {
            try
            {
                var aboutMessage = @"ApiSAPBridge Configuration Tool

Versión: 1.0.0
Framework: .NET 8.0
Desarrollado para: ApiSAPBridge Project

© 2025 - Herramienta de configuración para
sistema de sincronización SAP

Características implementadas:
✅ Configuración SQL Server completa
✅ Validación en tiempo real
✅ Prueba de conectividad SQL
✅ Sistema de logging con Serilog
✅ Interfaz profesional Windows Forms

Próximas funcionalidades:
🔄 Configuración de métodos SAP (Fase 4)
📊 Control de endpoints Swagger (Fase 5)
🔒 Sistema de autenticación avanzado

Soporte técnico: Consulte la documentación
del proyecto ApiSAPBridge en el repositorio.";

                MessageBox.Show(aboutMessage, "Acerca de ApiSAPBridge Configuration",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Log.Information("Usuario accedió a información 'Acerca de'");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en btnAbout_Click");
            }
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Evento de carga del formulario
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                UpdateStatusBar("Aplicación inicializada correctamente");

                // Mostrar información de bienvenida
                if (_sqlConfigControl != null)
                {
                    Log.Information("MainForm cargado correctamente con SqlConfigurationControl");
                }

                // Establecer foco en la pestaña SQL
                var sqlTab = tabControl.TabPages["sqlTab"];
                if (sqlTab != null)
                {
                    tabControl.SelectedTab = sqlTab;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en MainForm_Load");
                UpdateStatusBar("Error al cargar la aplicación");
            }
        }

        /// <summary>
        /// Evento al cerrar el formulario
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Verificar si hay cambios sin guardar en la configuración SQL
                if (_sqlConfigControl != null)
                {
                    var result = MessageBox.Show(
                        "¿Desea guardar los cambios antes de salir?",
                        "Guardar Cambios",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            // Intentar guardar antes de salir
                            try
                            {
                                string errorMessage = string.Empty;
                                if (_sqlConfigControl.ValidateConfiguration(out errorMessage))
                                {
                                    SaveSqlConfiguration();
                                    UpdateStatusBar("Configuración guardada antes de salir");
                                }
                                else
                                {
                                    var saveResult = MessageBox.Show(
                                        $"Hay errores de validación:\n\n{errorMessage}\n\n" +
                                        "¿Desea salir sin guardar?",
                                        "Error de Validación",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning);

                                    if (saveResult == DialogResult.No)
                                    {
                                        e.Cancel = true;
                                        return;
                                    }
                                }
                            }
                            catch (Exception saveEx)
                            {
                                Log.Error(saveEx, "Error al guardar antes de salir");
                                var errorResult = MessageBox.Show(
                                    $"Error al guardar:\n\n{saveEx.Message}\n\n" +
                                    "¿Desea salir sin guardar?",
                                    "Error de Guardado",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Error);

                                if (errorResult == DialogResult.No)
                                {
                                    e.Cancel = true;
                                    return;
                                }
                            }
                            break;

                        case DialogResult.Cancel:
                            // Cancelar el cierre
                            e.Cancel = true;
                            return;

                        case DialogResult.No:
                            // Continuar sin guardar
                            break;
                    }
                }

                if (!e.Cancel)
                {
                    Log.Information("Aplicación cerrándose correctamente");
                    UpdateStatusBar("Cerrando aplicación...");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en MainForm_FormClosing");
            }
        }

        #endregion

        #region Dispose Override

        /// <summary>
        /// Limpieza de recursos
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    _sqlConfigControl?.Dispose();
                    components?.Dispose();
                    Log.Information("Recursos del MainForm liberados correctamente");
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Error al liberar recursos del MainForm");
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}