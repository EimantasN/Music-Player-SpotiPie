using System;
using System.Diagnostics;
using System.IO;

namespace Service.Helpers
{
    public static class Ffmpeg
    {
        public static void ConvertFile(string path, string oldName, string name)
        {
            string command = $"ffmpeg -i {path + oldName } -sample_fmt s16 -ar 48000 {path + name}";
            ExecuteBashCommand(command);
        }

        public static void ConvertFile(string newFile)
        {
            string command = $"ffmpeg -i {newFile} -sample_fmt s16 -ar 48000 {newFile.Replace(".flac", "-C_16.flac")}";
            ExecuteBashCommand(command);

            if (File.Exists(newFile.Replace(".flac", "-C_16.flac")))
            {
                File.Copy(newFile.Replace(".flac", "-C_16.flac"), newFile, true);
                //File.Delete(newFile.Replace(".flac", "-C_16.flac"));
            }
            else
            {
                throw new Exception("FFpeg failed");
            }
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
