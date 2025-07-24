using ApiSAPBridge.Configuration.Models;
using ApiSAPBridge.Configuration.Services;

namespace ApiSAPBridge.Configuration.UserControls
{
    public partial class SwaggerConfigurationControl : UserControl
    {
        private readonly IConfigurationService _configurationService;
        private SwaggerConfiguration _configuration;

        public SwaggerConfigurationControl(IConfigurationService configurationService, SwaggerConfiguration configuration)
        {
            _configurationService = configurationService;
            _configuration = configuration;
            InitializeComponent();
        }

        public SwaggerConfiguration GetConfiguration() => _configuration;

        private void InitializeComponent()
        {
            // TODO: Implementar en Fase 5
            this.Name = "SwaggerConfigurationControl";
            this.Size = new Size(800, 600);
        }
    }
}