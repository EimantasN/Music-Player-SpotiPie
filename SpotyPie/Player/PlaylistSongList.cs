using Android.Support.Constraints;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.RecycleView;

namespace SpotyPie.Player
{
    public class PlaylistSongList : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.player_song_list;

        private BaseRecycleView<Songs> RvData { get; set; }

        private FrameLayout SongOptionFragmentLayout;

        protected override void InitView()
        {
        }

        public override int GetParentView()
        {
            return Resource.Id.innerWrapper;
        }

        private void Update()
        {
            RvData.GetData().AddList(GetState().Current_Song_List);
        }

        public override void ForceUpdate()
        {
            if (RvData == null)
            {
                RvData = new BaseRecycleView<Songs>(this, Resource.Id.song_list);
                RvData.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical, () => { LoadFragmentInner(Enums.Activitys.Player.SongDetails); });
                RvData.DisableScroolNested();
            }
            Update();
        }

        //private void LoadOptionFragment()
        //{
        //    CheckForLayout();
        //    if (SongOptionFragmentLayout == null)
        //    {
        //        SongOptionFragmentLayout = new FrameLayout(this.Context);

        //        SongOptionFragmentLayout.LayoutParameters = new ConstraintLayout.LayoutParams(
        //            ConstraintLayout.LayoutParams.MatchParent,
        //            ConstraintLayout.LayoutParams.MatchParent);

        //        SongOptionFragmentLayout.Id = 10;
        //        ParentView.AddView(SongOptionFragmentLayout);

        //        SongOptionsFragment SongOptionFragment = new SongOptionsFragment();
        //        ChildFragmentManager.BeginTransaction()
        //                .Replace(SongOptionFragmentLayout.Id, SongOptionFragment)
        //                .Commit();
        //    }
        //}

        public bool CheckForLayout()
        {
            return true;
        }

        public override void ReleaseData()
        {
            if (RvData != null)
            {
                RvData.Dispose();
                RvData = null;
            }
        }

        public override void LoadFragment(dynamic switcher)
        {
            CurrentFragment = new SongOptionsFragment();
        }
    }
}