using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using SpotyPie.Helpers;
using SpotyPie.Models;
using SpotyPie.RecycleView.Models;
using System;

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
            if (typeof(T) != typeof(object))
            {
                if (Dataset[position] == null)
                {
                    return Resource.Layout.Loading;
                }
                else if (typeof(T) == typeof(BlockWithImage))
                {
                    return Resource.Layout.big_rv_list;
                }
                else if (typeof(T) == typeof(TwoBlockWithImage))
                {
                    return Resource.Layout.boxed_rv_list_two;
                }
                else if (typeof(T) == typeof(SongItem))
                {
                    return Resource.Layout.song_list_rv;
                }
            }
            else
            {
                if (Dataset[position].GetType().Name == "Item")
                {
                    return Resource.Layout.song_list_rv;
                }
                else if (Dataset[position].GetType().Name == "BlockWithImage")
                {
                    return Resource.Layout.big_rv_list;
                }
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

                TextView mTitle = EmptyTime.FindViewById<TextView>(Resource.Id.Title);
                TextView mSubTitle = EmptyTime.FindViewById<TextView>(Resource.Id.subtitle);
                ImageButton mImage = EmptyTime.FindViewById<ImageButton>(Resource.Id.option);

                Models.SongItem view = new Models.SongItem(EmptyTime)
                {
                    Title = mTitle,
                    SubTitile = mSubTitle,
                    Options = mImage
                };

                return view;
            }
            throw new System.Exception("View Id in RV not found");
        }

        public override int ItemCount
        {
            get { return Dataset.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is Models.Loading)
            {
                ;
            }
            else if (holder is Models.BlockImage)
            {
                Models.BlockImage view = holder as Models.BlockImage;
                view.PrepareView(Dataset[position], Context);
            }
            else if (holder is Models.BlockImageTwo)
            {
                Models.BlockImageTwo view = holder as Models.BlockImageTwo;
                view.PrepareView(Dataset[position], Context);
            }
        }
    }
}