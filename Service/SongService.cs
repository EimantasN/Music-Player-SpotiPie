﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Config.Net;
using Database;
using Microsoft.EntityFrameworkCore;
using Models.FrontEnd;
using Service.Settings;

namespace Services
{
    public class SongService : ISongService
    {
        private readonly SpotyPieIDbContext _ctx;

        public SongService(SpotyPieIDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Song> GetAsync(int id)
        {
            try
            {
                var song = await _ctx.Items.AsNoTracking()
                    .Select(x => new Song(x.Id, x.Name, x.Artists))
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

        public async Task<List<Song>> GetAllAsync(int count)
        {
            try
            {
                return await _ctx.Items.AsNoTracking()
                    .Select(x => new Song(x.Id, x.Name, x.Artists))
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Song>> GetRecentAsync(int count = 10)
        {
            try
            {
                return await _ctx.Items.AsNoTracking()
                     .OrderBy(x => x.LastActiveTime)
                    .Select(x => new Song(x.Id, x.Name, x.Artists))
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Song>> GetPopularAsync(int count = 10)
        {
            try
            {
                return await _ctx.Items.AsNoTracking()
                    .OrderBy(x => x.Popularity)
                    .Select(x => new Song(x.Id, x.Name, x.Artists))
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Song>> GetSongsByAlbumAsync(int albumId)
        {
            try
            {
                var Album = await _ctx.Albums.Include(x => x.Songs).AsNoTracking().Select(x => new { x.Id, x.Songs }).FirstAsync(x => x.Id == albumId);
                if (Album == null)
                    throw new Exception("Cant find album");

                return Song.ConvertList(Album.Songs);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                var song = await _ctx.Items.FirstOrDefaultAsync(x => x.Id == id);
                if (song == null)
                    throw new Exception("Cant remove song");

                _ctx.Items.Remove(song);
                await _ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Song>> SearchAsync(string query)
        {
            try
            {
                return await _ctx.Items.AsNoTracking()
                    .Select(x => new Song(x.Id, x.Name, x.Artists))
                    .Where(x => x.Title.Contains(query))
                    .Take(10)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task UpdateAsync(int id)
        {
            try
            {
                var song = await _ctx.Items.FirstOrDefaultAsync(x => x.Id == id);
                if (song != null)
                {
                    song.LastActiveTime = DateTime.Now.ToUniversalTime();
                    song.Popularity++;

                    _ctx.Entry(song).State = EntityState.Modified;
                    await _ctx.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
