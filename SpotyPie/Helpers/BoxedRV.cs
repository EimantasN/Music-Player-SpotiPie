using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SpotyPie.Models;
using Square.Picasso;

namespace SpotyPie.Helpers
{
    public class BoxedRV : RecyclerView.Adapter
    {
        private RvList<TwoBlockWithImage> Dataset;
        private readonly RecyclerView mRecyclerView;
        private Context Context;

        public BoxedRV(RvList<TwoBlockWithImage> data, RecyclerView recyclerView, Context context)
        {
            Dataset = data;
            mRecyclerView = recyclerView;
            Context = context;
        }

        public class Loading : RecyclerView.ViewHolder
        {
            public View LoadingView { get; set; }

            public Loading(View view) : base(view)
            { }
        }

        public class BlockImage : RecyclerView.ViewHolder
        {
            public View EmptyTimeView { get; set; }

            public TextView L_Title { get; set; }

            public TextView L_SubTitile { get; set; }

            public ImageView L_Image { get; set; }

            public TextView R_Title { get; set; }

            public TextView R_SubTitile { get; set; }

            public ImageView R_Image { get; set; }

            public BlockImage(View view) : base(view) { }
        }

        public override int GetItemViewType(int position)
        {
            if (Dataset[position] == null)
            {
                return Resource.Layout.Loading;
            }
            else
            {
                return Resource.Layout.boxed_rv_list_two;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == Resource.Layout.Loading)
            {
                View Loading = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Loading, parent, false);

                Loading view = new Loading(Loading) { };

                return view;
            }
            else
            {
                View EmptyTime = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.boxed_rv_list_two, parent, false);

                TextView mL_Title = EmptyTime.FindViewById<TextView>(Resource.Id.album_title_left);
                TextView mL_SubTitle = EmptyTime.FindViewById<TextView>(Resource.Id.left_subtitle);
                ImageView mL_Image = EmptyTime.FindViewById<ImageView>(Resource.Id.left_image);

                TextView mR_Title = EmptyTime.FindViewById<TextView>(Resource.Id.album_title_right);
                TextView mR_SubTitle = EmptyTime.FindViewById<TextView>(Resource.Id.right_subtitle);
                ImageView mR_Image = EmptyTime.FindViewById<ImageView>(Resource.Id.right_image);

                BlockImage view = new BlockImage(EmptyTime)
                {
                    L_Title = mL_Title,
                    L_SubTitile = mL_SubTitle,
                    L_Image = mL_Image,
                    R_Title = mR_Title,
                    R_SubTitile = mR_SubTitle,
                    R_Image = mR_Image,
                    IsRecyclable = false
                };

                return view;
            }
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
                view.L_Title.Text = Dataset[position].Left.Title;
                view.L_SubTitile.Text = Dataset[position].Left.SubTitle;
                Picasso.With(Context).Load(Dataset[position].Left.Image).Resize(300, 300).CenterCrop().Into(view.L_Image);
                if (Dataset[position].Right != null)
                {
                    view.R_Title.Text = Dataset[position].Right.Title;
                    view.R_SubTitile.Text = Dataset[position].Right.SubTitle;
                    Picasso.With(Context).Load(Dataset[position].Right.Image).Resize(300, 300).CenterCrop().Into(view.R_Image);
                }
                else
                {
                    view.R_Title.Visibility = ViewStates.Gone;
                    view.R_SubTitile.Visibility = ViewStates.Gone;
                    view.R_Image.Visibility = ViewStates.Gone;
                }
            }
        }

        public override int ItemCount
        {
            get { return Dataset.Count; }
        }
    }
}