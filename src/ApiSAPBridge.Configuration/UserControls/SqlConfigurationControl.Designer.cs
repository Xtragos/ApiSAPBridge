namespace ApiSAPBridge.Configuration.UserControls
{
    partial class SqlConfigurationControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpConnectionSettings = new GroupBox();
            this.lblServer = new Label();
            this.txtServer = new TextBox();
            this.lblDatabase = new Label();
            this.txtDatabase = new TextBox();
            this.lblConnectionTimeout = new Label();
            this.numConnectionTimeout = new NumericUpDown();
            this.chkTrustServerCertificate = new CheckBox();

            this.grpAuthentication = new GroupBox();
            this.chkWindowsAuth = new CheckBox();
            this.lblUsername = new Label();
            this.txtUsername = new TextBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();

            this.grpValidation = new GroupBox();
            this.lblValidationMessage = new Label();
            this.btnTestConnection = new Button();
            this.lblConnectionStatus = new Label();

            this.grpActions = new GroupBox();
            this.btnResetDefaults = new Button();
            this.btnGenerateConnectionString = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.numConnectionTimeout)).BeginInit();
            this.grpConnectionSettings.SuspendLayout();
            this.grpAuthentication.SuspendLayout();
            this.grpValidation.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.SuspendLayout();

            // grpConnectionSettings
            this.grpConnectionSettings.Controls.Add(this.chkTrustServerCertificate);
            this.grpConnectionSettings.Controls.Add(this.numConnectionTimeout);
            this.grpConnectionSettings.Controls.Add(this.lblConnectionTimeout);
            this.grpConnectionSettings.Controls.Add(this.txtDatabase);
            this.grpConnectionSettings.Controls.Add(this.lblDatabase);
            this.grpConnectionSettings.Controls.Add(this.txtServer);
            this.grpConnectionSettings.Controls.Add(this.lblServer);
            this.grpConnectionSettings.Location = new Point(12, 12);
            this.grpConnectionSettings.Name = "grpConnectionSettings";
            this.grpConnectionSettings.Size = new Size(400, 180);
            this.grpConnectionSettings.TabIndex = 0;
            this.grpConnectionSettings.TabStop = false;
            this.grpConnectionSettings.Text = "Configuración de Conexión";

            // lblServer
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new Point(20, 30);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new Size(46, 13);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "Servidor:";

            // txtServer
            this.txtServer.Location = new Point(120, 27);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new Size(260, 20);
            this.txtServer.TabIndex = 1;
            this.txtServer.Text = "localhost";

            // lblDatabase
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new Point(20, 60);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new Size(78, 13);
            this.lblDatabase.TabIndex = 2;
            this.lblDatabase.Text = "Base de Datos:";

            // txtDatabase
            this.txtDatabase.Location = new Point(120, 57);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new Size(260, 20);
            this.txtDatabase.TabIndex = 3;
            this.txtDatabase.Text = "ApiSAPBridge";

            // lblConnectionTimeout
            this.lblConnectionTimeout.AutoSize = true;
            this.lblConnectionTimeout.Location = new Point(20, 90);
            this.lblConnectionTimeout.Name = "lblConnectionTimeout";
            this.lblConnectionTimeout.Size = new Size(90, 13);
            this.lblConnectionTimeout.TabIndex = 4;
            this.lblConnectionTimeout.Text = "Timeout (seg):";

            // numConnectionTimeout
            this.numConnectionTimeout.Location = new Point(120, 87);
            this.numConnectionTimeout.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            this.numConnectionTimeout.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            this.numConnectionTimeout.Name = "numConnectionTimeout";
            this.numConnectionTimeout.Size = new Size(100, 20);
            this.numConnectionTimeout.TabIndex = 5;
            this.numConnectionTimeout.Value = new decimal(new int[] { 30, 0, 0, 0 });

            // chkTrustServerCertificate
            this.chkTrustServerCertificate.AutoSize = true;
            this.chkTrustServerCertificate.Checked = true;
            this.chkTrustServerCertificate.CheckState = CheckState.Checked;
            this.chkTrustServerCertificate.Location = new Point(20, 120);
            this.chkTrustServerCertificate.Name = "chkTrustServerCertificate";
            this.chkTrustServerCertificate.Size = new Size(180, 17);
            this.chkTrustServerCertificate.TabIndex = 6;
            this.chkTrustServerCertificate.Text = "Confiar en certificado del servidor";
            this.chkTrustServerCertificate.UseVisualStyleBackColor = true;

            // grpAuthentication
            this.grpAuthentication.Controls.Add(this.txtPassword);
            this.grpAuthentication.Controls.Add(this.lblPassword);
            this.grpAuthentication.Controls.Add(this.txtUsername);
            this.grpAuthentication.Controls.Add(this.lblUsername);
            this.grpAuthentication.Controls.Add(this.chkWindowsAuth);
            this.grpAuthentication.Location = new Point(430, 12);
            this.grpAuthentication.Name = "grpAuthentication";
            this.grpAuthentication.Size = new Size(350, 180);
            this.grpAuthentication.TabIndex = 1;
            this.grpAuthentication.TabStop = false;
            this.grpAuthentication.Text = "Autenticación";

            // chkWindowsAuth
            this.chkWindowsAuth.AutoSize = true;
            this.chkWindowsAuth.Checked = true;
            this.chkWindowsAuth.CheckState = CheckState.Checked;
            this.chkWindowsAuth.Location = new Point(20, 30);
            this.chkWindowsAuth.Name = "chkWindowsAuth";
            this.chkWindowsAuth.Size = new Size(180, 17);
            this.chkWindowsAuth.TabIndex = 0;
            this.chkWindowsAuth.Text = "Usar autenticación de Windows";
            this.chkWindowsAuth.UseVisualStyleBackColor = true;

            // lblUsername
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new Point(20, 70);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new Size(49, 13);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Usuario:";

            // txtUsername
            this.txtUsername.Location = new Point(20, 87);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new Size(300, 20);
            this.txtUsername.TabIndex = 2;

            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new Point(20, 120);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new Size(64, 13);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Contraseña:";

            // txtPassword
            this.txtPassword.Location = new Point(20, 137);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new Size(300, 20);
            this.txtPassword.TabIndex = 4;

            // grpValidation
            this.grpValidation.Controls.Add(this.lblConnectionStatus);
            this.grpValidation.Controls.Add(this.btnTestConnection);
            this.grpValidation.Controls.Add(this.lblValidationMessage);
            this.grpValidation.Location = new Point(12, 210);
            this.grpValidation.Name = "grpValidation";
            this.grpValidation.Size = new Size(400, 120);
            this.grpValidation.TabIndex = 2;
            this.grpValidation.TabStop = false;
            this.grpValidation.Text = "Validación";

            // lblValidationMessage
            this.lblValidationMessage.Location = new Point(20, 30);
            this.lblValidationMessage.Name = "lblValidationMessage";
            this.lblValidationMessage.Size = new Size(360, 20);
            this.lblValidationMessage.TabIndex = 0;
            this.lblValidationMessage.Text = "✅ Configuración válida";
            this.lblValidationMessage.ForeColor = Color.Green;

            // btnTestConnection
            this.btnTestConnection.Location = new Point(20, 60);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new Size(120, 30);
            this.btnTestConnection.TabIndex = 1;
            this.btnTestConnection.Text = "Probar Conexión";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new EventHandler(this.btnTestConnection_Click);

            // lblConnectionStatus
            this.lblConnectionStatus.Location = new Point(160, 60);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new Size(220, 30);
            this.lblConnectionStatus.TabIndex = 2;
            this.lblConnectionStatus.Text = "Presione 'Probar Conexión' para validar";
            this.lblConnectionStatus.TextAlign = ContentAlignment.MiddleLeft;

            // grpActions
            this.grpActions.Controls.Add(this.btnGenerateConnectionString);
            this.grpActions.Controls.Add(this.btnResetDefaults);
            this.grpActions.Location = new Point(430, 210);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new Size(350, 120);
            this.grpActions.TabIndex = 3;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Acciones";

            // btnResetDefaults
            this.btnResetDefaults.Location = new Point(20, 30);
            this.btnResetDefaults.Name = "btnResetDefaults";
            this.btnResetDefaults.Size = new Size(150, 30);
            this.btnResetDefaults.TabIndex = 0;
            this.btnResetDefaults.Text = "Valores por Defecto";
            this.btnResetDefaults.UseVisualStyleBackColor = true;
            this.btnResetDefaults.Click += new EventHandler(this.btnResetDefaults_Click);

            // btnGenerateConnectionString
            this.btnGenerateConnectionString.Location = new Point(20, 70);
            this.btnGenerateConnectionString.Name = "btnGenerateConnectionString";
            this.btnGenerateConnectionString.Size = new Size(150, 30);
            this.btnGenerateConnectionString.TabIndex = 1;
            this.btnGenerateConnectionString.Text = "Ver Cadena Conexión";
            this.btnGenerateConnectionString.UseVisualStyleBackColor = true;
            this.btnGenerateConnectionString.Click += new EventHandler(this.btnGenerateConnectionString_Click);

            // SqlConfigurationControl
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpValidation);
            this.Controls.Add(this.grpAuthentication);
            this.Controls.Add(this.grpConnectionSettings);
            this.Name = "SqlConfigurationControl";
            this.Size = new Size(800, 350);
            ((System.ComponentModel.ISupportInitialize)(this.numConnectionTimeout)).EndInit();
            this.grpConnectionSettings.ResumeLayout(false);
            this.grpConnectionSettings.PerformLayout();
            this.grpAuthentication.ResumeLayout(false);
            this.grpAuthentication.PerformLayout();
            this.grpValidation.ResumeLayout(false);
            this.grpActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private GroupBox grpConnectionSettings;
        private Label lblServer;
        private TextBox txtServer;
        private Label lblDatabase;
        private TextBox txtDatabase;
        private Label lblConnectionTimeout;
        private NumericUpDown numConnectionTimeout;
        private CheckBox chkTrustServerCertificate;

        private GroupBox grpAuthentication;
        private CheckBox chkWindowsAuth;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;

        private GroupBox grpValidation;
        private Label lblValidationMessage;
        private Button btnTestConnection;
        private Label lblConnectionStatus;

        private GroupBox grpActions;
        private Button btnResetDefaults;
        private Button btnGenerateConnectionString;
    }
}