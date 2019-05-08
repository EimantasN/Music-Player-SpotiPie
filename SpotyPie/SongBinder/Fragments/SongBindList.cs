using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.RecycleView;
using System.Threading.Tasks;

namespace SpotyPie.SongBinder.Fragments
{
    public class SongBindList : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.song_bind_list;

        private BaseRecycleView<Songs> Songs { get; set; }

        protected override void InitView()
        {
        }

        public override void ForceUpdate()
        {
            if (Songs == null)
            {
                Songs = new BaseRecycleView<Songs>(this, Resource.Id.song_list);
                Songs.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical);
            }
            Task.Run(() => ParentActivity.GetAPIService().GetAll<Songs>(Songs.GetData(), null, RvType.SongBindList));
        }

        public override void ReleaseData()
        {
            if (Songs != null)
            {
                Songs.Dispose();
                Songs = null;
            }
        }
    }
}