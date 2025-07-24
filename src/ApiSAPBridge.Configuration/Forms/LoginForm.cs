using ApiSAPBridge.Configuration.Helpers;
using ApiSAPBridge.Configuration.Models;

namespace ApiSAPBridge.Configuration.Forms
{
    public partial class LoginForm : Form
    {
        private readonly SecurityConfiguration _securityConfig;
        private readonly string _purpose;

        public bool IsAuthenticated { get; private set; }

        public LoginForm(SecurityConfiguration securityConfig, string purpose = "configuration")
        {
            _securityConfig = securityConfig;
            _purpose = purpose;

            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Autenticación Requerida";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            // Check if locked out
            if (SecurityHelper.IsLockedOut(_purpose, _securityConfig.MaxLoginAttempts, _securityConfig.LockoutMinutes))
            {
                var remaining = SecurityHelper.GetLockoutTimeRemaining(_purpose, _securityConfig.LockoutMinutes);
                MessageBox.Show($"Acceso bloqueado. Intente nuevamente en {remaining.Minutes} minutos y {remaining.Seconds} segundos.",
                    "Acceso Bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.Cancel;
                return;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var enteredPassword = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(enteredPassword))
            {
                MessageBox.Show("Por favor ingrese la contraseña.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Simple password verification (in production, use hashed passwords)
            if (enteredPassword == _securityConfig.ConfigurationPassword)
            {
                SecurityHelper.ClearAttempts(_purpose);
                IsAuthenticated = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                SecurityHelper.RegisterFailedAttempt(_purpose);
                var attemptCount = SecurityHelper.GetLockoutTimeRemaining(_purpose).TotalMinutes;
                var remainingAttempts = Math.Max(0, _securityConfig.MaxLoginAttempts - attemptCount);

                MessageBox.Show($"Contraseña incorrecta. Intentos restantes: {remainingAttempts}",
                    "Error de Autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtPassword.Text = "";
                txtPassword.Focus();

                if (SecurityHelper.IsLockedOut(_purpose, _securityConfig.MaxLoginAttempts, _securityConfig.LockoutMinutes))
                {
                    MessageBox.Show($"Demasiados intentos fallidos. Acceso bloqueado por {_securityConfig.LockoutMinutes} minutos.",
                        "Acceso Bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }
    }
}