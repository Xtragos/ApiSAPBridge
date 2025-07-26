using ApiSAPBridge.Configuration.UI.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ApiSAPBridge.Configuration.UI.Controls
{
    public class ModernTabControl : TabControl
    {
        private Color _activeTabColor = Color.FromArgb(0, 123, 255);
        private Color _inactiveTabColor = Color.FromArgb(233, 236, 239);
        private Color _protectedTabColor = Color.FromArgb(220, 53, 69);
        private Color _textColor = Color.FromArgb(73, 80, 87);
        private Color _activeTextColor = Color.White;

        public List<int> ProtectedTabs { get; set; } = new List<int>();
        public List<int> HiddenTabs { get; set; } = new List<int>();

        public ModernTabControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.DoubleBuffer |
                    ControlStyles.ResizeRedraw, true);

            DrawMode = TabDrawMode.OwnerDrawFixed;
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(150, 45);
            Padding = new Point(20, 0);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            TabPage tabPage = TabPages[e.Index];
            Rectangle tabRect = GetTabRect(e.Index);

            // Determinar colores según el estado de la pestaña
            Color backColor = _inactiveTabColor;
            Color textColor = _textColor;
            string tabText = tabPage.Text;

            if (SelectedIndex == e.Index)
            {
                backColor = _activeTabColor;
                textColor = _activeTextColor;
            }
            else if (ProtectedTabs.Contains(e.Index))
            {
                backColor = _protectedTabColor;
                textColor = _activeTextColor;
                tabText += " 🔒";
            }

            // Dibujar fondo de la pestaña
            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, tabRect);
            }

            // Dibujar texto
            TextRenderer.DrawText(g, tabText, Font, tabRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            // Dibujar borde inferior para pestaña activa
            if (SelectedIndex == e.Index)
            {
                using (var pen = new Pen(Color.FromArgb(0, 86, 179), 3))
                {
                    g.DrawLine(pen, tabRect.Left, tabRect.Bottom - 1,
                              tabRect.Right, tabRect.Bottom - 1);
                }
            }
        }

        protected override void OnSelecting(TabControlCancelEventArgs e)
        {
            // Si la pestaña está oculta, cancelar la selección
            if (HiddenTabs.Contains(e.TabPageIndex))
            {
                e.Cancel = true;
                return;
            }

            // Si la pestaña está protegida, verificar autenticación
            if (ProtectedTabs.Contains(e.TabPageIndex))
            {
                var loginForm = new LoginForm();
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    e.Cancel = true;
                }
            }

            base.OnSelecting(e);
        }

        public void ShowProtectedTabs()
        {
            HiddenTabs.Clear();
            Invalidate();
        }

        public void HideProtectedTabs()
        {
            HiddenTabs.AddRange(ProtectedTabs);

            // Si la pestaña actual está oculta, cambiar a la primera visible
            if (HiddenTabs.Contains(SelectedIndex))
            {
                for (int i = 0; i < TabPages.Count; i++)
                {
                    if (!HiddenTabs.Contains(i))
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
            Invalidate();
        }
    }
}