using Models.BackEnd;
using System;
using HtmlAgilityPack;
using RestSharp;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Service.GeniusAPI.Models;
using System.Linq;
using Database;
using System.Drawing;
using System.IO;

namespace Service.Helpers
{
    public class Genius
    {
        private Song _song { get; set; }

        private HtmlDocument _doc { get; set; }

        private string _url { get; set; }

        public Genius(Song song)
        {
            _song = song ?? throw new Exception("Song can't be null");
        }

        public async Task StartAsync(SpotyPieIDbContext _ctx)
        {
            var songImg = await GetHtmlCodeAsync();

            if (songImg.Response?.Sections != null && songImg.Response.Sections.Count > 0)
            {
                if (songImg.Response.Sections.First().Type == "song")
                {
                    var section = songImg.Response.Sections[0];
                    if (section.Hits != null && section.Hits.Count > 0)
                    {
                        var hit = section.Hits.First();
                        if (hit.Result != null)
                        {
                            //Checking song name
                            var result = hit.Result;
                            if (result.Title.Contains(_song.Name) ||
                               _song.Name.Contains(result.Title) ||
                               result.TitleWithFeatured.Contains(_song.Name) ||
                               _song.Name.Contains(result.TitleWithFeatured)
                            )
                            {
                                //Checking song artist name
                                if (result.PrimaryArtist != null &&
                                    result.PrimaryArtist.Name.Contains(_song.ArtistName) ||
                                    _song.ArtistName.Contains(result.PrimaryArtist.Name))
                                {
                                    await FormatImage(_ctx, result);
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task FormatImage(SpotyPieIDbContext _ctx, string url)
        {
            try
            {
                Bitmap image = DownloadImage(url);
            }
            catch (Exception e)
            {

            }
        }

        public Bitmap DownloadImage(string url)
        {
            RestClient client = new RestClient($"{url}");
            RestRequest request = new RestRequest(Method.GET);
            byte[] imageBytes = client.DownloadData(request);

            Bitmap bmp;
            using (var ms = new MemoryStream(imageBytes))
            {
                bmp = new Bitmap(ms);
            }

            return bmp;
        }

        private async Task<GeniusBase> GetHtmlCodeAsync()
        {
            string songName = HttpUtility.UrlEncode(_song.Name);
            RestClient client = new RestClient($"https://genius.com/api/search/multi?per_page=1&q={songName}");
            RestRequest request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<GeniusBase>(response.Content);
            }
            else
            {
                throw new Exception("Cant downlaod page");
            }
        }
    }
}
