// Services/VideoService.cs
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Clipper.Modern
{
    public class VideoService
    {
        private readonly Action<string> _logAction;
        private readonly ProcessService _processService;

        public VideoService(Action<string> logAction)
        {
            _logAction = logAction;
            _processService = new ProcessService();
        }

        /// <summary>
        /// Obtém a duração de um vídeo de uma URL usando yt-dlp.
        /// </summary>
        public async Task<TimeSpan?> GetUrlVideoDurationAsync(string url)
        {
            string args = $"--print duration \"{url}\"";
            var result = await _processService.RunAsync("yt-dlp", args, _logAction);

            if (result.ExitCode == 0 && !string.IsNullOrWhiteSpace(result.Output))
            {
                // Tenta converter a saída (em segundos) para um TimeSpan
                if (double.TryParse(result.Output, NumberStyles.Any, CultureInfo.InvariantCulture, out double seconds))
                {
                    return TimeSpan.FromSeconds(seconds);
                }
            }

            _logAction($"Falha ao obter duração da URL. Erro: {result.Error}");
            return null;
        }

        /// <summary>
        /// Obtém a duração de um arquivo de vídeo local usando ffprobe.
        /// </summary>
        public async Task<TimeSpan?> GetLocalFileDurationAsync(string filePath)
        {
            string args = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{filePath}\"";
            var result = await _processService.RunAsync("ffprobe", args, _logAction);

            if (result.ExitCode == 0 && !string.IsNullOrWhiteSpace(result.Output))
            {
                // Tenta converter a saída (em segundos) para um TimeSpan
                if (double.TryParse(result.Output, NumberStyles.Any, CultureInfo.InvariantCulture, out double seconds))
                {
                    return TimeSpan.FromSeconds(seconds);
                }
            }

            _logAction($"Falha ao obter duração do arquivo local. Erro: {result.Error}");
            return null;
        }

        public async Task<string> DownloadVideoAsync(string url, string quality, string outputFolder)
        {
            _logAction("Iniciando download do vídeo...");

            string qualityFilter = quality.Replace("p", "");
            if (quality == "2K") qualityFilter = "1440";
            if (quality == "4K") qualityFilter = "2160";

            string tempFileName = $"original_download_{Guid.NewGuid()}.mp4";
            string videoPath = Path.Combine(outputFolder, tempFileName);

            string args = $"-f \"bv[height<={qualityFilter}]+ba/best\" --merge-output-format mp4 -o \"{videoPath}\" \"{url}\"";
            var result = await _processService.RunAsync("yt-dlp", args, _logAction);

            if (result.ExitCode != 0 || !File.Exists(videoPath))
            {
                _logAction($"Falha no download. yt-dlp terminou com código {result.ExitCode}.");
                _logAction($"Erro: {result.Error}");
                return null;
            }

            _logAction("Download concluído com sucesso.");
            return videoPath;
        }

        public async Task CutClipAsync(string sourceVideoPath, string outputFolder, ClipInfo clip, string format)
        {
            string outputFileName = $"{clip.Name}.{format}";
            string outputPath = Path.Combine(outputFolder, outputFileName);

            string startTime = clip.StartTime.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
            string duration = (clip.EndTime - clip.StartTime).ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
            string args = $"-ss {startTime} -i \"{sourceVideoPath}\" -t {duration} -c:v copy -c:a copy \"{outputPath}\" -y";
            var result = await _processService.RunAsync("ffmpeg", args, _logAction);

            if (result.ExitCode != 0)
            {
                _logAction($"Falha ao gerar o clipe '{clip.Name}'. ffmpeg terminou com código {result.ExitCode}.");
                _logAction($"Erro: {result.Error}");
            }
            else
            {
                _logAction($"Clipe '{clip.Name}' gerado com sucesso.");
            }
        }
    }

    // Um record simples para guardar as informações do clipe
    public record ClipInfo(TimeSpan StartTime, TimeSpan EndTime, string Name);
}