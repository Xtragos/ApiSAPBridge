using ApiSAPBridge.Configuration.Models;
using ApiSAPBridge.Configuration.Services;

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
        }

        private void LoadConfiguration()
        {
            // TODO: Implementar en Fase 3
            // Cargar la configuración en los controles
        }

        public SqlServerConfiguration GetConfiguration()
        {
            // TODO: Implementar en Fase 3
            // Obtener la configuración de los controles
            return _configuration;
        }

        // TODO: Implementar controles en Fase 3
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SqlConfigurationControl
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Name = "SqlConfigurationControl";
            this.Size = new Size(800, 600);
            this.ResumeLayout(false);
        }
    }
}