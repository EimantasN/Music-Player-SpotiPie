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

        public override int LayoutId { get; set; } = Resource.Layout.library_layout;

        protected override void InitView()
        {
            GetState().Activity.ActionName.Text = "Library";
            GetState().Activity.ActionName.Alpha = 1.0f;

            Tabs = RootView.FindViewById<TabLayout>(Resource.Id.tabs);
            ViewPager = RootView.FindViewById<ViewPager>(Resource.Id.viewpager);

            SetUpViewPager(ViewPager);
            Tabs.SetupWithViewPager(ViewPager);
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            TabAdapter adapter = new TabAdapter(ChildFragmentManager);
            adapter.AddFragment(new Artists(), "Artists");
            adapter.AddFragment(new Albums(), "Albums");
            viewPager.Adapter = adapter;
        }

        public override void ForceUpdate()
        {

        }

        public override void ReleaseData()
        {

        }
    }
}