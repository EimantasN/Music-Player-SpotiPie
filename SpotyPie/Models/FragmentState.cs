using SpotyPie.Base;
using SpotyPie.Enums;
using System;

namespace SpotyPie.Models
{
    public enum StateStatus
    {
        Stateful,
        Stateless
    }

    public class FragmentState
    {
        public int LayoutId { get; set; }

        public Screen ScreenState { get; set; } = Screen.Holder;

        public string JsonData { get; set; }

        public dynamic FragmentEnum { get; set; }

        public dynamic FatherFragmentEnum { get; set; }

        public StateStatus State { get; set; } = StateStatus.Stateful;

        public bool SkipMe { get; set; }

        public FragmentBase Fragment { get; set; }

        public FragmentState FatherState { get; set; }

        public Action BackButton { get; set; }

        public bool AddToBackStack { get; set; } = true;

        public void SetLayoutId(int Id)
        {
            this.LayoutId = Id;
        }

        public void SetFragmentData(string JsonData)
        {
            this.JsonData = JsonData;
        }

        public void SetCurrentFragment(dynamic fragment)
        {
            this.FragmentEnum = fragment;
        }

        public void SerFatherFragment(dynamic fatherFragment)
        {
            this.FatherFragmentEnum = fatherFragment;
        }

        internal void SendData(string jsonModel)
        {
            this.Fragment.SendData(jsonModel);
            this.JsonData = jsonModel;
        }
    }
}