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
            var api = GetService();

            List<Album> Albums = await api.GetAll<Album>();
            data.AddRange(Albums);

            List<dynamic> newlist = new List<dynamic>();
            var Album = new Album();
            for (int i = 1; i < data.Count; i++)
            {
                Album = data[i];
                Album.SetModelType(RvType.AlbumList);
                newlist.Add(Album);
            }

            RvData.AddList(newlist);
        }

        public override void ForceUpdate()
        {
            Task.Run(() => LoadAllAlbums());
        }
    }
}