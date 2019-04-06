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
        public Album CurrentALbum { get; set; }

        private RvList<dynamic> RvData;

        ImageView AlbumPhoto;
        TextView AlbumTitle;
        Button PlayableButton;
        TextView AlbumByText;

        TextView ButtonBackGround;
        TextView ButtonBackGround2;

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
            var parameters = backViewContainer.LayoutParameters;
            parameters.Height = Resources.DisplayMetrics.WidthPixels;

            backViewContainer.LayoutParameters = parameters;

            Height = backViewContainer.LayoutParameters.Height;

            ScrollFather.ScrollChange += Scroll_ScrollChange;

            if (RvData == null)
            {
                var rvBase = new BaseRecycleView<dynamic>(this, Resource.Id.song_list);
                RvData = rvBase.Setup(LinearLayoutManager.Vertical);
                rvBase.DisableScroolNested();
            }
            SetAlbum(CurrentALbum);
        }

        public override void OnResume()
        {
            base.OnResume();
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
                GetState().Activity.ActionName.Alpha = (float)((Scrolled * 100) / Height) / 100;
                ButtonBackGround.Alpha = (float)((Scrolled * 100) / Height) / 100;
                relative.Visibility = ViewStates.Invisible;
            }
            else
            {
                if (isPlayable)
                    relative.Visibility = ViewStates.Visible;
            }
        }

        public void SetAlbum(Album album = null)
        {
            try
            {
                CurrentALbum = album;
                if (Context != null)
                {
                    GetState().Activity.ActionName.Text = CurrentALbum.Name;
                    isPlayable = true;
                    IsMeniuActive = false;
                    Scrolled = 0;

                    Picasso.With(Context).Load(CurrentALbum.LargeImage).Resize(1200, 1200).CenterCrop().Into(AlbumPhoto);

                    AlbumTitle.Text = CurrentALbum.Name;
                    AlbumByText.Text = "Muse";
                    List<dynamic> songs = new List<dynamic>();
                    CurrentALbum.Songs.ForEach(x => songs.Add((dynamic)x));
                    RvData.AddList(songs);
                }
            }
            catch (Exception e)
            {
            }
        }

        public override void ForceUpdate()
        {
            if (RvData != null)
            {
                List<dynamic> songs = new List<dynamic>();
                CurrentALbum.Songs.ForEach(x => songs.Add((dynamic)x));
                RvData.AddList(songs);
            }
        }
    }
}