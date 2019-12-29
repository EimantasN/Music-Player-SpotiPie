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
                var album = await _ctx.Albums.Include(x => x.Songs).FirstAsync(x => x.Id == id);
                await UpdateAsync(album);
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
                await UpdateAsync(album);
                if (album == null)
                    throw new Exception("album with id " + id + " not found");
                return album;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task UpdateAsync(Album album)
        {
            try
            {
                album.Popularity++;
                album.LastActiveTime = DateTime.Now.ToUniversalTime();
                _ctx.Entry(album).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();
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

                return artist.Albums.ToList();
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
                    .OrderBy(x => x.LastActiveTime)
                    .Take(12)
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
                    .OrderByDescending(x => x.Popularity)
                    .Take(12)
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
                        .OrderByDescending(x => x.LastActiveTime)
                        .Take(12)
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
