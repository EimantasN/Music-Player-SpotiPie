﻿using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Interfaces;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.RecycleView;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class ArtistFragment : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.Artist_layout;

        protected override Enums.LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Default;

        private TextView SongListTitle { get; set; }
        private TextView AlbumListTitle { get; set; }
        private TextView ArtistListTitle { get; set; }

        private BaseRecycleView<Songs> RvSongs { get; set; }
        private BaseRecycleView<Album> RvAlbums { get; set; }
        private BaseRecycleView<Artist> RvRevated { get; set; }

        private Artist CurrentArtist;

        ImageView Photo;
        TextView AlbumTitle;
        Button PlayableButton;
        TextView AlbumByText;

        Button ShufflePlay;

        private NestedScrollView ScrollFather;

        int scrolled = 0;

        private void Scroll_ScrollChange(object sender, NestedScrollView.ScrollChangeEventArgs e)
        {
            //scrolled = ScrollFather.ScrollY;
            //if (scrolled < Height) //761 mazdaug
            //{
            //    //MainActivity.ActionName.Alpha = (float)((scrolled * 100) / Height) / 100;
            //    Background.Alpha = (float)((scrolled * 100) / Height) / 100;
            //    ButtonBackGround.Alpha = (float)((scrolled * 100) / Height) / 100;
            //    ButtonBackGround2.Alpha = (float)((scrolled * 100) / Height) / 100;
            //    relative.Visibility = ViewStates.Invisible;
            //}
            //else
            //{
            //    relative.Visibility = ViewStates.Visible;
            //}
        }

        protected override void InitView()
        {
            ShufflePlay = RootView.FindViewById<Button>(Resource.Id.button_text);
            ShufflePlay.Click += ShufflePlay_Click;

            Photo = RootView.FindViewById<ImageView>(Resource.Id.album_photo);
            AlbumTitle = RootView.FindViewById<TextView>(Resource.Id.album_title);
            PlayableButton = RootView.FindViewById<Button>(Resource.Id.playable_button);
            AlbumByText = RootView.FindViewById<TextView>(Resource.Id.album_by_title);

            SongListTitle = RootView.FindViewById<TextView>(Resource.Id.song_list_title);
            SongListTitle.Visibility = ViewStates.Gone;

            AlbumListTitle = RootView.FindViewById<TextView>(Resource.Id.albums_list_title);
            AlbumListTitle.Visibility = ViewStates.Gone;

            ArtistListTitle = RootView.FindViewById<TextView>(Resource.Id.related_list_title);
            ArtistListTitle.Visibility = ViewStates.Gone;

            ScrollFather = RootView.FindViewById<NestedScrollView>(Resource.Id.fatherScrool);
            ScrollFather.ScrollChange += ScrollFather_ScrollChange;

            LoadArtist(CurrentArtist);
        }

        private void ShufflePlay_Click(object sender, EventArgs e)
        {
            ShufflePlay.Text = "Loading songs";

            Task.Run(async () =>
            {
                var data = await GetAPIService().GetArtistSongsAsync(CurrentArtist);
                Activity.RunOnUiThread(() =>
                {
                    GetState().SetSong(data, 0);
                    ShufflePlay.Text = "Playing";
                });
            });
        }

        public async Task LoadSongs()
        {
            SearchBase<Songs>(RvSongs.GetData(), await GetActivity().GetAPIService().GetTopTrackByArtistIdAsync(CurrentArtist.Id), SongListTitle, RvType.SongWithImage, limit: 5);
        }

        public async Task LoadAlbums()
        {
            SearchBase<Album>(RvAlbums.GetData(), await GetActivity().GetAPIService().GetArtistAlbumsAsync(CurrentArtist.Id), AlbumListTitle, RvType.AlbumGrid);
        }

        public async Task LoadRelatedArtists()
        {
            SearchBase<Artist>(RvRevated.GetData(), await GetActivity().GetAPIService().GetRelatedAsync(CurrentArtist.Id), ArtistListTitle, RvType.Artist);
        }
        public void SearchBase<T>(ThreadSafeRvList<T> RvList, List<T> DataList, TextView header, RvType type, int limit = int.MaxValue) where T : IBaseInterface<T>
        {
            try
            {
                if (DataList != null && DataList.Count > 0)
                {
                    if (header.Visibility == ViewStates.Gone)
                        header.Post(() => header.Visibility = ViewStates.Visible);

                    FormatView(ref DataList, type);
                    RvList.AddList(DataList.Take(limit).ToList());
                }
                else
                {
                    if (header.Visibility == ViewStates.Visible)
                    {
                        header.Post(() => header.Visibility = ViewStates.Gone);
                        RvList.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                Activity?.RunOnUiThread(() =>
                {
                    Toast.MakeText(this.Context, e.Message, ToastLength.Short).Show();
                });
            }
        }

        private void FormatView<T>(ref List<T> dataList, RvType type) where T : IBaseInterface<T>
        {
            if (dataList == null || dataList.Count == 0)
                return;

            if (dataList.Count == 1)
            {
                Activity?.RunOnUiThread(() =>
                {
                    if (typeof(T) == typeof(Album))
                    {
                        RvAlbums.SetLayoutManager(RecycleView.Enums.LayoutManagers.Linear_vertical);
                    }
                    else if (typeof(T) == typeof(Artist))
                    {
                        RvRevated.SetLayoutManager(RecycleView.Enums.LayoutManagers.Linear_vertical);
                    }
                });

                if (typeof(T) != typeof(Songs))
                    dataList.First().SetModelType(RvType.BigOne);
                else
                    dataList.First().SetModelType(type);
            }
            else
            {
                Activity?.RunOnUiThread(() =>
                {
                    if (typeof(T) == typeof(Album))
                    {
                        RvAlbums.SetLayoutManager(RecycleView.Enums.LayoutManagers.Grind_2_col);
                    }
                    else if (typeof(T) == typeof(Artist))
                    {
                        RvRevated.SetLayoutManager(RecycleView.Enums.LayoutManagers.Linear_horizontal);
                    }
                });

                dataList.ForEach(x => x.SetModelType(type));
            }
        }

        public new void LoadArtist(Artist artist)
        {
            CurrentArtist = GetModel<Artist>();
            if (Context != null)
            {
                ScrollFather.ScrollTo(0, 0);

                Picasso.With(Context).Load(CurrentArtist.LargeImage).Into(Photo);

                AlbumTitle.Text = CurrentArtist.Name;

                //TODO connect artist name
                AlbumByText.Text = "Coming soon";
                ForceUpdate();
            }
        }


        private void ScrollFather_ScrollChange(object sender, NestedScrollView.ScrollChangeEventArgs e)
        {

        }

        public override void ForceUpdate()
        {
            if (RvSongs == null)
            {
                RvSongs = new BaseRecycleView<Songs>(this, Resource.Id.song_list);
                RvSongs.Init(RecycleView.Enums.LayoutManagers.Linear_vertical);
                RvSongs.DisableScroolNested();
            }

            if (RvAlbums == null)
            {
                RvAlbums = new BaseRecycleView<Album>(this, Resource.Id.artist_albums_list);
                RvAlbums.Init(RecycleView.Enums.LayoutManagers.Grind_2_col);
                RvAlbums.DisableScroolNested();
            }

            if (RvRevated == null)
            {
                RvRevated = new BaseRecycleView<Artist>(this, Resource.Id.related_artist_list);
                RvRevated.Init(RecycleView.Enums.LayoutManagers.Linear_horizontal);
                RvRevated.DisableScroolNested();
            }

            Task.Run(async () => await LoadSongs());
            Task.Run(async () => await LoadAlbums());
            Task.Run(async () => await LoadRelatedArtists());
        }

        public override void ReleaseData()
        {
            if (RvSongs != null)
            {
                RvSongs.Dispose();
                RvSongs = null;
            }

            if (RvAlbums != null)
            {
                RvAlbums.Dispose();
                RvAlbums = null;
            }

            if (RvRevated != null)
            {
                RvRevated.Dispose();
                RvRevated = null;
            }
        }

        public override int GetParentView()
        {
            throw new NotImplementedException();
        }

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            return null;
        }
    }
}