using System.Collections.Generic;
using Android.Support.V7.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.RecycleView;

namespace SpotyPie.Library.Fragments
{
    public class PlaylistFragment : FragmentBase
    {
        private List<Playlist> PlaylistLocal;
        private ThreadSafeRvList<Playlist> PlaylistsData;
        //private RecyclerView.LayoutManager PlaylistSongsLayoutManager;
        //private RecyclerView.Adapter PlaylistSongsAdapter;
        //private RecyclerView PlaylistsSongsRecyclerView;

        public override int LayoutId { get; set; } = Resource.Layout.library_playlist_layout;

        protected override Enums.LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Default;

        protected override void InitView()
        {
        }

        public override void ForceUpdate()
        {
        }

        public override void ReleaseData()
        {
        }

        public override int GetParentView()
        {
            throw new System.NotImplementedException();
        }

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            return null;
        }
    }
}