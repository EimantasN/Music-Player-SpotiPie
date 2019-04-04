﻿using Mobile_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mobile_Api.Models
{
    public class Artist : BaseModel
    {
        public  override int Id { get; set; }

        public string Name { get; set; }

        //[Json] List<string>
        public string Genres { get; set; }

        public List<Image> Images { get; set; }

        public List<Song> Songs { get; set; }

        public List<Album> Albums { get; set; }

        public long Popularity { get; set; }
    }
}
