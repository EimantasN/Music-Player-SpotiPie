using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database;
using IdSharp.Tagging.VorbisComment;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.BackEnd;
using Service.Helpers;

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
                var song = await _ctx.Songs
                    .FirstOrDefaultAsync(x => x.Id == id);
                Update(song);
                return song;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Update(Song song)
        {
            try
            {
                song.Popularity++;
                song.LastActiveTime = DateTime.Now.ToUniversalTime();
                _ctx.Entry(song).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            catch (Exception e)
            {

            }
        }

        public async Task<List<Song>> GetAllAsync(int count)
        {
            try
            {
                return await _ctx.Songs
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

        public async Task<List<Song>> GetRecentAsync(int count = 10)
        {
            try
            {
                return await _ctx.Songs
                    .AsNoTracking()
                    .Where(x => x.IsPlayable == true)
                    .OrderBy(x => x.LastActiveTime)
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
                return await _ctx.Songs
                    .AsNoTracking()
                    .Where(x => x.IsPlayable == true)
                    .OrderBy(x => x.Popularity)
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
                var Album = await _ctx.Albums
                    .AsNoTracking()
                    .Include(x => x.Songs)
                    .Select(x => new { x.Id, x.Songs }).
                    FirstAsync(x => x.Id == albumId);
                if (Album == null)
                    throw new Exception("Cant find album");

                return Album.Songs.Where(x => x.IsPlayable == true).ToList();
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
                var song = await _ctx.Songs.
                    FirstOrDefaultAsync(x => x.Id == id);
                if (song == null)
                    throw new Exception("Cant remove song");

                _ctx.Songs.Remove(song);
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
                return await _ctx.Songs
                    .AsNoTracking()
                    .Where(x => x.IsPlayable == true)
                    .Where(x => x.Name.Contains(query))
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
                var song = await _ctx.Songs
                    .FirstOrDefaultAsync(x => x.Id == id);
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

        private void CheckForDirectory(string path)
        {
            string[] directorys = path.Replace(GetEnviromentPath(), "").Split(Path.DirectorySeparatorChar);

            string tempPath = GetEnviromentPath();
            foreach (var x in directorys)
            {
                if (!string.IsNullOrEmpty(x))
                {
                    tempPath += Path.DirectorySeparatorChar + x;
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);
                }
            }
        }

        public async Task<List<AudioBindError>> BindData()
        {
            List<AudioBindError> Errors = new List<AudioBindError>();
            try
            {
                foreach (var path in System.IO.Directory.EnumerateFiles(GetEnviromentPath()))
                {
                    try
                    {
                        var data = await SaveFileAsync(path);
                        if (data != null)
                            Errors.Add(data);
                    }
                    catch (Exception e)
                    {
                        Errors.Add(new AudioBindError(e));
                    }
                }
                return Errors;
            }
            catch (Exception e)
            {
                Errors.Add(new AudioBindError(e));
                return Errors;
            }
        }

        public async Task<List<AudioBindError>> AddAudioToLibrary(IFormFile file)
        {
            List<AudioBindError> Errors = new List<AudioBindError>();
            try
            {
                using (var fileStream = new FileStream(GetEnviromentPath() + Path.DirectorySeparatorChar + file.FileName, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                try
                {
                    var data = await SaveFileAsync(GetEnviromentPath() + Path.DirectorySeparatorChar + file.FileName);
                    if (data != null)
                        Errors.Add(data);
                }
                catch (Exception e)
                {
                    Errors.Add(new AudioBindError(e));
                }
                return Errors;
            }
            catch (Exception e)
            {
                Errors.Add(new AudioBindError(e));
                return Errors;
            }
        }

        private async Task<AudioBindError> SaveFileAsync(string filePath)
        {
            var flacTag = new VorbisComment(filePath);

            if (string.IsNullOrEmpty(flacTag.Album) || string.IsNullOrEmpty(flacTag.Title) || string.IsNullOrEmpty(flacTag.Artist))
            {
                return new AudioBindError(filePath, flacTag.Artist, flacTag.Album, flacTag.Title, "Wrong Flac");
            }

            var artist = await _ctx.Artists.FirstOrDefaultAsync(x => x.Name.ToLower().Trim() == flacTag.Artist.ToLower().Trim());
            if (artist == null)
                return new AudioBindError(filePath, flacTag.Artist, flacTag.Album, flacTag.Title, "Artist not found in database");

            CheckForDirectory(
                GetEnviromentPath() +
                Path.DirectorySeparatorChar +
                Replacer.RemoveSpecialCharacters(flacTag.Artist.ToLower().Trim()));

            string AlbumName = "";
            Replacer.CorrentAlbum(flacTag.Album);
            var album = await _ctx.Albums.FirstOrDefaultAsync(x => x.Name.ToLower().Trim().Contains(flacTag.Album.ToLower().Trim()));
            if (album == null)
            {
                int ID = FindSimilarName.findSimilarAlbumName(await _ctx.Albums.AsNoTracking().ToListAsync(), flacTag.Album);
                if (ID == 0)
                {
                    return new AudioBindError(filePath, flacTag.Artist, flacTag.Album, flacTag.Title, "Album not found in database");
                }
                else
                {
                    album = await _ctx.Albums.FirstAsync(x => x.Id == ID);
                    AlbumName = album.Name.ToLower().Trim();
                }
            }
            else
            {
                AlbumName = album.Name.ToLower().Trim();
            }

            CheckForDirectory(
                GetEnviromentPath() +
                Path.DirectorySeparatorChar +
                flacTag.Artist.ToLower().Trim() +
                Path.DirectorySeparatorChar +
                Replacer.RemoveSpecialCharacters(AlbumName));

            string destinationPath = "";
            string SongName = null;
            var song = await _ctx.Songs.FirstOrDefaultAsync(x => x.Name.Replace("&", "and").ToLower().Trim().Contains(flacTag.Title.Replace("&", "and").ToLower().Trim()));
            if (song == null)
            {
                int ID = FindSimilarName.findSimilarSongName(await _ctx.Songs.AsNoTracking().ToListAsync(), flacTag.Title);
                if (ID == 0)
                {
                    return new AudioBindError(filePath, flacTag.Artist, flacTag.Album, flacTag.Title, "Album not found in database");
                }
                else
                {
                    song = await _ctx.Songs.FirstAsync(x => x.Id == ID);
                    SongName = song.Name.ToLower().Trim();
                }
            }
            else
            {
                SongName = song.Name.ToLower().Trim();
            }

            if (!string.IsNullOrEmpty(SongName))
            {
                destinationPath = GetEnviromentPath() +
                    Path.DirectorySeparatorChar +
                    flacTag.Artist.ToLower().Trim() +
                    Path.DirectorySeparatorChar +
                    Replacer.RemoveSpecialCharacters(AlbumName) +
                    Path.DirectorySeparatorChar +
                    Replacer.RemoveSpecialCharacters(SongName) + ".flac";

                long before = new FileInfo(filePath).Length;
                Ffmpeg.ConvertFile(filePath);

                if (File.Exists(filePath + "Converted"))
                {
                    long after = new FileInfo(filePath).Length;
                    return new AudioBindError(destinationPath, flacTag.Artist, flacTag.Album, flacTag.Title, $"{after} -> {before} CONVERTAVOSI ===========================================================================");
                }

                if (File.Exists(destinationPath) && song.Size > new FileInfo(filePath).Length)
                {
                    File.Delete(filePath);
                    return new AudioBindError(destinationPath, flacTag.Artist, flacTag.Album, flacTag.Title, "Size is bigger current Song " + song.Size + " new song - " + new FileInfo(filePath).Length);
                }
                else
                {
                    File.Copy(filePath,
                    destinationPath,
                    true);

                    if (song.LocalUrl == null || song.LocalUrl != destinationPath)
                    {
                        song.LocalUrl = destinationPath;
                        song.IsLocal = true;
                        song.IsPlayable = true;
                        song.UploadTime = DateTime.Now;
                        song.Size = new FileInfo(destinationPath).Length;
                        _ctx.Entry(song).State = EntityState.Modified;

                        if (!album.IsPlayable)
                        {
                            album.IsPlayable = true;
                            _ctx.Entry(album).State = EntityState.Modified;
                        }
                        await _ctx.SaveChangesAsync();
                    }
                }
            }

            if (!string.IsNullOrEmpty(destinationPath) && !File.Exists(destinationPath))
            {
                return new AudioBindError(filePath, flacTag.Artist, flacTag.Album, flacTag.Title, "Failed to create file or get path");
            }
            else
            {
                File.Delete(filePath);
            }

            return null;
        }

        private string GetEnviromentPath()
        {
            if (!Environment.OSVersion.ToString().Contains("W"))
            {
                return
                    System.IO.Path.DirectorySeparatorChar + "root" +
                    System.IO.Path.DirectorySeparatorChar + "Content" +
                    System.IO.Path.DirectorySeparatorChar + "Flac";

            }
            else
            {
                return
                    Environment.CurrentDirectory +
                    System.IO.Path.DirectorySeparatorChar + "Music";
            }
        }
    }
}
