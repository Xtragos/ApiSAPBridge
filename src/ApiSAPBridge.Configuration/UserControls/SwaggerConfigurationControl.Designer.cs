namespace ApiSAPBridge.Configuration.UserControls
{
    partial class SwaggerConfigurationControl
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
            this.grpSwaggerSettings = new GroupBox();
            this.chkEnableSwagger = new CheckBox();
            this.chkEnableSwaggerUI = new CheckBox();

            this.grpMethods = new GroupBox();
            this.chklstMethods = new CheckedListBox();

            this.grpEndpoints = new GroupBox();
            this.dgvEndpoints = new DataGridView();
            this.colController = new DataGridViewTextBoxColumn();
            this.colAction = new DataGridViewTextBoxColumn();
            this.colMethod = new DataGridViewTextBoxColumn();
            this.colVisible = new DataGridViewCheckBoxColumn();

            this.grpActions = new GroupBox();
            this.btnHideSelected = new Button();
            this.btnShowSelected = new Button();
            this.btnHideAll = new Button();
            this.btnShowAll = new Button();
            this.btnFilterByMethod = new Button();
            this.btnPreviewSwagger = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvEndpoints)).BeginInit();
            this.grpSwaggerSettings.SuspendLayout();
            this.grpMethods.SuspendLayout();
            this.grpEndpoints.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.SuspendLayout();

            // grpSwaggerSettings
            this.grpSwaggerSettings.Controls.Add(this.chkEnableSwaggerUI);
            this.grpSwaggerSettings.Controls.Add(this.chkEnableSwagger);
            this.grpSwaggerSettings.Location = new Point(12, 12);
            this.grpSwaggerSettings.Name = "grpSwaggerSettings";
            this.grpSwaggerSettings.Size = new Size(760, 80);
            this.grpSwaggerSettings.TabIndex = 0;
            this.grpSwaggerSettings.TabStop = false;
            this.grpSwaggerSettings.Text = "Configuración General de Swagger";

            // chkEnableSwagger
            this.chkEnableSwagger.AutoSize = true;
            this.chkEnableSwagger.Checked = true;
            this.chkEnableSwagger.CheckState = CheckState.Checked;
            this.chkEnableSwagger.Location = new Point(20, 30);
            this.chkEnableSwagger.Name = "chkEnableSwagger";
            this.chkEnableSwagger.Size = new Size(130, 17);
            this.chkEnableSwagger.TabIndex = 0;
            this.chkEnableSwagger.Text = "Habilitar Swagger";
            this.chkEnableSwagger.UseVisualStyleBackColor = true;
            this.chkEnableSwagger.CheckedChanged += new EventHandler(this.chkEnableSwagger_CheckedChanged);

            // chkEnableSwaggerUI
            this.chkEnableSwaggerUI.AutoSize = true;
            this.chkEnableSwaggerUI.Checked = true;
            this.chkEnableSwaggerUI.CheckState = CheckState.Checked;
            this.chkEnableSwaggerUI.Location = new Point(200, 30);
            this.chkEnableSwaggerUI.Name = "chkEnableSwaggerUI";
            this.chkEnableSwaggerUI.Size = new Size(150, 17);
            this.chkEnableSwaggerUI.TabIndex = 1;
            this.chkEnableSwaggerUI.Text = "Habilitar Swagger UI";
            this.chkEnableSwaggerUI.UseVisualStyleBackColor = true;

            // grpMethods
            this.grpMethods.Controls.Add(this.chklstMethods);
            this.grpMethods.Location = new Point(12, 110);
            this.grpMethods.Name = "grpMethods";
            this.grpMethods.Size = new Size(200, 200);
            this.grpMethods.TabIndex = 1;
            this.grpMethods.TabStop = false;
            this.grpMethods.Text = "Métodos HTTP Permitidos";

            // chklstMethods
            this.chklstMethods.CheckOnClick = true;
            this.chklstMethods.Dock = DockStyle.Fill;
            this.chklstMethods.FormattingEnabled = true;
            this.chklstMethods.Location = new Point(3, 16);
            this.chklstMethods.Name = "chklstMethods";
            this.chklstMethods.Size = new Size(194, 181);
            this.chklstMethods.TabIndex = 0;
            this.chklstMethods.ItemCheck += new ItemCheckEventHandler(this.chklstMethods_ItemCheck);

            // grpEndpoints
            this.grpEndpoints.Controls.Add(this.dgvEndpoints);
            this.grpEndpoints.Location = new Point(230, 110);
            this.grpEndpoints.Name = "grpEndpoints";
            this.grpEndpoints.Size = new Size(400, 370);
            this.grpEndpoints.TabIndex = 2;
            this.grpEndpoints.TabStop = false;
            this.grpEndpoints.Text = "Visibilidad de Endpoints";

            // dgvEndpoints
            this.dgvEndpoints.AllowUserToAddRows = false;
            this.dgvEndpoints.AllowUserToDeleteRows = false;
            this.dgvEndpoints.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEndpoints.Columns.AddRange(new DataGridViewColumn[] {
                this.colController,
                this.colAction,
                this.colMethod,
                this.colVisible});
            this.dgvEndpoints.Dock = DockStyle.Fill;
            this.dgvEndpoints.Location = new Point(3, 16);
            this.dgvEndpoints.Name = "dgvEndpoints";
            this.dgvEndpoints.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEndpoints.Size = new Size(394, 351);
            this.dgvEndpoints.TabIndex = 0;
            this.dgvEndpoints.CellValueChanged += new DataGridViewCellEventHandler(this.dgvEndpoints_CellValueChanged);

            // Configurar columnas
            this.colController.HeaderText = "Controlador";
            this.colController.Name = "colController";
            this.colController.ReadOnly = true;
            this.colController.Width = 100;

            this.colAction.HeaderText = "Acción";
            this.colAction.Name = "colAction";
            this.colAction.ReadOnly = true;
            this.colAction.Width = 120;

            this.colMethod.HeaderText = "Método";
            this.colMethod.Name = "colMethod";
            this.colMethod.ReadOnly = true;
            this.colMethod.Width = 70;

            this.colVisible.HeaderText = "Visible";
            this.colVisible.Name = "colVisible";
            this.colVisible.Width = 60;

            // grpActions
            this.grpActions.Controls.Add(this.btnPreviewSwagger);
            this.grpActions.Controls.Add(this.btnFilterByMethod);
            this.grpActions.Controls.Add(this.btnShowAll);
            this.grpActions.Controls.Add(this.btnHideAll);
            this.grpActions.Controls.Add(this.btnShowSelected);
            this.grpActions.Controls.Add(this.btnHideSelected);
            this.grpActions.Location = new Point(650, 110);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new Size(122, 370);
            this.grpActions.TabIndex = 3;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Acciones";

            // Configurar botones
            var buttonY = 30;
            var buttonHeight = 35;
            var buttonSpacing = 45;

            this.btnHideSelected.Location = new Point(10, buttonY);
            this.btnHideSelected.Name = "btnHideSelected";
            this.btnHideSelected.Size = new Size(100, buttonHeight);
            this.btnHideSelected.TabIndex = 0;
            this.btnHideSelected.Text = "Ocultar";
            this.btnHideSelected.UseVisualStyleBackColor = true;
            this.btnHideSelected.Click += new EventHandler(this.btnHideSelected_Click);

            buttonY += buttonSpacing;
            this.btnShowSelected.Location = new Point(10, buttonY);
            this.btnShowSelected.Name = "btnShowSelected";
            this.btnShowSelected.Size = new Size(100, buttonHeight);
            this.btnShowSelected.TabIndex = 1;
            this.btnShowSelected.Text = "Mostrar";
            this.btnShowSelected.UseVisualStyleBackColor = true;
            this.btnShowSelected.Click += new EventHandler(this.btnShowSelected_Click);

            buttonY += buttonSpacing;
            this.btnHideAll.Location = new Point(10, buttonY);
            this.btnHideAll.Name = "btnHideAll";
            this.btnHideAll.Size = new Size(100, buttonHeight);
            this.btnHideAll.TabIndex = 2;
            this.btnHideAll.Text = "Ocultar Todo";
            this.btnHideAll.UseVisualStyleBackColor = true;
            this.btnHideAll.Click += new EventHandler(this.btnHideAll_Click);

            buttonY += buttonSpacing;
            this.btnShowAll.Location = new Point(10, buttonY);
            this.btnShowAll.Name = "btnShowAll";
            this.btnShowAll.Size = new Size(100, buttonHeight);
            this.btnShowAll.TabIndex = 3;
            this.btnShowAll.Text = "Mostrar Todo";
            this.btnShowAll.UseVisualStyleBackColor = true;
            this.btnShowAll.Click += new EventHandler(this.btnShowAll_Click);

            buttonY += buttonSpacing;
            this.btnFilterByMethod.Location = new Point(10, buttonY);
            this.btnFilterByMethod.Name = "btnFilterByMethod";
            this.btnFilterByMethod.Size = new Size(100, buttonHeight);
            this.btnFilterByMethod.TabIndex = 4;
            this.btnFilterByMethod.Text = "Filtro Método";
            this.btnFilterByMethod.UseVisualStyleBackColor = true;
            this.btnFilterByMethod.Click += new EventHandler(this.btnFilterByMethod_Click);

            buttonY += buttonSpacing;
            this.btnPreviewSwagger.Location = new Point(10, buttonY);
            this.btnPreviewSwagger.Name = "btnPreviewSwagger";
            this.btnPreviewSwagger.Size = new Size(100, buttonHeight);
            this.btnPreviewSwagger.TabIndex = 5;
            this.btnPreviewSwagger.Text = "Vista Previa";
            this.btnPreviewSwagger.UseVisualStyleBackColor = true;
            this.btnPreviewSwagger.Click += new EventHandler(this.btnPreviewSwagger_Click);

            // SwaggerConfigurationControl
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpEndpoints);
            this.Controls.Add(this.grpMethods);
            this.Controls.Add(this.grpSwaggerSettings);
            this.Name = "SwaggerConfigurationControl";
            this.Size = new Size(800, 500);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEndpoints)).EndInit();
            this.grpSwaggerSettings.ResumeLayout(false);
            this.grpSwaggerSettings.PerformLayout();
            this.grpMethods.ResumeLayout(false);
            this.grpEndpoints.ResumeLayout(false);
            this.grpActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private GroupBox grpSwaggerSettings;
        private CheckBox chkEnableSwagger;
        private CheckBox chkEnableSwaggerUI;

        private GroupBox grpMethods;
        private CheckedListBox chklstMethods;

        private GroupBox grpEndpoints;
        private DataGridView dgvEndpoints;
        private DataGridViewTextBoxColumn colController;
        private DataGridViewTextBoxColumn colAction;
        private DataGridViewTextBoxColumn colMethod;
        private DataGridViewCheckBoxColumn colVisible;

        private GroupBox grpActions;
        private Button btnHideSelected;
        private Button btnShowSelected;
        private Button btnHideAll;
        private Button btnShowAll;
        private Button btnFilterByMethod;
        private Button btnPreviewSwagger;
    }
}