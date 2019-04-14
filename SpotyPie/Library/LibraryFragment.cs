using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using SpotyPie.Base;
using SpotyPie.Library.Fragments;
using System.Collections.Generic;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;


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
            TabAdapter adapter = new TabAdapter(GetState().Activity.SupportFragmentManager);
            adapter.AddFragment(new Artists(), "Artists");
            adapter.AddFragment(new Albums(), "Albums");
            viewPager.Adapter = adapter;
        }

        public class TabAdapter : FragmentPagerAdapter
        {
            public List<SupportFragment> Fragments { get; set; }
            public List<string> FragmentsNames { get; set; }

            public TabAdapter(SupportFragmentManager sfm) : base(sfm)
            {
                Fragments = new List<SupportFragment>();
                FragmentsNames = new List<string>();
            }

            public void AddFragment(SupportFragment fragment, string name)
            {
                Fragments.Add(fragment);
                FragmentsNames.Add(name);
            }

            public override int Count => Fragments.Count;

            public override SupportFragment GetItem(int position)
            {
                return Fragments[position];
            }

            public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
            {
                return new Java.Lang.String(FragmentsNames[position]);
            }
        }
    }
}