// Services/ProcessService.cs
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Clipper.Modern
{
    public class ProcessService
    {
        public record ProcessResult(int ExitCode, string Output, string Error);

        public Task<ProcessResult> RunAsync(string fileName, string arguments, Action<string> logger)
        {
            return Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    },
                    EnableRaisingEvents = true
                };

                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();

                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        outputBuilder.AppendLine(args.Data);
                        logger?.Invoke(args.Data); // Log em tempo real
                    }
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        errorBuilder.AppendLine(args.Data);
                        logger?.Invoke($"ERROR: {args.Data}"); // Log de erros em tempo real
                    }
                };

                logger?.Invoke($"Executando: {fileName} {arguments}");

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                return new ProcessResult(process.ExitCode, outputBuilder.ToString(), errorBuilder.ToString());
            });
        }
    }
}