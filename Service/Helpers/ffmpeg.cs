using System;
using System.Diagnostics;

namespace Service.Helpers
{
    public static class Ffmpeg
    {
        public static void ConvertFile(string newFile)
        {
            string command = $"ffmpeg - i {newFile} - sample_fmt s16 - ar 48000 {newFile + "Converted"}";
            ExecuteBashCommand(command);
        }

        private static string ExecuteBashCommand(string command)
        {
            try
            {
                command = command.Replace("\"", "\"\"");

                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = "-c \"" + command + "\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                proc.Start();
                proc.WaitForExit();

                return proc.StandardOutput.ReadToEnd();
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
