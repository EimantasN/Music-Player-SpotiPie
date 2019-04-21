using Android.Support.V7.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.RecycleView;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotyPie.Library.Fragments
{
    public class Albums : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.library_album_layout;

        private RvList<Album> RvData { get; set; }

        protected override void InitView()
        {
            if (RvData == null)
            {
                var rvBase = new BaseRecycleView<Album>(this, Resource.Id.albums);
                RvData = rvBase.Setup(LinearLayoutManager.Vertical);
                rvBase.DisableScroolNested();
            }
        }

        public override void ForceUpdate()
        {
            Task.Run(async () => await GetAPIService().GetAll<Album>(RvData, null, RvType.AlbumList));
        }
    }
}