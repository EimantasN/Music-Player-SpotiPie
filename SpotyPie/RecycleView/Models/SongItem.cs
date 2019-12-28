using Android.Content.Res;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using SpotyPie.Music.Manager;
using System;

namespace SpotyPie.RecycleView.Models
{
    public class SongItem : RecyclerView.ViewHolder
    {
        public Action<Songs> OnSongClickAction { get; set; }

        public Action<Songs> OnSongOptionClickAction { get; set; }

        private Songs Song { get; set; }

        public View SongView { get; set; }

        public ImageView SmallIcon { get; set; }

        public TextView Title { get; set; }

        public TextView SubTitile { get; set; }

        public ImageButton Options { get; set; }

        public SongItem(View view, ViewGroup parent) : base(view)
        {
            SongView = view;
            Title = view.FindViewById<TextView>(Resource.Id.Title);
            SubTitile = view.FindViewById<TextView>(Resource.Id.subtitle);
            Options = view.FindViewById<ImageButton>(Resource.Id.option);
            SmallIcon = view.FindViewById<ImageView>(Resource.Id.small_img);

            SongView.Click += OnSongClick;
            Options.Click += OnSongOptionClick;

            IsRecyclable = true;
        }

        private void OnSongOptionClick(object sender, EventArgs e)
        {
            OnSongOptionClickAction?.Invoke(Song);
        }

        private void OnSongClick(object sender, EventArgs e)
        {
            OnSongClickAction?.Invoke(Song);
        }

        internal void PrepareView(Songs song)
        {
            Song = song;
            if (song.Id == SongManager.SongId)
            {
                SmallIcon.SetImageResource(Resource.Drawable.music_pause_small);
                Title.SetTextColor(ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#1db954")));
                SubTitile.SetTextColor(ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#1db954")));
            }
            else
            {
                SmallIcon.SetImageResource(Resource.Drawable.music_note_small);
                Title.SetTextColor(ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#ffffff")));
                SubTitile.SetTextColor(ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#ffffff")));
            }
            Title.Text = song.Name;
            SubTitile.Text = song.ArtistName;
        }
    }
}