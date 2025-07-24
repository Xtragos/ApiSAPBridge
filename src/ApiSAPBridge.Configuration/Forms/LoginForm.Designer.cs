namespace ApiSAPBridge.Configuration.Forms
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.btnCancel = new Button();
            this.lblTitle = new Label();
            this.lblPassword = new Label();
            this.pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();

            // pictureBox1
            this.pictureBox1.Location = new Point(20, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(48, 48);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            this.lblTitle.Location = new Point(80, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(250, 20);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Acceso a Configuración Avanzada";

            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new Point(20, 90);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new Size(64, 13);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Contraseña:";

            // txtPassword
            this.txtPassword.Location = new Point(20, 110);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new Size(340, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.KeyDown += new KeyEventHandler(this.txtPassword_KeyDown);

            // btnLogin
            this.btnLogin.Location = new Point(190, 160);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new Size(80, 30);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Ingresar";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

            // btnCancel
            this.btnCancel.Location = new Point(280, 160);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // LoginForm
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(384, 211);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pictureBox1);
            this.Name = "LoginForm";
            this.Text = "Autenticación";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnCancel;
        private Label lblTitle;
        private Label lblPassword;
        private PictureBox pictureBox1;
    }
}