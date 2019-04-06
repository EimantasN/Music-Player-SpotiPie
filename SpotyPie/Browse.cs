using Android.Support.V7.Widget;
using Mobile_Api;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.RecycleView;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class Browse : FragmentBase
    {
        //main_rv
        private static RvList<dynamic> RvData;

        public override int LayoutId { get; set; } = Resource.Layout.browse_layout;

        protected override void InitView()
        {
            if (RvData == null)
            {
                var rvBase = new BaseRecycleView<dynamic>(this, Resource.Id.main_rv);
                RvData = rvBase.Setup(LinearLayoutManager.Vertical);
                rvBase.DisableScroolNested();
            }
            Task.Run(() => PopulateData());
        }

        public async Task PopulateData()
        {
            await Task.Run(() => LoadSongs());
        }

        public async Task LoadSongs()
        {
            List<dynamic> data = new List<dynamic>() { null };
            RvData.AddList(data);
            var api = (SongService)GetService(ApiServices.Songs);

            data.AddRange(await api.GetRecent());
            RvData.AddList(data);

            data.AddRange(await api.GetPopular());
            RvData.AddList(data);
            RvData.RemoveLoading(data);
        }
    }
}