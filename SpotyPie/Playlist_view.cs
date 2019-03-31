using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Player
{
    public class Playlist_view : SupportFragment
    {
        View RootView;

        //Album Songs
        public static List<Item> AlbumSongsItem = new List<Item>();
        public static RvList<Item> AlbumSongs = new RvList<Item>();
        private RecyclerView.LayoutManager AlbumSongsLayoutManager;
        private static RecyclerView.Adapter AlbumSongsAdapter;
        private static RecyclerView AlbumSongsRecyclerView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.player_song_list, container, false);

            //ALBUM song list
            AlbumSongsLayoutManager = new LinearLayoutManager(this.Activity);
            AlbumSongsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.song_list);
            AlbumSongsRecyclerView.SetLayoutManager(AlbumSongsLayoutManager);
            AlbumSongsAdapter = new VerticalRV(AlbumSongs, this.Context);
            AlbumSongs.Adapter = AlbumSongsAdapter;
            AlbumSongsRecyclerView.SetAdapter(AlbumSongsAdapter);
            AlbumSongsRecyclerView.NestedScrollingEnabled = false;

            AlbumSongsRecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (AlbumSongsRecyclerView != null && AlbumSongsRecyclerView.ChildCount != 0)
                {
                    Current_state.SetSong(Current_state.Current_Song_List[position]);
                }
            });

            return RootView;
        }

        public override void OnResume()
        {
            Task.Run(() => GetSongsAsync(Current_state.Current_Playlist.Id));
            base.OnResume();
        }

        public async Task GetSongsAsync(int id)
        {
            try
            {
                RestClient Client = new RestClient("http://pie.pertrauktiestaskas.lt/api/Playlist/" + id + "/tracks");
                var request = new RestRequest(Method.GET);
                IRestResponse response = await Client.ExecuteGetTaskAsync(request);
                if (response.IsSuccessful)
                {
                    Playlist album = JsonConvert.DeserializeObject<Playlist>(response.Content);
                    await AlbumSongs.ClearAsync();
                    Application.SynchronizationContext.Post(_ =>
                    {
                        Current_state.Current_Song_List = album.Items;
                        foreach (var x in album.Items)
                        {
                            AlbumSongs.Add(x);
                        }
                        //List<Copyright> Copyright = JsonConvert.DeserializeObject<List<Copyright>>(album.Created);
                    }, null);
                }
                else
                {
                    Application.SynchronizationContext.Post(_ =>
                    {
                        Toast.MakeText(this.Context, "GetSongsAsync API call error", ToastLength.Short).Show();
                    }, null);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}