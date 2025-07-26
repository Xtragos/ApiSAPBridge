using System.Drawing;
using System.Windows.Forms;
using ApiSAPBridge.Configuration.Models.DTOs;

namespace ApiSAPBridge.Configuration.UI.Controls
{
    public partial class ConnectionTestPanel : UserControl
    {
        private Label _statusLabel;
        private Label _messageLabel;
        private Label _timeLabel;
        private PictureBox _statusIcon;

        public ConnectionTestPanel()
        {
            InitializeComponent();
            SetupControls();
        }

        private void SetupControls()
        {
            Size = new Size(400, 80);
            BackColor = Color.FromArgb(248, 249, 250);
            BorderStyle = BorderStyle.FixedSingle;

            // Icono de estado
            _statusIcon = new PictureBox
            {
                Size = new Size(24, 24),
                Location = new Point(10, 28),
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            // Label de estado
            _statusLabel = new Label
            {
                Location = new Point(45, 10),
                Size = new Size(340, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            // Label de mensaje
            _messageLabel = new Label
            {
                Location = new Point(45, 30),
                Size = new Size(340, 20),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            // Label de tiempo
            _timeLabel = new Label
            {
                Location = new Point(45, 50),
                Size = new Size(340, 15),
                Font = new Font("Segoe UI", 7),
                ForeColor = Color.FromArgb(134, 142, 150)
            };

            Controls.AddRange(new Control[] { _statusIcon, _statusLabel, _messageLabel, _timeLabel });

            // Estado inicial
            SetIdle();
        }

        public void SetTesting()
        {
            _statusIcon.Image = CreateStatusIcon(Color.FromArgb(255, 193, 7)); // Amarillo
            _statusLabel.Text = "Probando conexión...";
            _statusLabel.ForeColor = Color.FromArgb(255, 193, 7);
            _messageLabel.Text = "Verificando conectividad con el servidor";
            _timeLabel.Text = "";
            BackColor = Color.FromArgb(255, 248, 220);
        }

        public void SetResult(ConnectionTestResult result)
        {
            if (result.IsSuccess)
            {
                _statusIcon.Image = CreateStatusIcon(Color.FromArgb(40, 167, 69)); // Verde
                _statusLabel.Text = "✓ Conexión exitosa";
                _statusLabel.ForeColor = Color.FromArgb(40, 167, 69);
                _messageLabel.Text = result.Message;
                _timeLabel.Text = $"Tiempo de respuesta: {result.ConnectionTime.TotalMilliseconds:F0} ms";
                BackColor = Color.FromArgb(212, 237, 218);
            }
            else
            {
                _statusIcon.Image = CreateStatusIcon(Color.FromArgb(220, 53, 69)); // Rojo
                _statusLabel.Text = "✗ Error de conexión";
                _statusLabel.ForeColor = Color.FromArgb(220, 53, 69);
                _messageLabel.Text = result.Message;
                _timeLabel.Text = result.ErrorDetails ?? "";
                BackColor = Color.FromArgb(248, 215, 218);
            }
        }

        public void SetIdle()
        {
            _statusIcon.Image = CreateStatusIcon(Color.FromArgb(108, 117, 125)); // Gris
            _statusLabel.Text = "Sin probar";
            _statusLabel.ForeColor = Color.FromArgb(73, 80, 87);
            _messageLabel.Text = "Haga clic en 'Probar Conexión' para verificar";
            _timeLabel.Text = "";
            BackColor = Color.FromArgb(248, 249, 250);
        }

        private Bitmap CreateStatusIcon(Color color)
        {
            var bitmap = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, 2, 2, 20, 20);
                }
            }
            return bitmap;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            ResumeLayout(false);
        }
    }
}