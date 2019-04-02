using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SpotyPie.Models
{
    public class BlockWithImage
    {
        public int Id { get; set; }
        public RvType Type { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Image { get; set; }

        public BlockWithImage()
        {

        }

        public BlockWithImage(bool fake)
        {
            Title = "Title";
            SubTitle = "SubTitle";
        }

        public BlockWithImage(int id, RvType type, string title, string subtitle, string url)
        {
            Id = id;
            Type = type;
            Title = title;
            SubTitle = subtitle;
            Image = url;
        }
    }

    public class TwoBlockWithImage
    {
        public bool Full { get; set; }
        public BlockWithImage Left { get; set; }
        public BlockWithImage Right { get; set; }

        public TwoBlockWithImage()
        {

        }

        public TwoBlockWithImage(BlockWithImage left, BlockWithImage right)
        {
            Full = true;
            Left = left;
            Right = right;
        }

        public TwoBlockWithImage(BlockWithImage left)
        {
            Full = false;
            Left = left;
        }
    }

    public enum RvType
    {
        Artist,
        Album,
        Playlist
    }
}