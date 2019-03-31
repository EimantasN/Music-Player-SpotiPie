using Android.Content;
using Android.Support.V7.Widget;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using Newtonsoft.Json;
using SpotyPie.Base;
using SpotyPie.Models;
using SpotyPie.Player;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class Browse : FragmentBase
    {
        //Recent albums
        public List<Album> RecentAlbumsData;
        public RvList<BlockWithImage> RecentAlbums;

        //Best albums
        public List<Album> BestAlbumsData;
        public RvList<BlockWithImage> BestAlbums;

        //Best artists
        public List<Artist> BestArtistData;
        public RvList<BlockWithImage> BestArtists;

        //Jump back albums
        public List<Album> JumpBackData;
        public RvList<BlockWithImage> JumpBack;

        //Top playlist
        public List<Playlist> TopPlaylistData;
        public RvList<BlockWithImage> TopPlaylist;

        public override int GetLayout()
        {
            return Resource.Layout.home_layout;
        }

        protected override void InitView()
        {
            base.InitView();
            RecentAlbums = new BaseRecycleView<Album>(this, Resource.Id.recent_rv, RecentAlbumsData).Setup();
            BestAlbums = new BaseRecycleView<Album>(this, Resource.Id.best_albums_rv, BestAlbumsData).Setup();
            //BestArtists = new BaseRecycleView<Album>(this, Resource.Id.best_artists_rv, BestArtistData).Setup();
            JumpBack = new BaseRecycleView<Album>(this, Resource.Id.albums_old_rv, JumpBackData).Setup();
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
                var albums = await GetService().GetListAsync<Album>(AlbumType.Recent);
                InvokeOnMainThread(() =>
                {
                    RecentAlbumsData = albums;
                    foreach (var x in albums)
                    {
                        RecentAlbums.Add(new BlockWithImage(x.Id, RvType.Album, x.Name, JsonConvert.DeserializeObject<List<Artist>>(x.Artists).First().Name, x.Images.First().Url));
                    }
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
                var albums = await GetService().GetListAsync<Album>(AlbumType.Popular);
                InvokeOnMainThread(() =>
                {
                    BestAlbumsData = albums;
                    foreach (var x in albums)
                    {
                        BestAlbums.Add(new BlockWithImage(x.Id, RvType.Album, x.Name, JsonConvert.DeserializeObject<List<Artist>>(x.Artists).First().Name, x.Images.First().Url));
                    }
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
                    BestArtistData = artists;
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

                        BestArtists.Add(new BlockWithImage(x.Id, RvType.Artist, x.Name, DisplayGenre, img));
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
                var albums = await GetService().GetListAsync<Album>(AlbumType.Popular);
                InvokeOnMainThread(() =>
                {
                    JumpBackData = albums;
                    foreach (var x in albums)
                    {
                        JumpBack.Add(new BlockWithImage(x.Id, RvType.Album, x.Name, JsonConvert.DeserializeObject<List<Artist>>(x.Artists).First().Name, x.Images.First().Url));
                    }
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
                    TopPlaylistData = playlists;
                    foreach (var x in playlists)
                    {
                        TopPlaylist.Add(new BlockWithImage(x.Id, RvType.Playlist, x.Name, x.Created.ToString("yyyy-MM-dd"), x.ImageUrl));
                    }
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}