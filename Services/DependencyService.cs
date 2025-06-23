// Services/DependencyService.cs
using System;
using System.Threading.Tasks;

namespace Clipper.Modern
{
    public class DependencyService
    {
        private readonly ProcessService _processService;
        private readonly Action<string> _logAction; // <--- ADICIONADO

        // O construtor agora recebe a a��o de log
        public DependencyService(Action<string> logAction)
        {
            _processService = new ProcessService();
            _logAction = logAction; // <--- ADICIONADO
        }

        public async Task<bool> CheckDependenciesAsync()
        {
            _logAction("Verificando depend�ncias (ffmpeg e yt-dlp)...");

            var ffmpegOk = await CheckFfmpegAsync();
            var ytDlpOk = await CheckYtDlpAsync();

            if (ffmpegOk && ytDlpOk)
            {
                _logAction("Todas as depend�ncias foram encontradas.");
                return true;
            }

            _logAction("ERRO: Uma ou mais depend�ncias n�o foram encontradas. Verifique se ffmpeg e yt-dlp est�o no PATH do sistema ou na pasta do programa.");
            return false;
        }

        private async Task<bool> CheckFfmpegAsync()
        {
            try
            {
                // A chamada agora inclui o _logAction (ou null se n�o quiser logar esta verifica��o)
                var result = await _processService.RunAsync("ffmpeg", "-version", _logAction); // <--- CORRIGIDO
                if (result.ExitCode == 0 && result.Output.Contains("ffmpeg version"))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logAction($"Erro ao verificar ffmpeg: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> CheckYtDlpAsync()
        {
            try
            {
                // A chamada agora inclui o _logAction
                var result = await _processService.RunAsync("yt-dlp", "--version", _logAction); // <--- CORRIGIDO
                // yt-dlp --version retorna apenas a data/vers�o, ent�o verificamos se a sa�da n�o est� vazia
                if (result.ExitCode == 0 && !string.IsNullOrWhiteSpace(result.Output))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logAction($"Erro ao verificar yt-dlp: {ex.Message}");
                return false;
            }
        }
    }
}