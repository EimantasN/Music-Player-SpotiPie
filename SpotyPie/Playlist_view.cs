using Android.App;
using Android.Support.V7.Widget;
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

namespace SpotyPie.Player
{
    public class Playlist_view : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.player_song_list;

        ////Album Songs
        //public List<Songs> AlbumSongsItem = new List<Songs>(this);
        //public RvList<Songs> AlbumSongs = new RvList<Songs>(this);
        private RecyclerView.LayoutManager AlbumSongsLayoutManager;
        private RecyclerView.Adapter AlbumSongsAdapter;
        private RecyclerView AlbumSongsRecyclerView;

        protected override void InitView()
        {
            ////ALBUM song list
            //AlbumSongsLayoutManager = new LinearLayoutManager(this.Activity);
            //AlbumSongsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.song_list);
            //AlbumSongsRecyclerView.SetLayoutManager(AlbumSongsLayoutManager);
            //AlbumSongsAdapter = new VerticalRV(AlbumSongs, this.Context);
            //AlbumSongs.Adapter = AlbumSongsAdapter;
            //AlbumSongsRecyclerView.SetAdapter(AlbumSongsAdapter);
            //AlbumSongsRecyclerView.NestedScrollingEnabled = false;

            //AlbumSongsRecyclerView.SetItemClickListener((rv, position, view) =>
            //{
            //    if (AlbumSongsRecyclerView != null && AlbumSongsRecyclerView.ChildCount != 0)
            //    {
            //        GetState().SetSong(GetState().Current_Song_List, position);
            //    }
            //});
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public async Task GetSongsAsync(int id)
        {
            try
            {
                RestClient Client = new RestClient("https://pie.pertrauktiestaskas.lt/api/Playlist/" + id + "/tracks");
                var request = new RestRequest(Method.GET);
                IRestResponse response = await Client.ExecuteGetTaskAsync(request);
                if (response.IsSuccessful)
                {
                    Playlist album = JsonConvert.DeserializeObject<Playlist>(response.Content);
                    //AlbumSongs.Clear();
                    Application.SynchronizationContext.Post(_ =>
                    {
                        GetState().Current_Song_List = album.Songs;
                        foreach (var x in album.Songs)
                        {
                            //AlbumSongs.Add(x);
                        }
                        //List<Copyright> Copyright = JsonConvert.DeserializeObject<List<Copyright>>(album.Created);
                    }, null);
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        Toast.MakeText(this.Context, "GetSongsAsync API call error", ToastLength.Short).Show();
                    });
                }
            }
            catch (Exception)
            {

            }
        }

        public override void ForceUpdate()
        {
            throw new NotImplementedException();
        }

        public override void ReleaseData()
        {
            throw new NotImplementedException();
        }

        public override int GetParentView()
        {
            throw new NotImplementedException();
        }

        public override void LoadFragment(dynamic switcher)
        {
            throw new NotImplementedException();
        }
    }
}