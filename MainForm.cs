using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO.Compression;

namespace Clipper
{
    public partial class MainForm : Form
    {
        private List<(MaskedTextBox startTime, MaskedTextBox endTime, TextBox name)> clipTimeControls = new();
        private ProgressBar progressBar;
        private CheckBox chkKeepOriginal;
        private ComboBox comboFormat;
        private bool isPortuguese;

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            numClips_ValueChanged(null, null);
        }

        private async Task EnsureYtDlpAndFfmpegAsync()
        {
            // === yt-dlp ===
            if (!await TestCommandAsync("yt-dlp", "--version"))
            {
                AppendLog("yt-dlp not found. Trying to install with pip...");
                try
                {
                    await RunProcessAsync("pip", "install yt-dlp");
                    AppendLog("yt-dlp installed via pip.");
                }
                catch
                {
                    MessageBox.Show("yt-dlp not found and automatic installation via pip failed.");
                    throw;
                }
            }

            // === ffmpeg ===
            if (!await TestCommandAsync("ffmpeg", "-version"))
            {
                AppendLog("ffmpeg not found. Trying to install with winget...");
                try
                {
                    await RunProcessAsync("winget", "install --id Gyan.FFmpeg -e --accept-source-agreements --accept-package-agreements");
                    AppendLog("ffmpeg installed via winget.");
                }
                catch
                {
                    MessageBox.Show("ffmpeg not found and automatic installation via winget failed.");
                    throw;
                }
            }
        }


        private async Task<bool> TestCommandAsync(string command, string arguments)
        {
            try
            {
                var tcs = new TaskCompletionSource<bool>();
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = command,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };

                proc.Exited += (s, e) => tcs.TrySetResult(proc.ExitCode == 0);
                proc.Start();
                await tcs.Task;

                return true;
            }
            catch
            {
                return false;
            }
        }


        private void InitializeCustomComponents()
        {
            isPortuguese = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "pt";

            comboQuality.Items.AddRange(new string[] { "360p", "480p", "720p", "1080p", "2K", "4K" });
            comboQuality.SelectedIndex = 3;

            comboFormat = new ComboBox
            {
                Location = new System.Drawing.Point(210, 45),
                Size = new System.Drawing.Size(100, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboFormat.Items.AddRange(new string[] { "mp4", "avi", "mov", "mkv" });
            comboFormat.SelectedIndex = 0;
            Controls.Add(comboFormat);

            progressBar = new ProgressBar
            {
                Location = new System.Drawing.Point(12, 320),
                Size = new System.Drawing.Size(518, 20),
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };
            Controls.Add(progressBar);

            chkKeepOriginal = new CheckBox
            {
                Text = isPortuguese ? "Manter vídeo original" : "Keep original video",
                Location = new System.Drawing.Point(12, 350),
                AutoSize = true
            };
            Controls.Add(chkKeepOriginal);
        }

        private void btnChooseFolder_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
                txtOutputPath.Text = fbd.SelectedPath;
        }

        private void numClips_ValueChanged(object sender, EventArgs e)
        {
            panelClips.Controls.Clear();
            clipTimeControls.Clear();

            for (int i = 0; i < numClips.Value; i++)
            {
                int top = i * 30;

                Label lblStart = new() { Text = $"Start {i + 1}", Top = top, Left = 0, Width = 60 };
                MaskedTextBox startBox = new() { Mask = "00:00", Text = "0000", Top = top, Left = 65, Width = 50 };

                Label lblEnd = new() { Text = "End", Top = top, Left = 120, Width = 40 };
                MaskedTextBox endBox = new() { Mask = "00:00", Text = "0000", Top = top, Left = 165, Width = 50 };

                Label lblName = new() { Text = "Name", Top = top, Left = 220, Width = 45 };
                TextBox nameBox = new() { Top = top, Left = 270, Width = 120 };

                panelClips.Controls.AddRange(new Control[] { lblStart, startBox, lblEnd, endBox, lblName, nameBox });
                clipTimeControls.Add((startBox, endBox, nameBox));
            }
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            btnGenerate.Enabled = false;
            progressBar.Value = 0;
            AppendLog("Verifying dependencies...");

            try
            {
                await EnsureYtDlpAndFfmpegAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao garantir dependências: " + ex.Message);
                btnGenerate.Enabled = true;
                return;
            }

            string url = txtUrl.Text.Trim();
            string quality = comboQuality.SelectedItem?.ToString() ?? "1080p";
            quality = quality switch
            {
                "2K" => "1440p",
                "4K" => "2160p",
                _ => quality
            };
            string format = comboFormat.SelectedItem?.ToString() ?? "mp4";
            string outputFolder = txtOutputPath.Text.Trim();

            // === Validações iniciais ===
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show(isPortuguese ? "Informe a URL do vídeo." : "Please enter the video URL.");
                btnGenerate.Enabled = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(outputFolder) || !Directory.Exists(outputFolder))
            {
                MessageBox.Show(isPortuguese ? "Selecione uma pasta de saída válida." : "Please select a valid output folder.");
                btnGenerate.Enabled = true;
                return;
            }

            if (clipTimeControls.Count == 0)
            {
                MessageBox.Show(isPortuguese ? "Adicione ao menos um clipe." : "Please add at least one clip.");
                btnGenerate.Enabled = true;
                return;
            }

            HashSet<string> usedNames = new HashSet<string>();
            for (int i = 0; i < clipTimeControls.Count; i++)
            {
                var (startBox, endBox, nameBox) = clipTimeControls[i];

                // Verifica formatação dos tempos
                if (!TimeSpan.TryParseExact(startBox.Text, "mm\\:ss", CultureInfo.InvariantCulture, out TimeSpan start) ||
                    !TimeSpan.TryParseExact(endBox.Text, "mm\\:ss", CultureInfo.InvariantCulture, out TimeSpan end))
                {
                    MessageBox.Show($"{(isPortuguese ? "Erro no tempo do clipe" : "Invalid time in clip")} #{i + 1}");
                    btnGenerate.Enabled = true;
                    return;
                }

                if (end <= start)
                {
                    MessageBox.Show($"{(isPortuguese ? "Tempo final menor que o inicial no clipe" : "End time must be after start time in clip")} #{i + 1}");
                    btnGenerate.Enabled = true;
                    return;
                }

                string name = string.IsNullOrWhiteSpace(nameBox.Text) ? $"clip_{i + 1}" : nameBox.Text.Trim();

                if (!usedNames.Add(name))
                {
                    MessageBox.Show($"{(isPortuguese ? "Nomes duplicados detectados. Nome do clipe repetido:" : "Duplicate clip name detected:")} \"{name}\"");
                    btnGenerate.Enabled = true;
                    return;
                }

                if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    MessageBox.Show($"{(isPortuguese ? "Nome de clipe inválido:" : "Invalid clip name:")} \"{name}\"");
                    btnGenerate.Enabled = true;
                    return;
                }
            }

            string videoPath = Path.Combine(outputFolder, "original_downloaded.mp4");
            string downloadArgs = $"-f \"bv[height<={quality.Replace("p", "")}] + ba\" --merge-output-format mp4 -o \"{videoPath}\" {url}";

            try
            {
                await RunProcessAsync("yt-dlp", downloadArgs);
            }
            catch (Exception ex)
            {
                MessageBox.Show((isPortuguese ? "Erro ao baixar vídeo: " : "Error downloading video: ") + ex.Message);
                btnGenerate.Enabled = true;
                return;
            }

            int count = 0;
            foreach (var (startBox, endBox, nameBox) in clipTimeControls)
            {
                TimeSpan start = TimeSpan.ParseExact(startBox.Text, "mm\\:ss", CultureInfo.InvariantCulture);
                TimeSpan end = TimeSpan.ParseExact(endBox.Text, "mm\\:ss", CultureInfo.InvariantCulture);

                string name = string.IsNullOrWhiteSpace(nameBox.Text) ? $"clip_{count + 1}" : nameBox.Text.Trim();
                string output = Path.Combine(outputFolder, $"{name}.{format}");
                string ffmpegArgs = $"-ss {start} -i \"{videoPath}\" -to {(end - start)} -c copy \"{output}\"";

                try
                {
                    await RunProcessAsync("ffmpeg", ffmpegArgs);
                }
                catch (Exception ex)
                {
                    MessageBox.Show((isPortuguese ? "Erro ao gerar clipe: " : "Error generating clip: ") + ex.Message);
                }

                progressBar.Value = (++count * 100) / clipTimeControls.Count;
            }

            if (!chkKeepOriginal.Checked && File.Exists(videoPath))
            {
                try { File.Delete(videoPath); } catch { }
            }

            MessageBox.Show(isPortuguese ? "Todos os clipes foram gerados!" : "All clips generated successfully!");
            btnGenerate.Enabled = true;
        }

        private Task RunProcessAsync(string fileName, string arguments)
        {
            var tcs = new TaskCompletionSource<bool>();

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true

                },
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (s, e) => AppendLog(e.Data);
            process.ErrorDataReceived += (s, e) => AppendLog(e.Data);

            process.Exited += (s, e) => tcs.TrySetResult(true);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }

        private void AppendLog(string? message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            if (logBox.InvokeRequired)
            {
                logBox.Invoke(new Action(() =>
                {
                    logBox.AppendText(message + Environment.NewLine);
                    logBox.SelectionStart = logBox.TextLength;
                    logBox.ScrollToCaret();
                }));
            }
            else
            {
                logBox.AppendText(message + Environment.NewLine);
                logBox.SelectionStart = logBox.TextLength;
                logBox.ScrollToCaret();
            }
        }
    }
}
