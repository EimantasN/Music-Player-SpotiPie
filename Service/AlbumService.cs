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
                var album = await _ctx.Albums.FirstOrDefaultAsync(x => x.Id == id);
                if (album == null)
                    throw new Exception("album with id " + id + " not found");

                album.Popularity++;
                album.LastActiveTime = DateTime.Now.ToUniversalTime();
                _ctx.Entry(album).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();

                return album;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Album>> GetAlbumsAsync(int count = 10)
        {
            try
            {
                return await _ctx.Albums
                    .AsNoTracking()
                    .Where(x => x.IsPlayable == true)
                    .Take(count)
                    .Where(x => x.IsPlayable == true)
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
                return await _ctx.Albums
                    .AsNoTracking()
                    .Where(x => x.IsPlayable)
                    .Where(x => x.Name.Contains(query))
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
