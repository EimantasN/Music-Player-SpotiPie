using Android.App;
using Android.Support.Constraints;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Base;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Android.Views.ViewGroup;

namespace SpotyPie
{
    public class ArtistFragment : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.Artist_layout;

        //Background info

        ImageView Photo;
        TextView AlbumTitle;
        Button PlayableButton;
        TextView AlbumByText;
        TextView Background;

        TextView ButtonBackGround;
        TextView ButtonBackGround2;

        //Artist Songs
        public List<Song> ArtistTopSongsData;
        public RvList<Song> ArtistTopSongs;
        private RecyclerView.LayoutManager ArtistSongsLayoutManager;
        private RecyclerView.Adapter ArtistSongsAdapter;
        private RecyclerView ArtistSongsRecyclerView;

        ////Artist Albums
        //List<Album> AlbumsData;
        //public RvList<TwoBlockWithImage> Albums;
        //private RecyclerView.LayoutManager AlbumsLayoutManager;
        //private RecyclerView.Adapter AlbumsAdapter;
        //private RecyclerView AlbumsRecyclerView;

        private TextView download;
        private TextView Copyrights;
        private ConstraintLayout backViewContainer;
        int Height = 0;

        MarginLayoutParams MarginParrams;
        RelativeLayout relative;
        private NestedScrollView ScrollFather;
        int scrolled = 0;

        public async Task GetSongsAsync(int id)
        {
            try
            {
                RestClient Client = new RestClient("http://pie.pertrauktiestaskas.lt/api/artist/" + id + "/top-tracks");
                var request = new RestRequest(Method.GET);
                IRestResponse response = await Client.ExecuteGetTaskAsync(request);
                if (response.IsSuccessful)
                {
                    List<Song> songs = JsonConvert.DeserializeObject<List<Song>>(response.Content);
                    foreach (var x in songs.OrderByDescending(x => x.LastActiveTime).Take(6))
                    {
                        ArtistTopSongs.Add(x);
                    }
                    ArtistTopSongsData = songs;
                    Application.SynchronizationContext.Post(_ =>
                    {
                        //List<string> Genres = JsonConvert.DeserializeObject<List<string>>(Current_state.Current_Artist.Genres);
                        //Copyrights.Text = string.Join("\n", Genres);
                    }, null);
                }
                else
                {
                    Application.SynchronizationContext.Post(_ =>
                    {
                        Toast.MakeText(this.Context, "GetSongsAsync API call error", ToastLength.Short).Show();
                    }, null);
                }
            }
            catch (Exception)
            {

            }
        }

        public async Task GetArtistAlbums(int id)
        {
            //try
            //{
            //    RestClient Client = new RestClient("http://pie.pertrauktiestaskas.lt/api/artist/" + id + "/Albums");
            //    var request = new RestRequest(Method.GET);
            //    IRestResponse response = await Client.ExecuteGetTaskAsync(request);
            //    if (response.IsSuccessful)
            //    {
            //        Artist ArtistWithAlbums = JsonConvert.DeserializeObject<Artist>(response.Content);
            //        Application.SynchronizationContext.Post(_ =>
            //        {
            //            AlbumsData = ArtistWithAlbums.Albums;
            //            for (int i = 0; i < ArtistWithAlbums.Albums.Count; i = i + 2)
            //            {
            //                if (ArtistWithAlbums.Albums.Count - i == 1)
            //                {
            //                    var x = ArtistWithAlbums.Albums[i];
            //                    Albums.Add(new TwoBlockWithImage(
            //                    new BlockWithImage(
            //                        x.Id,
            //                        RvType.Album,
            //                        x.Name,
            //                        x.Label,
            //                        x.Images.First().Url)));
            //                }
            //                else
            //                {
            //                    var x = ArtistWithAlbums.Albums[i];
            //                    var y = ArtistWithAlbums.Albums[i + 1];
            //                    Albums.Add(new TwoBlockWithImage(
            //                        new BlockWithImage(
            //                            x.Id,
            //                            RvType.Album,
            //                            x.Name,
            //                            x.Label,
            //                            x.Images.First().Url),
            //                            new BlockWithImage(
            //                            y.Id,
            //                            RvType.Album,
            //                            y.Name,
            //                            x.Label,
            //                            y.Images.First().Url)));
            //                }
            //            }
            //            List<string> Genres = JsonConvert.DeserializeObject<List<string>>(Current_state.Current_Artist.Genres);
            //            Copyrights.Text = string.Join("\n", Genres);
            //        }, null);
            //    }
            //    else
            //    {
            //        Application.SynchronizationContext.Post(_ =>
            //        {
            //            Toast.MakeText(this.Context, "GetArtistAlbums API call error", ToastLength.Short).Show();
            //        }, null);
            //    }
            //}
            //catch (Exception)
            //{

            //}
        }

        private void Scroll_ScrollChange(object sender, NestedScrollView.ScrollChangeEventArgs e)
        {
            scrolled = ScrollFather.ScrollY;
            if (scrolled < Height) //761 mazdaug
            {
                //MainActivity.ActionName.Alpha = (float)((scrolled * 100) / Height) / 100;
                Background.Alpha = (float)((scrolled * 100) / Height) / 100;
                ButtonBackGround.Alpha = (float)((scrolled * 100) / Height) / 100;
                ButtonBackGround2.Alpha = (float)((scrolled * 100) / Height) / 100;
                relative.Visibility = ViewStates.Invisible;
            }
            else
            {
                relative.Visibility = ViewStates.Visible;
            }
        }

        protected override void InitView()
        {
            //MainActivity.ActionName.Text = GetState().Current_Artist.Name;

            ////Background binding
            //Photo = RootView.FindViewById<ImageView>(Resource.Id.album_photo);
            //AlbumTitle = RootView.FindViewById<TextView>(Resource.Id.album_title);
            //PlayableButton = RootView.FindViewById<Button>(Resource.Id.playable_button);
            //AlbumByText = RootView.FindViewById<TextView>(Resource.Id.album_by_title);
            //Background = RootView.FindViewById<TextView>(Resource.Id.view);
            //Background.Alpha = 0.0f;

            //ButtonBackGround = RootView.FindViewById<TextView>(Resource.Id.backgroundHalf);
            //ButtonBackGround2 = RootView.FindViewById<TextView>(Resource.Id.backgroundHalfInner);

            //Picasso.With(Context).Load(GetState().Current_Artist.LargeImage).Resize(300, 300).CenterCrop().Into(Photo);

            //AlbumTitle.Text = GetState().Current_Artist.Name;

            ////TODO error if genres is null
            ////AlbumByText.Text = JsonConvert.DeserializeObject<List<string>>(Current_state.Current_Artist)[0];

            //GetState().ShowHeaderNavigationButtons();

            //download = RootView.FindViewById<TextView>(Resource.Id.download_text);
            //Copyrights = RootView.FindViewById<TextView>(Resource.Id.copyrights);
            //MarginParrams = (MarginLayoutParams)download.LayoutParameters;

            //relative = RootView.FindViewById<RelativeLayout>(Resource.Id.hide);

            //ScrollFather = RootView.FindViewById<NestedScrollView>(Resource.Id.fatherScrool);
            ////ScrollFather.SetOnTouchListener(this);
            //backViewContainer = RootView.FindViewById<ConstraintLayout>(Resource.Id.backViewContainer);
            //Height = backViewContainer.LayoutParameters.Height;
            //ScrollFather.ScrollChange += Scroll_ScrollChange;


            ////Artist song list

            //ArtistTopSongsData = new List<Song>();
            //ArtistTopSongs = new RvList<Song>();
            //ArtistSongsLayoutManager = new LinearLayoutManager(this.Activity);
            //ArtistSongsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.song_list);
            //ArtistSongsRecyclerView.SetLayoutManager(ArtistSongsLayoutManager);
            //ArtistSongsAdapter = new VerticalRV(ArtistTopSongs, this.Context);
            //ArtistTopSongs.Adapter = ArtistSongsAdapter;
            //ArtistSongsRecyclerView.SetAdapter(ArtistSongsAdapter);
            //ArtistSongsRecyclerView.NestedScrollingEnabled = false;

            //Artist song list
            //AlbumsData = new List<Album>();
            //Albums = new RvList<TwoBlockWithImage>();
            //AlbumsLayoutManager = new LinearLayoutManager(this.Activity);
            //AlbumsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.artist_albums_list);
            //AlbumsRecyclerView.SetLayoutManager(AlbumsLayoutManager);
            //AlbumsAdapter = new BaseRv<TwoBlockWithImage>(Albums, AlbumsRecyclerView, this.Context);
            //Albums.Adapter = AlbumsAdapter;
            //AlbumsRecyclerView.SetAdapter(AlbumsAdapter);
            //AlbumsRecyclerView.NestedScrollingEnabled = false;
        }

        public override void ForceUpdate()
        {
            throw new NotImplementedException();
        }
    }
}