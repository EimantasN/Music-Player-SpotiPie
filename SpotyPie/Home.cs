using Android.Support.V7.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using Mobile_Api.Models.Rv;
using SpotyPie.Base;
using SpotyPie.Models;
using SpotyPie.RecycleView;
using System.Collections.Generic;
using System.Linq;
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
            RvData = new BaseRecycleView<dynamic>(this, Resource.Id.main_rv).Setup(LinearLayoutManager.Vertical);
            Task.Run(() => PopulateData());
        }

        public async Task PopulateData()
        {
            await Task.Run(() => LoadSongs());
        }

        public async Task LoadSongs()
        {
            RvData.Add(null);
            var api = (SongService)GetService(ApiServices.Songs);

            var data = await api.GetRecent();
            data.Take(2).ToList().ForEach(x => RvData.Add(x));

            data = await api.GetPopular();
            data.Take(2).ToList().ForEach(x => RvData.Add(x));
            RvData.RemoveLoading();
        }
    }
}