using System.Collections.Generic;
using Android.Support.V7.Util;
using Android.Support.V7.Widget;
using Android.Widget;
using Mobile_Api.Interfaces;
using SpotyPie.Base;
using SpotyPie.RecycleView.Helpers;

namespace SpotyPie.RecycleView
{
    public class ThreadSafeRvList<T> where T : IBaseInterface<T>
    {
        public bool Updating = false;

        private List<T> mItems;

        private FragmentBase _activity;

        public List<T> GetList()
        {
            return mItems;
        }

        public void Erase()
        {
            mItems = new List<T>();
        }

        public ThreadSafeRvList(FragmentBase activity)
        {
            _activity = activity;
            mItems = new List<T>();
        }

        public RecyclerView.Adapter Adapter { get; set; }

        public void Clear()
        {
            _activity?.RunOnUiThread(() =>
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
            _activity?.RunOnUiThread(() =>
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
            _activity?.RunOnUiThread(() =>
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
            if (position < 0 || position > mItems.Count || position > Adapter.ItemCount)
                return;

            Updating = true;
            mItems.RemoveAt(position);
            Adapter?.NotifyItemRemoved(0);
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