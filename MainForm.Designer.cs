namespace Clipper
{

    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtUrl, txtOutputPath;
        private System.Windows.Forms.ComboBox comboQuality;
        private System.Windows.Forms.NumericUpDown numClips;
        private System.Windows.Forms.Button btnChooseFolder, btnGenerate;
        private System.Windows.Forms.Label lblUrl, lblQuality, lblClips, lblOutput;
        private System.Windows.Forms.Panel panelClips;
        private RichTextBox logBox;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // --- Form Setup ---
            this.Text = "YouTube Clipper";
            this.Size = new System.Drawing.Size(800, 600);
            this.MinimumSize = new System.Drawing.Size(600, 500);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            // --- Labels ---
            lblUrl = new Label { Text = "URL", Location = new Point(12, 15), Width = 60 };
            lblQuality = new Label { Text = "Quality", Location = new Point(12, 48), Width = 60 };
            lblClips = new Label { Text = "Clips", Location = new Point(12, 80), Width = 60 };
            lblOutput = new Label { Text = "Output", Location = new Point(12, 115), Width = 60 };

            // --- Input Fields ---
            txtUrl = new TextBox
            {
                Location = new Point(80, 12),
                Width = 600,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            comboQuality = new ComboBox
            {
                Location = new Point(80, 45),
                Size = new Size(100, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            comboQuality.Items.AddRange(new string[] { "360p", "480p", "720p", "1080p", "2K", "4K" });
            comboQuality.SelectedIndex = 3;

            comboFormat = new ComboBox
            {
                Location = new Point(190, 45),
                Size = new Size(100, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            comboFormat.Items.AddRange(new string[] { "mp4", "avi", "mov", "mkv" });
            comboFormat.SelectedIndex = 0;

            numClips = new NumericUpDown
            {
                Location = new Point(80, 78),
                Minimum = 1,
                Maximum = 10,
                Value = 1,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            numClips.ValueChanged += numClips_ValueChanged;

            txtOutputPath = new TextBox
            {
                Location = new Point(80, 112),
                Width = 580,
                ReadOnly = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // --- Buttons ---
            btnChooseFolder = new Button
            {
                Text = "Browse",
                Location = new Point(670, 110),
                Width = 80,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnChooseFolder.Click += btnChooseFolder_Click;

            btnGenerate = new Button
            {
                Text = "Generate",
                Location = new Point(650, 510),
                Width = 100,
                Height = 30,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnGenerate.Click += btnGenerate_Click;

            // --- Panel ---
            panelClips = new Panel
            {
                Location = new Point(12, 150),
                Size = new Size(740, 300),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // --- ProgressBar & Options ---
            progressBar = new ProgressBar
            {
                Location = new Point(12, 460),
                Size = new Size(738, 20),
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            logBox = new RichTextBox
            {
                Location = new Point(12, 380),
                Size = new Size(738, 120),
                ReadOnly = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Consolas", 9),
                BackColor = Color.Black,
                ForeColor = Color.LightGreen
            };
            this.Controls.Add(logBox);

            chkKeepOriginal = new CheckBox
            {
                Text = "Keep original video",
                Location = new Point(12, 510),
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            // --- Add Controls to Form ---
            this.Controls.AddRange(new Control[]
            {
        lblUrl, txtUrl,
        lblQuality, comboQuality, comboFormat,
        lblClips, numClips,
        lblOutput, txtOutputPath, btnChooseFolder,
        panelClips,
        progressBar, chkKeepOriginal,
        btnGenerate
            });
        }

    }
}
