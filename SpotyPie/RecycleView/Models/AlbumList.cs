using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Refractored.Controls;
using Square.Picasso;

namespace SpotyPie.RecycleView.Models
{
    class AlbumList : RecyclerView.ViewHolder
    {
        public View AlbumListView { get; set; }

        public TextView Title { get; set; }

        public CircleImageView Image { get; set; }

        public AlbumList(View view, ViewGroup parent) : base(view)
        {
            Image = view.FindViewById<CircleImageView>(Resource.Id.image);
            Title = view.FindViewById<TextView>(Resource.Id.title);
        }

        public void PrepareView(dynamic data, Context Context)
        {
            Title.Text = data.Name;
            Picasso.With(Context).Load(data.MediumImage).NoFade().Into(Image);
        }
    }
}