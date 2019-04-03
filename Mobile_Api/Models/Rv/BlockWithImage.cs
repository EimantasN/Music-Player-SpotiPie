using Mobile_Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mobile_Api.Models.Rv
{
    public class BlockWithImage
    {
        public int Id { get; set; }
        public RvType Type { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Image { get; set; }
    }
}
