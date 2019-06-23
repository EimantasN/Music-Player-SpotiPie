using System;
using System.Collections.Generic;
using Android.Content.Res;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using SpotyPie.Music.Helpers;

namespace SpotyPie.RecycleView.Models
{
    public class SongItem : RecyclerView.ViewHolder
    {
        public View EmptyTimeView { get; set; }

        public int SongId { get; set; }

        public List<Action> Actions { get; set; }

        public ImageView SmallIcon { get; set; }

        public TextView Title { get; set; }

        public TextView SubTitile { get; set; }

        public ImageButton Options { get; set; }

        public SongItem(View view, ViewGroup parent, List<Action> actions) : base(view)
        {
            EmptyTimeView = view;

            Title = view.FindViewById<TextView>(Resource.Id.Title);
            SubTitile = view.FindViewById<TextView>(Resource.Id.subtitle);
            Options = view.FindViewById<ImageButton>(Resource.Id.option);
            SmallIcon = view.FindViewById<ImageView>(Resource.Id.small_img);

            IsRecyclable = true;

            if (actions != null)
            {
                Actions = actions;
                EmptyTimeView.Click += EmptyTimeView_Click;
                Options.Click += Options_Click1;
            }
        }

        private void EmptyTimeView_Click(object sender, EventArgs e)
        {
            QueueHelper.Id = SongId;
            Actions[1]?.Invoke();
        }

        private void Options_Click1(object sender, EventArgs e)
        {
            Actions[0]?.Invoke();
        }

        protected override void Dispose(bool disposing)
        {
            if (Actions != null)
            {
                Actions = null;
                EmptyTimeView.Click -= EmptyTimeView_Click;
                Options.Click -= Options_Click1;
            }
            base.Dispose(disposing);
        }

        internal void PrepareView(Songs t)
        {
            SongId = t.Id;
            if (t.IsPlaying)
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
            Title.Text = t.Name;
            SubTitile.Text = t.ArtistName;
        }
    }
}