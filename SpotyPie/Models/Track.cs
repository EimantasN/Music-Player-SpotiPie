using Mobile_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotyPie
{
    public class Tracks
    {
        public int Id { get; set; }
        public List<Item> Items { get; set; }

        public long Total { get; set; }
    }
}
