﻿using System;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Square.Picasso;

namespace SpotyPie.RecycleView.Models
{
    public class BlockImageTwo : RecyclerView.ViewHolder
    {
        public View EmptyTimeView { get; set; }

        public TextView L_Title { get; set; }

        public TextView L_SubTitile { get; set; }

        public ImageView L_Image { get; set; }

        public TextView R_Title { get; set; }

        public TextView R_SubTitile { get; set; }

        public ImageView R_Image { get; set; }

        public BlockImageTwo(View view) : base(view) { }

        internal void PrepareView(dynamic data, Context context)
        {
            L_Title.Text = data.Left.Title;
            L_SubTitile.Text = data.Left.SubTitle;
            Picasso.With(context).Load(data.Left.Image).Resize(300, 300).CenterCrop().Into(L_Image);
            if (data.Right != null)
            {
                R_Title.Text = data.Right.Title;
                R_SubTitile.Text = data.Right.SubTitle;
                Picasso.With(context).Load(data.Right.Image).Resize(300, 300).CenterCrop().Into(R_Image);
            }
            else
            {
                R_Title.Visibility = ViewStates.Gone;
                R_SubTitile.Visibility = ViewStates.Gone;
                R_Image.Visibility = ViewStates.Gone;
            }
        }
    }
}