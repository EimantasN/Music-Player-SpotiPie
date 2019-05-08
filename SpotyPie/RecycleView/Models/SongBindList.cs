using System;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;

namespace SpotyPie.RecycleView.Models
{
    public class SongBindList : RecyclerView.ViewHolder
    {
        public View EmptyTimeView { get; set; }

        public TextView Title { get; set; }

        public TextView Album { get; set; }

        public TextView Artist { get; set; }

        //public ImageButton Options { get; set; }

        public SongBindList(View view, ViewGroup parent) : base(view)
        {
            Title = view.FindViewById<TextView>(Resource.Id.song);
            Album = view.FindViewById<TextView>(Resource.Id.album);
            Artist = view.FindViewById<TextView>(Resource.Id.artist);
            //Options = view.FindViewById<ImageButton>(Resource.Id.option);
        }


        internal void PrepareView(SongTag song, Context context)
        {
            Title.Text = song.Title;
            Album.Text = song.Album;
            Artist.Text = song.Artist;
        }
    }
}