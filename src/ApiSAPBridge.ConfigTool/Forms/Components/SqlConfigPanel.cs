using ApiSAPBridge.ConfigTool.Models;

namespace ApiSAPBridge.ConfigTool.Forms
{
    public partial class SqlConfigPanel : UserControl
    {
        private SqlConnectionConfig _config = new SqlConnectionConfig();

        // Controles del formulario
        private TextBox txtServer, txtDatabase, txtUsername, txtPassword;
        private NumericUpDown numPort, numTimeout, numMinPool, numMaxPool;
        private CheckBox chkWindowsAuth, chkConnectionPooling;
        private GroupBox grpConnection, grpAdvanced, grpPooling;

        public SqlConfigPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 600);
            this.BackColor = Color.White;
            this.AutoScroll = true;

            // Título
            var lblTitle = new Label
            {
                Text = "🗄️ Configuración de SQL Server",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Location = new Point(20, 20),
                Size = new Size(400, 30)
            };

            var lblSubtitle = new Label
            {
                Text = "Configure la conexión a la base de datos que utilizará la API",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Gray,
                Location = new Point(20, 55),
                Size = new Size(500, 20)
            };

            // Grupo de Conexión Básica
            grpConnection = new GroupBox
            {
                Text = "📡 Conexión Básica",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 90),
                Size = new Size(750, 200),
                ForeColor = Color.FromArgb(52, 58, 64)
            };

            // Servidor
            var lblServer = new Label
            {
                Text = "Servidor:",
                Location = new Point(20, 35),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10F)
            };

            txtServer = new TextBox
            {
                Location = new Point(130, 32),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "localhost o IP del servidor"
            };

            // Puerto
            var lblPort = new Label
            {
                Text = "Puerto:",
                Location = new Point(350, 35),
                Size = new Size(50, 20),
                Font = new Font("Segoe UI", 10F)
            };

            numPort = new NumericUpDown
            {
                Location = new Point(410, 32),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10F),
                Minimum = 1,
                Maximum = 65535,
                Value = 1433
            };

            // Base de Datos
            var lblDatabase = new Label
            {
                Text = "Base de Datos:",
                Location = new Point(20, 70),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10F)
            };

            txtDatabase = new TextBox
            {
                Location = new Point(130, 67),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10F),
                Text = "ApiSAP"
            };

            // Windows Authentication
            chkWindowsAuth = new CheckBox
            {
                Text = "🔐 Usar Windows Authentication",
                Location = new Point(20, 105),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10F),
                Checked = true
            };
            chkWindowsAuth.CheckedChanged += ChkWindowsAuth_CheckedChanged;

            // Usuario
            var lblUsername = new Label
            {
                Text = "Usuario:",
                Location = new Point(20, 140),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10F)
            };

            txtUsername = new TextBox
            {
                Location = new Point(130, 137),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10F),
                Enabled = false
            };

            // Contraseña
            var lblPassword = new Label
            {
                Text = "Contraseña:",
                Location = new Point(300, 140),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 10F)
            };

            txtPassword = new TextBox
            {
                Location = new Point(390, 137),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10F),
                UseSystemPasswordChar = true,
                Enabled = false
            };

            grpConnection.Controls.AddRange(new Control[] {
                lblServer, txtServer, lblPort, numPort,
                lblDatabase, txtDatabase, chkWindowsAuth,
                lblUsername, txtUsername, lblPassword, txtPassword
            });

            // Grupo de Configuración Avanzada
            grpAdvanced = new GroupBox
            {
                Text = "⚙️ Configuración Avanzada",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 310),
                Size = new Size(750, 100),
                ForeColor = Color.FromArgb(52, 58, 64)
            };

            // Timeout
            var lblTimeout = new Label
            {
                Text = "Timeout (seg):",
                Location = new Point(20, 35),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10F)
            };

            numTimeout = new NumericUpDown
            {
                Location = new Point(130, 32),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10F),
                Minimum = 5,
                Maximum = 300,
                Value = 30
            };

            // Connection Pooling
            chkConnectionPooling = new CheckBox
            {
                Text = "🔄 Habilitar Pool de Conexiones",
                Location = new Point(250, 35),
                Size = new Size(220, 25),
                Font = new Font("Segoe UI", 10F),
                Checked = true
            };

            grpAdvanced.Controls.AddRange(new Control[] {
                lblTimeout, numTimeout, chkConnectionPooling
            });

            // Grupo de Pool de Conexiones
            grpPooling = new GroupBox
            {
                Text = "🔄 Pool de Conexiones",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 430),
                Size = new Size(750, 100),
                ForeColor = Color.FromArgb(52, 58, 64)
            };

            // Pool Mínimo
            var lblMinPool = new Label
            {
                Text = "Tamaño Mínimo:",
                Location = new Point(20, 35),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 10F)
            };

            numMinPool = new NumericUpDown
            {
                Location = new Point(150, 32),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10F),
                Minimum = 0,
                Maximum = 100,
                Value = 5
            };

            // Pool Máximo
            var lblMaxPool = new Label
            {
                Text = "Tamaño Máximo:",
                Location = new Point(250, 35),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 10F)
            };

            numMaxPool = new NumericUpDown
            {
                Location = new Point(380, 32),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10F),
                Minimum = 1,
                Maximum = 1000,
                Value = 100
            };

            grpPooling.Controls.AddRange(new Control[] {
                lblMinPool, numMinPool, lblMaxPool, numMaxPool
            });

            // Ejemplo de Connection String
            var lblConnectionString = new Label
            {
                Text = "🔗 Vista Previa de Connection String:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 550),
                Size = new Size(300, 25),
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            var txtConnectionString = new TextBox
            {
                Location = new Point(20, 580),
                Size = new Size(750, 50),
                Font = new Font("Consolas", 9F),
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = Color.FromArgb(52, 58, 64)
            };

            // Evento para actualizar connection string en tiempo real
            void UpdateConnectionString()
            {
                var tempConfig = GetConfig();
                txtConnectionString.Text = tempConfig.GetConnectionString();
            }

            txtServer.TextChanged += (s, e) => UpdateConnectionString();
            txtDatabase.TextChanged += (s, e) => UpdateConnectionString();
            txtUsername.TextChanged += (s, e) => UpdateConnectionString();
            numPort.ValueChanged += (s, e) => UpdateConnectionString();
            chkWindowsAuth.CheckedChanged += (s, e) => UpdateConnectionString();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblSubtitle, grpConnection, grpAdvanced,
                grpPooling, lblConnectionString, txtConnectionString
            });

            // Actualizar vista inicial
            UpdateConnectionString();
        }

        private void ChkWindowsAuth_CheckedChanged(object sender, EventArgs e)
        {
            bool useWindowsAuth = chkWindowsAuth.Checked;
            txtUsername.Enabled = !useWindowsAuth;
            txtPassword.Enabled = !useWindowsAuth;

            if (useWindowsAuth)
            {
                txtUsername.Clear();
                txtPassword.Clear();
            }
        }

        public void LoadConfig(SqlConnectionConfig config)
        {
            _config = config ?? new SqlConnectionConfig();

            txtServer.Text = _config.Server;
            txtDatabase.Text = _config.Database;
            txtUsername.Text = _config.Username;
            txtPassword.Text = _config.Password;
            numPort.Value = _config.Port;
            numTimeout.Value = _config.ConnectionTimeout;
            chkWindowsAuth.Checked = _config.UseWindowsAuth;
            chkConnectionPooling.Checked = _config.EnableConnectionPooling;
            numMinPool.Value = _config.MinPoolSize;
            numMaxPool.Value = _config.MaxPoolSize;
        }

        public SqlConnectionConfig GetConfig()
        {
            return new SqlConnectionConfig
            {
                Server = txtServer.Text.Trim(),
                Database = txtDatabase.Text.Trim(),
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text,
                Port = (int)numPort.Value,
                ConnectionTimeout = (int)numTimeout.Value,
                UseWindowsAuth = chkWindowsAuth.Checked,
                EnableConnectionPooling = chkConnectionPooling.Checked,
                MinPoolSize = (int)numMinPool.Value,
                MaxPoolSize = (int)numMaxPool.Value
            };
        }
    }
}