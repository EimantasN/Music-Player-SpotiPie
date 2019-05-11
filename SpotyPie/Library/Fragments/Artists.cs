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

        private BaseRecycleView<Artist> RvData { get; set; }

        protected override void InitView()
        {
        }

        public override void ForceUpdate()
        {
            if (RvData == null)
            {
                RvData = new BaseRecycleView<Artist>(this, Resource.Id.artists);
                RvData.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical);
                RvData.DisableScroolNested();
            }

            Task.Run(async () => await GetAPIService().GetAll<Artist>(RvData.GetData(), null, RvType.ArtistList));
        }

        public override void ReleaseData()
        {
            if (RvData != null)
            {
                RvData.Dispose();
                RvData = null;
            }
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