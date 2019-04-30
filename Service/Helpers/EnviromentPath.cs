using Models.BackEnd;
using System;
using System.IO;
using System.Linq;

namespace Service.Helpers
{
    public enum Content
    {
        Music,
        Images
    }

    public static class EnviromentPath
    {
        private static string GetBase()
        {
            if (!Environment.OSVersion.ToString().Contains("W"))
            {
                //Not adding checks for direcotry because is a 
                //wrong place and it will be needed to move everything to another place
                return
                    Path.DirectorySeparatorChar + "root" +
                    Path.DirectorySeparatorChar + "Content";
            }
            else
            {
                return Environment.CurrentDirectory;
            }
        }

        public static string GetEnviromentPathMusic()
        {
            string path;
            if (!Environment.OSVersion.ToString().Contains("W"))
            {
                path = GetBase() + System.IO.Path.DirectorySeparatorChar + "Flac";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
            else
            {
                path = GetBase() + System.IO.Path.DirectorySeparatorChar + "Music";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string GetEnviromentPathImages()
        {
            //Not mergin in one to leave space for additional folder that may be needed
            string path;
            if (!Environment.OSVersion.ToString().Contains("W"))
            {
                path = GetBase() + System.IO.Path.DirectorySeparatorChar + "Images";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;

            }
            else
            {
                path = GetBase() + System.IO.Path.DirectorySeparatorChar + "Images";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        public static void CheckForDirectory(string path, Content content)
        {
            string tempPath;
            if (content == Content.Images)
            {
                tempPath = GetEnviromentPathImages();
                path = path.Replace(GetEnviromentPathImages(), "");
            }
            else
            {
                tempPath = GetEnviromentPathMusic();
                path = path.Replace(GetEnviromentPathMusic(), "");
            }

            string[] directorys = path.Split(Path.DirectorySeparatorChar);
            foreach (var x in directorys)
            {
                if (!string.IsNullOrEmpty(x))
                {
                    tempPath += Path.DirectorySeparatorChar + x;
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);
                }
            }
        }

        public static string GetSongImgDestinationPath(Song song, int width, int height, string imgUrl)
        {
            var path = EnviromentPath.GetEnviromentPathImages() +
                Path.DirectorySeparatorChar +
                Replacer.RemoveSpecialCharacters(song.ArtistName.ToLower().Trim()) +
                Path.DirectorySeparatorChar +
                Replacer.RemoveSpecialCharacters(song.AlbumName) +
                Path.DirectorySeparatorChar +
                $"{width}x{height}" +
                Path.DirectorySeparatorChar;

            CheckForDirectory(path, Content.Images);

            path = path + Path.GetRandomFileName().Split(".").First() + "." + imgUrl.Split(".").Last();
            while (File.Exists(path))
            {
                path = path + Path.GetRandomFileName() + "." + imgUrl.Split(".").Last();
            }

            return path;// not nice but it will work
        }

    }
}
