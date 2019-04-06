﻿using Android.App;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.RecycleView;
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
    public class AlbumFragment : FragmentBase
    {
        ImageView AlbumPhoto;
        TextView AlbumTitle;
        Button PlayableButton;
        TextView AlbumByText;

        TextView ButtonBackGround;
        TextView ButtonBackGround2;

        //Album Songs
        public List<Song> AlbumSongsItem;
        public RvList<Song> AlbumSongs;
        private RecyclerView.LayoutManager AlbumSongsLayoutManager;
        private RecyclerView.Adapter AlbumSongsAdapter;
        private RecyclerView AlbumSongsRecyclerView;
        private Button ShufflePlay;//button_text

        private TextView download;
        private TextView Copyrights;
        private ConstraintLayout backViewContainer;
        private ConstraintLayout InnerViewContainer;

        private MarginLayoutParams MarginParrams;
        private RelativeLayout relative;
        private NestedScrollView ScrollFather;
        private FrameLayout Holder;

        private int Height = 0;
        private int Scrolled;
        private bool isPlayable;
        private bool IsMeniuActive = false;

        public override int LayoutId { get; set; } = Resource.Layout.Album_layout;

        protected override void InitView()
        {
            //Background binding
            Holder = RootView.FindViewById<FrameLayout>(Resource.Id.frameLayout);
            Holder.Touch += Containerx_Touch;
            ShufflePlay = RootView.FindViewById<Button>(Resource.Id.button_text);
            ShufflePlay.Visibility = ViewStates.Gone;

            AlbumPhoto = RootView.FindViewById<ImageView>(Resource.Id.album_photo);
            AlbumTitle = RootView.FindViewById<TextView>(Resource.Id.album_title);
            PlayableButton = RootView.FindViewById<Button>(Resource.Id.playable_button);
            AlbumByText = RootView.FindViewById<TextView>(Resource.Id.album_by_title);

            ButtonBackGround = RootView.FindViewById<TextView>(Resource.Id.backgroundHalf);
            ButtonBackGround2 = RootView.FindViewById<TextView>(Resource.Id.backgroundHalfInner);

            GetState().ShowHeaderNavigationButtons();

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

            ////ALBUM song list
            //AlbumSongs = new RvList<Song>();
            //AlbumSongsItem = new List<Song>();
            //AlbumSongsLayoutManager = new LinearLayoutManager(this.Activity);
            //AlbumSongsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.song_list);
            //AlbumSongsRecyclerView.SetLayoutManager(AlbumSongsLayoutManager);
            //AlbumSongsAdapter = new VerticalRV(AlbumSongs, this.Context);
            //AlbumSongs.Adapter = AlbumSongsAdapter;
            //AlbumSongsRecyclerView.SetAdapter(AlbumSongsAdapter);
            //AlbumSongsRecyclerView.NestedScrollingEnabled = false;

            //AlbumSongsRecyclerView.SetItemClickListener((rv, position, view) =>
            //{
            //    try
            //    {
            //        if (AlbumSongsRecyclerView != null && AlbumSongsRecyclerView.ChildCount != 0)
            //        {
            //            var c = AlbumSongsRecyclerView.Width;
            //            float Procent = (Search.Action * 100) / AlbumSongsRecyclerView.Width;
            //            if (Procent <= 80 && AlbumSongsItem[position] != null)
            //            {
            //                GetState().SetSong(AlbumSongsItem[position]);
            //            }
            //            else
            //            {
            //                if (!IsMeniuActive)
            //                {
            //                    IsMeniuActive = true;
            //                    MainActivity activity = (MainActivity)this.Activity;
            //                    //activity.LoadOptionsMeniu();
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //    }
            //});
        }

        private void Containerx_Touch(object sender, TouchEventArgs e)
        {
            Search.Action = e.Event.GetX();
        }

        private void Scroll_ScrollChange(object sender, NestedScrollView.ScrollChangeEventArgs e)
        {
            Scrolled = ScrollFather.ScrollY;
            if (Scrolled < Height) //761 mazdaug
            {
                MainActivity.ActionName.Alpha = (float)((Scrolled * 100) / Height) / 100;
                ButtonBackGround.Alpha = (float)((Scrolled * 100) / Height) / 100;
                relative.Visibility = ViewStates.Invisible;
            }
            else
            {
                if (isPlayable)
                    relative.Visibility = ViewStates.Visible;
            }
        }

        public void SetAlbum(Album album)
        {
            try
            {
                MainActivity.ActionName.Text = album.Name;
                isPlayable = true;
                IsMeniuActive = false;
                Scrolled = 0;

                Picasso.With(Context).Load(album.LargeImage).Resize(600, 600).CenterCrop().Into(AlbumPhoto);

                AlbumTitle.Text = album.Name;
                AlbumByText.Text = "Muse";
            }
            catch (Exception e)
            {

            }
        }
    }
}