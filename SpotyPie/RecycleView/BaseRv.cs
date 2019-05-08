using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Interfaces;
using Mobile_Api.Models;
using SpotyPie.RecycleView.Models;

namespace SpotyPie.RecycleView
{
    public class BaseRv<T> : RecyclerView.Adapter where T : IBaseInterface
    {
        protected RvList<T> Dataset;
        protected RecyclerView mRecyclerView;
        protected Context Context;

        public BaseRv(RvList<T> data, RecyclerView recyclerView, Context context)
        {
            Dataset = data;
            mRecyclerView = recyclerView;
            Context = context;
            this.HasStableIds = true;
        }

        public override long GetItemId(int position)
        {
            if (Dataset[position] != null)
                return Dataset[position].GetId();
            return 0;
        }

        public override int GetItemViewType(int position)
        {
            if (Dataset[position] == null)
            {
                return Resource.Layout.Loading;
            }
            else if (typeof(T) == typeof(Album) || Dataset[position].GetType().Name == "Album")
            {
                Album al = Dataset[position] as Album;
                if (al.GetModelType() == Mobile_Api.Models.Enums.RvType.Album)
                    return Resource.Layout.big_rv_list;
                else if (al.GetModelType() == Mobile_Api.Models.Enums.RvType.AlbumList)
                    return Resource.Layout.album_list;
                else if (al.GetModelType() == Mobile_Api.Models.Enums.RvType.AlbumGrid)
                    return Resource.Layout.grid_rv;
                else if (al.GetModelType() == Mobile_Api.Models.Enums.RvType.BigOne)
                    return Resource.Layout.big_rv_list_one;
            }
            else if (typeof(T) == typeof(Artist) || Dataset[position].GetType().Name == "Artist")
            {
                Artist ar = Dataset[position] as Artist;
                if (ar.GetModelType() == Mobile_Api.Models.Enums.RvType.Artist)
                    return Resource.Layout.big_rv_list_one;
                else if (ar.GetModelType() == Mobile_Api.Models.Enums.RvType.ArtistList)
                    return Resource.Layout.artist_list;
                else if (ar.GetModelType() == Mobile_Api.Models.Enums.RvType.ArtistGrid)
                    return Resource.Layout.grid_rv;
                else if (ar.GetModelType() == Mobile_Api.Models.Enums.RvType.BigOne)
                    return Resource.Layout.big_rv_list_one;
            }
            else if (typeof(T) == typeof(SongItem) || Dataset[position].GetType().Name == "Songs")
            {
                Songs ar = Dataset[position] as Songs;
                if (ar.GetModelType() == Mobile_Api.Models.Enums.RvType.SongWithImage)
                    return Resource.Layout.song_list_with_image;
                else
                    return Resource.Layout.song_list_rv;
            }
            else if (typeof(T) == typeof(SongTag) || Dataset[position].GetType().Name == "SongTag")
            {
                return Resource.Layout.song_detail_list;
            }
            throw new System.Exception("No view found");
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case Resource.Layout.Loading:
                    return new Loading(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Loading, parent, false), parent);
                case Resource.Layout.big_rv_list:
                    return new BlockImage(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.big_rv_list, parent, false), parent);
                case Resource.Layout.song_list_rv:
                    return new SongItem(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.song_list_rv, parent, false), parent);
                case Resource.Layout.grid_rv:
                    return new BlockImage(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.grid_rv, parent, false), parent);
                case Resource.Layout.big_rv_list_one:
                    return new BlockImage(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.big_rv_list_one, parent, false), parent);
                case Resource.Layout.boxed_rv_list_two:
                    return new BlockImageTwo(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.boxed_rv_list_two, parent, false), parent);
                case Resource.Layout.song_list_with_image:
                    return new SongWithImage(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.song_list_with_image, parent, false), parent);
                case Resource.Layout.song_detail_list:
                    return new SongBindList(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.song_detail_list, parent, false), parent);
                case Resource.Layout.artist_list:
                    return new ArtistList(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.artist_list, parent, false), parent);
                case Resource.Layout.album_list:
                    return new AlbumList(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.album_list, parent, false), parent);
                default:
                    throw new System.Exception("View Id in RV not found");
            }
        }

        public override int ItemCount
        {
            get { return Dataset.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is Loading)
            {
                return;
            }
            else if (holder is BlockImage)
            {
                BlockImage view = holder as BlockImage;
                view.PrepareView(Dataset[position], Context);
                return;
            }
            else if (holder is BlockImageTwo)
            {
                BlockImageTwo view = holder as BlockImageTwo;
                view.PrepareView(Dataset[position], Context);
                return;
            }
            else if (holder is SongItem)
            {
                SongItem view = holder as SongItem;
                view.PrepareView(Dataset[position] as Songs, Context);
                return;
            }
            else if (holder is ArtistList)
            {
                ArtistList view = holder as ArtistList;
                view.PrepareView(Dataset[position], Context);
                return;
            }
            else if (holder is AlbumList)
            {
                AlbumList view = holder as AlbumList;
                view.PrepareView(Dataset[position], Context);
                return;
            }
            else if (holder is SongWithImage)
            {
                SongWithImage view = holder as SongWithImage;
                view.PrepareView(Dataset[position] as Songs, Context);
                return;
            }
            else if (holder is SongBindList)
            {
                SongBindList view = holder as SongBindList;
                view.PrepareView(Dataset[position] as SongTag, Context);
                return;
            }

            throw new System.Exception("Failed to find holder");
        }
    }
}