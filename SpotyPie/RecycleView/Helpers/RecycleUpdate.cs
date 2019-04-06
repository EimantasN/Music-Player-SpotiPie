﻿using System.Collections.Generic;
using Android.Support.V7.Util;
using Newtonsoft.Json;

namespace SpotyPie.RecycleView.Helpers
{
    public class RecycleUpdate : DiffUtil.Callback
    {
        private List<dynamic> oldList;
        private List<dynamic> newList;

        public RecycleUpdate(List<dynamic> oldList, List<dynamic> newList)
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

            return oldList[oldItemPosition].Id == newList[newItemPosition].Id;
        }

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            if (newList[newItemPosition].Id == Current_state.Id || newList[newItemPosition].Id == Current_state.PrevId)
                return false;
            return true;
        }
    }
}