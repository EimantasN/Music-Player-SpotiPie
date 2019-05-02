using System;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;

namespace SpotyPie.RecycleView
{
    public class SpotyPieRv : RecyclerView, RecyclerView.IOnChildAttachStateChangeListener
    {
        private AttachStateChangeListener Lisiner;

        public SpotyPieRv(Context context) : base(context)
        {
        }

        public void SetAction(Action<RecyclerView, int, View> action)
        {
            Lisiner = new AttachStateChangeListener(this, action);
        }

        public void OnChildViewAttachedToWindow(View view)
        {
            this.AddOnChildAttachStateChangeListener(Lisiner);
        }

        public void OnChildViewDetachedFromWindow(View view)
        {
            if(Lisiner != null)
                this.RemoveOnChildAttachStateChangeListener(Lisiner);
        }
    }
}