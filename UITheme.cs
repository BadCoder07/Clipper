// UITheme.cs
using System.Drawing;

namespace Clipper.Modern
{
    /// <summary>
    /// Tema visual centralizado para toda a aplicação.
    /// </summary>
    public static class UITheme
    {
        // Cores principais
        public static Color PrimaryColor { get; } = Color.FromArgb(0, 120, 215); // Azul vibrante
        public static Color PrimaryHover { get; } = Color.FromArgb(51, 153, 255);
        public static Color PrimaryPressed { get; } = Color.FromArgb(0, 84, 153);

        public static Color BackgroundColor { get; } = Color.White;
        public static Color SurfaceColor { get; } = Color.FromArgb(245, 247, 250); // Card
        public static Color BorderColor { get; } = Color.FromArgb(222, 226, 230);
        public static Color TextPrimary { get; } = Color.FromArgb(33, 37, 41);
        public static Color TextSecondary { get; } = Color.FromArgb(108, 117, 125);

        // Cores de estado
        public static Color SuccessColor { get; } = Color.FromArgb(40, 167, 69);
        public static Color WarningColor { get; } = Color.FromArgb(255, 193, 7);
        public static Color ErrorColor { get; } = Color.FromArgb(220, 53, 69);

        // Fontes
        public static Font FontLarge { get; } = new Font("Segoe UI Variable", 18F, FontStyle.Bold);
        public static Font FontMedium { get; } = new Font("Segoe UI Variable", 12F, FontStyle.Regular);
        public static Font FontSmall { get; } = new Font("Segoe UI Variable", 10F, FontStyle.Regular);
        public static Font FontBold { get; } = new Font("Segoe UI Variable", 12F, FontStyle.Bold);

        // Espaçamentos e Bordas
        public static int Spacing { get; } = 10;
        public static int Padding { get; } = 18;
        public static int BorderRadius { get; } = 8;
    }
}