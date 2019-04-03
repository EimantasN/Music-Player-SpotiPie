using Database;
using Microsoft.EntityFrameworkCore;
using Models.FrontEnd;
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

        public async Task<BlockWithImage> GetAlbumAsync(int id)
        {
            try
            {
                var album = await _ctx.Albums.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);
                if (album == null)
                    throw new Exception("album with id " + id + " not found");

                album.Popularity++;
                album.LastActiveTime = DateTime.Now.ToUniversalTime();
                _ctx.Entry(album).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();

                return new BlockWithImage(album.Id, RvType.Album, album.Name, album.Label, album.Images?.First().Url);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<BlockWithImage>> GetAlbumsAsync(int count = 10)
        {
            try
            {
                return await _ctx.Albums
                    .AsNoTracking()
                    .Include(x => x.Images)
                    .Select(x => new BlockWithImage(x.Id, RvType.Album, x.Name, x.Label, x.Images[0].Url))
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<BlockWithImage>> GetAlbumsByArtistAsync(int id)
        {
            try
            {
                var artist = await _ctx.Artists
                    .AsNoTracking()
                    .Include(x => x.Albums)
                    .ThenInclude(x => x.Images)
                    .Select(x => new { x.Id, x.Albums })
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (artist == null)
                    throw new Exception("Cant find artist with id - " + id);

                List<BlockWithImage> Albums = new List<BlockWithImage>();
                artist.Albums.ForEach(x =>
                {
                    Albums.Add(new BlockWithImage(x.Id, RvType.Album, x.Name, x.Label, x.Images?[0].Url));
                });

                return Albums;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<BlockWithImage>> GetOldAlbumsAsync()
        {
            try
            {
                return await _ctx.Albums
                    .AsNoTracking()
                    .Include(x => x.Images)
                    .Where(x => x.Popularity >= 1)
                    .OrderByDescending(x => x.LastActiveTime)
                    .Select(x => new BlockWithImage(x.Id, RvType.Album, x.Name, x.Label, x.Images[0].Url))
                    .Take(6)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<BlockWithImage>> GetPopularAlbumsAsync()
        {
            try
            {
                return await _ctx.Albums
                    .AsNoTracking()
                    .Include(x => x.Images)
                    .OrderByDescending(x => x.Popularity)
                    .Select(x => new BlockWithImage(x.Id, RvType.Album, x.Name, x.Label, x.Images[0].Url))
                    .Take(6)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<BlockWithImage>> GetRecentAlbumsAsync()
        {
            try
            {
                return await _ctx.Albums
                        .AsNoTracking()
                        .Include(x => x.Images)
                        .OrderByDescending(x => x.LastActiveTime)
                        .Select(x => new BlockWithImage(x.Id, RvType.Album, x.Name, x.Label, x.Images[0].Url))
                        .Take(6)
                        .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<BlockWithImage>> Search(string query)
        {
            try
            {
                return await _ctx.Albums
                    .AsNoTracking()
                    .Include(x => x.Images)
                    .Where(x => x.Name.Contains(query))
                    .Take(6)
                    .Select(x => new BlockWithImage(x.Id, RvType.Album, x.Name, x.Label, x.Images[0].Url))
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
