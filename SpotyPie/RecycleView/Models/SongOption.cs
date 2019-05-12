using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models.Enums;

namespace SpotyPie.RecycleView.Models
{
    public class SongOption : RecyclerView.ViewHolder
    {
        public ImageView SmallIcon { get; set; }

        public TextView Title { get; set; }

        public SongOption(View view, ViewGroup parent) : base(view)
        {
            SmallIcon = view.FindViewById<ImageView>(Resource.Id.option_image);
            Title = view.FindViewById<TextView>(Resource.Id.title);
        }

        internal void PrepareView(Mobile_Api.Models.SongOptions t)
        {
            SmallIcon.SetImageResource(GetIcon(t.ItemType));
            Title.Text = t.Value;
        }

        public int GetIcon(SongOptions option)
        {
            switch (option)
            {
                case SongOptions.Like:
                    return Resource.Drawable.song_love;
                case SongOptions.Unlike:
                    return Resource.Drawable.song_unlove;
                case SongOptions.HideSong:
                    return Resource.Drawable.not_visible_song;
                case SongOptions.AddToPlaylist:
                    return Resource.Drawable.add_song_interface_symbol;
                case SongOptions.ViewArtist:
                    return Resource.Drawable.artist_icon;
                case SongOptions.ReportError:
                    return Resource.Drawable.report;
                case SongOptions.ShowCredits:
                    return Resource.Drawable.group;
                case SongOptions.Share:
                    return Resource.Drawable.share;
            }
            throw new Exception("Image not found");
        }
    }
}