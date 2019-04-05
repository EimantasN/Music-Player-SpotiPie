//using Models.BackEnd;
//using Newtonsoft.Json;
//using System.Collections.Generic;

//namespace Models.FrontEnd
//{
//    public class Song
//    {
//        public int Id { get; set; }
//        public string Title { get; set; }
//        public string Artists { get; set; }

//        public Song() { }

//        public Song(int id, string title, string artistJson)
//        {
//            Id = id;
//            Title = title;
//            Artists = JsonConvert.DeserializeObject<List<string>>(artistJson)[0];
//        }

//        public static List<Song> ConvertList(List<BackEnd.Song> data)
//        {
//            List<Song> songs = new List<Song>();
//            return songs;
//        }
//    }
//}
