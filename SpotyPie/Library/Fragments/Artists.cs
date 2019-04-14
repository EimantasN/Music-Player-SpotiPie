using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.RecycleView;
using Square.Picasso;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Library.Fragments
{
    public class Artists : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.library_artist_layout;

        private RvList<dynamic> RvData { get; set; }

        protected override void InitView()
        {
            if (RvData == null)
            {
                var rvBase = new BaseRecycleView<dynamic>(this, Resource.Id.artists);
                RvData = rvBase.Setup(LinearLayoutManager.Vertical);
                rvBase.DisableScroolNested();
            }
        }

        public async Task LoadAllAlbums()
        {
            List<dynamic> data = new List<dynamic>() { null };
            RvData.AddList(data);
            var api = (ArtistService)GetService(ApiServices.Artist);

            data.AddRange(await api.GetAll());

            List<dynamic> newlist = new List<dynamic>();
            var Album = new Artist();
            for (int i = 1; i < data.Count; i++)
            {
                Album = data[i];
                Album.SetModelType(RvType.ArtistList);
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