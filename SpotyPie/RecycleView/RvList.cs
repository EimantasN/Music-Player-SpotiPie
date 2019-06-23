using System.Collections.Generic;
using Android.Support.V7.Util;
using Android.Support.V7.Widget;
using Android.Widget;
using Mobile_Api.Interfaces;
using SpotyPie.Base;
using SpotyPie.RecycleView.Helpers;

namespace SpotyPie.RecycleView
{
    public class RvList<T> where T : IBaseInterface<T>
    {
        public bool Updating = false;
        private List<T> mItems;
        private RecyclerView.Adapter mAdapter;
        private FragmentBase Activity;

        public List<T> GetList()
        {
            return mItems;
        }

        public void Erase()
        {
            mItems = new List<T>();
        }

        public RvList(FragmentBase activity)
        {
            Activity = activity;
            mItems = new List<T>();
        }

        public RecyclerView.Adapter Adapter
        {
            get { return mAdapter; }
            set { mAdapter = value; }
        }

        public void Clear()
        {
            Activity.RunOnUiThread(() =>
            {
                Updating = true;
                DiffUtil.DiffResult result = DiffUtil.CalculateDiff(new RecycleUpdate<T>(mItems, new List<T>()), false);

                // Overwrite the old data
                Erase();
                mItems.AddRange(new List<T>());
                result.DispatchUpdatesTo(Adapter);
                Updating = false;
            });
        }

        public void AddList(List<T> newData)
        {
            Activity.RunOnUiThread(() =>
            {
                Updating = true;
                DiffUtil.DiffResult result = DiffUtil.CalculateDiff(new RecycleUpdate<T>(mItems, newData), true);

                // Overwrite the old data
                Erase();
                mItems.AddRange(newData);

                // Despatch the updates to your RecyclerAdapter
                result.DispatchUpdatesTo(Adapter);
                Updating = false;
            });
        }

        public void Add(T item)
        {
            Activity.RunOnUiThread(() =>
            {
                Updating = true;
                mItems.Add(item);

                if (Adapter != null)
                {
                    Adapter.NotifyItemInserted(Count);
                }
                Updating = false;
            });
        }

        public void Remove(int position)
        {
            Updating = true;
            mItems.RemoveAt(position);

            if (Adapter != null)
            {
                Adapter.NotifyItemRemoved(0);
            }
            Updating = false;
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
    }
}