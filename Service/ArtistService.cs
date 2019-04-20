using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Microsoft.EntityFrameworkCore;
using Models.BackEnd;
using Newtonsoft.Json;

namespace Service
{
    public class ArtistService : IArtistService
    {
        private readonly SpotyPieIDbContext _ctx;

        public ArtistService(SpotyPieIDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Artist> GetArtistAsync(int id)
        {
            try
            {
                //Task.Run(() => Update(id));
                var album = await _ctx.Artists
                    .Include(x => x.Albums)
                    .ThenInclude(x => x.Songs)
                    .FirstAsync(x => x.Id == id);

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

        public async Task<Artist> UpdateArtist(int id)
        {
            try
            {
                //Task.Run(() => Update(id));
                var artist = await _ctx.Artists.FirstAsync(x => x.Id == id);
                Update(artist);
                if (artist == null)
                    throw new Exception("album with id " + id + " not found");
                return artist;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Update(Artist artist)
        {
            try
            {
                artist.Popularity++;
                artist.LastActiveTime = DateTime.Now.ToUniversalTime();
                _ctx.Entry(artist).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            catch (Exception e)
            {
            }
        }

        public async Task<List<Artist>> GetArtistsAsync(int count = int.MaxValue)
        {
            try
            {
                return await _ctx.Artists
                    .AsNoTracking()
                    .Include(x => x.Albums)
                    .ThenInclude(x => x.Songs)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Artist>> GetOldArtistsAsync()
        {
            try
            {
                return await _ctx.Artists
                    .AsNoTracking()
                    .Include(x => x.Albums)
                    .ThenInclude(x => x.Songs)
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

        public async Task<List<Artist>> GetPopularArtistsAsync()
        {
            try
            {
                return await _ctx.Artists
                    .AsNoTracking()
                    .Include(x => x.Albums)
                    .ThenInclude(x => x.Songs)
                    .OrderByDescending(x => x.Popularity)
                    .Take(6)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Artist>> GetRecentArtistsAsync()
        {
            try
            {
                return await _ctx.Artists
                        .AsNoTracking()
                        .Include(x => x.Albums)
                        .ThenInclude(x => x.Songs)
                        .OrderByDescending(x => x.LastActiveTime)
                        .Take(6)
                        .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Artist>> SearchAsync(string query)
        {
            try
            {
                query = $"SELECT * FROM[SpotyPie].[dbo].[Artists] where Name Like '%{query.Replace("'", "\"")}%'";

                return await _ctx.Artists
                    .FromSql(query)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Song>> GetArtistTopTracksAsync(int id)
        {
            try
            {
                var Artist = await _ctx.Artists
                    .Include(x => x.Albums)
                    .ThenInclude(x => x.Songs)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (Artist == null)
                    throw new Exception("Artist not found");

                List<Song> PopularSongs = new List<Song>();
                Artist.Albums.ForEach(x => PopularSongs.Add(x.Songs.OrderByDescending(y => y.Popularity).First()));
                return PopularSongs;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Artist>> GetRelatedArtistsAsync(int id)
        {
            try
            {
                var Artist = await _ctx.Artists.FirstOrDefaultAsync(x => x.Id == id);
                if (Artist == null)
                    throw new Exception("Artist not found in database");

                var sql = FormatSql(JsonConvert.DeserializeObject<List<string>>(Artist.Genres));
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }

            string FormatSql(List<string> genres)
            {
                for (int i = 0; i < genres.Count; i++)
                {
                    genres[i] = $"Genres LIKE '%{genres[i].Replace("'", "\"")}%'";
                }

                return $"SELECT * FROM [SpotyPie].[dbo].[Artists] where {string.Join(" OR ", genres)};";
            }
        }
    }
}
