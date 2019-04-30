using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Helpers
{
    public static class EnviromentPath
    {
        private static string GetBase()
        {
            if (!Environment.OSVersion.ToString().Contains("W"))
            {
                return
                    System.IO.Path.DirectorySeparatorChar + "root" +
                    System.IO.Path.DirectorySeparatorChar + "Content";
            }
            else
            {
                return Environment.CurrentDirectory;
            }
        }

        public static string GetEnviromentPathMusic()
        {
            if (!Environment.OSVersion.ToString().Contains("W"))
            {
                return GetBase() + System.IO.Path.DirectorySeparatorChar + "Flac";
            }
            else
            {
                return GetBase() + System.IO.Path.DirectorySeparatorChar + "Music";
            }
        }

        public static string GetEnviromentPathImages()
        {
            if (!Environment.OSVersion.ToString().Contains("W"))
            {
                return GetBase() + System.IO.Path.DirectorySeparatorChar + "Images";

            }
            else
            {
                return GetBase() + System.IO.Path.DirectorySeparatorChar + "Images";
            }
        }
    }
}
