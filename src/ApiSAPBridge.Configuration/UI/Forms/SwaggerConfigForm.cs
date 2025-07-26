using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Configuration.UI.Forms
{
    public partial class SwaggerConfigForm : Form
    {
        private readonly ILogger<SwaggerConfigForm> _logger;

        public SwaggerConfigForm(ILogger<SwaggerConfigForm> logger)
        {
            _logger = logger;
            InitializeComponent();
            SetupControls();
        }

        private void SetupControls()
        {
            Text = "Configuración Swagger";
            Size = new Size(700, 500);
            BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "📖 Configuración Swagger (Implementación Fase 6)",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Location = new Point(30, 20),
                Size = new Size(640, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var placeholderLabel = new Label
            {
                Text = "Este formulario se implementará en la Fase 6\n\n" +
                       "Funcionalidades planificadas:\n" +
                       "• Control de visibilidad de endpoints en Swagger\n" +
                       "• Organización por categorías\n" +
                       "• Gestión de documentación\n" +
                       "• Vista previa de cambios",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(108, 117, 125),
                Location = new Point(30, 80),
                Size = new Size(640, 200),
                TextAlign = ContentAlignment.TopLeft
            };

            Controls.AddRange(new Control[] { titleLabel, placeholderLabel });
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 500);
            Name = "SwaggerConfigForm";
            ResumeLayout(false);
        }
    }
}