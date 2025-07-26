using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Configuration.UI.Forms
{
    public partial class MethodsConfigForm : Form
    {
        private readonly ILogger<MethodsConfigForm> _logger;

        public MethodsConfigForm(ILogger<MethodsConfigForm> logger)
        {
            _logger = logger;
            InitializeComponent();
            SetupControls();
        }

        private void SetupControls()
        {
            Text = "Gestión de Métodos SAP";
            Size = new Size(700, 500);
            BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "⚙️ Gestión de Métodos SAP (Implementación Fase 5)",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Location = new Point(30, 20),
                Size = new Size(640, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var placeholderLabel = new Label
            {
                Text = "Este formulario se implementará en la Fase 5\n\n" +
                       "Funcionalidades planificadas:\n" +
                       "• Control de métodos POST automáticos\n" +
                       "• Configuración de intervalos de sincronización\n" +
                       "• Activación/desactivación de endpoints\n" +
                       "• Monitoreo de ejecuciones",
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
            Name = "MethodsConfigForm";
            ResumeLayout(false);
        }
    }
}
