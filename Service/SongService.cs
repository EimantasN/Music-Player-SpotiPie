using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.BackEnd;
using Models.BackEnd.Enumerators;
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
                query = $"SELECT * FROM[SpotyPie].[dbo].[Song] where IsPlayable=1 AND Name Like '%{query.Replace("'", "\"")}%'";
                return await _ctx.Songs
                    .FromSql(query)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Song>> SongByArtistId(int artistId)
        {
            try
            {
                return await _ctx.Songs.AsNoTracking().Where(x => x.ArtistId == artistId && x.IsPlayable).ToListAsync(); ;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Song> UpdateAsync(int id)
        {
            try
            {
                var song = await _ctx.Songs
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (song != null)
                {
                    song.LastActiveTime = DateTime.Now.ToUniversalTime();
                    song.Popularity++;
                    song.Corrupted = 0;

                    _ctx.Entry(song).State = EntityState.Modified;
                    await _ctx.SaveChangesAsync();
                }
                return song;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public async Task<List<AudioBindError>> BindData()
        {
            List<AudioBindError> Errors = new List<AudioBindError>();
            try
            {
                string fileName;
                string newPath;
                foreach (var path in System.IO.Directory.EnumerateFiles(EnviromentPath.GetEnviromentPathMusic()))
                {
                    try
                    {
                        if (path.Contains(".flac"))
                        {
                            fileName = Path.GetFileName(path);
                            newPath = path.Replace(fileName, Replacer.RemoveSpecialCharacters(fileName)).Replace("_flac", ".flac");
                            File.Move(path, newPath);
                            var data = await SaveFileAsync(newPath, SongType.Flac);
                            if (data != null)
                                Errors.Add(data);
                        }
                        else if (path.Contains(".mp3"))
                        {
                            fileName = Path.GetFileName(path);
                            newPath = path.Replace(fileName, Replacer.RemoveSpecialCharacters(fileName)).Replace("_mp3", ".mp3");
                            File.Move(path, newPath);
                            var data = await SaveFileAsync(newPath, SongType.Mp3);
                            if (data != null)
                                Errors.Add(data);
                        }
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
                string FilePath = string.Empty;
                SongType type = SongType.Flac;
                if (file.FileName.Contains(".flac"))
                {
                    FilePath = EnviromentPath.GetEnviromentPathMusic() + Path.DirectorySeparatorChar + Replacer.RemoveSpecialCharacters(file.FileName).Replace("_flac", ".flac");
                }
                else if (file.FileName.Contains(".mp3"))
                {
                    FilePath = EnviromentPath.GetEnviromentPathMusic() + Path.DirectorySeparatorChar + Replacer.RemoveSpecialCharacters(file.FileName).Replace("_mp3", ".mp3");
                    type = SongType.Mp3;
                }

                if (string.IsNullOrEmpty(FilePath))
                    return Errors;

                using (var fileStream = new FileStream(FilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                try
                {
                    var data = await SaveFileAsync(FilePath, type);
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

        private async Task<AudioBindError> SaveFileAsync(string filePath, SongType type)
        {
            SongTag tag = new SongTag(filePath, 0);

            if (string.IsNullOrEmpty(tag.Album) || string.IsNullOrEmpty(tag.Title) || string.IsNullOrEmpty(tag.Artist))
            {
                return new AudioBindError(filePath, tag.Artist, tag.Album, tag.Title, "Wrong Flac");
            }

            var artist = await _ctx.Artists.FirstOrDefaultAsync(x => x.Name.ToLower().Trim() == tag.Artist.ToLower().Trim());
            if (artist == null)
                return new AudioBindError(filePath, tag.Artist, tag.Album, tag.Title, "Artist not found in database");

            EnviromentPath.CheckForDirectory(
                EnviromentPath.GetEnviromentPathMusic() +
                Path.DirectorySeparatorChar +
                Replacer.RemoveSpecialCharacters(artist.Name.ToLower().Trim()), Content.Music);

            string AlbumName = "";
            Replacer.CorrentAlbum(tag.Album);
            var album = await _ctx.Albums.FirstOrDefaultAsync(x => x.Name.ToLower().Trim().Contains(tag.Album.ToLower().Trim()));
            if (album == null)
            {
                int ID = FindSimilarName.findSimilarAlbumName(await _ctx.Albums.AsNoTracking().ToListAsync(), tag.Album);
                if (ID == 0)
                {
                    List<Song> thirdCheck = await _ctx.Songs.Where(x => x.Name.ToLower().Trim().Contains(tag.Title.ToLower().Trim())
                     && x.ArtistId == artist.Id).ToListAsync();

                    ID = FindSimilarName.findSimilarSongName(_ctx, thirdCheck, tag.Title, 0, artist.Id);

                    if (ID == 0)
                        return new AudioBindError(filePath, tag.Artist, tag.Album, tag.Title, "Album not found in database");

                    album = await _ctx.Albums.FirstAsync(x => x.Id == thirdCheck.First(y => y.Id == ID).AlbumId);
                    AlbumName = album.Name.ToLower().Trim();
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

            EnviromentPath.CheckForDirectory(
                EnviromentPath.GetEnviromentPathMusic() +
                Path.DirectorySeparatorChar +
                Replacer.RemoveSpecialCharacters(artist.Name.ToLower().Trim()) +
                Path.DirectorySeparatorChar +
                Replacer.RemoveSpecialCharacters(AlbumName), Content.Music);

            string destinationPath = "";
            string SongName = null;
            Song song = null;
            if (song == null)
            {
                int ID = FindSimilarName.findSimilarSongName(_ctx, await _ctx.Songs.AsNoTracking().ToListAsync(), tag.Title, album.Id, artist.Id);
                if (ID == 0)
                {
                    return new AudioBindError(filePath, tag.Artist, tag.Album, tag.Title, "Song not found in database");
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
                string Extension = type == SongType.Flac ? ".flac" : ".mp3";
                destinationPath = EnviromentPath.GetEnviromentPathMusic() +
                    Path.DirectorySeparatorChar +
                    Replacer.RemoveSpecialCharacters(artist.Name.ToLower().Trim()) +
                    Path.DirectorySeparatorChar +
                    Replacer.RemoveSpecialCharacters(AlbumName) +
                    Path.DirectorySeparatorChar +
                    Replacer.RemoveSpecialCharacters(SongName) + Extension;

                //if (type == SongType.Flac && (tag.Bitrate == 0 || tag.Bitrate > 1000))
                //{
                //    Ffmpeg.ConvertFile(filePath);
                //}

                long newFileSize = new FileInfo(filePath).Length;

                File.Copy(filePath,
                destinationPath,
                true);

                if (song.LocalUrl == null || song.LocalUrl != destinationPath)
                {
                    song.LocalUrl = destinationPath;
                    song.IsLocal = true;
                    song.IsPlayable = true;
                    song.Type = type;
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

            if (!string.IsNullOrEmpty(destinationPath) && !File.Exists(destinationPath))
            {
                return new AudioBindError(filePath, tag.Artist, tag.Album, tag.Title, "Failed to create file or get path");
            }
            else
            {
                File.Delete(filePath);
            }

            return null;
        }

        public async Task<Song> SetLenght(int id, long durationMs)
        {
            try
            {
                var song = await _ctx.Songs.FirstOrDefaultAsync(x => x.Id == id);
                if (song != null)
                {
                    song.DurationMs = durationMs;
                    _ctx.Entry(song).State = EntityState.Modified;
                    await _ctx.SaveChangesAsync();
                }
                return song;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task SetCorruptedAsync(int id)
        {
            try
            {
                var song = await _ctx.Songs.FirstOrDefaultAsync(x => x.Id == id && x.Popularity == 0);
                var album = await _ctx.Albums.Include(x => x.Songs).FirstOrDefaultAsync(x => x.Id == song.AlbumId);
                if (song != null && album != null)
                {
                    if (album.Songs.Count(x => x.IsPlayable && x.Corrupted < 4) > 1)
                        _ctx.Entry(album).State = EntityState.Modified;
                    song.Corrupted++;
                    song.IsPlayable = false;
                    if (File.Exists(song.LocalUrl))
                        File.Delete(song.LocalUrl);
                    song.LocalUrl = null;
                    _ctx.Entry(song).State = EntityState.Modified;
                    await _ctx.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task SetStateAsync(int songId, int artistId, int albumId, int playlistId)
        {
            try
            {
                var currentSong = await _ctx.CurrentSong.FirstOrDefaultAsync();
                if (currentSong != null)
                {
                    currentSong.SongId = songId != 0 ? songId : currentSong.SongId;

                    if (songId > 0)
                    {
                        var song = await _ctx.Songs.FirstOrDefaultAsync(x => x.Id == songId);
                        if (song != null)
                        {
                            currentSong.AlbumId = song.AlbumId;
                            currentSong.ArtistId = song.ArtistId;
                            //currentSong.PlaylistId = song.PlaylistId;
                        }
                    }
                    else
                    {
                        currentSong.AlbumId = albumId != 0 ? albumId : currentSong.AlbumId;
                        currentSong.ArtistId = artistId != 0 ? artistId : currentSong.ArtistId;
                        currentSong.PlaylistId = playlistId != 0 ? playlistId : currentSong.PlaylistId;
                    }

                    _ctx.Entry(currentSong).State = EntityState.Modified;
                }
                else
                {
                    var model = new CurrentSong
                    {
                        SongId = songId,
                        AlbumId = albumId,
                        ArtistId = artistId,
                        PlaylistId = playlistId
                    };
                    await _ctx.CurrentSong.AddAsync(model);
                }
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<CurrentSong> GetState()
        {
            return _ctx.CurrentSong.FirstOrDefaultAsync();
        }

        public async Task<Song> GetNextSong()
        {
            try
            {
                var current = await _ctx.CurrentSong.FirstOrDefaultAsync();

                if (current == null)
                {
                    var song = await _ctx.Songs.FirstOrDefaultAsync(x => x.IsPlayable && x.Corrupted < 4);
                    return await ReturnSongAsync(song);
                }
                else
                {
                    var CurrentSong = await _ctx.Songs.FirstAsync(x => x.Id == current.SongId);

                    var song = await _ctx.Songs
                        .OrderBy(x => x.AlbumId)
                        .ThenBy(x => x.TrackNumber)
                        .FirstOrDefaultAsync(x => x.TrackNumber > CurrentSong.TrackNumber && x.AlbumId == current.AlbumId && x.IsPlayable && x.Corrupted < 4);

                    if (song == null)
                    {
                        song = await _ctx.Songs
                            .OrderBy(x => x.AlbumId)
                            .ThenBy(x => x.TrackNumber)
                            .FirstOrDefaultAsync(x => x.AlbumId > current.AlbumId && x.IsPlayable && x.Corrupted < 4);
                        if (song == null)
                        {
                            return new Song { Id = -1 };
                        }
                        return await ReturnSongAsync(song);
                    }
                    return await ReturnSongAsync(song);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            async Task<Song> ReturnSongAsync(Song song)
            {
                await SetStateAsync(song.Id, song.ArtistId, song.AlbumId, 0);
                return song;
            }
        }

        public async Task<List<Image>> GetNewImageAsync(int id)
        {
            try
            {
                Song song = await _ctx.Songs.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);
                //TODO Add to check for updates after two weeks of last download
                if (song.Images == null || song.Images.Count == 0)
                {
                    Genius imgGetter = new Genius(song);
                    await imgGetter.StartAsync(_ctx);
                    song = await _ctx.Songs.Include(x => x.Images).Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);
                    return song.Images;
                }
                else
                {
                    return song.Images;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<SongTag> UnbindedSongs()
        {
            List<SongTag> SongTags = new List<SongTag>();
            try
            {
                string fileName;
                string newPath;

                int id = 0;
                foreach (var path in Directory.EnumerateFiles(EnviromentPath.GetEnviromentPathMusic()))
                {
                    try
                    {
                        id++;
                        if (path.Contains(".flac"))
                        {
                            fileName = Path.GetFileName(path);
                            newPath = path.Replace(fileName, Replacer.RemoveSpecialCharacters(fileName)).Replace("_flac", ".flac");
                            File.Move(path, newPath);
                            SongTags.Add(new SongTag(newPath, id));
                        }
                        else if (path.Contains(".mp3"))
                        {
                            fileName = Path.GetFileName(path);
                            newPath = path.Replace(fileName, Replacer.RemoveSpecialCharacters(fileName)).Replace("_mp3", ".mp3");
                            File.Move(path, newPath);
                            SongTags.Add(new SongTag(newPath, id));
                        }
                    }
                    catch (Exception)
                    {
                        //Need to log errors
                    }
                }
            }
            catch (Exception)
            {
                //Need to log errors
            }

            return SongTags;
        }

        public async Task<List<Song>> GetSongToBindAsync(string songTitle, int songCof, string album, int albumCof, string artist, int artistCof)
        {
            List<Song> filtered = new List<Song>();
            try
            {
                List<Song> songs = await _ctx.Songs.ToListAsync();

                List<Song> SongtitleFiltered = songs.Where(x => FindSimilarName.CheckSimilarity(songTitle, x.Name, songCof)).ToList();
                List<Song> songAlbumFiltered = SongtitleFiltered.Where(x => FindSimilarName.CheckSimilarity(album, x.AlbumName, albumCof)).ToList();
                List<Song> songArtistFiltered = songAlbumFiltered.Where(x => FindSimilarName.CheckSimilarity(artist, x.ArtistName, artistCof)).ToList();
                return songArtistFiltered;
            }
            catch (Exception e)
            {
            }

            return filtered;
        }

        public async Task<dynamic> GetBindingStatistics()
        {
            List<bool> Songs = await _ctx.Songs.Select(x => x.IsPlayable).ToListAsync();
            return new
            {
                UnbindedCount = Directory.GetFiles(EnviromentPath.GetEnviromentPathMusic()).Count(),
                BindedCount = Songs.Count(x => x),
                SongTotal = Songs.Count(x => !x)
            };
        }

        public string DeleteLocalSongFile(string localUrl)
        {
            //TODO added safe delete in future
            if (File.Exists(localUrl))
            {
                File.Delete(localUrl);
                return "Success";
            }
            else
            {
                return "File not found";
            }
        }

        public async Task<Song> BindSongWithFileAsync(string localUrl, int songId)
        {
            try
            {
                if (string.IsNullOrEmpty(localUrl) || songId == 0)
                    return null;

                var song = await _ctx.Songs.FirstOrDefaultAsync(x => x.Id == songId);
                if (song == null)
                    return null;

                var album = await _ctx.Albums.FirstAsync(x => x.Id == song.AlbumId);

                SongType type = SongType.Flac;
                if (!Path.GetExtension(localUrl).ToLower().Contains("flac"))
                    type = SongType.Mp3;

                string Extension = Path.GetExtension(localUrl);
                string destinationPath = EnviromentPath.GetEnviromentPathMusic() +
                    Path.DirectorySeparatorChar +
                    Replacer.RemoveSpecialCharacters(song.ArtistName.ToLower().Trim()) +
                    Path.DirectorySeparatorChar +
                    Replacer.RemoveSpecialCharacters(song.AlbumName.ToLower().Trim()) +
                    Path.DirectorySeparatorChar +
                    Replacer.RemoveSpecialCharacters(song.Name.ToLower().Trim()) + Extension;

                File.Copy(localUrl, destinationPath, true);

                if (song.LocalUrl == null || song.LocalUrl != destinationPath)
                {
                    song.LocalUrl = destinationPath;
                    song.IsLocal = true;
                    song.IsPlayable = true;
                    song.Type = type;
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
                return song;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
