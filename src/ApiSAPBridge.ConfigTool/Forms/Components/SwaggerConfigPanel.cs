using ApiSAPBridge.ConfigTool.Models;

namespace ApiSAPBridge.ConfigTool.Forms
{
    public partial class SwaggerConfigPanel : UserControl
    {
        private SwaggerConfig _config = new SwaggerConfig();

        // Controles principales
        private CheckBox chkSwaggerEnabled, chkShowInProduction, chkRequireAuth;
        private TextBox txtApiTitle, txtApiVersion, txtApiDescription;
        private DataGridView dgvEndpoints;
        private GroupBox grpGeneral, grpEndpoints;
        private Button btnRefreshEndpoints, btnToggleAll, btnExportConfig;
        private ComboBox cmbControllerFilter, cmbMethodFilter;
        private TextBox txtEndpointSearch;

        public SwaggerConfigPanel()
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
                Text = "📋 Configuración de Swagger",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Location = new Point(20, 20),
                Size = new Size(400, 30)
            };

            var lblSubtitle = new Label
            {
                Text = "Controle qué endpoints serán visibles en la documentación Swagger",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Gray,
                Location = new Point(20, 55),
                Size = new Size(600, 20)
            };

            // Grupo de Configuración General
            grpGeneral = new GroupBox
            {
                Text = "⚙️ Configuración General de Swagger",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 90),
                Size = new Size(750, 180),
                ForeColor = Color.FromArgb(52, 58, 64)
            };

            // Swagger habilitado
            chkSwaggerEnabled = new CheckBox
            {
                Text = "📋 Habilitar Swagger",
                Location = new Point(20, 30),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            chkShowInProduction = new CheckBox
            {
                Text = "🏭 Mostrar en Producción",
                Location = new Point(200, 30),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(220, 53, 69)
            };

            chkRequireAuth = new CheckBox
            {
                Text = "🔐 Requerir Autenticación",
                Location = new Point(400, 30),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10F)
            };

            // Título de la API
            var lblApiTitle = new Label
            {
                Text = "Título de la API:",
                Location = new Point(20, 70),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10F)
            };

            txtApiTitle = new TextBox
            {
                Location = new Point(130, 67),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10F),
                Text = "API SAP Bridge"
            };

            // Versión de la API
            var lblApiVersion = new Label
            {
                Text = "Versión:",
                Location = new Point(350, 70),
                Size = new Size(60, 20),
                Font = new Font("Segoe UI", 10F)
            };

            txtApiVersion = new TextBox
            {
                Location = new Point(420, 67),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 10F),
                Text = "v1"
            };

            // Descripción
            var lblApiDescription = new Label
            {
                Text = "Descripción:",
                Location = new Point(20, 105),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 10F)
            };

            txtApiDescription = new TextBox
            {
                Location = new Point(20, 130),
                Size = new Size(700, 35),
                Font = new Font("Segoe UI", 10F),
                Multiline = true,
                Text = "API para integración con SAP - Gestión de maestros, productos y facturación"
            };

            grpGeneral.Controls.AddRange(new Control[] {
                chkSwaggerEnabled, chkShowInProduction, chkRequireAuth,
                lblApiTitle, txtApiTitle, lblApiVersion, txtApiVersion,
                lblApiDescription, txtApiDescription
            });

            // Grupo de Endpoints
            grpEndpoints = new GroupBox
            {
                Text = "🌐 Control de Visibilidad de Endpoints",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 290),
                Size = new Size(750, 380),
                ForeColor = Color.FromArgb(52, 58, 64)
            };

            // Filtros
            var lblControllerFilter = new Label
            {
                Text = "Controlador:",
                Location = new Point(20, 30),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 9F)
            };

            cmbControllerFilter = new ComboBox
            {
                Location = new Point(110, 27),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 9F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbControllerFilter.Items.AddRange(new[] {
                "Todos", "Departamentos", "Secciones", "Familias",
                "Clientes", "Articulos", "Precios", "Facturas"
            });
            cmbControllerFilter.SelectedIndex = 0;
            cmbControllerFilter.SelectedIndexChanged += FilterEndpoints;

            var lblMethodFilter = new Label
            {
                Text = "Método:",
                Location = new Point(250, 30),
                Size = new Size(50, 20),
                Font = new Font("Segoe UI", 9F)
            };

            cmbMethodFilter = new ComboBox
            {
                Location = new Point(310, 27),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 9F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbMethodFilter.Items.AddRange(new[] { "Todos", "GET", "POST", "PUT", "DELETE" });
            cmbMethodFilter.SelectedIndex = 0;
            cmbMethodFilter.SelectedIndexChanged += FilterEndpoints;

            var lblSearch = new Label
            {
                Text = "Buscar:",
                Location = new Point(410, 30),
                Size = new Size(50, 20),
                Font = new Font("Segoe UI", 9F)
            };

            txtEndpointSearch = new TextBox
            {
                Location = new Point(470, 27),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 9F),
                PlaceholderText = "Filtrar endpoints..."
            };
            txtEndpointSearch.TextChanged += FilterEndpoints;

            // Botones de gestión
            btnRefreshEndpoints = new Button
            {
                Text = "🔄 Actualizar",
                Location = new Point(610, 25),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnRefreshEndpoints.Click += BtnRefreshEndpoints_Click;

            btnToggleAll = new Button
            {
                Text = "🔄 Alternar Todo",
                Location = new Point(20, 65),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnToggleAll.Click += BtnToggleAll_Click;

            btnExportConfig = new Button
            {
                Text = "📤 Exportar Config",
                Location = new Point(150, 65),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnExportConfig.Click += BtnExportConfig_Click;

            // DataGridView para endpoints
            dgvEndpoints = new DataGridView
            {
                Location = new Point(20, 105),
                Size = new Size(710, 260),
                Font = new Font("Segoe UI", 9F),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                ReadOnly = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };

            grpEndpoints.Controls.AddRange(new Control[] {
                lblControllerFilter, cmbControllerFilter, lblMethodFilter, cmbMethodFilter,
                lblSearch, txtEndpointSearch, btnRefreshEndpoints, btnToggleAll,
                btnExportConfig, dgvEndpoints
            });

            this.Controls.AddRange(new Control[] {
                lblTitle, lblSubtitle, grpGeneral, grpEndpoints
            });
        }

        private void InitializeDataGridView()
        {
            dgvEndpoints.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "IsVisible",
                HeaderText = "👁️",
                Width = 40,
                ToolTipText = "Visible en Swagger"
            });

            dgvEndpoints.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "RequiresAuth",
                HeaderText = "🔐",
                Width = 40,
                ToolTipText = "Requiere Autenticación"
            });

            dgvEndpoints.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Controller",
                HeaderText = "Controlador",
                Width = 120,
                ReadOnly = true
            });

            dgvEndpoints.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Action",
                HeaderText = "Acción",
                Width = 140,
                ReadOnly = true
            });

            dgvEndpoints.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HttpMethod",
                HeaderText = "Método",
                Width = 70,
                ReadOnly = true
            });

            dgvEndpoints.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FullPath",
                HeaderText = "Ruta Completa",
                Width = 200,
                ReadOnly = true
            });

            dgvEndpoints.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "Categoría",
                Width = 100,
                ReadOnly = true
            });

            // Estilo de headers
            dgvEndpoints.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 123, 255);
            dgvEndpoints.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEndpoints.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvEndpoints.EnableHeadersVisualStyles = false;

            // Estilo de filas
            dgvEndpoints.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgvEndpoints.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 123, 255);
            dgvEndpoints.DefaultCellStyle.SelectionForeColor = Color.White;

            // Eventos
            dgvEndpoints.CellFormatting += DgvEndpoints_CellFormatting;
        }

        private void DgvEndpoints_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var endpoint = dgvEndpoints.Rows[e.RowIndex].Tag as EndpointVisibilityConfig;
            if (endpoint == null) return;

            // Colorear según método HTTP
            Color methodColor = endpoint.HttpMethod.ToUpper() switch
            {
                "GET" => Color.FromArgb(40, 167, 69),
                "POST" => Color.FromArgb(0, 123, 255),
                "PUT" => Color.FromArgb(255, 193, 7),
                "DELETE" => Color.FromArgb(220, 53, 69),
                _ => Color.Gray
            };

            if (e.ColumnIndex == dgvEndpoints.Columns["HttpMethod"].Index)
            {
                e.CellStyle.BackColor = methodColor;
                e.CellStyle.ForeColor = endpoint.HttpMethod.ToUpper() == "PUT" ? Color.Black : Color.White;
                e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }

            // Marcar endpoints no visibles
            if (!endpoint.IsVisible)
            {
                e.CellStyle.ForeColor = Color.Gray;
                e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            }
        }

        private void FilterEndpoints(object sender, EventArgs e)
        {
            LoadEndpointsToGrid();
        }

        private void LoadEndpointsToGrid()
        {
            if (_config?.Endpoints == null) return;

            dgvEndpoints.Rows.Clear();

            var filteredEndpoints = _config.Endpoints.AsEnumerable();

            // Filtrar por controlador
            string selectedController = cmbControllerFilter.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedController) && selectedController != "Todos")
            {
                filteredEndpoints = filteredEndpoints.Where(e => e.Controller == selectedController);
            }

            // Filtrar por método HTTP
            string selectedMethod = cmbMethodFilter.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedMethod) && selectedMethod != "Todos")
            {
                filteredEndpoints = filteredEndpoints.Where(e => e.HttpMethod == selectedMethod);
            }

            // Filtrar por búsqueda
            string searchText = txtEndpointSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredEndpoints = filteredEndpoints.Where(e =>
                    e.Controller.ToLower().Contains(searchText) ||
                    e.Action.ToLower().Contains(searchText) ||
                    e.FullPath.ToLower().Contains(searchText) ||
                    e.Category.ToLower().Contains(searchText));
            }

            foreach (var endpoint in filteredEndpoints.OrderBy(e => e.Controller).ThenBy(e => e.Action))
            {
                var row = dgvEndpoints.Rows[dgvEndpoints.Rows.Add()];
                row.Cells["IsVisible"].Value = endpoint.IsVisible;
                row.Cells["RequiresAuth"].Value = endpoint.RequiresAuth;
                row.Cells["Controller"].Value = endpoint.Controller;
                row.Cells["Action"].Value = endpoint.Action;
                row.Cells["HttpMethod"].Value = endpoint.HttpMethod;
                row.Cells["FullPath"].Value = endpoint.FullPath;
                row.Cells["Category"].Value = endpoint.Category;
                row.Tag = endpoint;
            }
        }

        private void BtnRefreshEndpoints_Click(object sender, EventArgs e)
        {
            // Aquí implementarías la lógica para obtener endpoints desde la API
            MessageBox.Show("Funcionalidad de actualización automática se implementará próximamente.\n\nPor ahora, agregue endpoints manualmente desde la configuración.",
                "Actualizar Endpoints", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnToggleAll_Click(object sender, EventArgs e)
        {
            bool newState = !dgvEndpoints.Rows.Cast<DataGridViewRow>()
                .All(row => Convert.ToBoolean(row.Cells["IsVisible"].Value));

            foreach (DataGridViewRow row in dgvEndpoints.Rows)
            {
                row.Cells["IsVisible"].Value = newState;
                if (row.Tag is EndpointVisibilityConfig endpoint)
                {
                    endpoint.IsVisible = newState;
                }
            }
        }

        private void BtnExportConfig_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Title = "Exportar Configuración de Swagger",
                    Filter = "Archivos JSON (*.json)|*.json",
                    DefaultExt = "json",
                    FileName = $"swagger-config-{DateTime.Now:yyyyMMdd}.json"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var config = GetConfig();
                    var json = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    File.WriteAllText(saveDialog.FileName, json);
                    MessageBox.Show("Configuración exportada exitosamente.",
                        "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar configuración: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadConfig(SwaggerConfig config)
        {
            _config = config ?? new SwaggerConfig();

            chkSwaggerEnabled.Checked = _config.SwaggerEnabled;
            chkShowInProduction.Checked = _config.ShowInProduction;
            chkRequireAuth.Checked = _config.RequireAuthentication;
            txtApiTitle.Text = _config.ApiTitle;
            txtApiVersion.Text = _config.ApiVersion;
            txtApiDescription.Text = _config.ApiDescription;

            LoadEndpointsToGrid();
        }

        public SwaggerConfig GetConfig()
        {
            // Actualizar estados desde la grid
            foreach (DataGridViewRow row in dgvEndpoints.Rows)
            {
                if (row.Tag is EndpointVisibilityConfig endpoint)
                {
                    endpoint.IsVisible = Convert.ToBoolean(row.Cells["IsVisible"].Value);
                    endpoint.RequiresAuth = Convert.ToBoolean(row.Cells["RequiresAuth"].Value);
                }
            }

            return new SwaggerConfig
            {
                SwaggerEnabled = chkSwaggerEnabled.Checked,
                ShowInProduction = chkShowInProduction.Checked,
                RequireAuthentication = chkRequireAuth.Checked,
                ApiTitle = txtApiTitle.Text.Trim(),
                ApiVersion = txtApiVersion.Text.Trim(),
                ApiDescription = txtApiDescription.Text.Trim(),
                Endpoints = _config.Endpoints
            };
        }
    }
}