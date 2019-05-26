using Android.Graphics.Drawables;
using Android.Views;

namespace SpotyPie.Helpers
{
    public static class GradientBG
    {
        public static void SetBacground(View view)
        {
            int[] colors = { Android.Graphics.Color.ParseColor("#222222"), Android.Graphics.Color.ParseColor("#222222") };

            //create a new gradient color
            GradientDrawable gd = new GradientDrawable(
            GradientDrawable.Orientation.TopBottom, colors);

            gd.SetCornerRadius(0f);
            view.SetBackgroundDrawable(gd);
        }
    }
}