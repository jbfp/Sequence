using System.Diagnostics;

namespace Sequence.Test.Postgres
{
    internal static class ShellHelper
    {
        public static string Bash(this string command)
        {
            var escapedArgs = command.Replace("\"", "\\\"", System.StringComparison.Ordinal);

            var startInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            string result;

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
                result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }

            return result;
        }
    }
}
