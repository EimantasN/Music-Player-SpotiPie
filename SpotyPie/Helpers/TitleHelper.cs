using Android.Content.Res;
using Android.Util;
using Android.Widget;

namespace SpotyPie.Helpers
{
    public static class TitleHelper
    {
        public readonly static string[] Spilt = { "- Live", " feat", " - ", " (" };

        public readonly static string[] Remove = { "Remix", "remix" };

        public static void Format(TextView text, string title, int maxSp)
        {
            foreach (var x in Spilt)
            {
                if (title.Contains(x))
                {
                    title = title.Split(x)[0];
                }
            }

            foreach (var x in Remove)
            {
                if (title.Contains(x))
                {
                    title = title.Replace(x, "");
                }
            }
            text.Text = title.Trim();
            text.Measure(0, 0);
            var newSp = (int)((text.TextSize / Resources.System.DisplayMetrics.ScaledDensity * text.Width) / text.MeasuredWidth);
            if(newSp < maxSp)
                text.SetTextSize(ComplexUnitType.Sp, newSp);
            else
                text.SetTextSize(ComplexUnitType.Sp, maxSp);
        }
    }
}