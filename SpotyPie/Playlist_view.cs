using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Player
{
    public class Playlist_view : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.player_song_list;

        //Album Songs
        public List<Song> AlbumSongsItem = new List<Song>();
        public RvList<Song> AlbumSongs = new RvList<Song>();
        private RecyclerView.LayoutManager AlbumSongsLayoutManager;
        private RecyclerView.Adapter AlbumSongsAdapter;
        private RecyclerView AlbumSongsRecyclerView;

        protected override void InitView()
        {
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
                    GetState().SetSong(GetState().Current_Song_List[position]);
                }
            });
        }

        public override void OnResume()
        {
            Task.Run(() => GetSongsAsync(GetState().Current_Playlist.Id));
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
                    AlbumSongs.Clear();
                    Application.SynchronizationContext.Post(_ =>
                    {
                        GetState().Current_Song_List = album.Songs;
                        foreach (var x in album.Songs)
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