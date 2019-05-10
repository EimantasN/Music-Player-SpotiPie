using Database;
using Models.BackEnd;
using System.Collections.Generic;
using System.Linq;

namespace Service.Helpers
{
    public static class FindSimilarName
    {
        public static float Barrier = 0.85f;

        public static int findSimilarSongName(SpotyPieIDbContext _ctx, List<Song> songs, string title, int albumId, int artistId)
        {
            double maxSimilarity = 0;
            int Id = 0;
            double temp;
            title = title.ToLower().Trim();
            foreach (var x in songs)
            {
                temp = StringSimilarity.CalculateSimilarity(x.Name.ToLower().Trim(), title);
                if (artistId == x.ArtistId)
                {
                    temp += mathWords(title, x.Name.ToLower().Trim());
                    temp += 0.15f;
                    if (albumId == x.AlbumId)
                    {
                        temp += 0.30f;
                    }
                }

                if (temp > maxSimilarity)
                {
                    Id = x.Id;
                    maxSimilarity = temp;
                }
            }

            if (maxSimilarity > Barrier)
            {
                return Id;
            }
            else
            {
                return 0;
            }
        }

        public static float mathWords(string file, string db)
        {
            float similar = 0;
            List<string> filedata = file.Split(' ').ToList();
            List<string> dbdata = file.Split(' ').ToList();
            foreach (var x in filedata)
            {
                if (x.Length > 3)
                {
                    if (dbdata.Any(y => y == x))
                    {
                        similar += 2.5f;
                    }
                }
            }
            return similar;
        }

        public static int findSimilarAlbumName(List<Album> album, string title)
        {
            double maxSimilarity = 0;
            int Id = 0;
            double temp;
            foreach (var x in album)
            {
                temp = StringSimilarity.CalculateSimilarity(x.Name.ToLower().Trim(), title.ToLower().Trim());
                if (temp > maxSimilarity)
                {
                    Id = x.Id;
                    maxSimilarity = temp;
                }
            }

            if (maxSimilarity > Barrier)
            {
                return Id;
            }
            else
            {
                return 0;
            }
        }

        public static bool CheckSimilarity(string query, string value, int cof)
        {
            double temp = StringSimilarity.CalculateSimilarity(query.ToLower().Trim(), value.ToLower().Trim());
            temp += (double)(cof * 0.01f);

            if (temp >= Barrier)
            {
                return true;
            }
            return false;
        }
    }
}
