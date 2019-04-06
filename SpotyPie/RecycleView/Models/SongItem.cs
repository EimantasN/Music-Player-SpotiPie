using System;
using Android.Content;
using Android.Content.Res;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace SpotyPie.RecycleView.Models
{
    public class SongItem : RecyclerView.ViewHolder
    {
        public View EmptyTimeView { get; set; }

        public ImageView SmallIcon { get; set; }

        public TextView Title { get; set; }

        public TextView SubTitile { get; set; }

        public ImageButton Options { get; set; }

        public SongItem(View view) : base(view) { }

        internal void PrepareView(dynamic t, Context context)
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
            SubTitile.Text = "Coming soon";
        }
    }
}