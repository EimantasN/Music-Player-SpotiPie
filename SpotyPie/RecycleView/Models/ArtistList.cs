using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Square.Picasso;

namespace SpotyPie.RecycleView.Models
{
    class ArtistList : RecyclerView.ViewHolder
    {
        public View ArtistListView { get; set; }

        public TextView Title { get; set; }

        public ImageView Image { get; set; }

        public ArtistList(View view, ViewGroup parent) : base(view)
        {
            Image = view.FindViewById<ImageView>(Resource.Id.image);
            Title = view.FindViewById<TextView>(Resource.Id.title);

            IsRecyclable = true;
        }

        public void PrepareView(dynamic data, Context Context)
        {
            Title.Text = data.Name;
            Picasso.With(Context).Load(data.SmallImage).NoFade().Fit().CenterCrop().Into(Image);
        }
    }
}