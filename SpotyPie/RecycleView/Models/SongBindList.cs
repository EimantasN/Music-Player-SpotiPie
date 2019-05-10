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

        public ImageView DownloadedIcon { get; set; }

        public SongBindList(View view, ViewGroup parent) : base(view)
        {
            Title = view.FindViewById<TextView>(Resource.Id.song);
            Album = view.FindViewById<TextView>(Resource.Id.album);
            Artist = view.FindViewById<TextView>(Resource.Id.artist);
            DownloadedIcon = view.FindViewById<ImageView>(Resource.Id.downloaded_icon);
        }


        internal void PrepareView(SongTag song, Context context)
        {
            Title.Text = song.Title;
            Album.Text = song.Album;
            Artist.Text = song.Artist;
        }

        internal void PrepareView(Songs song, Context context)
        {
            Title.Text = song.Name;
            Album.Text = song.AlbumName;
            Artist.Text = song.ArtistName;

            if (!string.IsNullOrEmpty(song.LocalUrl))
                DownloadedIcon.Visibility = ViewStates.Visible;
        }
    }
}