using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Square.Picasso;
using System;

namespace SpotyPie.RecycleView.Models
{
    //Layout -> 

    public class BlockImage : RecyclerView.ViewHolder
    {
        public View EmptyTimeView { get; set; }

        public TextView Title { get; set; }

        public TextView SubTitile { get; set; }

        public ImageView Image { get; set; }

        public BlockImage(View view, ViewGroup parent) : base(view)
        {
            Image = view.FindViewById<ImageView>(Resource.Id.se_image);
            SubTitile = view.FindViewById<TextView>(Resource.Id.subtitle);
            Title = view.FindViewById<TextView>(Resource.Id.title);
            IsRecyclable = true;
        }

        public void PrepareView(dynamic data, Context Context)
        {
            try
            {
                if (data.Name.Length > 13) Title.Selected = true;
                Title.Text = data.Name;

                //TODO ADD SUBTITLE
                SubTitile.Text = "Coming soon";
                Picasso.With(Context).Load(data.MediumImage).NoFade().Fit().CenterCrop().Into(Image);
            }
            catch (Exception e)
            {

            }
        }
    }
}