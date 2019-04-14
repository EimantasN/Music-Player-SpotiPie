using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.RecycleView;
using Square.Picasso;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotyPie.Library.Fragments
{
    public class Albums : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.library_album_layout;

        private RvList<dynamic> RvData { get; set; }

        protected override void InitView()
        {
            if (RvData == null)
            {
                var rvBase = new BaseRecycleView<dynamic>(this, Resource.Id.albums);
                RvData = rvBase.Setup(LinearLayoutManager.Vertical);
                rvBase.DisableScroolNested();
            }
        }

        public async Task LoadAllAlbums()
        {
            List<dynamic> data = new List<dynamic>() { null };
            RvData.AddList(data);
            var api = (AlbumService)GetService(ApiServices.Albums);

            data.AddRange(await api.GetAll());

            List<dynamic> newlist = new List<dynamic>();
            var Album = new Album();
            for (int i = 1; i < data.Count; i++)
            {
                Album = data[i];
                Album.SetModelType(RvType.AlbumList);
                newlist.Add(Album);
            }

            RvData.AddList(newlist);
            RvData.RemoveLoading(data);
        }

        public override void ForceUpdate()
        {
            Task.Run(() => LoadAllAlbums());
        }
    }
}