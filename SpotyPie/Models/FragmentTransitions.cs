using SpotyPie.Base;

namespace SpotyPie.Models
{
    public static class FragmentTransitions
    {
        public static void SetCustomTransitions(ref Android.Support.V4.App.FragmentTransaction transaction, FragmentBase fragment)
        {
            if (fragment is Player.PlayerSongList)
            {
                transaction.SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_right);
            }
            else if (fragment is SongOptionsFragment)
            {
                transaction.SetCustomAnimations(Resource.Animation.enter_from_bottom, Resource.Animation.exit_to_bottom);
            }
            else
            {
                //transaction.SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_right);
            }
        }
    }
}