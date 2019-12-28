using Android.Content;
using Android.Support.Constraints;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Realms;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.Music.Manager;
using SpotyPie.RecycleView;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Android.Views.ViewGroup;

namespace SpotyPie
{
    public class AlbumFragment : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.Album_layout;

        protected override LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Default;

        private BaseRecycleView<Songs> RvData;

        ImageView AlbumPhoto;
        TextView AlbumTitle;
        Button PlayableButton;
        TextView AlbumByText;

        TextView ButtonBackGround;
        TextView ButtonBackGround2;

        private Button ShufflePlay;

        private TextView download;
        private TextView Copyrights;
        private ConstraintLayout InnerViewContainer;

        private MarginLayoutParams MarginParrams;
        private RelativeLayout relative;
        private NestedScrollView ScrollFather;

        private int Height { get; set; } = 0;
        private int Scrolled { get; set; }
        private bool isPlayable { get; set; }

        private bool IsMeniuActive { get; set; } = false;

        protected override void InitView()
        {
            ShufflePlay = RootView.FindViewById<Button>(Resource.Id.button_text);
            ShufflePlay.Visibility = ViewStates.Gone;

            AlbumPhoto = RootView.FindViewById<ImageView>(Resource.Id.album_photo);
            AlbumTitle = RootView.FindViewById<TextView>(Resource.Id.album_title);
            PlayableButton = RootView.FindViewById<Button>(Resource.Id.playable_button);
            AlbumByText = RootView.FindViewById<TextView>(Resource.Id.album_by_title);

            ButtonBackGround = RootView.FindViewById<TextView>(Resource.Id.backgroundHalf);
            ButtonBackGround2 = RootView.FindViewById<TextView>(Resource.Id.backgroundHalfInner);

            download = RootView.FindViewById<TextView>(Resource.Id.download_text);
            Copyrights = RootView.FindViewById<TextView>(Resource.Id.copyrights);
            MarginParrams = (MarginLayoutParams)download.LayoutParameters;

            relative = RootView.FindViewById<RelativeLayout>(Resource.Id.hide);

            InnerViewContainer = RootView.FindViewById<ConstraintLayout>(Resource.Id.innerWrapper);
            ScrollFather = RootView.FindViewById<NestedScrollView>(Resource.Id.fatherScrool);

            SetAlbum(GetModel<Album>());
        }

        public override void ForceUpdate()
        {
            if (RvData == null)
            {
                RvData = new BaseRecycleView<Songs>(this, Resource.Id.song_list);
                RvData.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical);
                RvData.DisableScroolNested();
                LoadAlbumSongs();
            }
            SongManager.SongListHandler += OnSongListChange;
            SongManager.SongHandler += OnSongChange;
        }

        public override void ReleaseData()
        {
            if (RvData != null)
            {
                RvData.Dispose();
                RvData = null;
            }
            SongManager.SongListHandler -= OnSongListChange;
            SongManager.SongHandler -= OnSongChange;
        }

        public void OnSongChange(Songs song)
        {
            RvData?.GetData()?.Adapter?.NotifyDataSetChanged();
        }

        public void OnSongListChange(List<Songs> songs)
        {
            RvData?.GetData()?.AddList(songs);
        }

        public void SetAlbum(Album album = null)
        {
            if (Context != null)
            {
                if (album == null)
                    album = GetModel<Album>();

                ScrollFather.ScrollTo(0, 0);
                isPlayable = true;
                IsMeniuActive = false;
                Scrolled = 0;

                Picasso.With(Context).Load(album.LargeImage).Into(AlbumPhoto);

                AlbumTitle.Text = album.Name;

                //TODO connect artist name
                AlbumByText.Text = $"Popularity {album.Popularity}";

                ForceUpdate();
            }
        }

        private void LoadAlbumSongs()
        {
            Task.Run(async () =>
            {
                List<Songs> songs = await GetAPIService().GetSongsByAlbumAsync(GetModel<Album>());
                RvData?.GetData()?.AddList(songs);
            });
        }

        public override int GetParentView()
        {
            return Resource.Id.parent_view;
        }

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            return null;
        }
    }
}