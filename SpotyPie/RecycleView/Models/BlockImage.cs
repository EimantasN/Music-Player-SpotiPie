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
            Image = view.FindViewById<ImageView>(Resource.Id.image);
            SubTitile = view.FindViewById<TextView>(Resource.Id.subtitle);
            Title = view.FindViewById<TextView>(Resource.Id.title);
        }

        public void PrepareView(dynamic data, Context Context)
        {
            if (data.Name.Length > 13) Title.Selected = true;
            Title.Text = data.Name;

            //TODO ADD SUBTITLE
            SubTitile.Text = "Coming soon";
            Picasso.With(Context).Load(data.MediumImage).Into(Image);
        }
    }
}