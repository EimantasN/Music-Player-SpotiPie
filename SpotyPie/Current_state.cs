using Android.App;
using Android.Content;
using Android.Views;
using Mobile_Api.Models;
using Mobile_Api.Models.Realm;
using Realms;
using SpotyPie.Base;
using SpotyPie.Enums.Activitys;
using SpotyPie.Music.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class Current_state
    {
        public bool IsPlaying = false;

        public static int PrevId { get; set; }

        public bool Start_music { get; set; } = false;

        public bool IsPlayerLoaded { get; set; } = false;

        public bool PlayerIsVisible { get; set; } = false;

        public float Progress { get; set; }

        public ActivityBase Activity { get; set; }

        public string Current_Player_Image { get; set; }

        private Player.Player Player { get; set; }

        public Current_state(ActivityBase activity)
        {
            this.Activity = activity;
            //Player = new Player.Player();
            //activity.mSupportFragmentManager.BeginTransaction()
            //        .Replace(Resource.Id.player_frame, Player)
            //        .Commit();
        }

        public void SetSong(List<Songs> songs, int position, bool refresh = false)
        {
            UpdateRealSongList(songs, position);
            Activity?.StartPlayer();
        }

        private void UpdateRealSongList(List<Songs> songs, int position = 0)
        {
            using (Realm realm = Realm.GetInstance())
            {
                realm.Write(() => realm.RemoveAll<Mobile_Api.Models.Realm.Music>());

                bool playing = false;
                for (int i = 0; i < songs.Count; i++)
                {
                    playing = false;
                    if (position == -1)
                    {
                        if (songs[i].Id == QueueHelper.Id)
                            playing = true;
                    }
                    else if (position == i)
                        playing = true;

                    songs[i].IsPlaying = playing;
                    realm.Write(() =>
                    {
                        realm.Add(new Mobile_Api.Models.Realm.Music(songs[i], playing));
                    });
                }
            }
            Task.Run(() =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        Activity.SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.play"));
                    });
                });
        }

        public void SetCurrentSongList(List<Songs> songs)
        {
            if (songs != null && songs.Count > 0)
            {
                songs.First().SetIsPlaying(true);
                //CurrentSongList.AddRange(songs);
            }
        }

        public void UpdateSongList(Songs song)
        {
            //CurrentSongList.First(x => x.Id == Current_Song.Id).SetIsPlaying(true);
        }

        public void SetArtist(Artist art)
        {
        }

        public void SetAlbum(Album album)
        {
            //Current_Album = album;
            Activity.RunOnUiThread(() =>
            {
        //GetPlayer().PlayerPlaylistName.Text = Current_Album.Name;
    });
        }

        internal void Dispose()
        {
        }

        internal void SetSongDuration(int Duration)
        {
            try
            {
                //if (Current_Song.DurationMs != Duration)
                //{
                //    Task.Run(() => Activity.GetAPIService().SetSongDurationAsync(Current_Song.Id, Duration));
                //}
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}