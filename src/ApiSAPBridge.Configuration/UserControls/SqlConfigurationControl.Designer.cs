namespace ApiSAPBridge.Configuration.UserControls
{
    partial class SqlConfigurationControl
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grpServerConfig = new GroupBox();
            this.lblServer = new Label();
            this.txtServer = new TextBox();
            this.lblDatabase = new Label();
            this.txtDatabase = new TextBox();
            this.lblConnectionTimeout = new Label();
            this.numConnectionTimeout = new NumericUpDown();
            this.chkTrustServerCert = new CheckBox();

            this.grpAuthentication = new GroupBox();
            this.chkWindowsAuth = new CheckBox();
            this.lblUsername = new Label();
            this.txtUsername = new TextBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();

            this.grpConnectionTest = new GroupBox();
            this.btnTestConnection = new Button();
            this.lblConnectionStatus = new Label();
            this.progressBar = new ProgressBar();

            this.grpConnectionString = new GroupBox();
            this.txtConnectionString = new TextBox();
            this.btnCopyConnectionString = new Button();

            this.grpActions = new GroupBox();
            this.btnResetDefaults = new Button();
            this.lblValidationStatus = new Label();

            this.toolTip = new ToolTip(this.components);

            this.grpServerConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConnectionTimeout)).BeginInit();
            this.grpAuthentication.SuspendLayout();
            this.grpConnectionTest.SuspendLayout();
            this.grpConnectionString.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.SuspendLayout();

            // 
            // grpServerConfig
            // 
            this.grpServerConfig.Controls.Add(this.lblServer);
            this.grpServerConfig.Controls.Add(this.txtServer);
            this.grpServerConfig.Controls.Add(this.lblDatabase);
            this.grpServerConfig.Controls.Add(this.txtDatabase);
            this.grpServerConfig.Controls.Add(this.lblConnectionTimeout);
            this.grpServerConfig.Controls.Add(this.numConnectionTimeout);
            this.grpServerConfig.Controls.Add(this.chkTrustServerCert);
            this.grpServerConfig.Location = new Point(15, 15);
            this.grpServerConfig.Name = "grpServerConfig";
            this.grpServerConfig.Size = new Size(370, 180);
            this.grpServerConfig.TabIndex = 0;
            this.grpServerConfig.TabStop = false;
            this.grpServerConfig.Text = "📡 Configuración del Servidor";

            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new Point(15, 30);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new Size(110, 13);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "Servidor SQL Server:";

            // 
            // txtServer
            // 
            this.txtServer.Location = new Point(15, 45);
            this.txtServer.Name = "txtServer";
            this.txtServer.PlaceholderText = "localhost o SERVIDOR\\INSTANCIA";
            this.txtServer.Size = new Size(340, 23);
            this.txtServer.TabIndex = 1;
            this.toolTip.SetToolTip(this.txtServer, "Nombre del servidor SQL Server o instancia");

            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new Point(15, 80);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new Size(85, 13);
            this.lblDatabase.TabIndex = 2;
            this.lblDatabase.Text = "Base de datos:";

            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new Point(15, 95);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.PlaceholderText = "ApiSAPBridge";
            this.txtDatabase.Size = new Size(340, 23);
            this.txtDatabase.TabIndex = 3;
            this.toolTip.SetToolTip(this.txtDatabase, "Nombre de la base de datos a utilizar");

            // 
            // lblConnectionTimeout
            // 
            this.lblConnectionTimeout.AutoSize = true;
            this.lblConnectionTimeout.Location = new Point(15, 130);
            this.lblConnectionTimeout.Name = "lblConnectionTimeout";
            this.lblConnectionTimeout.Size = new Size(120, 13);
            this.lblConnectionTimeout.TabIndex = 4;
            this.lblConnectionTimeout.Text = "Timeout conexión (seg):";

            // 
            // numConnectionTimeout
            // 
            this.numConnectionTimeout.Location = new Point(150, 128);
            this.numConnectionTimeout.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            this.numConnectionTimeout.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            this.numConnectionTimeout.Name = "numConnectionTimeout";
            this.numConnectionTimeout.Size = new Size(60, 23);
            this.numConnectionTimeout.TabIndex = 5;
            this.numConnectionTimeout.Value = new decimal(new int[] { 30, 0, 0, 0 });
            this.toolTip.SetToolTip(this.numConnectionTimeout, "Tiempo límite para la conexión en segundos");

            // 
            // chkTrustServerCert
            // 
            this.chkTrustServerCert.AutoSize = true;
            this.chkTrustServerCert.Checked = true;
            this.chkTrustServerCert.CheckState = CheckState.Checked;
            this.chkTrustServerCert.Location = new Point(250, 130);
            this.chkTrustServerCert.Name = "chkTrustServerCert";
            this.chkTrustServerCert.Size = new Size(105, 17);
            this.chkTrustServerCert.TabIndex = 6;
            this.chkTrustServerCert.Text = "Confiar en certificado";
            this.chkTrustServerCert.UseVisualStyleBackColor = true;
            this.toolTip.SetToolTip(this.chkTrustServerCert, "Confiar en el certificado del servidor SQL");

            // 
            // grpAuthentication
            // 
            this.grpAuthentication.Controls.Add(this.chkWindowsAuth);
            this.grpAuthentication.Controls.Add(this.lblUsername);
            this.grpAuthentication.Controls.Add(this.txtUsername);
            this.grpAuthentication.Controls.Add(this.lblPassword);
            this.grpAuthentication.Controls.Add(this.txtPassword);
            this.grpAuthentication.Location = new Point(400, 15);
            this.grpAuthentication.Name = "grpAuthentication";
            this.grpAuthentication.Size = new Size(350, 180);
            this.grpAuthentication.TabIndex = 1;
            this.grpAuthentication.TabStop = false;
            this.grpAuthentication.Text = "🔐 Autenticación";

            // 
            // chkWindowsAuth
            // 
            this.chkWindowsAuth.AutoSize = true;
            this.chkWindowsAuth.Checked = true;
            this.chkWindowsAuth.CheckState = CheckState.Checked;
            this.chkWindowsAuth.Location = new Point(15, 30);
            this.chkWindowsAuth.Name = "chkWindowsAuth";
            this.chkWindowsAuth.Size = new Size(150, 17);
            this.chkWindowsAuth.TabIndex = 0;
            this.chkWindowsAuth.Text = "Usar autenticación Windows";
            this.chkWindowsAuth.UseVisualStyleBackColor = true;
            this.toolTip.SetToolTip(this.chkWindowsAuth, "Usar credenciales del usuario actual de Windows");

            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new Point(15, 70);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new Size(50, 13);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Usuario:";

            // 
            // txtUsername
            // 
            this.txtUsername.Enabled = false;
            this.txtUsername.Location = new Point(15, 85);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.PlaceholderText = "sa o usuario SQL";
            this.txtUsername.Size = new Size(320, 23);
            this.txtUsername.TabIndex = 2;
            this.toolTip.SetToolTip(this.txtUsername, "Usuario de SQL Server (solo si no usa autenticación Windows)");

            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new Point(15, 120);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new Size(64, 13);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Contraseña:";

            // 
            // txtPassword
            // 
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new Point(15, 135);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.PlaceholderText = "Contraseña del usuario SQL";
            this.txtPassword.Size = new Size(320, 23);
            this.txtPassword.TabIndex = 4;
            this.toolTip.SetToolTip(this.txtPassword, "Contraseña del usuario de SQL Server");

            // 
            // grpConnectionTest
            // 
            this.grpConnectionTest.Controls.Add(this.btnTestConnection);
            this.grpConnectionTest.Controls.Add(this.lblConnectionStatus);
            this.grpConnectionTest.Controls.Add(this.progressBar);
            this.grpConnectionTest.Location = new Point(15, 210);
            this.grpConnectionTest.Name = "grpConnectionTest";
            this.grpConnectionTest.Size = new Size(370, 100);
            this.grpConnectionTest.TabIndex = 2;
            this.grpConnectionTest.TabStop = false;
            this.grpConnectionTest.Text = "🧪 Prueba de Conexión";

            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new Point(15, 30);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new Size(120, 35);
            this.btnTestConnection.TabIndex = 0;
            this.btnTestConnection.Text = "🔗 Probar Conexión";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new EventHandler(this.btnTestConnection_Click);
            this.toolTip.SetToolTip(this.btnTestConnection, "Probar la conexión con la configuración actual");

            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Location = new Point(150, 40);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new Size(120, 13);
            this.lblConnectionStatus.TabIndex = 1;
            this.lblConnectionStatus.Text = "⏳ No probado aún";

            // 
            // progressBar
            // 
            this.progressBar.Location = new Point(15, 75);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new Size(340, 15);
            this.progressBar.Style = ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 2;
            this.progressBar.Visible = false;

            // 
            // grpConnectionString
            // 
            this.grpConnectionString.Controls.Add(this.txtConnectionString);
            this.grpConnectionString.Controls.Add(this.btnCopyConnectionString);
            this.grpConnectionString.Location = new Point(400, 210);
            this.grpConnectionString.Name = "grpConnectionString";
            this.grpConnectionString.Size = new Size(350, 100);
            this.grpConnectionString.TabIndex = 3;
            this.grpConnectionString.TabStop = false;
            this.grpConnectionString.Text = "🔗 Cadena de Conexión";

            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Location = new Point(15, 30);
            this.txtConnectionString.Multiline = true;
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.ReadOnly = true;
            this.txtConnectionString.ScrollBars = ScrollBars.Vertical;
            this.txtConnectionString.Size = new Size(320, 35);
            this.txtConnectionString.TabIndex = 0;
            this.toolTip.SetToolTip(this.txtConnectionString, "Cadena de conexión generada automáticamente");

            // 
            // btnCopyConnectionString
            // 
            this.btnCopyConnectionString.Location = new Point(15, 70);
            this.btnCopyConnectionString.Name = "btnCopyConnectionString";
            this.btnCopyConnectionString.Size = new Size(100, 23);
            this.btnCopyConnectionString.TabIndex = 1;
            this.btnCopyConnectionString.Text = "📋 Copiar";
            this.btnCopyConnectionString.UseVisualStyleBackColor = true;
            this.btnCopyConnectionString.Click += new EventHandler(this.btnCopyConnectionString_Click);
            this.toolTip.SetToolTip(this.btnCopyConnectionString, "Copiar cadena de conexión al portapapeles");

            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnResetDefaults);
            this.grpActions.Controls.Add(this.lblValidationStatus);
            this.grpActions.Location = new Point(15, 330);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new Size(735, 60);
            this.grpActions.TabIndex = 4;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "⚙️ Acciones";

            // 
            // btnResetDefaults
            // 
            this.btnResetDefaults.Location = new Point(15, 25);
            this.btnResetDefaults.Name = "btnResetDefaults";
            this.btnResetDefaults.Size = new Size(120, 25);
            this.btnResetDefaults.TabIndex = 0;
            this.btnResetDefaults.Text = "🔄 Valores por defecto";
            this.btnResetDefaults.UseVisualStyleBackColor = true;
            this.btnResetDefaults.Click += new EventHandler(this.btnResetDefaults_Click);
            this.toolTip.SetToolTip(this.btnResetDefaults, "Restaurar valores por defecto");

            // 
            // lblValidationStatus
            // 
            this.lblValidationStatus.AutoSize = true;
            this.lblValidationStatus.Location = new Point(200, 30);
            this.lblValidationStatus.Name = "lblValidationStatus";
            this.lblValidationStatus.Size = new Size(120, 13);
            this.lblValidationStatus.TabIndex = 1;
            this.lblValidationStatus.Text = "⏳ Validando...";

            // 
            // SqlConfigurationControl
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.grpServerConfig);
            this.Controls.Add(this.grpAuthentication);
            this.Controls.Add(this.grpConnectionTest);
            this.Controls.Add(this.grpConnectionString);
            this.Controls.Add(this.grpActions);
            this.Name = "SqlConfigurationControl";
            this.Size = new Size(770, 410);
            this.grpServerConfig.ResumeLayout(false);
            this.grpServerConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConnectionTimeout)).EndInit();
            this.grpAuthentication.ResumeLayout(false);
            this.grpAuthentication.PerformLayout();
            this.grpConnectionTest.ResumeLayout(false);
            this.grpConnectionTest.PerformLayout();
            this.grpConnectionString.ResumeLayout(false);
            this.grpConnectionString.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpActions.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private GroupBox grpServerConfig;
        private Label lblServer;
        private TextBox txtServer;
        private Label lblDatabase;
        private TextBox txtDatabase;
        private Label lblConnectionTimeout;
        private NumericUpDown numConnectionTimeout;
        private CheckBox chkTrustServerCert;

        private GroupBox grpAuthentication;
        private CheckBox chkWindowsAuth;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;

        private GroupBox grpConnectionTest;
        private Button btnTestConnection;
        private Label lblConnectionStatus;
        private ProgressBar progressBar;

        private GroupBox grpConnectionString;
        private TextBox txtConnectionString;
        private Button btnCopyConnectionString;

        private GroupBox grpActions;
        private Button btnResetDefaults;
        private Label lblValidationStatus;

        private ToolTip toolTip;
    }
}