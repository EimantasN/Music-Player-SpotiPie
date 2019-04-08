using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V7.Util;
using Android.Support.V7.Widget;
using Android.Widget;
using SpotyPie.RecycleView.Helpers;

namespace SpotyPie.RecycleView
{
    public class RvList<T>
    {
        private List<dynamic> mItems;
        private RecyclerView.Adapter mAdapter;

        public List<dynamic> GetList()
        {
            return mItems;
        }

        private int LoadingIndex { get; set; } = -1;

        public void Erase()
        {
            mItems = new List<dynamic>();
        }

        public RvList()
        {
            mItems = new List<dynamic>();
        }

        public RecyclerView.Adapter Adapter
        {
            get { return mAdapter; }
            set { mAdapter = value; }
        }

        public void AddList(List<T> newData)
        {
            List<dynamic> newList = new List<dynamic>();
            newData.ForEach(x => newList.Add((dynamic)x));
            AddList(newList);
        }

        public void AddList(List<dynamic> newData)
        {
            Application.SynchronizationContext.Post(_ =>
            {
                DiffUtil.DiffResult result = DiffUtil.CalculateDiff(new RecycleUpdate(mItems, newData), true);

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

        internal void RemoveLoading(List<dynamic> data)
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

        public void Clear()
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
    }
}