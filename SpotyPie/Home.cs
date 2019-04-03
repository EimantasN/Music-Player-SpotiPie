using Android.Support.V7.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.Models;
using SpotyPie.RecycleView;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            RvData = new BaseRecycleView<dynamic, dynamic>(this, Resource.Id.main_rv, Data).Setup(LinearLayoutManager.Vertical);
            Task.Run(() => PopulateData());
        }

        public async Task PopulateData()
        {
            RvData.Add(new Item(true));
            await Task.Delay(250);
            RvData.Add(new Item(true));
            await Task.Delay(250);
            RvData.Add(new Item(true));
            await Task.Delay(250);
            RvData.Add(new Item(true));
            await Task.Delay(250);
            RvData.Add(new Item(true));
            await Task.Delay(250);
            RvData.Add(new BlockWithImage(false));
        }
    }
}