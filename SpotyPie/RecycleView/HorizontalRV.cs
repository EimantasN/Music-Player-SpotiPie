using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SpotyPie.Models;
using Square.Picasso;

namespace SpotyPie.RecycleView
{
    public class HorizontalRV : RecyclerView.Adapter
    {
        private SpotyPie.RecycleView.RvList<BlockWithImage> Dataset;
        private readonly RecyclerView mRecyclerView;
        private Context Context;

        public HorizontalRV(SpotyPie.RecycleView.RvList<BlockWithImage> data, RecyclerView recyclerView, Context context)
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

            public TextView Title { get; set; }

            public TextView SubTitile { get; set; }

            public ImageView Image { get; set; }

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
                return Resource.Layout.big_rv_list;
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
                View BoxView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.big_rv_list, parent, false);
                TextView mTitle = BoxView.FindViewById<TextView>(Resource.Id.textView10);
                TextView mSubTitle = BoxView.FindViewById<TextView>(Resource.Id.textView11);
                ImageView mImage = BoxView.FindViewById<ImageView>(Resource.Id.imageView5);

                BlockImage view = new BlockImage(BoxView)
                {
                    Image = mImage,
                    SubTitile = mSubTitle,
                    Title = mTitle,
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
                view.Title.Text = Dataset[position].Title;
                view.SubTitile.Text = Dataset[position].SubTitle;
                if (Dataset[position].Image != string.Empty)
                    Picasso.With(Context).Load(Dataset[position].Image).Resize(300, 300).CenterCrop().Into(view.Image);
                else
                    view.Image.SetImageResource(Resource.Drawable.noimg);

            }
        }

        public override int ItemCount
        {
            get { return Dataset.Count; }
        }
    }
}