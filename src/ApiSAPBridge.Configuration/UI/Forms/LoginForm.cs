using System.Drawing;
using System.Windows.Forms;
using ApiSAPBridge.Configuration.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApiSAPBridge.Configuration.UI.Forms
{
    public partial class LoginForm : Form
    {
        private readonly ISecurityService _securityService;
        private TextBox _passwordTextBox;
        private Button _loginButton;
        private Button _cancelButton;
        private Label _messageLabel;
        private Panel _headerPanel;
        private bool _isAuthenticated = false;

        public LoginForm()
        {
            // Obtener servicio desde el service provider global (se configurará en Program.cs)
            var serviceProvider = Program.ServiceProvider;
            _securityService = serviceProvider.GetRequiredService<ISecurityService>();

            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            // Configuración del formulario
            Text = "Autenticación Requerida";
            Size = new Size(400, 280);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            // Panel de encabezado
            _headerPanel = new Panel
            {
                Size = new Size(400, 80),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(52, 58, 64)
            };

            var titleLabel = new Label
            {
                Text = "🔐 Acceso Restringido",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(360, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var subtitleLabel = new Label
            {
                Text = "Ingrese la contraseña para acceder a configuraciones avanzadas",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(206, 212, 218),
                Location = new Point(20, 45),
                Size = new Size(360, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _headerPanel.Controls.AddRange(new Control[] { titleLabel, subtitleLabel });

            // Campo de contraseña
            var passwordLabel = new Label
            {
                Text = "Contraseña de Administrador:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(30, 110),
                Size = new Size(340, 20),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            _passwordTextBox = new TextBox
            {
                Location = new Point(30, 135),
                Size = new Size(340, 30),
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Mensaje de estado
            _messageLabel = new Label
            {
                Location = new Point(30, 170),
                Size = new Size(340, 20),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(220, 53, 69),
                Text = ""
            };

            // Botones
            _loginButton = new Button
            {
                Text = "🔓 Desbloquear",
                Location = new Point(195, 210),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            _cancelButton = new Button
            {
                Text = "Cancelar",
                Location = new Point(325, 210),
                Size = new Size(80, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            // Eventos
            _loginButton.Click += LoginButton_Click;
            _cancelButton.Click += CancelButton_Click;
            _passwordTextBox.KeyDown += PasswordTextBox_KeyDown;

            // Agregar controles
            Controls.AddRange(new Control[]
            {
                _headerPanel, passwordLabel, _passwordTextBox,
                _messageLabel, _loginButton, _cancelButton
            });

            // Foco inicial
            _passwordTextBox.Focus();
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            await AuthenticateAsync();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private async void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await AuthenticateAsync();
            }
        }

        private async Task AuthenticateAsync()
        {
            if (string.IsNullOrWhiteSpace(_passwordTextBox.Text))
            {
                _messageLabel.Text = "Por favor ingrese la contraseña";
                return;
            }

            _loginButton.Enabled = false;
            _loginButton.Text = "Verificando...";
            _messageLabel.Text = "";

            try
            {
                // Verificar si está bloqueado
                if (await _securityService.IsLockedOutAsync())
                {
                    _messageLabel.Text = "Cuenta bloqueada temporalmente. Intente más tarde.";
                    return;
                }

                var result = await _securityService.AuthenticateAsync(_passwordTextBox.Text);

                if (result.IsAuthenticated)
                {
                    _isAuthenticated = true;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    _messageLabel.Text = result.Message;
                    _passwordTextBox.Clear();
                    _passwordTextBox.Focus();
                }
            }
            catch (Exception ex)
            {
                _messageLabel.Text = $"Error de autenticación: {ex.Message}";
            }
            finally
            {
                _loginButton.Enabled = true;
                _loginButton.Text = "🔓 Desbloquear";
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 280);
            Name = "LoginForm";
            ResumeLayout(false);
        }
    }
}