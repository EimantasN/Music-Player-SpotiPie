using System.Collections.Generic;
using Android.Support.V7.Util;
using Mobile_Api.Interfaces;

namespace SpotyPie.RecycleView.Helpers
{
    public class RecycleUpdate<T> : DiffUtil.Callback where T : IBaseInterface
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
            if (newList[newItemPosition].GetId() == Current_state.Id || newList[newItemPosition].GetId() == Current_state.PrevId)
                return false;
            return true;
        }
    }
}