using System;
using Android.Support.V7.Widget;
using Android.Views;

namespace SpotyPie.RecycleView
{
    public class SpotyPieRv : Java.Lang.Object, RecyclerView.IOnChildAttachStateChangeListener
    {
        private AttachStateChangeListener Lisiner;
        private RecyclerView Rv;

        public SpotyPieRv(RecyclerView rv)
        {
            Rv = rv;
        }

        public void SetAction(Action<RecyclerView, int, View> action)
        {
            Lisiner = new AttachStateChangeListener(this.Rv, action);
        }

        public void OnChildViewAttachedToWindow(View view)
        {
            if (Lisiner != null)
                this.Rv.AddOnChildAttachStateChangeListener(Lisiner);
            throw new Exception("Lisiner cant be null");
        }

        public void OnChildViewDetachedFromWindow(View view)
        {
            if (Lisiner != null)
                this.Rv.RemoveOnChildAttachStateChangeListener(Lisiner);
        }

        public RecyclerView GetRecycleView()
        {
            return Rv;
        }
    }
}