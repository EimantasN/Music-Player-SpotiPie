using System.Collections.Generic;
using Android.Support.V7.Util;
using Mobile_Api.Interfaces;
using Newtonsoft.Json;

namespace SpotyPie.RecycleView.Helpers
{
    public class RecycleUpdate<T> : DiffUtil.Callback where T : IBaseInterface<T>
    {
        private List<T> oldList;
        private List<T> newList;

        public RecycleUpdate(List<T> oldList, List<T> newList)
        {
            this.oldList = oldList;
            this.newList = newList;
        }

        public override int OldListSize => oldList.Count;

        public override int NewListSize => newList.Count;

        public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
        {
            if (oldList[oldItemPosition] == null || newList[newItemPosition] == null)
                return false;

            return oldList[oldItemPosition].GetId() == newList[newItemPosition].GetId();
        }

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            if (oldList[oldItemPosition].GetId() == newList[newItemPosition].GetId())
            {
                if (oldList[oldItemPosition].Equals(newList[newItemPosition]))
                    return true;
                return true;
            }
            return true;
        }
    }
}