using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using SpotyPie.RecycleView.Models;

namespace SpotyPie.RecycleView
{
    public class BaseRv<T> : RecyclerView.Adapter
    {
        protected RvList<T> Dataset;
        protected RecyclerView mRecyclerView;
        protected Context Context;

        public BaseRv(RvList<T> data, RecyclerView recyclerView, Context context)
        {
            Dataset = data;
            mRecyclerView = recyclerView;
            Context = context;
        }

        public override int GetItemViewType(int position)
        {
            if (Dataset[position] == null)
            {
                return Resource.Layout.Loading;
            }
            else if (typeof(T) == typeof(Album) || Dataset[position].GetType().Name == "Album")
            {
                var al = Dataset[position] as Album;
                if (al.GetModelType() == Mobile_Api.Models.Enums.RvType.Album)
                    return Resource.Layout.big_rv_list;
                else if (al.GetModelType() == Mobile_Api.Models.Enums.RvType.AlbumList)
                    return Resource.Layout.album_list;
                else
                    return Resource.Layout.big_rv_list_one;
            }
            else if (typeof(T) == typeof(Artist) || Dataset[position].GetType().Name == "Artist")
            {
                var al = Dataset[position] as Artist;
                if (al.GetModelType() == Mobile_Api.Models.Enums.RvType.Artist)
                    return Resource.Layout.big_rv_list_one;
                else if (al.GetModelType() == Mobile_Api.Models.Enums.RvType.ArtistList)
                {
                    return Resource.Layout.artist_list;
                }
            }
            //else if (typeof(T) == typeof(TwoBlockWithImage))
            //{
            //    return Resource.Layout.boxed_rv_list_two;
            //}
            else if (typeof(T) == typeof(SongItem) || Dataset[position].GetType().Name == "Song")
            {
                return Resource.Layout.song_list_rv;
            }
            throw new System.Exception("No view found");
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == Resource.Layout.Loading)
            {
                View LoadingView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Loading, parent, false);

                Models.Loading view = new Models.Loading(LoadingView) { };

                return view;
            }
            else if (viewType == Resource.Layout.big_rv_list)
            {
                return new Models.BlockImage(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.big_rv_list, parent, false), parent);
            }
            else if (viewType == Resource.Layout.big_rv_list_one)
            {
                return new Models.BlockImage(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.big_rv_list_one, parent, false), parent);
            }
            else if (viewType == Resource.Layout.boxed_rv_list_two)
            {
                View EmptyTime = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.boxed_rv_list_two, parent, false);

                TextView mL_Title = EmptyTime.FindViewById<TextView>(Resource.Id.album_title_left);
                TextView mL_SubTitle = EmptyTime.FindViewById<TextView>(Resource.Id.left_subtitle);
                ImageView mL_Image = EmptyTime.FindViewById<ImageView>(Resource.Id.left_image);

                TextView mR_Title = EmptyTime.FindViewById<TextView>(Resource.Id.album_title_right);
                TextView mR_SubTitle = EmptyTime.FindViewById<TextView>(Resource.Id.right_subtitle);
                ImageView mR_Image = EmptyTime.FindViewById<ImageView>(Resource.Id.right_image);

                Models.BlockImageTwo view = new Models.BlockImageTwo(EmptyTime)
                {
                    L_Title = mL_Title,
                    L_SubTitile = mL_SubTitle,
                    L_Image = mL_Image,
                    R_Title = mR_Title,
                    R_SubTitile = mR_SubTitle,
                    R_Image = mR_Image
                };

                return view;
            }
            else if (viewType == Resource.Layout.song_list_rv)
            {
                View EmptyTime = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.song_list_rv, parent, false);

                SongItem view = new SongItem(EmptyTime)
                {
                    Title = EmptyTime.FindViewById<TextView>(Resource.Id.Title),
                    SubTitile = EmptyTime.FindViewById<TextView>(Resource.Id.subtitle),
                    Options = EmptyTime.FindViewById<ImageButton>(Resource.Id.option),
                    SmallIcon = EmptyTime.FindViewById<ImageView>(Resource.Id.small_img)
                };

                return view;
            }
            else if (viewType == Resource.Layout.artist_list)
            {
                return new ArtistList(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.artist_list, parent, false), parent);
            }
            else if (viewType == Resource.Layout.album_list)
            {
                return new AlbumList(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.album_list, parent, false), parent);
            }
            throw new System.Exception("View Id in RV not found");
        }

        public override int ItemCount
        {
            get { return Dataset.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is Loading)
            {
                ;
            }
            else if (holder is BlockImage)
            {
                BlockImage view = holder as BlockImage;
                view.PrepareView(Dataset[position], Context);
            }
            else if (holder is BlockImageTwo)
            {
                BlockImageTwo view = holder as BlockImageTwo;
                view.PrepareView(Dataset[position], Context);
            }
            else if (holder is SongItem)
            {
                SongItem view = holder as SongItem;
                view.PrepareView(Dataset[position], Context);
            }
            else if (holder is ArtistList)
            {
                ArtistList view = holder as ArtistList;
                view.PrepareView(Dataset[position], Context);
            }
            else if (holder is AlbumList)
            {
                AlbumList view = holder as AlbumList;
                view.PrepareView(Dataset[position], Context);
            }
        }
    }
}