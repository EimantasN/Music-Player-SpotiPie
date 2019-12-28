using System.Collections.Generic;
using Android.Support.V7.Util;
using Android.Support.V7.Widget;
using Android.Widget;
using Mobile_Api.Interfaces;
using SpotyPie.RecycleView.Helpers;

namespace SpotyPie.RecycleView
{
    public class RvList<T> where T : IBaseInterface<T>
    {
        private List<T> mItems = new List<T>();

        public RecyclerView.Adapter Adapter { get; set; }

        public List<T> GetList()
        {
            return mItems;
        }

        public void Erase()
        {
            mItems.Clear();
        }

        public void Clear()
        {
            DiffUtil.DiffResult result = DiffUtil.CalculateDiff(new RecycleUpdate<T>(mItems, new List<T>()), false);

            // Overwrite the old data
            Erase();
            mItems.AddRange(new List<T>());

            result.DispatchUpdatesTo(Adapter);
        }

        public void AddList(List<T> newData)
        {
            DiffUtil.DiffResult result = DiffUtil.CalculateDiff(new RecycleUpdate<T>(mItems, newData), true);

            // Overwrite the old data
            Erase();
            mItems.AddRange(newData);

            // Despatch the updates to your RecyclerAdapter
            result.DispatchUpdatesTo(Adapter);
        }

        public void Add(T item)
        {
            mItems.Add(item);

            if (Adapter != null)
            {
                Adapter.NotifyItemInserted(Count);
            }
        }

        public void Remove(int position)
        {
            if (position < 0 || position > mItems.Count || position > Adapter.ItemCount)
                return;

            mItems.RemoveAt(position);
            Adapter?.NotifyItemRemoved(0);
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