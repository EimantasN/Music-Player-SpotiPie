using Android.App;

namespace SpotyPie.Helpers
{
    public static class InternetConnection
    {
        public static bool Check()
        {
            if (Plugin.Connectivity.CrossConnectivity.IsSupported)
            {
                if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
                {
                    return true;
                }
            }
            return false;
        }
    }
}