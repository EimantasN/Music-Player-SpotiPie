using Models.BackEnd;
using System;
using HtmlAgilityPack;
using RestSharp;

namespace Service.Helpers
{
    public class ImageGetter
    {
        private Song _song { get; set; }

        private HtmlDocument _doc { get; set; }

        private string _url { get; set; }

        public ImageGetter(Song song)
        {
            _song = song ?? throw new Exception("Song can't be null");
        }

        public void Start()
        {
            _doc = new HtmlDocument();
            _doc.Load(@"C:\Temp\sample.txt");
        }

        private string GetHtmlCode()
        {

            RestClient client = new RestClient("https://genius.com/search?q=Burn%20It%20Down");
        }
    }
}
