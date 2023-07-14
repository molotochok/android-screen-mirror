using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AndroidScreenMirror
{
    class ScreenCopier : IDisposable
    {
        private readonly Process _process;

        private ScreenCopier()
        {
            _process = CreateProcess();
        }

        public static ScreenCopier Create()
        {
            return new ScreenCopier();
        }

        public void Dispose()
        {
            try
            {
                if (!_process.HasExited)
                    _process.Kill();
            } 
            catch (Exception) {} 
            finally
            {
                _process.Dispose();
            }
        }

        public async Task<RunProcessStatus> RunAsync()
        {
            _process.Start();

            await _process.WaitForExitAsync();

            return new RunProcessStatus
            {
                Output = await _process.StandardOutput.ReadToEndAsync(),
                Error = await _process.StandardError.ReadToEndAsync()
            };
        }

        public async Task StopAsync()
        {
            _process.Kill();
            await _process.WaitForExitAsync();
        }

        private static Process CreateProcess()
        {
            const string processName = "scrcpy";

            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, processName);

            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = directory,
                    FileName = Path.Combine(directory, $"{processName}.exe"),
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Verb = "runas",
                },
            };
        }

        public class RunProcessStatus
        {
            public string Output { get; set; }
            public string Error { get; set; }
        }
    }
}
