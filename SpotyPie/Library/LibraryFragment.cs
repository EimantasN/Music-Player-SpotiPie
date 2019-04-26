using Android.Support.Design.Widget;
using Android.Support.V4.View;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.Library.Fragments;

namespace SpotyPie
{
    public class LibraryFragment : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.library_layout;

        public override void ForceUpdate()
        {

        }

        protected override void InitView()
        {
            GetState().Activity.ActionName.Text = "Library";
            GetState().Activity.ActionName.Alpha = 1.0f;

            TabLayout tabs = RootView.FindViewById<TabLayout>(Resource.Id.tabs);
            ViewPager viewPager = RootView.FindViewById<ViewPager>(Resource.Id.viewpager);

            SetUpViewPager(viewPager);
            tabs.SetupWithViewPager(viewPager);
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            TabAdapter adapter = new TabAdapter(ChildFragmentManager);
            adapter.AddFragment(new Artists(), "Artists");
            adapter.AddFragment(new Albums(), "Albums");
            viewPager.Adapter = adapter;
        }
    }
}