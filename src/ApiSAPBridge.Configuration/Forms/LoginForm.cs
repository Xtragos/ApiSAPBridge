using ApiSAPBridge.Configuration.Services;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Configuration.Forms
{
    public partial class LoginForm : UserControl
    {
        private readonly ConfigurationService _configService;
        private TextBox _passwordTextBox;
        private Button _okButton;
        private Button _cancelButton;
        private Label _messageLabel;
        private PictureBox _iconPictureBox;

        public LoginForm(ConfigurationService configService)
        {
            _configService = configService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Configuración del formulario
            Text = "Autenticación Requerida";
            Size = new Size(400, 250);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            BackColor = Color.White;

            // Icono de seguridad
            _iconPictureBox = new PictureBox
            {
                Size = new Size(48, 48),
                Location = new Point(20, 20),
                Image = SystemIcons.Shield.ToBitmap(),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Mensaje
            _messageLabel = new Label
            {
                Text = "Esta sección está protegida.\nIngrese la contraseña de administrador:",
                Location = new Point(80, 25),
                Size = new Size(300, 40),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(68, 68, 68)
            };

            // Campo de contraseña
            var passwordLabel = new Label
            {
                Text = "Contraseña:",
                Location = new Point(80, 80),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(68, 68, 68)
            };

            _passwordTextBox = new TextBox
            {
                Location = new Point(80, 105),
                Size = new Size(280, 25),
                Font = new Font("Segoe UI", 10F),
                UseSystemPasswordChar = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            _passwordTextBox.KeyDown += PasswordTextBox_KeyDown;

            // Botones
            _okButton = new Button
            {
                Text = "Aceptar",
                Size = new Size(80, 35),
                Location = new Point(200, 160),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            _okButton.FlatAppearance.BorderSize = 0;
            _okButton.Click += OkButton_Click;

            _cancelButton = new Button
            {
                Text = "Cancelar",
                Size = new Size(80, 35),
                Location = new Point(290, 160),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            _cancelButton.FlatAppearance.BorderSize = 0;

            // Agregar controles
            Controls.AddRange(new Control[] {
                _iconPictureBox, _messageLabel, passwordLabel,
                _passwordTextBox, _okButton, _cancelButton
            });

            // Configurar botón por defecto
            AcceptButton = _okButton;
            CancelButton = _cancelButton;

            ResumeLayout(false);
        }

        private void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OkButton_Click(sender, e);
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            var password = _passwordTextBox.Text;

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor ingrese una contraseña",
                    "Contraseña Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _passwordTextBox.Focus();
                return;
            }

            if (_configService.ValidateAdminPassword(password))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Contraseña incorrecta",
                    "Error de Autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _passwordTextBox.Clear();
                _passwordTextBox.Focus();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _passwordTextBox.Focus();
        }
    }
}