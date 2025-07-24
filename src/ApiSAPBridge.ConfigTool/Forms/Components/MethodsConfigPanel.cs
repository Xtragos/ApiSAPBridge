using ApiSAPBridge.ConfigTool.Models;

namespace ApiSAPBridge.ConfigTool.Forms
{
    public partial class MethodsConfigPanel : UserControl
    {
        private MethodsConfig _config = new MethodsConfig();

        // Controles principales
        private CheckBox chkAutomationEnabled;
        private NumericUpDown numExecutionInterval;
        private DateTimePicker dtpStartTime, dtpEndTime;
        private CheckBox chkRunOnWeekends;
        private NumericUpDown numRetryAttempts, numRetryDelay;
        private DataGridView dgvMethods;
        private GroupBox grpSchedule, grpMethods, grpRetry;
        private Button btnAddMethod, btnEditMethod, btnDeleteMethod, btnTestMethod;
        private ComboBox cmbCategory;

        public MethodsConfigPanel()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 600);
            this.BackColor = Color.White;
            this.AutoScroll = true;

            // Título
            var lblTitle = new Label
            {
                Text = "⚙️ Configuración de Métodos Automatizados",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Location = new Point(20, 20),
                Size = new Size(500, 30)
            };

            var lblSubtitle = new Label
            {
                Text = "Configure qué métodos POST serán ejecutados automáticamente y cuándo",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Gray,
                Location = new Point(20, 55),
                Size = new Size(600, 20)
            };

            // Grupo de Programación
            grpSchedule = new GroupBox
            {
                Text = "📅 Programación de Ejecución",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 90),
                Size = new Size(750, 140),
                ForeColor = Color.FromArgb(52, 58, 64)
            };

            // Automatización habilitada
            chkAutomationEnabled = new CheckBox
            {
                Text = "🔄 Habilitar Automatización",
                Location = new Point(20, 30),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69)
            };
            chkAutomationEnabled.CheckedChanged += ChkAutomationEnabled_CheckedChanged;

            // Intervalo de ejecución
            var lblInterval = new Label
            {
                Text = "Intervalo (minutos):",
                Location = new Point(20, 65),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 10F)
            };

            numExecutionInterval = new NumericUpDown
            {
                Location = new Point(150, 62),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10F),
                Minimum = 5,
                Maximum = 1440,
                Value = 60,
                Increment = 5
            };

            // Hora de inicio
            var lblStartTime = new Label
            {
                Text = "Hora Inicio:",
                Location = new Point(250, 65),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 10F)
            };

            dtpStartTime = new DateTimePicker
            {
                Location = new Point(340, 62),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 10F),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = DateTime.Today.AddHours(8)
            };

            // Hora de fin
            var lblEndTime = new Label
            {
                Text = "Hora Fin:",
                Location = new Point(460, 65),
                Size = new Size(70, 20),
                Font = new Font("Segoe UI", 10F)
            };

            dtpEndTime = new DateTimePicker
            {
                Location = new Point(540, 62),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 10F),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = DateTime.Today.AddHours(18)
            };

            // Fines de semana
            chkRunOnWeekends = new CheckBox
            {
                Text = "📅 Ejecutar Fines de Semana",
                Location = new Point(20, 100),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10F)
            };

            grpSchedule.Controls.AddRange(new Control[] {
                chkAutomationEnabled, lblInterval, numExecutionInterval,
                lblStartTime, dtpStartTime, lblEndTime, dtpEndTime, chkRunOnWeekends
            });

            // Grupo de Reintentos
            grpRetry = new GroupBox
            {
                Text = "🔄 Configuración de Reintentos",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 250),
                Size = new Size(750, 80),
                ForeColor = Color.FromArgb(52, 58, 64)
            };

            var lblRetryAttempts = new Label
            {
                Text = "Reintentos:",
                Location = new Point(20, 35),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 10F)
            };

            numRetryAttempts = new NumericUpDown
            {
                Location = new Point(110, 32),
                Size = new Size(60, 25),
                Font = new Font("Segoe UI", 10F),
                Minimum = 0,
                Maximum = 10,
                Value = 3
            };

            var lblRetryDelay = new Label
            {
                Text = "Delay (seg):",
                Location = new Point(200, 35),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 10F)
            };

            numRetryDelay = new NumericUpDown
            {
                Location = new Point(290, 32),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10F),
                Minimum = 5,
                Maximum = 300,
                Value = 30,
                Increment = 5
            };

            grpRetry.Controls.AddRange(new Control[] {
                lblRetryAttempts, numRetryAttempts, lblRetryDelay, numRetryDelay
            });

            // Grupo de Métodos
            grpMethods = new GroupBox
            {
                Text = "📋 Métodos Configurados para Automatización",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 350),
                Size = new Size(750, 320),
                ForeColor = Color.FromArgb(52, 58, 64)
            };

            // Filtro por categoría
            var lblCategory = new Label
            {
                Text = "Filtrar por categoría:",
                Location = new Point(20, 30),
                Size = new Size(130, 20),
                Font = new Font("Segoe UI", 10F)
            };

            cmbCategory = new ComboBox
            {
                Location = new Point(160, 27),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.Items.AddRange(new[] { "Todas", "Maestros", "Productos", "Ventas", "Reportes" });
            cmbCategory.SelectedIndex = 0;
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;

            // Botones de gestión
            btnAddMethod = new Button
            {
                Text = "➕ Agregar",
                Location = new Point(330, 25),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnAddMethod.Click += BtnAddMethod_Click;

            btnEditMethod = new Button
            {
                Text = "✏️ Editar",
                Location = new Point(430, 25),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnEditMethod.Click += BtnEditMethod_Click;

            btnDeleteMethod = new Button
            {
                Text = "🗑️ Eliminar",
                Location = new Point(530, 25),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnDeleteMethod.Click += BtnDeleteMethod_Click;

            btnTestMethod = new Button
            {
                Text = "🧪 Probar",
                Location = new Point(630, 25),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnTestMethod.Click += BtnTestMethod_Click;

            // DataGridView para métodos
            dgvMethods = new DataGridView
            {
                Location = new Point(20, 65),
                Size = new Size(710, 240),
                Font = new Font("Segoe UI", 9F),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };

            grpMethods.Controls.AddRange(new Control[] {
                lblCategory, cmbCategory, btnAddMethod, btnEditMethod,
                btnDeleteMethod, btnTestMethod, dgvMethods
            });

            this.Controls.AddRange(new Control[] {
                lblTitle, lblSubtitle, grpSchedule, grpRetry, grpMethods
            });
        }

        private void InitializeDataGridView()
        {
            dgvMethods.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "Enabled",
                HeaderText = "✅",
                Width = 40,
                ToolTipText = "Habilitado"
            });

            dgvMethods.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DisplayName",
                HeaderText = "Método",
                Width = 150,
                ReadOnly = true
            });

            dgvMethods.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Endpoint",
                HeaderText = "Endpoint",
                Width = 180,
                ReadOnly = true
            });

            dgvMethods.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HttpMethod",
                HeaderText = "HTTP",
                Width = 60,
                ReadOnly = true
            });

            dgvMethods.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "Categoría",
                Width = 90,
                ReadOnly = true
            });

            dgvMethods.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ExecutionOrder",
                HeaderText = "Orden",
                Width = 60,
                ReadOnly = true
            });

            dgvMethods.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TimeoutSeconds",
                HeaderText = "Timeout",
                Width = 70,
                ReadOnly = true
            });

            dgvMethods.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "Descripción",
                Width = 200,
                ReadOnly = true
            });

            // Estilo de headers
            dgvMethods.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 123, 255);
            dgvMethods.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMethods.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvMethods.EnableHeadersVisualStyles = false;

            // Estilo de filas
            dgvMethods.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgvMethods.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 123, 255);
            dgvMethods.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        private void ChkAutomationEnabled_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chkAutomationEnabled.Checked;

            numExecutionInterval.Enabled = enabled;
            dtpStartTime.Enabled = enabled;
            dtpEndTime.Enabled = enabled;
            chkRunOnWeekends.Enabled = enabled;
            numRetryAttempts.Enabled = enabled;
            numRetryDelay.Enabled = enabled;
            dgvMethods.Enabled = enabled;
            btnAddMethod.Enabled = enabled;
            btnEditMethod.Enabled = enabled;
            btnDeleteMethod.Enabled = enabled;
            btnTestMethod.Enabled = enabled;
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterMethodsByCategory();
        }

        private void FilterMethodsByCategory()
        {
            if (_config?.Methods == null) return;

            string selectedCategory = cmbCategory.SelectedItem?.ToString();

            var filteredMethods = selectedCategory == "Todas"
                ? _config.Methods
                : _config.Methods.Where(m => m.Category == selectedCategory).ToList();

            LoadMethodsToGrid(filteredMethods);
        }

        private void LoadMethodsToGrid(List<MethodExecutionConfig> methods)
        {
            dgvMethods.Rows.Clear();

            foreach (var method in methods.OrderBy(m => m.ExecutionOrder))
            {
                var row = dgvMethods.Rows[dgvMethods.Rows.Add()];
                row.Cells["Enabled"].Value = method.IsEnabled;
                row.Cells["DisplayName"].Value = method.DisplayName;
                row.Cells["Endpoint"].Value = method.Endpoint;
                row.Cells["HttpMethod"].Value = method.HttpMethod;
                row.Cells["Category"].Value = method.Category;
                row.Cells["ExecutionOrder"].Value = method.ExecutionOrder;
                row.Cells["TimeoutSeconds"].Value = method.TimeoutSeconds;
                row.Cells["Description"].Value = method.Description;
                row.Tag = method;

                // Colorear fila según estado
                if (!method.IsEnabled)
                {
                    row.DefaultCellStyle.ForeColor = Color.Gray;
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
                }
            }
        }

        private void BtnAddMethod_Click(object sender, EventArgs e)
        {
            using (var form = new MethodEditForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var newMethod = form.GetMethod();
                    _config.Methods.Add(newMethod);
                    FilterMethodsByCategory();
                }
            }
        }

        private void BtnEditMethod_Click(object sender, EventArgs e)
        {
            if (dgvMethods.SelectedRows.Count == 0) return;

            var selectedMethod = dgvMethods.SelectedRows[0].Tag as MethodExecutionConfig;
            if (selectedMethod == null) return;

            using (var form = new MethodEditForm(selectedMethod))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var editedMethod = form.GetMethod();
                    var index = _config.Methods.FindIndex(m => m.MethodName == selectedMethod.MethodName);
                    if (index >= 0)
                    {
                        _config.Methods[index] = editedMethod;
                        FilterMethodsByCategory();
                    }
                }
            }
        }

        private void BtnDeleteMethod_Click(object sender, EventArgs e)
        {
            if (dgvMethods.SelectedRows.Count == 0) return;

            var selectedMethod = dgvMethods.SelectedRows[0].Tag as MethodExecutionConfig;
            if (selectedMethod == null) return;

            var result = MessageBox.Show(
                $"¿Está seguro de eliminar el método '{selectedMethod.DisplayName}'?",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _config.Methods.RemoveAll(m => m.MethodName == selectedMethod.MethodName);
                FilterMethodsByCategory();
            }
        }

        private async void BtnTestMethod_Click(object sender, EventArgs e)
        {
            if (dgvMethods.SelectedRows.Count == 0) return;

            var selectedMethod = dgvMethods.SelectedRows[0].Tag as MethodExecutionConfig;
            if (selectedMethod == null) return;

            // Aquí implementarías la prueba del método
            MessageBox.Show($"Prueba del método: {selectedMethod.DisplayName}\n\nEsta funcionalidad se implementará próximamente.",
                "Prueba de Método", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void LoadConfig(MethodsConfig config)
        {
            _config = config ?? new MethodsConfig();

            chkAutomationEnabled.Checked = _config.AutomationEnabled;
            numExecutionInterval.Value = _config.ExecutionIntervalMinutes;
            dtpStartTime.Value = DateTime.Today.Add(_config.StartTime);
            dtpEndTime.Value = DateTime.Today.Add(_config.EndTime);
            chkRunOnWeekends.Checked = _config.RunOnWeekends;
            numRetryAttempts.Value = _config.RetryAttempts;
            numRetryDelay.Value = _config.RetryDelaySeconds;

            FilterMethodsByCategory();
        }

        public MethodsConfig GetConfig()
        {
            // Actualizar estados desde la grid
            foreach (DataGridViewRow row in dgvMethods.Rows)
            {
                if (row.Tag is MethodExecutionConfig method)
                {
                    method.IsEnabled = Convert.ToBoolean(row.Cells["Enabled"].Value);
                }
            }

            return new MethodsConfig
            {
                AutomationEnabled = chkAutomationEnabled.Checked,
                ExecutionIntervalMinutes = (int)numExecutionInterval.Value,
                StartTime = dtpStartTime.Value.TimeOfDay,
                EndTime = dtpEndTime.Value.TimeOfDay,
                RunOnWeekends = chkRunOnWeekends.Checked,
                RetryAttempts = (int)numRetryAttempts.Value,
                RetryDelaySeconds = (int)numRetryDelay.Value,
                Methods = _config.Methods
            };
        }
    }
}