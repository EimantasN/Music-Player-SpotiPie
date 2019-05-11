using System;
using Android.Content.Res;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;

namespace SpotyPie.RecycleView.Models
{
    public class SongItem : RecyclerView.ViewHolder
    {
        public View EmptyTimeView { get; set; }

        public Action Action { get; set; }

        public ImageView SmallIcon { get; set; }

        public TextView Title { get; set; }

        public TextView SubTitile { get; set; }

        public ImageButton Options { get; set; }

        public SongItem(View view, ViewGroup parent, Action action) : base(view)
        {
            Action = action;
            EmptyTimeView = view;

            Title = view.FindViewById<TextView>(Resource.Id.Title);
            SubTitile = view.FindViewById<TextView>(Resource.Id.subtitle);
            Options = view.FindViewById<ImageButton>(Resource.Id.option);
            SmallIcon = view.FindViewById<ImageView>(Resource.Id.small_img);

            IsRecyclable = true;

            Options.Click += Options_Click1;
        }

        private void Options_Click1(object sender, EventArgs e)
        {
            Action?.Invoke();
        }

        internal void PrepareView(Songs t)
        {
            if (t.Id == Current_state.Id)
            {
                SmallIcon.SetImageResource(Resource.Drawable.music_pause_small);
                Title.SetTextColor(ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#1db954")));
                SubTitile.SetTextColor(ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#1db954")));
            }
            else
            {
                SmallIcon.SetImageResource(Resource.Drawable.music_note_small);
            }
            Title.Text = t.Name;
            SubTitile.Text = t.ArtistName;
        }
    }
}