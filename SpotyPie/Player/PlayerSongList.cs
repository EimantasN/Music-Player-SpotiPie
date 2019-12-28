using Android.Widget;
using Mobile_Api.Models;
using Realms;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.Music.Manager;
using SpotyPie.RecycleView;
using SpotyPie.RecycleView.Adapters;
using SpotyPie.RecycleView.Views;
using System;
using System.Collections.Generic;
using MusicList = Mobile_Api.Models.Realm.Music;

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
                Toast.MakeText(this.Context, "OnSongOptionClick", ToastLength.Long).Show();
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

        public void LoadSongOptionsFragment()
        {
            LoadFragmentInner(Enums.Activitys.Player.SongDetails, screen: LayoutScreenState.FullScreen);
        }

        public void SetSongActive()
        {
        }

        public override void LoadFragment(dynamic switcher)
        {
            ParentActivity.FManager.SetCurrentFragment(new SongOptionsFragment());
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