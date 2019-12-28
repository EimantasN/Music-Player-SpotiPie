using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.Music.Manager;
using SpotyPie.RecycleView.Adapters;
using SpotyPie.RecycleView.Views;
using System.Collections.Generic;

namespace SpotyPie.Player
{
    public class PlayerSongList : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.player_song_list;

        protected override LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Default;

        private SongListAdapter SongsAdapter { get; set; }

        private SpotyPieRecycleView Rv { get; set; }

        protected override void InitView()
        {
            SongsAdapter = new SongListAdapter().SetInitSongs(SongManager.SongQueue);
            Rv = new SpotyPieRecycleView(this, Resource.Id.song_list)
                .Setup(RecycleView.Enums.LayoutManagers.Linear_vertical)
                .DisableScroolNested()
                .SetAdapter(SongsAdapter);
        }

        public override void ForceUpdate()
        {
            if (SongsAdapter == null)
            {
                SongsAdapter = new SongListAdapter().SetInitSongs(SongManager.SongQueue);
            }

            SongsAdapter.OnSongClick = (song) =>
            {
                SongManager.Play(song);
                SongsAdapter.NotifyDataSetChanged();
            };

            SongsAdapter.OnSongOptionClick = (song) =>
            {
                LoadSongOptionsFragment(song);
            };

            SongManager.SongListHandler += OnSongListChange;
        }

        public override void ReleaseData()
        {
            if (SongsAdapter != null)
            {
                SongsAdapter.Release();
                SongsAdapter.Dispose();
                SongsAdapter = null;
            }

            SongManager.SongListHandler -= OnSongListChange;
        }

        public void OnSongListChange(List<Songs> songs)
        {
            SongsAdapter?.AddList(songs);
        }

        public void LoadSongOptionsFragment(Songs song)
        {
            LoadFragmentInner(FragmentEnum.SongDetails, screen: LayoutScreenState.FullScreen);
        }

        public void SetSongActive()
        {
        }

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            switch (switcher)
            {
                case FragmentEnum.SongOptionsFragment:
                    return new SongOptionsFragment();
                default:
                    return null;
            }
        }

        public bool CheckForLayout()
        {
            return true;
        }

        public override int GetParentView()
        {
            return Resource.Id.innerWrapper;
        }
    }
}