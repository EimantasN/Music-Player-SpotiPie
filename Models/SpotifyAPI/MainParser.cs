using Models.SpotifyAPI;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SpotifyAPI
{
    public class MainParser
    {
        //12Chz98pHFMPJEknJQMWvI   //Muse ID

        public string MuseId { get; set; } = "12Chz98pHFMPJEknJQMWvI";
        public string Token { get; set; } = "BQCp3y21wpyIPZ8JJQMQIx638G76v2fcLQkqdfn_csimkytqg6ca3R-tl_n4FP1FaaWZOSDxCm6i-zz9y6MKO3MIvVVfKZnl7C4in7Y5jeah59rC9tMAlCpsZRu-jsE3tM7g2jd2pwEl9iHLSm_JspusX1ed3mlO07DXEkBhKv8Z1AEfStUAw2qSqB5WYuS8j2Ks8qSvBa9MN7TZRtRHKR_81o0Sk_4g70s7KRE3L0dyUuGVKj5Om-5Ro6Ve63png6fUpgZy1INVeAA";
        Artist Artist;
        List<Album> Albums;

        public Models.BackEnd.Artist Bind()
        {
            Models.BackEnd.Artist MainArtist = new Models.BackEnd.Artist();
            MainArtist = GetMuse();
            var al = GetAlbums();
            MainArtist.Albums = al;
            SetSongToAlbums(MainArtist.Albums);

            return MainArtist;
        }

        public Models.BackEnd.Artist GetMuse()
        {
            try
            {
                var x = JsonConvert.DeserializeObject<Artist>(GetData("https://api.spotify.com/v1/artists/" + MuseId));
                return new Models.BackEnd.Artist
                {
                    SpotifyId = x.Id,
                    Genres = JsonConvert.SerializeObject(x.Genres),
                    LargeImage = x.Images[0].Url.ToString(),
                    MediumImage = x.Images[1].Url.ToString(),
                    SmallImage = x.Images[2].Url.ToString(),
                    LastActiveTime = DateTime.Now,
                    Name = x.Name,
                    Popularity = 0,
                    Albums = null
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<Models.BackEnd.Album> GetAlbums()
        {
            try
            {
                var spotifyAlbums = JsonConvert.DeserializeObject<AlbumResponse>(GetData($"https://api.spotify.com/v1/artists/{MuseId}/albums")).Albums.Where(x => x.AlbumType == "album").ToList();
                List<Models.BackEnd.Album> Albumsx = new List<Models.BackEnd.Album>();
                foreach (var x in spotifyAlbums)
                {
                    Albumsx.Add(new Models.BackEnd.Album
                    {
                        SpotifyId = x.Id,
                        IsPlayable = false,
                        Songs = null,
                        LargeImage = x.Images[0].Url.ToString(),
                        MediumImage = x.Images[1].Url.ToString(),
                        SmallImage = x.Images[2].Url.ToString(),
                        LastActiveTime = DateTime.Now,
                        Name =x.Name,
                        Popularity = 0,
                        ReleaseDate = x.ReleaseDate
                    });
                }

                return Albumsx;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public void SetSongToAlbums(List<Models.BackEnd.Album> Albumsx)
        {
            try
            {
                List<Models.BackEnd.Song> songs = new List<Models.BackEnd.Song>();
                foreach (var album in Albumsx)
                {
                    songs = new List<Models.BackEnd.Song>();
                    var Spotifysongs = JsonConvert.DeserializeObject<AlbumTrackResponse>(GetData($"https://api.spotify.com/v1/albums/{album.SpotifyId}/tracks")).Songs.Where(x => !x.Name.Contains("(")).ToList();

                    for (int i = 0; i < Spotifysongs.Count; i++)
                    {
                        Spotifysongs[i].LargeImage = album.LargeImage;
                        Spotifysongs[i].MediumImage = album.MediumImage;
                        Spotifysongs[i].SmallImage = album.SmallImage;
                    }
                    foreach (var x in Spotifysongs)
                    {
                        songs.Add(new Models.BackEnd.Song
                        {
                            SpotifyId = x.Id,
                            DiscNumber = x.DiscNumber,
                            DurationMs = x.DurationMs,
                            Explicit = x.Explicit,
                            IsLocal = false,
                            IsPlayable = false,
                            LargeImage = x.LargeImage,
                            LastActiveTime = DateTime.Now,
                            LocalUrl = null,
                            MediumImage = x.MediumImage,
                            Name = x.Name,
                            Popularity = 0,
                            SmallImage = x.SmallImage,
                            TrackNumber =x.TrackNumber
                        });
                    }
                    album.Tracks = songs.Count;
                    album.Songs = songs;
                }
            }
            catch (Exception e)
            {
            }
        }

        public string GetData(string Url)
        {
            var client = new RestClient(Url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Authorization", "Bearer " + Token);
            request.AddParameter("undefined", "undefined=", ParameterType.RequestBody);
            var response = client.Execute(request);
            return response.Content;
        }
    }
}
