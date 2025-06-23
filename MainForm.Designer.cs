// MainForm.Designer.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Clipper.Modern
{
    public partial class MainForm : Form
    {
        // ------------------------------------------------------------
        //  STYLE
        // ------------------------------------------------------------
        private static class Style
        {
            public static readonly Font FontSmall = new Font("Segoe UI", 9F);
            public static readonly Font FontMedium = new Font("Segoe UI", 10F);
            public static readonly Font FontLarge = new Font("Segoe UI", 14F, FontStyle.Bold);
            public static readonly Font FontBold = new Font("Segoe UI", 10F, FontStyle.Bold);

            public static readonly Color BackgroundColor = Color.FromArgb(245, 245, 245);
            public static readonly Color SurfaceColor = Color.White;
            public static readonly Color PrimaryColor = Color.FromArgb(0, 120, 215);
            public static readonly Color PrimaryHover = Color.FromArgb(0, 102, 183);
            public static readonly Color PrimaryPressed = Color.FromArgb(0, 76, 136);
            public static readonly Color ErrorColor = Color.FromArgb(200, 0, 0);
            public static readonly Color TextPrimary = Color.Black;
            public static readonly Color TextSecondary = Color.FromArgb(80, 80, 80);

            public const int Padding = 12;
            public const int ButtonWidth = 140;
            public const int ButtonHeight = 36;
            public const int LabelFieldGap = 20; // mandated gap
        }

        // ------------------------------------------------------------
        //  FIELDS
        // ------------------------------------------------------------
        private Panel headerPanel;
        private Label titleLabel;
        private RoundedButton settingsButton;

        private TabControl mainTabControl;
        private TabPage urlTab;
        private TabPage fileTab;

        private TextBox urlTextBox;
        private RoundedButton btnLoadVideo;
        private Label lblDuration;
        private ComboBox qualityComboBox;

        private TextBox filePathTextBox;
        private RoundedButton browseFileButton;
        private RoundedButton btnLoadLocalVideo;
        private Label lblLocalFileDuration;

        private RoundedPanel outputPanel;
        private TextBox outputPathTextBox;
        private RoundedButton browseOutputButton;
        private ComboBox formatComboBox;

        private RoundedPanel clipsPanel;
        private DataGridView clipsGridView;
        private RoundedButton addClipButton;
        private RoundedButton removeClipButton;

        private RoundedPanel actionPanel;
        private RoundedButton processButton;
        private ProgressBar progressBar;
        private RichTextBox logTextBox;

        // ------------------------------------------------------------
        //  INITIALISE COMPONENTS
        // ------------------------------------------------------------
        private void InitializeComponents()
        {
            this.SuspendLayout();

            this.BackColor = Style.BackgroundColor;
            this.Font = Style.FontSmall;
            this.Text = "Bida Clipper 2.0";
            this.ClientSize = new Size(900, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(Style.Padding)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 210));

            this.CreateHeader();
            this.CreateTabControl();
            this.CreateOutputSection();
            this.CreateClipsSection();
            this.CreateActionSection();

            mainLayout.Controls.Add(this.headerPanel, 0, 0);
            mainLayout.Controls.Add(this.mainTabControl, 0, 1);
            mainLayout.Controls.Add(this.outputPanel, 0, 2);
            mainLayout.Controls.Add(this.clipsPanel, 0, 3);
            mainLayout.Controls.Add(this.actionPanel, 0, 4);

            this.Controls.Add(mainLayout);
            this.ResumeLayout(false);
        }

        // ------------------------------------------------------------
        //  HEADER
        // ------------------------------------------------------------
        private void CreateHeader()
        {
            this.headerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Style.PrimaryColor };

            this.titleLabel = new Label
            {
                Text = "\uD83C\uDFAC Bida Clipper 2.0",
                Font = Style.FontLarge,
                ForeColor = Color.White,
                AutoSize = false, // Set to false to allow docking and alignment.
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft, // This centers the text vertically.
                Padding = new Padding(Style.Padding, 0, 0, 0)
            };

            // The settings button is commented out in the original code, preserving this.
            // settingsButton = CreateButton("⚙️", Style.SurfaceColor, Style.PrimaryHover, Style.PrimaryPressed);
            // ...

            this.headerPanel.Controls.Add(this.titleLabel);
            // this.headerPanel.Controls.Add(this.settingsButton);
        }

        // ------------------------------------------------------------
        //  TABCONTROL & TABS
        // ------------------------------------------------------------
        private void CreateTabControl()
        {
            this.mainTabControl = new TabControl { Dock = DockStyle.Fill, Font = Style.FontMedium, Padding = new Point(12, 6) };
            this.urlTab = new TabPage("URL do YouTube") { BackColor = Style.SurfaceColor };
            this.fileTab = new TabPage("Arquivo Local") { BackColor = Style.SurfaceColor };

            this.BuildUrlTab();
            this.BuildFileTab();

            this.mainTabControl.TabPages.Add(this.urlTab);
            this.mainTabControl.TabPages.Add(this.fileTab);
        }

        private void BuildUrlTab()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 3,
                Padding = new Padding(16, 10, 16, 10)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80)); // label
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Style.LabelFieldGap)); // gap
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70)); // field
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Style.ButtonWidth)); // button
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

            // URL row: Controls are vertically centered via Anchor and TextAlign properties.
            var lblUrl = new Label { Text = "URL:", Font = Style.FontSmall, AutoSize = true, TextAlign = ContentAlignment.MiddleRight, Anchor = AnchorStyles.Right };
            layout.Controls.Add(lblUrl, 0, 0);

            layout.Controls.Add(new Panel(), 1, 0); // gap

            this.urlTextBox = new TextBox { PlaceholderText = "Cole a URL do YouTube aqui...", Font = Style.FontMedium, Anchor = AnchorStyles.Left | AnchorStyles.Right };
            layout.Controls.Add(this.urlTextBox, 2, 0);

            this.btnLoadVideo = this.CreateButton("Carregar", Style.PrimaryColor, Style.PrimaryHover, Style.PrimaryPressed);
            this.btnLoadVideo.Anchor = AnchorStyles.None; // Center button in its cell.
            layout.Controls.Add(this.btnLoadVideo, 3, 0);

            // Qualidade row
            var lblQualidade = new Label { Text = "Qualidade:", Font = Style.FontSmall, AutoSize = true, TextAlign = ContentAlignment.MiddleRight, Anchor = AnchorStyles.Right };
            layout.Controls.Add(lblQualidade, 0, 1);

            layout.Controls.Add(new Panel(), 1, 1);

            this.qualityComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120, Font = Style.FontSmall, Anchor = AnchorStyles.Left };
            this.qualityComboBox.Items.AddRange(new object[] { "360p", "480p", "720p", "1080p", "1440p", "2160p" });
            this.qualityComboBox.SelectedIndex = 3;
            layout.Controls.Add(this.qualityComboBox, 2, 1);
            layout.SetColumnSpan(this.qualityComboBox, 2); // spans field and button columns

            // Duração row
            var lblDuracao = new Label { Text = "Duração:", Font = Style.FontSmall, AutoSize = true, TextAlign = ContentAlignment.MiddleRight, Anchor = AnchorStyles.Right };
            layout.Controls.Add(lblDuracao, 0, 2);

            layout.Controls.Add(new Panel(), 1, 2);

            this.lblDuration = new Label { Text = "--:--:--", AutoSize = true, ForeColor = Style.TextSecondary, Font = Style.FontSmall, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
            layout.Controls.Add(this.lblDuration, 2, 2);
            layout.SetColumnSpan(this.lblDuration, 2);

            this.urlTab.Controls.Add(layout);
        }

        private void BuildFileTab()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 2,
                Padding = new Padding(16, 10, 16, 10)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60)); // label
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Style.LabelFieldGap)); // gap
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); // field
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Style.ButtonWidth)); // browse
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Style.ButtonWidth)); // load
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

            // Path row: Controls are vertically centered via Anchor property.
            var lblPath = new Label { Text = "Path:", Font = Style.FontSmall, AutoSize = true, TextAlign = ContentAlignment.MiddleRight, Anchor = AnchorStyles.Right };
            layout.Controls.Add(lblPath, 0, 0);

            layout.Controls.Add(new Panel(), 1, 0);

            this.filePathTextBox = new TextBox { Font = Style.FontMedium, ReadOnly = true, BackColor = Style.SurfaceColor, Anchor = AnchorStyles.Left | AnchorStyles.Right };
            layout.Controls.Add(this.filePathTextBox, 2, 0);

            this.browseFileButton = this.CreateButton("Procurar", Style.PrimaryColor, Style.PrimaryHover, Style.PrimaryPressed);
            this.browseFileButton.Anchor = AnchorStyles.None;
            layout.Controls.Add(this.browseFileButton, 3, 0);

            this.btnLoadLocalVideo = this.CreateButton("Carregar", Style.PrimaryColor, Style.PrimaryHover, Style.PrimaryPressed);
            this.btnLoadLocalVideo.Enabled = false;
            this.btnLoadLocalVideo.Anchor = AnchorStyles.None;
            layout.Controls.Add(this.btnLoadLocalVideo, 4, 0);

            // Duration row
            this.lblLocalFileDuration = new Label { Text = "Duração: --:--:--", AutoSize = true, ForeColor = Style.TextSecondary, Font = Style.FontSmall, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft };
            layout.Controls.Add(this.lblLocalFileDuration, 0, 1);
            layout.SetColumnSpan(this.lblLocalFileDuration, 5);

            this.fileTab.Controls.Add(layout);
        }

        // ------------------------------
        //  OUTPUT SECTION
        // ------------------------------
        private void CreateOutputSection()
        {
            this.outputPanel = this.CreateSection("Configurações de Saída");

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 3
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120)); // label
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Style.LabelFieldGap)); // gap
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // field
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Style.ButtonWidth + 10)); // button

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // spacer
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36)); // pasta
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36)); // formato

            // Pasta destino row: Controls are vertically centered via Anchor property.
            var lblPasta = new Label { Text = "Pasta destino:", Font = Style.FontSmall, AutoSize = true, TextAlign = ContentAlignment.MiddleRight, Anchor = AnchorStyles.Right };
            layout.Controls.Add(lblPasta, 0, 1);

            layout.Controls.Add(new Panel(), 1, 1);

            this.outputPathTextBox = new TextBox { ReadOnly = true, BackColor = Style.SurfaceColor, Font = Style.FontMedium, Anchor = AnchorStyles.Left | AnchorStyles.Right };
            layout.Controls.Add(this.outputPathTextBox, 2, 1);

            this.browseOutputButton = this.CreateButton("Procurar", Style.PrimaryColor, Style.PrimaryHover, Style.PrimaryPressed);
            this.browseOutputButton.Anchor = AnchorStyles.None;
            layout.Controls.Add(this.browseOutputButton, 3, 1);

            // Formato row
            var lblFormato = new Label { Text = "Formato:", Font = Style.FontSmall, AutoSize = true, TextAlign = ContentAlignment.MiddleRight, Anchor = AnchorStyles.Right };
            layout.Controls.Add(lblFormato, 0, 2);

            layout.Controls.Add(new Panel(), 1, 2);

            this.formatComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 100, Font = Style.FontSmall, Anchor = AnchorStyles.Left };
            this.formatComboBox.Items.AddRange(new object[] { "mp4", "mkv", "mov", "avi" });
            layout.Controls.Add(this.formatComboBox, 2, 2);
            layout.SetColumnSpan(this.formatComboBox, 2);

            this.outputPanel.Controls.Add(layout);
        }

        // ------------------------------
        //  CLIPS SECTION
        // ------------------------------
        private void CreateClipsSection()
        {
            this.clipsPanel = this.CreateSection("Lista de Clipes");

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Style.ButtonWidth + 12));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var spacer = new Panel { Dock = DockStyle.Fill };
            layout.Controls.Add(spacer, 0, 0);
            layout.SetColumnSpan(spacer, 2);

            this.clipsGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                GridColor = Color.LightGray,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                RowTemplate = { Height = 28 },
                Font = Style.FontSmall,
                EnableHeadersVisualStyles = false
            };
            this.clipsGridView.DefaultCellStyle.SelectionBackColor = Style.PrimaryHover;
            this.clipsGridView.DefaultCellStyle.SelectionForeColor = Color.White;
            this.clipsGridView.DefaultCellStyle.Padding = new Padding(6, 2, 6, 2);
            this.clipsGridView.Columns.Add("StartTime", "Início (HH:MM:SS)");
            this.clipsGridView.Columns.Add("EndTime", "Fim (HH:MM:SS)");
            this.clipsGridView.Columns.Add("Name", "Nome do Clipe");
            this.clipsGridView.ColumnHeadersDefaultCellStyle.BackColor = Style.PrimaryColor;
            this.clipsGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.clipsGridView.ColumnHeadersDefaultCellStyle.Font = Style.FontBold;
            this.clipsGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 4, 6, 4);

            layout.Controls.Add(this.clipsGridView, 0, 1);

            var buttonsPanel = new FlowLayoutPanel { Dock = DockStyle.Top, FlowDirection = FlowDirection.TopDown };
            this.addClipButton = this.CreateButton("Adicionar", Style.PrimaryColor, Style.PrimaryHover, Style.PrimaryPressed);
            this.removeClipButton = this.CreateButton("Remover", Style.ErrorColor, Color.FromArgb(160, 0, 0), Color.FromArgb(120, 0, 0));
            buttonsPanel.Controls.Add(this.addClipButton);
            buttonsPanel.Controls.Add(this.removeClipButton);

            layout.Controls.Add(buttonsPanel, 1, 1);

            this.clipsPanel.Controls.Add(layout);
        }

        // ------------------------------------------------------------
        //  ACTION SECTION
        // ------------------------------------------------------------
        private void CreateActionSection()
        {
            this.actionPanel = new RoundedPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Style.SurfaceColor,
                Padding = new Padding(18, 12, 18, 12),
                CornerRadius = 0,
                BorderWidth = 1,
                BorderColor = Color.LightGray
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            this.processButton = this.CreateButton("Gerar Clipes", Style.PrimaryColor, Style.PrimaryHover, Style.PrimaryPressed);
            this.processButton.Width = 260;
            this.processButton.Font = Style.FontBold;
            this.processButton.Anchor = AnchorStyles.None; // Center the button in the available space.

            this.progressBar = new ProgressBar { Dock = DockStyle.Top, Height = 16, Style = ProgressBarStyle.Continuous };
            this.logTextBox = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.FromArgb(40, 40, 40), ForeColor = Color.LightGray, Font = new Font("Consolas", 10F), BorderStyle = BorderStyle.None };

            layout.Controls.Add(this.processButton, 0, 0);
            layout.Controls.Add(this.progressBar, 0, 1);
            layout.Controls.Add(this.logTextBox, 0, 2);

            this.actionPanel.Controls.Add(layout);
        }

        // ------------------------------------------------------------
        //  HELPERS
        // ------------------------------------------------------------
        private RoundedButton CreateButton(string text, Color backColor, Color hoverColor, Color pressedColor)
        {
            return new RoundedButton
            {
                Text = text,
                Width = Style.ButtonWidth,
                Height = Style.ButtonHeight,
                BackColor = backColor,
                HoverColor = hoverColor,
                PressedColor = pressedColor,
                Font = Style.FontMedium,
                ForeColor = Color.White,
                CornerRadius = 0
            };
        }

        private RoundedPanel CreateSection(string title)
        {
            var panel = new RoundedPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Style.SurfaceColor,
                Padding = new Padding(18),
                CornerRadius = 0,
                BorderWidth = 1,
                BorderColor = Color.LightGray
            };
            var titleLbl = new Label { Text = title, Font = Style.FontBold, ForeColor = Style.TextPrimary, Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 0, 0, 10) };
            panel.Controls.Add(titleLbl);
            return panel;
        }
    }
}
