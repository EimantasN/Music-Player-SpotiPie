using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.RecycleView;

namespace SpotyPie.Library.Fragments
{
    public class PlaylistFragment : FragmentBase
    {
        public List<Playlist> PlaylistLocal;
        public RvList<Playlist> PlaylistsData;
        private RecyclerView.LayoutManager PlaylistSongsLayoutManager;
        private RecyclerView.Adapter PlaylistSongsAdapter;
        private RecyclerView PlaylistsSongsRecyclerView;

        public override int LayoutId { get; set; } = Resource.Layout.library_playlist_layout;

        public override void OnResume()
        {
            base.OnResume();
        }

        public override void OnStop()
        {
            base.OnStop();
        }

        public override void ForceUpdate()
        {
        }

        protected override void InitView()
        {
            //RV ID Resource.Id.playlist
        }
    }
}