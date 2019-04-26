using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Interfaces;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
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
            //MainActivity.ActionName.Text = GetState().Current_Artist.Name;

            ////Background binding
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

            //Picasso.With(Context).Load(GetState().Current_Artist.LargeImage).Resize(300, 300).CenterCrop().Into(Photo);
            //AlbumTitle.Text = GetState().Current_Artist.Name;

            ScrollFather = RootView.FindViewById<NestedScrollView>(Resource.Id.fatherScrool);
            ScrollFather.ScrollChange += ScrollFather_ScrollChange;

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

            LoadArtist(CurrentArtist);
        }

        public async Task LoadSongs()
        {
            SearchBase<Songs>(RvSongs.GetData(), await ParentActivity.GetService().GetTopTrackByArtistId(CurrentArtist.Id), SongListTitle, RvType.SongWithImage, limit: 5);
        }

        public async Task LoadAlbums()
        {
            SearchBase<Album>(RvAlbums.GetData(), await ParentActivity.GetService().GetArtistAlbums(CurrentArtist.Id), AlbumListTitle, RvType.AlbumGrid);
        }

        public async Task LoadRelatedArtists()
        {
            SearchBase<Artist>(RvRevated.GetData(), await ParentActivity.GetService().GetRelated(CurrentArtist.Id), ArtistListTitle, RvType.Artist);
        }
        public void SearchBase<T>(RvList<T> RvList, List<T> DataList, TextView header, RvType type, int limit = int.MaxValue) where T : IBaseInterface
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
                Application.SynchronizationContext.Post(_ =>
                {
                    Toast.MakeText(this.Context, e.Message, ToastLength.Short).Show();
                }, null);
            }
        }

        private void FormatView<T>(ref List<T> dataList, RvType type) where T : IBaseInterface
        {
            if (dataList == null || dataList.Count == 0)
                return;

            if (dataList.Count == 1)
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    if (typeof(T) == typeof(Album))
                    {
                        RvAlbums.SetLayoutManager(RecycleView.Enums.LayoutManagers.Linear_vertical);
                    }
                    else if (typeof(T) == typeof(Artist))
                    {
                        RvRevated.SetLayoutManager(RecycleView.Enums.LayoutManagers.Linear_vertical);
                    }
                }, null);

                if (typeof(T) != typeof(Songs))
                    dataList.First().SetModelType(RvType.BigOne);
                else
                    dataList.First().SetModelType(type);
            }
            else
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    if (typeof(T) == typeof(Album))
                    {
                        RvAlbums.SetLayoutManager(RecycleView.Enums.LayoutManagers.Grind_2_col);
                    }
                    else if (typeof(T) == typeof(Artist))
                    {
                        RvRevated.SetLayoutManager(RecycleView.Enums.LayoutManagers.Linear_horizontal);
                    }
                }, null);

                dataList.ForEach(x => x.SetModelType(type));
            }
        }

        public new void LoadArtist(Artist artist)
        {
            try
            {
                CurrentArtist = artist;
                if (Context != null)
                {
                    ScrollFather.ScrollTo(0, 0);
                    GetState().Activity.ActionName.Text = CurrentArtist.Name;

                    Picasso.With(Context).Load(CurrentArtist.LargeImage).Into(Photo);

                    AlbumTitle.Text = CurrentArtist.Name;

                    //TODO connect artist name
                    AlbumByText.Text = "Coming soon";
                    LoadData();
                }
            }
            catch
            {
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
        }

        public override void OnStop()
        {
            RvSongs.GetData().Clear();
            RvAlbums.GetData().Clear();
            RvRevated.GetData().Clear();
            CurrentArtist = null;
            base.OnStop();
        }

        public void LoadData()
        {
            Task.Run(async () => await LoadSongs());
            Task.Run(async () => await LoadAlbums());
            Task.Run(async () => await LoadRelatedArtists());
        }

        private void ScrollFather_ScrollChange(object sender, NestedScrollView.ScrollChangeEventArgs e)
        {

        }

        public override void ForceUpdate()
        {
            LoadData();
        }
    }
}