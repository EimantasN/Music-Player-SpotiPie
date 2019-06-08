using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SpotyPie.Monitoring
{
    public class SystemInfo
    {
        public delegate void SystemInfoHandler(dynamic systemInfo, EventArgs e);

        public SystemInfoHandler Handler;

        public int UpdateInterval { get; set; } = 500;

        public EventArgs e = null;

        private API _api;

        private dynamic Data;

        private bool Update = false;

        public void Stop()
        {
            Update = false;
        }

        public SystemInfo(API api)
        {
            _api = api;
            Task.Run(() => StartAsync());
        }

        public async Task StartAsync()
        {
            Update = true;
            while (Update)
            {
                if (_api != null)
                {
                    Data = await _api.GetSystemInfo();
                    Handler?.Invoke(Data, e);
                    await Task.Delay(UpdateInterval);
                }
            }
        }
    }
}