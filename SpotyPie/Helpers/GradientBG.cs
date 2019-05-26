using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SpotyPie.Helpers
{
    public static class GradientBG
    {
        public static void SetBacground(View view)
        {
            int[] colors = { Android.Graphics.Color.ParseColor("#008000"), Android.Graphics.Color.ParseColor("#ADFF2F") };

            //create a new gradient color
            GradientDrawable gd = new GradientDrawable(
            GradientDrawable.Orientation.TopBottom, colors);

            gd.SetCornerRadius(0f);
            view.SetBackgroundDrawable(gd);
        }
    }
}