using System;
using System.Collections.Generic;
using System.Text;

namespace Mobile_Api.Models
{
    public class Image : BaseModel
    {
        public override int Id { get; set; }

        public string Url { get; set; }

        //public string LocalUrl { get; set; }

        public long Height { get; set; }

        public long Width { get; set; }
    }

}
