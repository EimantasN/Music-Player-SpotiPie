using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Support.V7.Widget;
using Android.Widget;

namespace SpotyPie.Helpers
{
    public class RecycleViewList<T>
    {
        private List<T> mItems;
        private RecyclerView.Adapter mAdapter;

        public void Erase()
        {
            mItems = new List<T>();
        }

        public RecycleViewList()
        {
            mItems = new List<T>();
        }

        public RecyclerView.Adapter Adapter
        {
            get { return mAdapter; }
            set { mAdapter = value; }
        }

        public void Add(T item)
        {
            Application.SynchronizationContext.Post(_ =>
            {
                mItems.Add(item);

                if (Adapter != null)
                {
                    Adapter.NotifyItemInserted(Count);
                }
            }, null);
        }

        public void Remove(int position)
        {
            mItems.RemoveAt(position);

            if (Adapter != null)
            {
                Adapter.NotifyItemRemoved(0);
            }
        }

        public T this[int index]
        {
            get { return mItems[index]; }
            set { mItems[index] = value; }
        }

        public int Count
        {
            get { return mItems.Count; }
        }

        public void clear()
        {
            Application.SynchronizationContext.Post(_ =>
            {
                try
                {
                    int size = mItems.Count;
                    for (int i = 0; i < mItems.Count; i++)
                    {
                        if (mItems[i] != null)
                        {
                            Remove(i);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Adapter.NotifyDataSetChanged();
            }, null);
        }

        internal Task ClearAsync()
        {
            throw new NotImplementedException();
        }
    }
}