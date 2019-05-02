using Android.Support.V7.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.RecycleView;

namespace SpotyPie.Player
{
    public class PlaylistSongList : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.player_song_list;

        private BaseRecycleView<Songs> RvData { get; set; }

        protected override void InitView()
        {
        }

        public void Update()
        {
            RvData.GetData().AddList(GetState().Current_Song_List);
        }

        public override void ForceUpdate()
        {
            if (RvData == null)
            {
                RvData = new BaseRecycleView<Songs>(this, Resource.Id.song_list);
                RvData.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical);
                RvData.DisableScroolNested();
            }
            Update();
        }

        public override void ReleaseData()
        {
            if (RvData != null)
            {
                RvData.Dispose();
                RvData = null;
            }
        }
    }
}