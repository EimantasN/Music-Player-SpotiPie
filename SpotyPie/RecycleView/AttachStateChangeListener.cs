using System;
using Android.Support.V7.Widget;
using Android.Views;

namespace SpotyPie.RecycleView
{
    public class AttachStateChangeListener : Java.Lang.Object, RecyclerView.IOnChildAttachStateChangeListener
    {
        private RecyclerView mRecyclerview;
        private Action<RecyclerView, int, View> mAction;

        public AttachStateChangeListener(RecyclerView rv, Action<RecyclerView, int, View> action) : base()
        {
            mRecyclerview = rv;
            mAction = action;
        }

        public void OnChildViewAttachedToWindow(View view)
        {
            view.Click += View_Click;
        }

        private void View_Click(object sender, EventArgs e)
        {
            mAction.Invoke(mRecyclerview, mRecyclerview.GetChildViewHolder(((View)sender)).AdapterPosition, ((View)sender));
        }

        public void OnChildViewDetachedFromWindow(View view)
        {
            view.Click -= View_Click;
        }
    }
}