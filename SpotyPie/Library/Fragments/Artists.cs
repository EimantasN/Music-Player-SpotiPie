using System.Threading.Tasks;
using Android.Support.V7.Widget;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.RecycleView;

namespace SpotyPie.Library.Fragments
{
    public class Artists : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.library_artist_layout;

        private RvList<Artist> RvData { get; set; }

        protected override void InitView()
        {
            if (RvData == null)
            {
                var rvBase = new BaseRecycleView<Artist>(this, Resource.Id.artists);
                RvData = rvBase.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical);
                rvBase.DisableScroolNested();
            }
        }

        public override void ForceUpdate()
        {
            Task.Run(async () => await GetAPIService().GetAll<Artist>(RvData, null, RvType.ArtistList));
        }
    }
}