using Android.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.RecycleView;
using System.Collections.Generic;

namespace SpotyPie
{
    public class SongOptionsFragment : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.song_option_layout;

        private ImageView SongImage;
        private TextView SongTitle;
        private TextView SongArtist;

        private BaseRecycleView<SongOptions> RvData { get; set; }

        protected override void InitView()
        {
            SongImage = RootView.FindViewById<ImageView>(Resource.Id.song_image);
            SongTitle = RootView.FindViewById<TextView>(Resource.Id.song_title);
            SongArtist = RootView.FindViewById<TextView>(Resource.Id.song_artist);
        }

        public override void ForceUpdate()
        {
            if (RvData == null)
            {
                RvData = new BaseRecycleView<SongOptions>(this, Resource.Id.song_option_rv);
                RvData.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical);
                RvData.DisableScroolNested();
            }

            List<SongOptions> Options = new List<SongOptions>()
            {
                new SongOptions() { Id = 1, ItemType = Mobile_Api.Models.Enums.SongOptions.Like, Value = "Like"},
                new SongOptions() { Id = 2, ItemType = Mobile_Api.Models.Enums.SongOptions.HideSong, Value = "Hide this song"},
                new SongOptions() { Id = 3, ItemType = Mobile_Api.Models.Enums.SongOptions.AddToPlaylist, Value = "Add to Playlist"},
                new SongOptions() { Id = 4, ItemType = Mobile_Api.Models.Enums.SongOptions.ViewArtist, Value = "View Artist"},
                new SongOptions() { Id = 5, ItemType = Mobile_Api.Models.Enums.SongOptions.Share, Value = "Share"},
                new SongOptions() { Id = 6, ItemType = Mobile_Api.Models.Enums.SongOptions.ReportError, Value = "Report Song Content"},
                new SongOptions() { Id = 7, ItemType = Mobile_Api.Models.Enums.SongOptions.ShowCredits, Value = "Show Credits"}
            };

            RvData.GetData().AddList(Options);
        }

        public override void ReleaseData()
        {
        }

        public override int GetParentView()
        {
            throw new System.NotImplementedException();
        }

        public override void LoadFragment(dynamic switcher)
        {
            throw new System.NotImplementedException();
        }
    }
}