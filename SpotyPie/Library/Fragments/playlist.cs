using System.Collections.Generic;
using Android.Support.V7.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.RecycleView;

namespace SpotyPie.Library.Fragments
{
    public class PlaylistFragment : FragmentBase
    {
        private List<Playlist> PlaylistLocal;
        private RvList<Playlist> PlaylistsData;
        //private RecyclerView.LayoutManager PlaylistSongsLayoutManager;
        //private RecyclerView.Adapter PlaylistSongsAdapter;
        //private RecyclerView PlaylistsSongsRecyclerView;

        public override int LayoutId { get; set; } = Resource.Layout.library_playlist_layout;

        protected override void InitView()
        {
        }

        public override void ForceUpdate()
        {
        }

        public override void ReleaseData()
        {
        }
    }
}