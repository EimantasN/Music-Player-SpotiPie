using SpotyPie.Base;
using SpotyPie.RecycleView;
using System.Collections.Generic;

namespace SpotyPie
{
    public class Home : FragmentBase
    {
        //main_rv

        public List<dynamic> Data;
        public RvList<dynamic> RvData;

        public override int GetLayout()
        {
            return Resource.Layout.browse_layout;
        }

        protected override void InitView()
        {
            base.InitView();
            RvData = new BaseRecycleView<dynamic, dynamic>(this, Resource.Id.main_rv, Data).Setup();
        }
    }
}