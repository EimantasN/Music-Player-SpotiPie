using Android.Support.V7.Widget;
using Android.Views;
using Mobile_Api.Models;
using SpotyPie.RecycleView.Models;
using System;
using System.Collections.Generic;

namespace SpotyPie.RecycleView.Adapters
{
    public class SongListAdapter : RecyclerView.Adapter
    {
        protected RvList<Songs> Songs = new RvList<Songs>();

        public Action<Songs> OnSongClick { get; set; }

        public Action<Songs> OnSongOptionClick { get; set; }

        public override int ItemCount => Songs.Count;

        public SongListAdapter SetInitSongs(List<Songs> songs)
        {
            Songs.Adapter = this;
            Songs.AddList(songs);
            return this;
        }

        public void NotifySongChange(int index)
        {
            if (index >= 0 && index < Songs.Count)
            {
                NotifyItemChanged(index);
            }
        }

        public void Add(Songs song)
        {
            if (Songs != null)
            {
                Songs.Add(song);
            }
        }

        public void AddList(List<Songs> song)
        {
            if (Songs != null)
            {
                Songs.AddList(song);
            }
        }

        public override int GetItemViewType(int position)
        {
            if (Songs[position] == null)
            {
                return Resource.Layout.Loading;
            }
            else
            {
                return Resource.Layout.song_list_rv;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is Loading)
            {
                return;
            }
            else if (holder is SongItem)
            {
                SongItem view = holder as SongItem;
                view.PrepareView(Songs[position] as Songs);

                view.OnSongClickAction = OnSongClick;
                view.OnSongOptionClickAction = OnSongOptionClick;
                return;
            }
            else
            {
                throw new Exception("Failed to find holder");
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case Resource.Layout.Loading:
                    return new Loading(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Loading, parent, false), parent);
                case Resource.Layout.song_list_rv:
                    return new SongItem(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.song_list_rv, parent, false), parent);
                default:
                    throw new Exception("No view found");
            }
        }

        public void Release()
        {
            OnSongClick = null;
            OnSongOptionClick = null;
        }
    }
}