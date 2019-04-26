using System;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Square.Picasso;

namespace SpotyPie.RecycleView.Models
{
    //Layout -> song_list_with_image

    public class SongWithImage : RecyclerView.ViewHolder
    {
        public View EmptyTimeView { get; set; }

        public ImageView Image { get; set; }

        public TextView Title { get; set; }

        public TextView SubTitile { get; set; }

        public ImageButton Options { get; set; }

        public SongWithImage(View view, ViewGroup parent) : base(view)
        {
            Image = view.FindViewById<ImageView>(Resource.Id.se_image);
            Title = view.FindViewById<TextView>(Resource.Id.title);
            SubTitile = view.FindViewById<TextView>(Resource.Id.subtitle);
            Options = view.FindViewById<ImageButton>(Resource.Id.option);
        }

        public void PrepareView(dynamic data, Context Context)
        {
            try
            {
                Title.Text = data.Name;
                SubTitile.Text = $"Popularity - {data.Popularity}";
                Picasso.With(Context).Load(data.MediumImage).Into(Image);
            }
            catch (Exception e)
            {

            }
        }
    }
}