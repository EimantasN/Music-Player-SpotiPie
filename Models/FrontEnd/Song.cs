﻿using Models.BackEnd;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Models.FrontEnd
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artists { get; set; }

        public Song() { }

        public Song(int id, string title, string artistJson)
        {
            Id = id;
            Title = title;
            Artists = JsonConvert.DeserializeObject<List<Artist>>(artistJson)[0].Name;
        }

        public static List<Song> ConvertList(List<Item> data)
        {
            List<Song> songs = new List<Song>();
            data.ForEach(x => songs.Add(new Song(x.Id, x.Name, x.Artists)));
            return songs;
        }
    }
}
