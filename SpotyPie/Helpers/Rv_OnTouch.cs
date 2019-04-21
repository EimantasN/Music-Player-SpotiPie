using Android.Support.V7.Widget;
using Android.Views;
using System;

namespace SpotyPie.Helpers
{
    public static class RV_clRv_OnTouchick
    {
        public static void SetItemTouchListener(this RecyclerView rv, Action<RecyclerView, int, View> action)
        {
            rv.AddOnChildAttachStateChangeListener(new AttachStateChangeListenerTouch(rv, action));
        }
    }

    public class AttachStateChangeListenerTouch : Java.Lang.Object, RecyclerView.IOnChildAttachStateChangeListener
    {
        private RecyclerView mRecyclerview;
        private Action<RecyclerView, int, View> mAction;

        public AttachStateChangeListenerTouch(RecyclerView rv, Action<RecyclerView, int, View> action) : base()
        {
            mRecyclerview = rv;
            mAction = action;
        }

        public void OnChildViewAttachedToWindow(View view)
        {
        }

        private void View_LongClick(object sender, View.LongClickEventArgs e)
        {
        }

        private void View_Touch(object sender, View.TouchEventArgs e)
        {
        }

        public void OnChildViewDetachedFromWindow(View view)
        {
        }
    }
}