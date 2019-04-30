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
using Microsoft.EntityFrameworkCore;

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
                var section = songImg.Response.Sections[0];
                if (section.Hits != null && section.Hits.Count > 0 && section.Hits.Any(x => x.Type == "song"))
                {
                    var hit = section.Hits.First(x => x.Type == "song");
                    if (hit.Result != null)
                    {
                        //Checking song name
                        var result = hit.Result;
                        if (result.Title.ToLower().Contains(_song.Name.ToLower()) ||
                           _song.Name.ToLower().Contains(result.Title.ToLower()) ||
                           result.TitleWithFeatured.ToLower().Contains(_song.Name.ToLower()) ||
                           _song.Name.ToLower().Contains(result.TitleWithFeatured.ToLower())
                        )
                        {
                            //Checking song artist name
                            if (result.PrimaryArtist != null &&
                                result.PrimaryArtist.Name.ToLower().Contains(_song.ArtistName.ToLower()) ||
                                _song.ArtistName.ToLower().Contains(result.PrimaryArtist.Name.ToLower()))
                            {
                                await FormatImage(_ctx, result.HeaderImageThumbnailUrl.ToString());
                                await FormatImage(_ctx, result.HeaderImageUrl.ToString());
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
            else
            {

            }
        }

        public async Task FormatImage(SpotyPieIDbContext _ctx, string url)
        {
            try
            {
                //It setted from because it is called two time, then i need only one time
                //TODO fix this and optimize database calls
                var song = _ctx.Songs.Include(x => x.Images).FirstOrDefault(x => x.Id == _song.Id);
                if (song == null)
                    throw new Exception("can't find song");

                if (song.Images == null || song.Images.Count == 0 || !song.Images.Any(x => x.Url == url))// || song.Images.Any(x => x.LocalUrl.Contains("C:")))
                {
                    var img = await _ctx.Images.FirstOrDefaultAsync(x => x.Url == url);
                    if (img == null)
                    {
                        string base64;
                        int width;
                        int height;
                        DownloadImage(url, out base64, out width, out height);
                        var image = new Models.BackEnd.Image
                        {
                            Id = 0,
                            Base64 = base64,
                            Height = height,
                            Width = width,
                            Url = url,
                            LocalUrl = EnviromentPath.GetSongImgDestinationPath(_song, width, height, url)
                        };
                        song.Images.Add(image);
                        _ctx.Entry(song).State = EntityState.Modified;
                        await _ctx.SaveChangesAsync();
                    }
                    else
                    {
                        var image = new Models.BackEnd.Image
                        {
                            Id = 0,
                            Base64 = img.Base64,
                            Height = img.Height,
                            Width = img.Width,
                            Url = img.Url,
                            LocalUrl = img.LocalUrl
                        };
                        song.Images.Add(image);
                        _ctx.Entry(song).State = EntityState.Modified;
                        await _ctx.SaveChangesAsync();
                    }
                }
                else
                {
                    //Song already exists
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void DownloadImage(string url, out string base64, out int width, out int height)
        {
            try
            {
                RestClient client = new RestClient($"{url}");
                RestRequest request = new RestRequest(Method.GET);
                byte[] imageBytes = client.DownloadData(request);

                if (imageBytes == null || imageBytes.Length == 0)
                    throw new Exception("Failed to download image");

                base64 = Convert.ToBase64String(imageBytes);
                string base64Img = base64;

                //Getting bitmap fro img dimensions
                Bitmap bmp;
                using (var ms = new MemoryStream(imageBytes))
                {
                    bmp = new Bitmap(ms);
                }
                width = bmp.Width;
                height = bmp.Height;

                if (width == 0 && height == 0)
                    throw new Exception("Failed to get img dimensions");

                //Saving file
                string filePath = EnviromentPath.GetSongImgDestinationPath(_song, width, height, url);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    stream.Write(imageBytes);
                }

                if (!File.Exists(filePath))
                    throw new Exception("Failed to create file");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task<GeniusBase> GetHtmlCodeAsync()
        {
            string artistName = HttpUtility.UrlEncode(_song.ArtistName);
            RestClient client = new RestClient($"https://genius.com/api/search/multi?per_page=1&q={GetFormatedSongName()} {artistName}");
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

        public string GetFormatedSongName()
        {
            string name = _song.Name;
            if (name.Contains("- Live"))
            {
                name = name.Split("- Live").First();
            }
            if (name.Contains("[") && name.First() != '[')
            {
                name = name.Split("[").First();
            }
            return HttpUtility.UrlEncode(name);
        }
    }
}
