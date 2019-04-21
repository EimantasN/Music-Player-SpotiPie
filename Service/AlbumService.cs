using Database;
using Microsoft.EntityFrameworkCore;
using Models.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class AlbumService : IAlbumService
    {
        private readonly SpotyPieIDbContext _ctx;

        public AlbumService(SpotyPieIDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Album> GetAlbumAsync(int id)
        {
            try
            {
                //Task.Run(() => Update(id));
                var album = await _ctx.Albums.FirstAsync(x => x.Id == id);
                Update(album);
                if (album == null)
                    throw new Exception("album with id " + id + " not found");
                return album;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Album> UpdateAlbum(int id)
        {
            try
            {
                //Task.Run(() => Update(id));
                var album = await _ctx.Albums.FirstAsync(x => x.Id == id);
                Update(album);
                if (album == null)
                    throw new Exception("album with id " + id + " not found");
                return album;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Update(Album album)
        {
            try
            {
                album.Popularity++;
                album.LastActiveTime = DateTime.Now.ToUniversalTime();
                _ctx.Entry(album).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            catch (Exception e)
            {
            }
        }

        public async Task<List<Album>> GetAlbumsAsync(int count = int.MaxValue)
        {
            try
            {
                return await _ctx.Albums
                    .AsNoTracking()
                    .Where(x => x.IsPlayable == true)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Album>> GetAlbumsByArtistAsync(int id)
        {
            try
            {
                var artist = await _ctx.Artists
                    .AsNoTracking()
                    .Include(x => x.Albums)
                    .Select(x => new { x.Id, x.Albums })
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (artist == null)
                    throw new Exception("Cant find artist with id - " + id);

                return artist.Albums.Where(x => x.IsPlayable).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Album>> GetOldAlbumsAsync()
        {
            try
            {
                return await _ctx.Albums
                    .AsNoTracking()
                    .Where(x => x.IsPlayable)
                    .OrderByDescending(x => x.LastActiveTime).ThenBy(x => x.Popularity)
                    .OrderByDescending(x => x.LastActiveTime)
                    .Take(6)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Album>> GetPopularAlbumsAsync()
        {
            try
            {
                return await _ctx.Albums
                    .AsNoTracking()
                    .Where(x => x.IsPlayable)
                    .OrderByDescending(x => x.Popularity)
                    .Take(6)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Album>> GetRecentAlbumsAsync()
        {
            try
            {
                return await _ctx.Albums
                        .AsNoTracking()
                        .Where(x => x.IsPlayable)
                        .OrderByDescending(x => x.LastActiveTime)
                        .Take(6)
                        .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Album>> Search(string query)
        {
            try
            {
                query = $"SELECT * FROM[SpotyPie].[dbo].[Albums] where IsPlayable=1 AND Name Like '%{query.Replace("'", "\"")}%'";

                return await _ctx.Albums
                    .FromSql(query)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
