using Android.OS;
using Android.Views;
using Android.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
namespace SpotyPie.Player
{
    public class PlaylistFragment : SupportFragment
    {
        View RootView;

        FrameLayout Container;//choose_playlist
        Button AddToPlaylist;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.playlist_add_layout, container, false);
            Container = RootView.FindViewById<FrameLayout>(Resource.Id.choose_playlist);

            AddToPlaylist = RootView.FindViewById<Button>(Resource.Id.add_to_album);
            AddToPlaylist.Click += AddToPlaylist_Click;
            return RootView;
        }

        private void AddToPlaylist_Click(object sender, System.EventArgs e)
        {
            LoadLists();
        }

        public void LoadLists()
        {
            ChildFragmentManager.BeginTransaction()
                .Replace(Resource.Id.choose_playlist, new PlaylistChoose())
                .Commit();
            Container.TranslationX = 0;
        }
    }
}