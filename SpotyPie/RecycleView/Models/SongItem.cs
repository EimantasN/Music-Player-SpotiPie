using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace SpotyPie.RecycleView.Models
{
    public class SongItem : RecyclerView.ViewHolder
    {
        public View EmptyTimeView { get; set; }

        public TextView Title { get; set; }

        public TextView SubTitile { get; set; }

        public ImageButton Options { get; set; }

        public SongItem(View view) : base(view) { }
    }
}