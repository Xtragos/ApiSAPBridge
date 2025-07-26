using System.Drawing;
using System.Windows.Forms;
using ApiSAPBridge.Configuration.UI.Controls;
using ApiSAPBridge.Configuration.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Configuration.UI.Forms
{
    public partial class MainForm : Form
    {
        private readonly ILogger<MainForm> _logger;
        private readonly ISecurityService _securityService;
        private ModernTabControl _tabControl;
        private SqlConfigForm _sqlConfigForm;
        private MethodsConfigForm _methodsConfigForm;
        private SwaggerConfigForm _swaggerConfigForm;
        private StatusStrip _statusStrip;
        private ToolStripStatusLabel _statusLabel;
        private bool _isAuthenticated = false;

        public MainForm(
            ILogger<MainForm> logger,
            ISecurityService securityService,
            SqlConfigForm sqlConfigForm,
            MethodsConfigForm methodsConfigForm,
            SwaggerConfigForm swaggerConfigForm)
        {
            _logger = logger;
            _securityService = securityService;
            _sqlConfigForm = sqlConfigForm;
            _methodsConfigForm = methodsConfigForm;
            _swaggerConfigForm = swaggerConfigForm;

            InitializeComponent();
            SetupMainForm();
            LoadForms();
        }

        private void SetupMainForm()
        {
            // Configuración de la ventana principal
            Text = "ApiSAPBridge - Configuración";
            Size = new Size(900, 700);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(800, 600);
            Icon = CreateAppIcon();

            // StatusStrip
            _statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(248, 249, 250)
            };

            _statusLabel = new ToolStripStatusLabel
            {
                Text = "Listo",
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _statusStrip.Items.Add(_statusLabel);

            // TabControl moderno
            _tabControl = new ModernTabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };

            // Configurar pestañas protegidas
            _tabControl.ProtectedTabs.AddRange(new[] { 1, 2 }); // Métodos y Swagger
            _tabControl.HideProtectedTabs(); // Inicialmente ocultas

            // Eventos
            _tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            // Agregar controles
            Controls.Add(_tabControl);
            Controls.Add(_statusStrip);

            _logger.LogInformation("Formulario principal inicializado");
            UpdateStatus("Aplicación iniciada correctamente");
        }

        private void LoadForms()
        {
            try
            {
                // Pestaña SQL (siempre visible)
                var sqlTab = new TabPage("🗄️ Configuración SQL")
                {
                    BackColor = Color.White,
                    Padding = new Padding(10)
                };
                _sqlConfigForm.TopLevel = false;
                _sqlConfigForm.FormBorderStyle = FormBorderStyle.None;
                _sqlConfigForm.Dock = DockStyle.Fill;
                sqlTab.Controls.Add(_sqlConfigForm);
                _sqlConfigForm.Show();

                // Pestaña Métodos (protegida)
                var methodsTab = new TabPage("⚙️ Métodos")
                {
                    BackColor = Color.White,
                    Padding = new Padding(10)
                };
                _methodsConfigForm.TopLevel = false;
                _methodsConfigForm.FormBorderStyle = FormBorderStyle.None;
                _methodsConfigForm.Dock = DockStyle.Fill;
                methodsTab.Controls.Add(_methodsConfigForm);
                _methodsConfigForm.Show();

                // Pestaña Swagger (protegida)
                var swaggerTab = new TabPage("📖 Swagger")
                {
                    BackColor = Color.White,
                    Padding = new Padding(10)
                };
                _swaggerConfigForm.TopLevel = false;
                _swaggerConfigForm.FormBorderStyle = FormBorderStyle.None;
                _swaggerConfigForm.Dock = DockStyle.Fill;
                swaggerTab.Controls.Add(_swaggerConfigForm);
                _swaggerConfigForm.Show();

                // Agregar pestañas al control
                _tabControl.TabPages.AddRange(new[] { sqlTab, methodsTab, swaggerTab });

                _logger.LogInformation("Formularios cargados correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formularios");
                MessageBox.Show($"Error al cargar formularios: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tabName = _tabControl.SelectedTab?.Text ?? "Desconocida";
            UpdateStatus($"Navegando a: {tabName.Replace("🗄️", "").Replace("⚙️", "").Replace("📖", "").Trim()}");

            // Si se autentica correctamente, mostrar pestañas protegidas
            if (!_isAuthenticated && (_tabControl.SelectedIndex == 1 || _tabControl.SelectedIndex == 2))
            {
                _isAuthenticated = true;
                _tabControl.ShowProtectedTabs();
                UpdateStatus("Autenticado - Pestañas avanzadas habilitadas");
                _logger.LogInformation("Usuario autenticado correctamente");
            }
        }

        private void UpdateStatus(string message)
        {
            _statusLabel.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
            Application.DoEvents();
        }

        private Icon CreateAppIcon()
        {
            // Crear un icono simple programáticamente
            var bitmap = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.FromArgb(0, 123, 255));
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillRectangle(brush, 8, 8, 16, 16);
                }
            }
            return Icon.FromHandle(bitmap.GetHicon());
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                "¿Está seguro que desea cerrar la aplicación?",
                "Confirmar cierre",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            _logger.LogInformation("Cerrando aplicación");
            base.OnFormClosing(e);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 700);
            Name = "MainForm";
            Text = "ApiSAPBridge - Configuración";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
