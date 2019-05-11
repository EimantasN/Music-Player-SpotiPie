using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SpotyPie.Base
{
    public abstract class ActivityBase : AppCompatActivity
    {
        public abstract int LayoutId { get; set; }

        protected virtual int FirstLayerFragmentHolder { get; set; } = Resource.Id.content_frame;

        private Stack<dynamic> FragmentStack { get; set; }

        private FrameLayout FragmentFrame;

        private ProgressBar FragmentLoading;

        public bool IsFragmentLoadedAdded = false;

        protected virtual FragmentBase CurrentFragment { get; set; }

        private API Api_service { get; set; }

        public SupportFragmentManager mSupportFragmentManager;

        public abstract dynamic GetInstance();

        private int GetLayout()
        {
            return LayoutId;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(GetLayout());
            InitView();
        }

        protected virtual void InitView()
        {
            mSupportFragmentManager = SupportFragmentManager;
            if (IsFragmentLoadedAdded)
            {
                FragmentFrame = FindViewById<FrameLayout>(FirstLayerFragmentHolder);
                FragmentFrame.Visibility = ViewStates.Gone;
                FragmentLoading = FindViewById<ProgressBar>(Resource.Id.fragmentloading);
                FragmentLoading.Visibility = ViewStates.Gone;
            }
        }


        public API GetAPIService()
        {
            if (Api_service == null)
                return Api_service = new API(new Mobile_Api.Service(), this);
            return Api_service;
        }

        protected abstract void LoadFragment(dynamic switcher);

        public override void OnBackPressed()
        {
            if (CheckFragments())
                base.OnBackPressed();
            else
            {
                LoadFragmentInner(GetFragmentStack().Pop());
                if (GetFragmentStack().Count == 0)
                {
                    if (IsFragmentLoadedAdded)
                    {
                        if (FragmentFrame.Visibility == ViewStates.Visible)
                            FragmentFrame.Visibility = ViewStates.Gone;

                        if (FragmentLoading.Visibility == ViewStates.Visible)
                            FragmentLoading.Visibility = ViewStates.Gone;
                    }
                }
            }
        }

        private bool CheckFragments()
        {
            if (GetFragmentStack() == null || GetFragmentStack().Count <= 1)
                return true;

            dynamic fragmentState = GetFragmentStack().Pop();
            if (fragmentState == null)
                return true;
            return false;
        }

        public virtual void FragmentLoaded()
        {
            if (IsFragmentLoadedAdded && FragmentLoading.Visibility == ViewStates.Visible)
            {
                FragmentLoading.Visibility = ViewStates.Gone;
            }
        }

        private Stack<dynamic> GetFragmentStack()
        {
            if (FragmentStack == null)
                FragmentStack = new Stack<dynamic>();
            return FragmentStack;
        }

        public void LoadFragmentInner(dynamic switcher, string jsonModel = null)
        {
            if (IsFragmentLoadedAdded)
            {
                if (FragmentFrame.Visibility == ViewStates.Gone)
                    FragmentFrame.Visibility = ViewStates.Visible;

                if (FragmentLoading.Visibility == ViewStates.Gone)
                    FragmentLoading.Visibility = ViewStates.Visible;
            }

            if (CurrentFragment != null)
            {
                CurrentFragment.Hide();
            }

            CurrentFragment = null;

            GetFragmentStack().Push(switcher);
            LoadFragment(switcher);

            if (CurrentFragment == null)
            {
                throw new System.Exception("Fragment not founded");
            }

            //Can send data to fragment
            if (!string.IsNullOrEmpty(jsonModel))
                CurrentFragment.SendData(jsonModel);

            if (!CurrentFragment.IsAdded)
            {
                SupportFragmentManager.BeginTransaction()
                .Replace(FirstLayerFragmentHolder, CurrentFragment)
                .Commit();
            }
            else
            {
                CurrentFragment.Show();
            }
        }

        protected override void OnDestroy()
        {
            FragmentStack = null;
            CurrentFragment = null;
            base.OnDestroy();
        }

        public void RemoveCurrentFragment()
        {
            throw new System.Exception("not implemented");
            //if (FirstLayerFragment != null)
            //{
            //    FirstLayerFragment.ReleaseData();
            //    var transaction = mSupportFragmentManager.BeginTransaction();
            //    transaction.Remove(FirstLayerFragment);
            //    transaction.Commit();
            //    transaction.SetTransition(Android.Support.V4.App.FragmentTransaction.TransitFragmentClose);
            //    transaction = null;
            //    FirstLayerFragment = null;
            //    FirstLayer.TranslationX = widthInDp;
            //}
        }
    }
}