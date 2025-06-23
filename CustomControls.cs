// CustomControls.cs
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Clipper.Modern
{
    public static class GraphicsExtensions
    {
        public static GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class RoundedPanel : Panel
    {
        [Category("Appearance")]
        public int CornerRadius { get; set; } = UITheme.BorderRadius;

        [Category("Appearance")]
        public Color BorderColor { get; set; } = UITheme.BorderColor;

        [Category("Appearance")]
        public int BorderWidth { get; set; } = 1;

        [Category("Appearance")]
        public bool DrawShadow { get; set; } = true;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Sombra leve
            if (DrawShadow)
            {
                using (var shadowPath = GraphicsExtensions.GetRoundedPath(new Rectangle(4, 4, this.Width - 8, this.Height - 8), CornerRadius + 2))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
            }

            using (var path = GraphicsExtensions.GetRoundedPath(this.ClientRectangle, CornerRadius))
            {
                using (var brush = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                if (BorderWidth > 0)
                {
                    var borderRect = new Rectangle(
                        this.ClientRectangle.X + BorderWidth / 2,
                        this.ClientRectangle.Y + BorderWidth / 2,
                        this.ClientRectangle.Width - BorderWidth,
                        this.ClientRectangle.Height - BorderWidth);

                    using (var borderPath = GraphicsExtensions.GetRoundedPath(borderRect, CornerRadius))
                    using (var pen = new Pen(BorderColor, BorderWidth))
                    {
                        e.Graphics.DrawPath(pen, borderPath);
                    }
                }
            }
        }
    }

    public class RoundedButton : Button
    {
        private Color _hoverColor;
        private Color _pressedColor;
        private Color _disabledColor = Color.FromArgb(189, 189, 189);
        private bool isHovering;
        private bool isPressed;

        [Category("Appearance")]
        public int CornerRadius { get; set; } = UITheme.BorderRadius;

        [Category("Appearance")]
        public Color HoverColor { get => _hoverColor; set => _hoverColor = value; }

        [Category("Appearance")]
        public Color PressedColor { get => _pressedColor; set => _pressedColor = value; }

        [Category("Appearance")]
        public Color DisabledColor { get => _disabledColor; set => _disabledColor = value; }

        [Category("Appearance")]
        public int BorderWidth { get; set; } = 1;

        [Category("Appearance")]
        public Color BorderColor { get; set; } = UITheme.BorderColor;

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            BackColor = UITheme.PrimaryColor;
            ForeColor = Color.White;
            Font = UITheme.FontMedium;
            Cursor = Cursors.Hand;
            TabStop = true;
            SetStyle(ControlStyles.Selectable, true);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovering = true;
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovering = false;
            isPressed = false;
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            isPressed = true;
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isPressed = false;
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Color fill = Enabled ? BackColor : DisabledColor;
            if (Enabled && isPressed && PressedColor != default) fill = PressedColor;
            else if (Enabled && isHovering && HoverColor != default) fill = HoverColor;

            using (var path = GraphicsExtensions.GetRoundedPath(this.ClientRectangle, CornerRadius))
            {
                using (var brush = new SolidBrush(fill))
                {
                    e.Graphics.FillPath(brush, path);
                }
                if (BorderWidth > 0)
                {
                    var borderRect = new Rectangle(
                        this.ClientRectangle.X + BorderWidth / 2,
                        this.ClientRectangle.Y + BorderWidth / 2,
                        this.ClientRectangle.Width - BorderWidth,
                        this.ClientRectangle.Height - BorderWidth);
                    using (var borderPath = GraphicsExtensions.GetRoundedPath(borderRect, CornerRadius))
                    using (var pen = new Pen(BorderColor, BorderWidth))
                    {
                        e.Graphics.DrawPath(pen, borderPath);
                    }
                }
            }
            // Texto centralizado
            TextRenderer.DrawText(
                e.Graphics,
                this.Text,
                this.Font,
                this.ClientRectangle,
                this.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine
            );
            // Foco visual
            if (Focused && ShowFocusCues)
            {
                var focusRect = this.ClientRectangle;
                focusRect.Inflate(-4, -4);
                ControlPaint.DrawFocusRectangle(e.Graphics, focusRect, ForeColor, fill);
            }
        }
    }
}