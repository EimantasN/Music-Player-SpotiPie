using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.RecycleView;
using System;

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

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            return null;
        }
    }
}