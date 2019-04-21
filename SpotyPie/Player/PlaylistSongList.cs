using Android.Support.V7.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.RecycleView;

namespace SpotyPie.Player
{
    public class PlaylistSongList : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.player_song_list;

        private RvList<Songs> RvData { get; set; }

        protected override void InitView()
        {
            //
            if (RvData == null)
            {
                var rvBase = new BaseRecycleView<Songs>(this, Resource.Id.song_list);
                RvData = rvBase.Setup(LinearLayoutManager.Vertical);
                rvBase.DisableScroolNested();
                Update();
            }
        }

        public void Update()
        {
            RvData.AddList(GetState().Current_Song_List);
        }

        public override void ForceUpdate()
        {
            RvData.AddList(GetState().Current_Song_List);
        }
    }
}