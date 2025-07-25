namespace ApiSAPBridge.Configuration.UserControls
{
    partial class MethodsConfigurationControl
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
            this.grpSyncSettings = new GroupBox();
            this.chkEnableSync = new CheckBox();
            this.lblSyncInterval = new Label();
            this.numSyncInterval = new NumericUpDown();
            this.lblLastSync = new Label();

            this.grpEndpoints = new GroupBox();
            this.dgvEndpoints = new DataGridView();
            this.colEnabled = new DataGridViewCheckBoxColumn();
            this.colName = new DataGridViewTextBoxColumn();
            this.colEndpoint = new DataGridViewTextBoxColumn();
            this.colMethod = new DataGridViewTextBoxColumn();
            this.colPriority = new DataGridViewTextBoxColumn();
            this.colLastExecuted = new DataGridViewTextBoxColumn();
            this.colRequiresAuth = new DataGridViewCheckBoxColumn();

            this.grpActions = new GroupBox();
            this.btnAddEndpoint = new Button();
            this.btnEditEndpoint = new Button();
            this.btnDeleteEndpoint = new Button();
            this.btnMoveUp = new Button();
            this.btnMoveDown = new Button();
            this.btnTestEndpoint = new Button();
            this.btnExecuteNow = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.numSyncInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndpoints)).BeginInit();
            this.grpSyncSettings.SuspendLayout();
            this.grpEndpoints.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.SuspendLayout();

            // grpSyncSettings
            this.grpSyncSettings.Controls.Add(this.lblLastSync);
            this.grpSyncSettings.Controls.Add(this.numSyncInterval);
            this.grpSyncSettings.Controls.Add(this.lblSyncInterval);
            this.grpSyncSettings.Controls.Add(this.chkEnableSync);
            this.grpSyncSettings.Location = new Point(12, 12);
            this.grpSyncSettings.Name = "grpSyncSettings";
            this.grpSyncSettings.Size = new Size(760, 100);
            this.grpSyncSettings.TabIndex = 0;
            this.grpSyncSettings.TabStop = false;
            this.grpSyncSettings.Text = "Configuración de Sincronización SAP";

            // chkEnableSync
            this.chkEnableSync.AutoSize = true;
            this.chkEnableSync.Checked = true;
            this.chkEnableSync.CheckState = CheckState.Checked;
            this.chkEnableSync.Location = new Point(20, 30);
            this.chkEnableSync.Name = "chkEnableSync";
            this.chkEnableSync.Size = new Size(180, 17);
            this.chkEnableSync.TabIndex = 0;
            this.chkEnableSync.Text = "Habilitar sincronización automática";
            this.chkEnableSync.UseVisualStyleBackColor = true;
            this.chkEnableSync.CheckedChanged += new EventHandler(this.chkEnableSync_CheckedChanged);

            // lblSyncInterval
            this.lblSyncInterval.AutoSize = true;
            this.lblSyncInterval.Location = new Point(250, 32);
            this.lblSyncInterval.Name = "lblSyncInterval";
            this.lblSyncInterval.Size = new Size(120, 13);
            this.lblSyncInterval.TabIndex = 1;
            this.lblSyncInterval.Text = "Intervalo (minutos):";

            // numSyncInterval
            this.numSyncInterval.Location = new Point(370, 30);
            this.numSyncInterval.Maximum = new decimal(new int[] { 1440, 0, 0, 0 });
            this.numSyncInterval.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            this.numSyncInterval.Name = "numSyncInterval";
            this.numSyncInterval.Size = new Size(80, 20);
            this.numSyncInterval.TabIndex = 2;
            this.numSyncInterval.Value = new decimal(new int[] { 30, 0, 0, 0 });

            // lblLastSync
            this.lblLastSync.Location = new Point(20, 60);
            this.lblLastSync.Name = "lblLastSync";
            this.lblLastSync.Size = new Size(500, 20);
            this.lblLastSync.TabIndex = 3;
            this.lblLastSync.Text = "Última sincronización: Nunca ejecutado";

            // grpEndpoints
            this.grpEndpoints.Controls.Add(this.dgvEndpoints);
            this.grpEndpoints.Location = new Point(12, 130);
            this.grpEndpoints.Name = "grpEndpoints";
            this.grpEndpoints.Size = new Size(600, 350);
            this.grpEndpoints.TabIndex = 1;
            this.grpEndpoints.TabStop = false;
            this.grpEndpoints.Text = "Endpoints SAP";

            // dgvEndpoints
            this.dgvEndpoints.AllowUserToAddRows = false;
            this.dgvEndpoints.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEndpoints.Columns.AddRange(new DataGridViewColumn[] {
                this.colEnabled,
                this.colName,
                this.colEndpoint,
                this.colMethod,
                this.colPriority,
                this.colLastExecuted,
                this.colRequiresAuth});
            this.dgvEndpoints.Dock = DockStyle.Fill;
            this.dgvEndpoints.Location = new Point(3, 16);
            this.dgvEndpoints.MultiSelect = false;
            this.dgvEndpoints.Name = "dgvEndpoints";
            this.dgvEndpoints.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndpoints.Size = new Size(594, 331);
            this.dgvEndpoints.TabIndex = 0;
            this.dgvEndpoints.CellValueChanged += new DataGridViewCellEventHandler(this.dgvEndpoints_CellValueChanged);

            // Configurar columnas
            this.colEnabled.HeaderText = "Activo";
            this.colEnabled.Name = "colEnabled";
            this.colEnabled.Width = 60;

            this.colName.HeaderText = "Nombre";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 120;

            this.colEndpoint.HeaderText = "Endpoint";
            this.colEndpoint.Name = "colEndpoint";
            this.colEndpoint.ReadOnly = true;
            this.colEndpoint.Width = 150;

            this.colMethod.HeaderText = "Método";
            this.colMethod.Name = "colMethod";
            this.colMethod.ReadOnly = true;
            this.colMethod.Width = 70;

            this.colPriority.HeaderText = "Prioridad";
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            this.colPriority.Width = 70;

            this.colLastExecuted.HeaderText = "Última Ejecución";
            this.colLastExecuted.Name = "colLastExecuted";
            this.colLastExecuted.ReadOnly = true;
            this.colLastExecuted.Width = 100;

            this.colRequiresAuth.HeaderText = "Auth";
            this.colRequiresAuth.Name = "colRequiresAuth";
            this.colRequiresAuth.Width = 50;

            // grpActions
            this.grpActions.Controls.Add(this.btnExecuteNow);
            this.grpActions.Controls.Add(this.btnTestEndpoint);
            this.grpActions.Controls.Add(this.btnMoveDown);
            this.grpActions.Controls.Add(this.btnMoveUp);
            this.grpActions.Controls.Add(this.btnDeleteEndpoint);
            this.grpActions.Controls.Add(this.btnEditEndpoint);
            this.grpActions.Controls.Add(this.btnAddEndpoint);
            this.grpActions.Location = new Point(630, 130);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new Size(142, 350);
            this.grpActions.TabIndex = 2;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Acciones";

            // Configurar botones
            var buttonY = 30;
            var buttonHeight = 35;
            var buttonSpacing = 45;

            this.btnAddEndpoint.Location = new Point(10, buttonY);
            this.btnAddEndpoint.Name = "btnAddEndpoint";
            this.btnAddEndpoint.Size = new Size(120, buttonHeight);
            this.btnAddEndpoint.TabIndex = 0;
            this.btnAddEndpoint.Text = "Agregar";
            this.btnAddEndpoint.UseVisualStyleBackColor = true;
            this.btnAddEndpoint.Click += new EventHandler(this.btnAddEndpoint_Click);

            buttonY += buttonSpacing;
            this.btnEditEndpoint.Location = new Point(10, buttonY);
            this.btnEditEndpoint.Name = "btnEditEndpoint";
            this.btnEditEndpoint.Size = new Size(120, buttonHeight);
            this.btnEditEndpoint.TabIndex = 1;
            this.btnEditEndpoint.Text = "Editar";
            this.btnEditEndpoint.UseVisualStyleBackColor = true;
            this.btnEditEndpoint.Click += new EventHandler(this.btnEditEndpoint_Click);

            buttonY += buttonSpacing;
            this.btnDeleteEndpoint.Location = new Point(10, buttonY);
            this.btnDeleteEndpoint.Name = "btnDeleteEndpoint";
            this.btnDeleteEndpoint.Size = new Size(120, buttonHeight);
            this.btnDeleteEndpoint.TabIndex = 2;
            this.btnDeleteEndpoint.Text = "Eliminar";
            this.btnDeleteEndpoint.UseVisualStyleBackColor = true;
            this.btnDeleteEndpoint.Click += new EventHandler(this.btnDeleteEndpoint_Click);

            buttonY += buttonSpacing;
            this.btnMoveUp.Location = new Point(10, buttonY);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new Size(120, buttonHeight);
            this.btnMoveUp.TabIndex = 3;
            this.btnMoveUp.Text = "↑ Subir";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new EventHandler(this.btnMoveUp_Click);

            buttonY += buttonSpacing;
            this.btnMoveDown.Location = new Point(10, buttonY);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new Size(120, buttonHeight);
            this.btnMoveDown.TabIndex = 4;
            this.btnMoveDown.Text = "↓ Bajar";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new EventHandler(this.btnMoveDown_Click);

            buttonY += buttonSpacing;
            this.btnTestEndpoint.Location = new Point(10, buttonY);
            this.btnTestEndpoint.Name = "btnTestEndpoint";
            this.btnTestEndpoint.Size = new Size(120, buttonHeight);
            this.btnTestEndpoint.TabIndex = 5;
            this.btnTestEndpoint.Text = "Probar";
            this.btnTestEndpoint.UseVisualStyleBackColor = true;
            this.btnTestEndpoint.Click += new EventHandler(this.btnTestEndpoint_Click);

            buttonY += buttonSpacing;
            this.btnExecuteNow.Location = new Point(10, buttonY);
            this.btnExecuteNow.Name = "btnExecuteNow";
            this.btnExecuteNow.Size = new Size(120, buttonHeight);
            this.btnExecuteNow.TabIndex = 6;
            this.btnExecuteNow.Text = "Ejecutar Ahora";
            this.btnExecuteNow.UseVisualStyleBackColor = true;
            this.btnExecuteNow.Click += new EventHandler(this.btnExecuteNow_Click);

            // MethodsConfigurationControl
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpEndpoints);
            this.Controls.Add(this.grpSyncSettings);
            this.Name = "MethodsConfigurationControl";
            this.Size = new Size(800, 500);
            ((System.ComponentModel.ISupportInitialize)(this.numSyncInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndpoints)).EndInit();
            this.grpSyncSettings.ResumeLayout(false);
            this.grpSyncSettings.PerformLayout();
            this.grpEndpoints.ResumeLayout(false);
            this.grpActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private GroupBox grpSyncSettings;
        private CheckBox chkEnableSync;
        private Label lblSyncInterval;
        private NumericUpDown numSyncInterval;
        private Label lblLastSync;

        private GroupBox grpEndpoints;
        private DataGridView dgvEndpoints;
        private DataGridViewCheckBoxColumn colEnabled;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colEndpoint;
        private DataGridViewTextBoxColumn colMethod;
        private DataGridViewTextBoxColumn colPriority;
        private DataGridViewTextBoxColumn colLastExecuted;
        private DataGridViewCheckBoxColumn colRequiresAuth;

        private GroupBox grpActions;
        private Button btnAddEndpoint;
        private Button btnEditEndpoint;
        private Button btnDeleteEndpoint;
        private Button btnMoveUp;
        private Button btnMoveDown;
        private Button btnTestEndpoint;
        private Button btnExecuteNow;
    }
}