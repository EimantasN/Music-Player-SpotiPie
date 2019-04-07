using Models.BackEnd;
using System.Collections.Generic;

namespace Service.Helpers
{
    public static class FindSimilarName
    {
        public static float Barrier = 0.85f;

        public static int findSimilarSongName(List<Song> songs, string title)
        {
            double maxSimilarity = 0;
            int Id = 0;
            double temp;
            foreach (var x in songs)
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

    }
}
