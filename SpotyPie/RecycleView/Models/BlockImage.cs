using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Square.Picasso;

namespace SpotyPie.RecycleView.Models
{
    public class BlockImage : RecyclerView.ViewHolder
    {
        public View EmptyTimeView { get; set; }

        public TextView Title { get; set; }

        public TextView SubTitile { get; set; }

        public ImageView Image { get; set; }

        public BlockImage(View view, ViewGroup parent) : base(view)
        {
            Image = view.FindViewById<ImageView>(Resource.Id.imageView5);
            SubTitile = view.FindViewById<TextView>(Resource.Id.textView11);
            Title = view.FindViewById<TextView>(Resource.Id.textView10);
            IsRecyclable = false;
        }

        public void PrepareView(dynamic data, Context Context)
        {
            Title.Text = data.Name;
            SubTitile.Text = "Coming soon";
            Picasso.With(Context).Load(data.LargeImage).Resize(300, 300).CenterCrop().Into(Image);
        }
    }
}