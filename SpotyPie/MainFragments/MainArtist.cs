using Android.App;
using Android.Support.V7.Widget;
using Android.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class MainArtist : FragmentBase
    {
        protected override Enums.LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Default;

        //main_rv
        private BaseRecycleView<Songs> RvData { get; set; }

        public override int LayoutId { get; set; } = Resource.Layout.browse_layout;

        protected override void InitView()
        {
        }

        public async Task PopulateData()
        {

        }

        public override void ForceUpdate()
        {
        }

        public override void ReleaseData()
        {
            if (RvData != null)
            {
                RvData.Dispose();
                RvData = null;
            }
        }

        public override int GetParentView()
        {
            throw new NotImplementedException();
        }

        public override void LoadFragment(dynamic switcher)
        {
            throw new NotImplementedException();
        }
    }
}