﻿using Mobile_Api.Models.Enums;
using Realms;

namespace Mobile_Api.Models
{
    public class Image : BaseModel
    {
        public override int Id { get; set; }
        public string Url { get; set; }

        public string LocalUrl { get; set; }

        public string Base64 { get; set; }

        public long Height { get; set; }

        public long Width { get; set; }

        protected override RvType Type { get; set; }
    }

}
