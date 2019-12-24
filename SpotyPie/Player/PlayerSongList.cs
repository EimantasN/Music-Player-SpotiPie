using Android.Support.Constraints;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Realms;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.Music.Manager;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Linq;
using MusicList = Mobile_Api.Models.Realm.Music;

namespace SpotyPie.Player
{
    public class PlayerSongList : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.player_song_list;

        protected override LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Default;

        private BaseRecycleView<Songs> RvData { get; set; }

        public override int GetParentView()
        {
            return Resource.Id.innerWrapper;
        }

        protected override void InitView()
        {
            SongManager.SongListHandler += OnSongListChange;
        }

        public void OnSongListChange(List<Songs> songs)
        {
            RvData?.GetData()?.AddList(songs);
        }

        public void Update(List<Songs> songs = null)
        {
            if (songs != null)
                RvData?.GetData()?.AddList(songs);
            else
                RvData?.GetData()?.AddList(SongManager.SongQueue);
        }

        public override void ForceUpdate()
        {
            if (RvData == null)
            {
                RvData = new BaseRecycleView<Songs>(this, Resource.Id.song_list);
                RvData.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical,
                    new List<Action>(){
                        () => { LoadFragmentInner(Enums.Activitys.Player.SongDetails, screen: LayoutScreenState.FullScreen); },
                        () => { GetState().SetSong(RvData.GetData().GetList() as List<Songs>, -1); }
                    });
                RvData.DisableScroolNested();
            }
            Update();
        }

        private void Songs_CollectionChanged(IRealmCollection<MusicList> songList, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (RvData != null)
            {
                Update();
            }
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
            ParentActivity.FManager.SetCurrentFragment(new SongOptionsFragment());
        }

        public bool CheckForLayout()
        {
            return true;
        }
    }
}