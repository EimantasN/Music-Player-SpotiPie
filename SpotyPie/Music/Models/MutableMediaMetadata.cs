using Android.Text;

namespace SpotyPie.Music.Models
{
    public class MutableMediaMetadata
    {
        public Android.Support.V4.Media.MediaMetadataCompat Metadata { get; set; }

        public string TrackId { get; private set; }

        public MutableMediaMetadata(string trackId, Android.Support.V4.Media.MediaMetadataCompat metadata)
        {
            Metadata = metadata;
            TrackId = trackId;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null || obj.GetType() != typeof(MutableMediaMetadata))
                return false;
            var that = (MutableMediaMetadata)obj;
            return TextUtils.Equals(TrackId, that.TrackId);
        }

        public override int GetHashCode()
        {
            return TrackId.GetHashCode();
        }
    }
}