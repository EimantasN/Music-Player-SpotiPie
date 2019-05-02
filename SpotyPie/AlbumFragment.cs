using Android.Support.Constraints;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.RecycleView;
using Square.Picasso;
using System.Threading.Tasks;
using static Android.Views.ViewGroup;

namespace SpotyPie
{
    public class AlbumFragment : FragmentBase
    {
        public Album CurrentALbum { get; set; }

        private BaseRecycleView<Songs> RvData;

        ImageView AlbumPhoto;
        TextView AlbumTitle;
        Button PlayableButton;
        TextView AlbumByText;

        TextView ButtonBackGround;
        TextView ButtonBackGround2;

        private Button ShufflePlay;//button_text

        private TextView download;
        private TextView Copyrights;
        private ConstraintLayout InnerViewContainer;

        private MarginLayoutParams MarginParrams;
        private RelativeLayout relative;
        private NestedScrollView ScrollFather;
        //private FrameLayout Holder;

        private int Height = 0;
        private int Scrolled;
        private bool isPlayable;
        private bool IsMeniuActive = false;

        public override int LayoutId { get; set; } = Resource.Layout.Album_layout;

        protected override void InitView()
        {
            //Background binding
            //Holder = RootView.FindViewById<FrameLayout>(Resource.Id.frameLayout);
            //Holder.Touch += Containerx_Touch;
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
            ScrollFather = RootView.FindViewById<NestedScrollView>(Resource.Id.fatherScrool);

            ScrollFather.ScrollChange += Scroll_ScrollChange;

            SetAlbum(CurrentALbum);
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        private void Scroll_ScrollChange(object sender, NestedScrollView.ScrollChangeEventArgs e)
        {
            //Scrolled = ScrollFather.ScrollY;
            //if (Scrolled < Height) //761 mazdaug
            //{
            //    GetState().Activity.ActionName.Alpha = (float)((Scrolled * 100) / Height) / 100;
            //    ButtonBackGround.Alpha = (float)((Scrolled * 100) / Height) / 100;
            //    relative.Visibility = ViewStates.Invisible;
            //}
            //else
            //{
            //    if (isPlayable)
            //        relative.Visibility = ViewStates.Visible;
            //}
        }

        public void SetAlbum(Album album = null)
        {
            try
            {
                CurrentALbum = album;
                if (Context != null)
                {
                    ScrollFather.ScrollTo(0, 0);
                    GetState().Activity.ActionName.Text = CurrentALbum.Name;
                    isPlayable = true;
                    IsMeniuActive = false;
                    Scrolled = 0;

                    Picasso.With(Context).Load(CurrentALbum.LargeImage).Into(AlbumPhoto);

                    AlbumTitle.Text = CurrentALbum.Name;

                    //TODO connect artist name
                    AlbumByText.Text = "Coming soon";

                    ForceUpdate();
                }
            }
            catch
            {
            }
        }

        public override void ForceUpdate()
        {
            if (RvData == null)
            {
                RvData = new BaseRecycleView<Songs>(this, Resource.Id.song_list);
                RvData.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical);
                RvData.DisableScroolNested();
            }

            Task.Run(async () => await GetAPIService().GetSongsByAlbumAsync(CurrentALbum, RvData.GetData(), () => { }));
        }

        public override void ReleaseData()
        {
            if (RvData != null)
            {
                RvData.Dispose();
                RvData = null;
            }
        }
    }
}