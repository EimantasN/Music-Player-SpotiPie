using Android.Content;
using Android.Support.V7.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using Newtonsoft.Json;
using SpotyPie.Base;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class Browse : FragmentBase
    {
        //Recent albums
        public RvList<Album> RecentAlbums;

        //Best albums
        public RvList<Album> BestAlbums;

        //Best artists
        public RvList<Album> BestArtists;

        //Jump back albums
        public RvList<Album> JumpBack;

        //Top playlist
        public RvList<Album> TopPlaylist;

        public override int LayoutId { get; set; } = Resource.Layout.home_layout;

        protected override void InitView()
        {
            RecentAlbums = new BaseRecycleView<Album>(this, Resource.Id.recent_rv).Setup(LinearLayoutManager.Horizontal);
            BestAlbums = new BaseRecycleView<Album>(this, Resource.Id.best_albums_rv).Setup(LinearLayoutManager.Horizontal);
            //BestArtists = new BaseRecycleView<Album>(this, Resource.Id.best_artists_rv, BestArtistData).Setup();
            JumpBack = new BaseRecycleView<Album>(this, Resource.Id.albums_old_rv).Setup(LinearLayoutManager.Horizontal);
            //TopPlaylist = new BaseRecycleView<Album>(this, Resource.Id.playlist_rv, TopPlaylistData).Setup();
        }

        public override void OnResume()
        {
            base.OnResume();
            Task.Run(() => GetRecentAlbumsAsync(this.Context));

            Task.Run(() => GetPolularAlbumsAsync(this.Context));

            //Task.Run(() => GetPolularArtistsAsync(this.Context));

            Task.Run(() => GetOldAlbumsAsync(this.Context));

            //Task.Run(() => GetPlaylists(this.Context));
        }

        public async Task GetRecentAlbumsAsync(Context cnt)
        {
            try
            {
                var api = (AlbumService)GetService(ApiServices.Albums);
                var albums = await api.GetRecent();
                InvokeOnMainThread(() =>
                {
                    albums.ForEach(x => RecentAlbums.Add(x));
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetPolularAlbumsAsync(Context cnt)
        {
            try
            {
                var api = (AlbumService)GetService(ApiServices.Albums);
                var albums = await api.GetPopular();
                InvokeOnMainThread(() =>
                {
                    albums.ForEach(x => BestAlbums.Add(x));
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetPolularArtistsAsync(Context cnt)
        {
            try
            {
                var artists = await GetService().GetListAsync<Artist>(ArtistType.Popular);
                InvokeOnMainThread(() =>
                {
                    //BestArtistData = artists;
                    foreach (var x in artists)
                    {
                        string DisplayGenre;
                        List<string> genres = new List<string>();

                        if (x.Genres != null)
                            genres = JsonConvert.DeserializeObject<List<string>>(x.Genres);

                        if (genres.Count > 1)
                        {
                            Random ran = new Random();
                            int index = ran.Next(0, genres.Count - 1);
                            DisplayGenre = genres[index];
                        }
                        else if (genres.Count == 1)
                        {
                            DisplayGenre = genres[0];
                        }
                        else
                            DisplayGenre = string.Empty;

                        var img = string.Empty;
                        if (x.Images.FirstOrDefault() != null)
                            img = x.Images.First().Url;

                        //BestArtists.Add(new BlockWithImage(x.Id, RvType.Artist, x.Name, DisplayGenre, img));
                    }
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetOldAlbumsAsync(Context cnt)
        {
            try
            {
                var api = (AlbumService)GetService(ApiServices.Albums);
                var albums = await api.GetOld();

                InvokeOnMainThread(() =>
                {
                    albums.ForEach(x => JumpBack.Add(x));
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetPlaylists(Context cnt)
        {
            try
            {
                var playlists = await GetService().GetListAsync<Playlist>(PlaylistType.Playlists);
                InvokeOnMainThread(() =>
                {
                    //TopPlaylistData = playlists;
                    //foreach (var x in playlists)
                    //{
                    //    TopPlaylist.Add(new BlockWithImage(x.Id, RvType.Playlist, x.Name, x.Created.ToString("yyyy-MM-dd"), x.ImageUrl));
                    //}
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}