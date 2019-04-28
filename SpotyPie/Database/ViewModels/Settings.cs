using Realms;

namespace SpotyPie.Database.ViewModels
{
    public class Settings : RealmObject
    {
        public bool DataSaver { get; set; }

        public bool ExplicitContentSwitch { get; set; }

        public bool UnplayableSongsSwitch { get; set; }

        public bool AutoplaySwitch { get; set; }

        public bool CustomImagesSwitch { get; set; }

        public bool AutoHeadsetSwitch { get; set; }

        public bool CellularSwitch { get; set; }

        public int MusicQuality { get; set; }

        public Settings()
        {

        }

    }
}