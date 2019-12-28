using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SpotyPie.Base;
using SpotyPie.Enums;

namespace SpotyPie.HelperFragments
{
    public class NoInternet : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.no_internet;

        protected override LayoutScreenState ScreenState { get; set; } = LayoutScreenState.FullScreen;

        protected override void InitView()
        {

        }

        public override void ForceUpdate()
        {
        }

        public override int GetParentView()
        {
            return RootView.Id;
        }

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            return null;
        }

        public override void ReleaseData()
        {
        }
    }
}