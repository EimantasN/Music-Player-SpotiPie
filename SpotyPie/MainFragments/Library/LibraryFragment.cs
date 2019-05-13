using Android.Support.Design.Widget;
using Android.Support.V4.View;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.Library.Fragments;

namespace SpotyPie
{
    public class LibraryFragment : FragmentBase
    {
        TabLayout Tabs;
        ViewPager ViewPager;
        TabAdapter Adapter;

        private int[] tabIcons = { Resource.Drawable.artist_icon, Resource.Drawable.album_icon };

        public override int LayoutId { get; set; } = Resource.Layout.library_layout;

        protected override void InitView()
        {
            Tabs = RootView.FindViewById<TabLayout>(Resource.Id.tabs);
            ViewPager = RootView.FindViewById<ViewPager>(Resource.Id.viewpager);

            SetUpViewPager(ViewPager);
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            Adapter = new TabAdapter(ChildFragmentManager);
            Adapter.AddFragment(new Artists(), "");
            Adapter.AddFragment(new Albums(), "");
            viewPager.Adapter = Adapter;

            Tabs.SetupWithViewPager(ViewPager);

            SetupTabIcons();

            void SetupTabIcons()
            {
                Tabs.GetTabAt(0).SetIcon(tabIcons[0]);
                Tabs.GetTabAt(1).SetIcon(tabIcons[1]);
            }
        }

        public override void ForceUpdate()
        {

        }

        public override void ReleaseData()
        {
            //if (Tabs != null)
            //{
            //    Tabs.Dispose();
            //    Tabs = null;

            //    ViewPager.Dispose();
            //    ViewPager = null;

            //    Adapter.Dispose();
            //    Adapter = null;
            //}
        }

        public override int GetParentView()
        {
            throw new System.NotImplementedException();
        }

        public override void LoadFragment(dynamic switcher)
        {
        }
    }
}