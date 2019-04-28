using System.Collections.Generic;
using Android.App;
using Android.Support.V7.Util;
using Android.Support.V7.Widget;
using Android.Widget;
using Mobile_Api.Interfaces;
using SpotyPie.RecycleView.Helpers;

namespace SpotyPie.RecycleView
{
    public class RvList<T> where T : IBaseInterface
    {
        private List<T> mItems;
        private RecyclerView.Adapter mAdapter;

        public List<T> GetList()
        {
            return mItems;
        }

        private int LoadingIndex { get; set; } = -1;

        public void Erase()
        {
            mItems = new List<T>();
        }

        public RvList()
        {
            mItems = new List<T>();
        }

        public RecyclerView.Adapter Adapter
        {
            get { return mAdapter; }
            set { mAdapter = value; }
        }


        public void Clear()
        {
            Application.SynchronizationContext.Post(_ =>
            {
                DiffUtil.DiffResult result = DiffUtil.CalculateDiff(new RecycleUpdate<T>(mItems, new List<T>()), false);

                // Overwrite the old data
                Erase();
                mItems.AddRange(new List<T>());
                result.DispatchUpdatesTo(Adapter);
            }, null);
        }

        public void AddList(List<T> newData)
        {
            Application.SynchronizationContext.Post(_ =>
            {
                DiffUtil.DiffResult result = DiffUtil.CalculateDiff(new RecycleUpdate<T>(mItems, newData), false);

                // Overwrite the old data
                Erase();
                mItems.AddRange(newData);
                result.DispatchUpdatesTo(Adapter);
            }, null);
            // Despatch the updates to your RecyclerAdapter
        }

        public void Add(T item)
        {
            Application.SynchronizationContext.Post(_ =>
            {
                if (item == null)
                    LoadingIndex = mItems.Count;

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

        internal void RemoveLoading(List<T> data)
        {
            try
            {
                if (mItems[mItems.Count - 1] == null)
                {
                    Application.SynchronizationContext.Post(_ =>
                    {
                        mItems.RemoveAt(mItems.Count - 1);
                        Adapter.NotifyItemRemoved(LoadingIndex);
                    }, null);
                }
                else
                {
                    data.RemoveAll(x => x == null);
                    AddList(data);
                }
            }
            catch
            {
            }
        }

        internal void RemoveLoading()
        {
            try
            {

                if (LoadingIndex > -1)
                {
                    Application.SynchronizationContext.Post(_ =>
                    {
                        Remove(LoadingIndex);
                        Adapter.NotifyItemRemoved(LoadingIndex);
                    }, null);
                }
            }
            catch
            {
            }
        }
    }
}