// MainForm.cs
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clipper.Modern
{
    public partial class MainForm : Form
    {
        private readonly VideoService _videoService;
        private string _loadedVideoPath = null; // Armazena o caminho do vídeo carregado

        public MainForm()
        {
            // Este método é gerado pelo Designer e cria toda a interface
            InitializeComponents();

            // Instancia nosso serviço de vídeo, passando o método de Log
            _videoService = new VideoService(Log);

            // Associa os métodos aos eventos de clique dos botões
            WireUpEventHandlers();
        }

        private void WireUpEventHandlers()
        {
            // Botões da aba URL
            btnLoadVideo.Click += BtnLoadVideo_Click;

            // Botões da aba Arquivo Local
            browseFileButton.Click += BrowseFileButton_Click;
            btnLoadLocalVideo.Click += BtnLoadLocalVideo_Click;

            // Botão de Processar
            processButton.Click += ProcessButton_Click;

            // Botão de Seleção de Pasta de Saída
            browseOutputButton.Click += BrowseOutputButton_Click;

            // Botão de Adicionar Clipe
            addClipButton.Click += AddClipButton_Click;
        }

        /// <summary>
        /// Manipula o clique do botão "Procurar..." na aba de arquivo local.
        /// </summary>
        private void BrowseFileButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Selecione um arquivo de vídeo";
                dialog.Filter = "Arquivos de Vídeo|*.mp4;*.mkv;*.avi;*.mov;*.webm|Todos os arquivos|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    filePathTextBox.Text = dialog.FileName;
                    btnLoadLocalVideo.Enabled = true; // Habilita o botão para carregar
                    lblLocalFileDuration.Text = "Duração: --:--:--"; // Reseta o label
                }
            }
        }

        /// <summary>
        /// Carrega informações de um vídeo a partir de uma URL.
        /// </summary>
        private async void BtnLoadVideo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(urlTextBox.Text))
            {
                MessageBox.Show("Por favor, insira uma URL do YouTube.", "URL Vazia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetLoadingState(true, lblDuration, btnLoadVideo);
            Log("Obtendo informações do vídeo da URL...");

            try
            {
                var duration = await _videoService.GetUrlVideoDurationAsync(urlTextBox.Text);
                UpdateDurationLabel(duration, lblDuration);
            }
            catch (Exception ex)
            {
                Log($"Erro ao obter duração da URL: {ex.Message}");
                lblDuration.Text = "Duração: Erro";
            }
            finally
            {
                SetLoadingState(false, lblDuration, btnLoadVideo);
            }
        }

        /// <summary>
        /// Carrega informações de um vídeo a partir de um arquivo local.
        /// </summary>
        private async void BtnLoadLocalVideo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(filePathTextBox.Text) || !File.Exists(filePathTextBox.Text))
            {
                MessageBox.Show("Por favor, selecione um arquivo de vídeo válido.", "Arquivo Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _loadedVideoPath = filePathTextBox.Text; // Armazena o caminho do vídeo local
            SetLoadingState(true, lblLocalFileDuration, btnLoadLocalVideo);
            Log($"Obtendo informações do arquivo: {_loadedVideoPath}");

            try
            {
                var duration = await _videoService.GetLocalFileDurationAsync(_loadedVideoPath);
                UpdateDurationLabel(duration, lblLocalFileDuration);
            }
            catch (Exception ex)
            {
                Log($"Erro ao obter duração do arquivo: {ex.Message}");
                lblLocalFileDuration.Text = "Duração: Erro";
            }
            finally
            {
                SetLoadingState(false, lblLocalFileDuration, btnLoadLocalVideo);
            }
        }

        /// <summary>
        /// Define o estado de carregamento dos controles da UI.
        /// </summary>
        private void SetLoadingState(bool isLoading, Label label, Button button)
        {
            if (isLoading)
            {
                label.Text = "Duração: Carregando...";
                button.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
            }
            else
            {
                button.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Atualiza o texto do label de duração com o valor formatado.
        /// </summary>
        private void UpdateDurationLabel(TimeSpan? duration, Label label)
        {
            if (duration.HasValue)
            {
                label.Text = $"Duração: {duration.Value:hh\\:mm\\:ss}";
                Log($"Duração obtida: {label.Text}");
            }
            else
            {
                label.Text = "Duração: Falha ao obter";
                Log("Não foi possível obter a duração do vídeo.");
            }
        }

        /// <summary>
        /// Lógica de processamento principal (exemplo).
        /// </summary>
        private async void ProcessButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validar campos obrigatórios
                string outputFolder = outputPathTextBox.Text;
                string format = formatComboBox.SelectedItem?.ToString() ?? "mp4";
                if (string.IsNullOrWhiteSpace(outputFolder) || !Directory.Exists(outputFolder))
                {
                    MessageBox.Show("Selecione uma pasta de saída válida.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (clipsGridView.Rows.Count == 0)
                {
                    MessageBox.Show("Adicione pelo menos um clipe à lista.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Determinar fonte do vídeo
                string videoPath = null;
                if (mainTabControl.SelectedTab == urlTab)
                {
                    // Download do vídeo
                    string url = urlTextBox.Text;
                    string quality = qualityComboBox.SelectedItem?.ToString() ?? "720p";
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        MessageBox.Show("Informe a URL do vídeo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    processButton.Enabled = false;
                    videoPath = await _videoService.DownloadVideoAsync(url, quality, outputFolder);
                    if (string.IsNullOrWhiteSpace(videoPath) || !File.Exists(videoPath))
                    {
                        MessageBox.Show("Falha ao baixar o vídeo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        processButton.Enabled = true;
                        return;
                    }
                }
                else if (mainTabControl.SelectedTab == fileTab)
                {
                    videoPath = filePathTextBox.Text;
                    if (string.IsNullOrWhiteSpace(videoPath) || !File.Exists(videoPath))
                    {
                        MessageBox.Show("Selecione um arquivo de vídeo válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Selecione a fonte do vídeo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3. Processar clipes
                int total = clipsGridView.Rows.Count;
                progressBar.Value = 0;
                progressBar.Maximum = total;
                for (int i = 0; i < total; i++)
                {
                    var row = clipsGridView.Rows[i];
                    if (row.IsNewRow) continue;
                    string startStr = row.Cells["StartTime"].Value?.ToString();
                    string endStr = row.Cells["EndTime"].Value?.ToString();
                    string name = row.Cells["Name"].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(startStr) || string.IsNullOrWhiteSpace(endStr) || string.IsNullOrWhiteSpace(name))
                    {
                        Log($"Linha {i + 1}: Dados do clipe incompletos. Pulando...");
                        continue;
                    }
                    if (!TimeSpan.TryParse(startStr, out var start) || !TimeSpan.TryParse(endStr, out var end) || end <= start)
                    {
                        Log($"Linha {i + 1}: Horários inválidos. Pulando...");
                        continue;
                    }
                    var clip = new ClipInfo(start, end, name);
                    await _videoService.CutClipAsync(videoPath, outputFolder, clip, format);
                    progressBar.Value = i + 1;
                    progressBar.Refresh();
                }
                Log("Processamento finalizado.");
                MessageBox.Show("Todos os clipes foram processados!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log($"Erro inesperado: {ex.Message}");
                MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                processButton.Enabled = true;
                progressBar.Value = 0;
            }
        }

        /// <summary>
        /// Manipula o clique do botão "Procurar Pasta" na seção de saída.
        /// </summary>
        private void BrowseOutputButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Selecione a pasta de saída para os clipes";
                dialog.UseDescriptionForTitle = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    outputPathTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// Manipula o clique do botão "Adicionar" na seção de clipes.
        /// </summary>
        private void AddClipButton_Click(object sender, EventArgs e)
        {
            // Adiciona uma linha vazia para o usuário preencher
            clipsGridView.Rows.Add("00:00:00", "00:00:00", "Novo Clipe");
        }

        /// <summary>
        /// Adiciona uma mensagem à caixa de log da interface.
        /// </summary>
        private void Log(string message)
        {
            if (logTextBox.InvokeRequired)
            {
                logTextBox.Invoke(new Action(() => Log(message)));
            }
            else
            {
                logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
                logTextBox.ScrollToCaret();
            }
        }
    }
}