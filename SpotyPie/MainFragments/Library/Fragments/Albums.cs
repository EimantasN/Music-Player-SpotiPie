using Android.Support.V7.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.RecycleView;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotyPie.Library.Fragments
{
    public class Albums : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.library_album_layout;

        protected override Enums.LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Default;

        private BaseRecycleView<Album> RvData { get; set; }

        protected override void InitView()
        {
        }

        public override void ForceUpdate()
        {
            if (RvData == null)
            {
                RvData = new BaseRecycleView<Album>(this, Resource.Id.albums);
                RvData.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical);
                RvData.DisableScroolNested();
            }

            Task.Run(async () => await GetAPIService().GetAll<Album>(RvData.GetData(), null, RvType.AlbumList));
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