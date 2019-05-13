using Android.OS;
using Android.Views;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Library.Fragments
{
    public class AlbumsFragment : SupportFragment
    {
        View RootView;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.library_album_layout, container, false);
            return RootView;
        }
    }
}