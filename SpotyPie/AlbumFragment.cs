using Android.App;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Helpers;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Android.Views.View;
using static Android.Views.ViewGroup;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie
{
    public class AlbumFragment : SupportFragment
    {
        View RootView;

        //Background info

        ImageView AlbumPhoto;
        TextView AlbumTitle;
        Button PlayableButton;
        TextView AlbumByText;

        TextView ButtonBackGround;
        TextView ButtonBackGround2;

        //Album Songs
        public List<Item> AlbumSongsItem;
        public RvList<Item> AlbumSongs;
        private RecyclerView.LayoutManager AlbumSongsLayoutManager;
        private RecyclerView.Adapter AlbumSongsAdapter;
        private RecyclerView AlbumSongsRecyclerView;

        private Button ShufflePlay;//button_text

        private TextView download;
        private TextView Copyrights;
        private ConstraintLayout backViewContainer;
        private ConstraintLayout InnerViewContainer;
        int Height = 0;

        MarginLayoutParams MarginParrams;
        RelativeLayout relative;
        private NestedScrollView ScrollFather;
        int scrolled;
        public bool isPlayable;
        public bool IsMeniuActive = false;
        FrameLayout containerx;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.Album_layout, container, false);

            MainActivity.ActionName.Text = Current_state.Current_Album.Name;
            isPlayable = false;
            IsMeniuActive = false;
            scrolled = 0;
            //Background binding

            containerx = RootView.FindViewById<FrameLayout>(Resource.Id.frameLayout);
            containerx.Touch += Containerx_Touch;
            ShufflePlay = RootView.FindViewById<Button>(Resource.Id.button_text);
            ShufflePlay.Visibility = ViewStates.Gone;

            AlbumPhoto = RootView.FindViewById<ImageView>(Resource.Id.album_photo);
            AlbumTitle = RootView.FindViewById<TextView>(Resource.Id.album_title);
            PlayableButton = RootView.FindViewById<Button>(Resource.Id.playable_button);
            AlbumByText = RootView.FindViewById<TextView>(Resource.Id.album_by_title);

            ButtonBackGround = RootView.FindViewById<TextView>(Resource.Id.backgroundHalf);
            ButtonBackGround2 = RootView.FindViewById<TextView>(Resource.Id.backgroundHalfInner);

            if (Current_state.GetAlbumPhoto() != null)
                Picasso.With(Context).Load(Current_state.GetAlbumPhoto()).Resize(600, 600).CenterCrop().Into(AlbumPhoto);
            else
                AlbumPhoto.SetImageResource(Resource.Drawable.noimg);

            AlbumTitle.Text = Current_state.Current_Album.Name;
            AlbumByText.Text = Current_state.Current_Album.Label;

            Current_state.ShowHeaderNavigationButtons();

            download = RootView.FindViewById<TextView>(Resource.Id.download_text);
            Copyrights = RootView.FindViewById<TextView>(Resource.Id.copyrights);
            MarginParrams = (MarginLayoutParams)download.LayoutParameters;

            relative = RootView.FindViewById<RelativeLayout>(Resource.Id.hide);

            InnerViewContainer = RootView.FindViewById<ConstraintLayout>(Resource.Id.innerWrapper);
            //InnerViewContainer.Visibility = ViewStates.Gone;
            ScrollFather = RootView.FindViewById<NestedScrollView>(Resource.Id.fatherScrool);
            backViewContainer = RootView.FindViewById<ConstraintLayout>(Resource.Id.backViewContainer);
            Height = backViewContainer.LayoutParameters.Height;
            ScrollFather.ScrollChange += Scroll_ScrollChange;

            //ALBUM song list
            AlbumSongs = new RvList<Item>();
            AlbumSongsItem = new List<Item>();
            AlbumSongsLayoutManager = new LinearLayoutManager(this.Activity);
            AlbumSongsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.song_list);
            AlbumSongsRecyclerView.SetLayoutManager(AlbumSongsLayoutManager);
            AlbumSongsAdapter = new VerticalRV(AlbumSongs, this.Context);
            AlbumSongs.Adapter = AlbumSongsAdapter;
            AlbumSongsRecyclerView.SetAdapter(AlbumSongsAdapter);
            AlbumSongsRecyclerView.NestedScrollingEnabled = false;

            AlbumSongsRecyclerView.SetItemClickListener((rv, position, view) =>
            {
                try
                {
                    if (AlbumSongsRecyclerView != null && AlbumSongsRecyclerView.ChildCount != 0)
                    {
                        var c = AlbumSongsRecyclerView.Width;
                        float Procent = (Search.Action * 100) / AlbumSongsRecyclerView.Width;
                        if (Procent <= 80 && AlbumSongsItem[position] != null)
                        {
                            Current_state.SetSong(AlbumSongsItem[position]);
                        }
                        else
                        {
                            if (!IsMeniuActive)
                            {
                                IsMeniuActive = true;
                                MainActivity activity = (MainActivity)this.Activity;
                                //activity.LoadOptionsMeniu();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }
            });

            return RootView;
        }

        private void Containerx_Touch(object sender, TouchEventArgs e)
        {
            Search.Action = e.Event.GetX();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        public override void OnResume()
        {
            base.OnResume();
            Task.Run(() => GetSongsAsync(Current_state.Current_Album.Id));
        }

        public async Task GetSongsAsync(int id)
        {
            try
            {
                await AlbumSongs.ClearAsync();
                AlbumSongs.Add(null);

                RestClient Client = new RestClient("http://pie.pertrauktiestaskas.lt/api/album/" + id + "/tracks");
                var request = new RestRequest(Method.GET);
                IRestResponse response = await Client.ExecuteGetTaskAsync(request);
                if (response.IsSuccessful)
                {
                    Album album = JsonConvert.DeserializeObject<Album>(response.Content);

                    if (album.Songs.Any(x => x.LocalUrl != null))
                        Application.SynchronizationContext.Post(_ =>
                        {
                            isPlayable = true;
                            PlayableButton.Text = "Playable";
                            PlayableButton.SetBackgroundResource(Resource.Drawable.playable_button);
                            ShufflePlay.Visibility = ViewStates.Visible;
                        }, null);

                    Application.SynchronizationContext.Post(_ =>
                    {
                        Current_state.Current_Song_List = album.Songs;
                        List<Copyright> Copyright = JsonConvert.DeserializeObject<List<Copyright>>(album.Copyrights);
                        Copyrights.Text = string.Join("\n", Copyright.Select(x => x.Text));
                    }, null);

                    AlbumSongsItem = album.Songs;
                    foreach (var x in album.Songs)
                    {
                        AlbumSongs.Add(x);
                        await Task.Delay(200);
                    }
                    //AlbumSongs.RemoveLoading();
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

        private void Scroll_ScrollChange(object sender, NestedScrollView.ScrollChangeEventArgs e)
        {
            scrolled = ScrollFather.ScrollY;
            if (scrolled < Height) //761 mazdaug
            {
                MainActivity.ActionName.Alpha = (float)((scrolled * 100) / Height) / 100;
                ButtonBackGround.Alpha = (float)((scrolled * 100) / Height) / 100;
                relative.Visibility = ViewStates.Invisible;
            }
            else
            {
                if (isPlayable)
                    relative.Visibility = ViewStates.Visible;
            }
        }
    }
}