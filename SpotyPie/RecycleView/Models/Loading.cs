using Android.Support.V7.Widget;
using Android.Views;

namespace SpotyPie.RecycleView.Models
{
    public class Loading : RecyclerView.ViewHolder
    {
        public View LoadingView { get; set; }

        public Loading(View view, ViewGroup parent) : base(view)
        {
        }
    }
}