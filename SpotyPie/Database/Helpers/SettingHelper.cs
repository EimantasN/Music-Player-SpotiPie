using Realms;
using System.Linq;

namespace SpotyPie.Database.Helpers
{
    public static class SettingHelper
    {
        public static bool IsCustomImageLoadingOn()
        {
            using (var realm = Realm.GetInstance())
            {
                var settings = realm.All<ViewModels.Settings>().FirstOrDefault();
                if (settings == null)
                {
                    settings = new ViewModels.Settings();
                    realm.Write(() => realm.Add(settings));
                }
                return settings.CustomImagesSwitch;
            }
        }
    }
}