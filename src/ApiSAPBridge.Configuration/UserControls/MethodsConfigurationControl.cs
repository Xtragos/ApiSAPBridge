using ApiSAPBridge.Configuration.Models;
using ApiSAPBridge.Configuration.Services;

namespace ApiSAPBridge.Configuration.UserControls
{
    public partial class MethodsConfigurationControl : UserControl
    {
        private readonly IConfigurationService _configurationService;
        private SapAutomationConfiguration _configuration;

        public MethodsConfigurationControl(IConfigurationService configurationService, SapAutomationConfiguration configuration)
        {
            _configurationService = configurationService;
            _configuration = configuration;
            InitializeComponent();
        }

        public SapAutomationConfiguration GetConfiguration() => _configuration;

        private void InitializeComponent()
        {
            // TODO: Implementar en Fase 4
            this.Name = "MethodsConfigurationControl";
            this.Size = new Size(800, 600);
        }
    }
}