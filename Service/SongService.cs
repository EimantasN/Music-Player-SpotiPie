using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Config.Net;
using Database;
using Microsoft.EntityFrameworkCore;
using Models.FrontEnd;
using Service.Settings;

namespace Service
{
    public class SongService : ISongService
    {
        private readonly SpotyPieIDbContext _ctx;
        private ISettings settings;

        public SongService(SpotyPieIDbContext ctx)
        {
            _ctx = ctx;
            settings = new ConfigurationBuilder<ISettings>()
                .UseJsonFile(Environment.CurrentDirectory + @"/settings.json")
                .Build();
        }

        public async Task<Song> GetAsync(int id)
        {
            try
            {
                var song = await _ctx.Items.AsNoTracking()
                    .Select(x => new Song(x.Id, x.Name, x.Popularity.ToString()))
                    .FirstOrDefaultAsync(x => x.Id == id);

                //TODO
                //Task.Run(() => _ctx.UpdateAsync(id));
                return song;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Song>> GetAllAsync()
        {
            try
            {
                return await _ctx.Items.AsNoTracking()
                    .Select(x => new Song(x.Id, x.Name, x.Popularity.ToString()))
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task<List<Song>> GetRecent(int count = 10)
        {
            try
            {
                return await _ctx.Items.AsNoTracking()
                     .OrderBy(x => x.LastActiveTime)
                    .Select(x => new Song(x.Id, x.Name, x.Popularity.ToString()))
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task GetSongsByAlbum(int albumId)
        {
            throw new System.NotImplementedException();
        }

        public Task Remove(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Song>> Search(string query)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateAsync(int id)
        {
            try
            {
                var song = await _ctx.Items.FirstAsync(x => x.Id == id);
                song.LastActiveTime = DateTime.Now.ToUniversalTime();
                song.Popularity++;
                _ctx.Entry(song).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {

            }
        }
    }
}
