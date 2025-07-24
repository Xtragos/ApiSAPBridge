using ApiSAPBridge.ConfigTool.Services;


namespace ApiSAPBridge.ConfigTool.Forms
{
    public partial class AuthForm : Form
    {
        private readonly AuthenticationService _authService;
        private readonly string _section;
        private TextBox txtPassword;
        private Button btnOK, btnCancel;

        public AuthForm(string section)
        {
            _authService = new AuthenticationService();
            _section = section;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"🔐 Autenticación - {_section}";
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Título
            var lblTitle = new Label
            {
                Text = $"🔒 Acceso a configuración de {_section}",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                Location = new Point(20, 20),
                Size = new Size(350, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblSubtitle = new Label
            {
                Text = "Esta sección está protegida. Ingrese la contraseña:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Gray,
                Location = new Point(20, 50),
                Size = new Size(350, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Campo de contraseña
            var lblPassword = new Label
            {
                Text = "Contraseña:",
                Location = new Point(20, 85),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 10F)
            };

            txtPassword = new TextBox
            {
                Location = new Point(110, 82),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10F),
                UseSystemPasswordChar = true
            };
            txtPassword.KeyPress += TxtPassword_KeyPress;

            // Botones
            btnOK = new Button
            {
                Text = "✅ Acceder",
                Location = new Point(180, 120),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button
            {
                Text = "❌ Cancelar",
                Location = new Point(280, 120),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] {
                lblTitle, lblSubtitle, lblPassword, txtPassword, btnOK, btnCancel
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;

            // Focus inicial
            txtPassword.Focus();
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnOK_Click(sender, e);
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string password = txtPassword.Text;

            if (_authService.ValidatePassword(_section, password))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("❌ Contraseña incorrecta. Inténtelo de nuevo.",
                    "Error de Autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
