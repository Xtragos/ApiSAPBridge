namespace ApiSAPBridge.Configuration.Forms
{
    partial class MainForm
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
            this.tabControl = new TabControl();
            this.statusStrip = new StatusStrip();
            this.lblStatus = new ToolStripStatusLabel();
            this.lblConfigPath = new ToolStripStatusLabel();
            this.lblVersion = new ToolStripStatusLabel();
            this.toolStrip = new ToolStrip();
            this.btnShowMethods = new ToolStripButton();
            this.btnShowSwagger = new ToolStripButton();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.btnSaveAll = new ToolStripButton();
            this.btnExit = new ToolStripButton();
            this.statusStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();

            // tabControl
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Location = new Point(0, 25);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new Size(1024, 600);
            this.tabControl.TabIndex = 0;

            // statusStrip
            this.statusStrip.Items.AddRange(new ToolStripItem[] {
                this.lblStatus,
                this.lblConfigPath,
                this.lblVersion});
            this.statusStrip.Location = new Point(0, 625);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(1024, 22);
            this.statusStrip.TabIndex = 1;

            // lblStatus
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(150, 17);
            this.lblStatus.Text = "Listo";
            this.lblStatus.Spring = true;

            // lblConfigPath
            this.lblConfigPath.Name = "lblConfigPath";
            this.lblConfigPath.Size = new Size(300, 17);
            this.lblConfigPath.Text = "Archivo: ";

            // lblVersion
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new Size(100, 17);
            this.lblVersion.Text = "Versión: 1.0.0";

            // toolStrip
            this.toolStrip.Items.AddRange(new ToolStripItem[] {
                this.btnShowMethods,
                this.btnShowSwagger,
                this.toolStripSeparator1,
                this.btnSaveAll,
                this.btnExit});
            this.toolStrip.Location = new Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new Size(1024, 25);
            this.toolStrip.TabIndex = 2;

            // btnShowMethods
            this.btnShowMethods.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.btnShowMethods.Name = "btnShowMethods";
            this.btnShowMethods.Size = new Size(100, 22);
            this.btnShowMethods.Text = "🔒 Métodos SAP";
            this.btnShowMethods.ToolTipText = "Mostrar configuración de métodos SAP (requiere autenticación)";
            this.btnShowMethods.Click += new EventHandler(this.btnShowMethods_Click);

            // btnShowSwagger
            this.btnShowSwagger.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.btnShowSwagger.Name = "btnShowSwagger";
            this.btnShowSwagger.Size = new Size(100, 22);
            this.btnShowSwagger.Text = "🔒 Swagger";
            this.btnShowSwagger.ToolTipText = "Mostrar configuración de Swagger (requiere autenticación)";
            this.btnShowSwagger.Click += new EventHandler(this.btnShowSwagger_Click);

            // toolStripSeparator1
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(6, 25);

            // btnSaveAll
            this.btnSaveAll.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new Size(100, 22);
            this.btnSaveAll.Text = "💾 Guardar Todo";
            this.btnSaveAll.ToolTipText = "Guardar toda la configuración";
            this.btnSaveAll.Click += new EventHandler(this.btnSaveAll_Click);

            // btnExit
            this.btnExit.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new Size(50, 22);
            this.btnExit.Text = "❌ Salir";
            this.btnExit.ToolTipText = "Salir de la aplicación";
            this.btnExit.Click += new EventHandler(this.btnExit_Click);

            // MainForm
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1024, 647);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.MinimumSize = new Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = ApiSAPBridge.Configuration.Models.ConfigurationConstants.APP_TITLE;
            this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private TabControl tabControl;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblConfigPath;
        private ToolStripStatusLabel lblVersion;
        private ToolStrip toolStrip;
        private ToolStripButton btnShowMethods;
        private ToolStripButton btnShowSwagger;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btnSaveAll;
        private ToolStripButton btnExit;
    }
}