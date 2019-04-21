using Android.App;
using Android.Support.V7.Widget;
using Android.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class Browse : FragmentBase
    {
        //main_rv
        private RvList<dynamic> RvData { get; set; }

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
            try
            {
                List<dynamic> data = new List<dynamic>() { null };
                RvData.AddList(data);
                var api = GetService();

                data.AddRange(await api.GetRecent<Songs>());
                RvData.AddList(data);

                data.AddRange(await api.GetPopular<Songs>());
                RvData.AddList(data);

                RvData.RemoveLoading(data);
            }
            catch (Exception e)
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    Toast.MakeText(this.Context, e.Message, ToastLength.Long).Show();
                }, null);
            }
        }

        public override void ForceUpdate()
        {
            Task.Run(() => PopulateData());
        }
    }
}