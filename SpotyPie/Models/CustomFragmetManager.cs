using SpotyPie.Base;
using System;
using System.Collections.Generic;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SpotyPie.Models
{
    public class CustomFragmetManager
    {
        public ActivityBase Activity { get; set; }

        public FragmentState CurrentFragmentState { get; set; }

        public Stack<FragmentState> FragmentHistory { get; private set; } = new Stack<FragmentState>();

        public Stack<dynamic> FragmentStack { get; set; }

        public Stack<Action> FragmentBackBtn { get; set; }

        public CustomFragmetManager(ActivityBase activity)
        {
            this.Activity = activity;
        }

        public bool CheckFragments()
        {
            if (GetFragmentStack() == null || GetFragmentStack().Count <= 1)
                return true;

            dynamic fragmentState = GetFragmentStack().Pop();
            if (fragmentState == null)
                return true;
            return false;
        }

        public Stack<dynamic> GetFragmentStack()
        {
            if (FragmentStack == null)
                FragmentStack = new Stack<dynamic>();
            return FragmentStack;
        }

        public void SetCurrentFragment(FragmentBase fragment)
        {
            FragmentHistory.Peek().Fragment = fragment;
        }

        public FragmentBase GetCurrentFragment()
        {
            return FragmentHistory?.Peek()?.Fragment;
        }

        public void LoadFragmentInner(dynamic switcher, string jsonModel = null, bool AddToBackButtonStack = true, SupportFragmentManager supportFragmentManager = null)
        {
            FragmentHistory.Push(new FragmentState());
            FragmentHistory.Peek().AddToBackStack = AddToBackButtonStack;
            FragmentHistory.Peek().FatherState = CurrentFragmentState;

            //if (IsFragmentLoadedAdded)
            //{
            //    if (FragmentFrame.Visibility == ViewStates.Gone)
            //        FragmentFrame.Visibility = ViewStates.Visible;

            //    if (FragmentLoading.Visibility == ViewStates.Gone)
            //        FragmentLoading.Visibility = ViewStates.Visible;
            //}

            if (FragmentHistory.Peek()?.FatherState?.Fragment != null)
            {
                CurrentFragmentState?.FatherState?.Fragment.Hide();
            }

            //CurrentFragment = null;

            GetFragmentStack().Push(switcher);

            Activity.LoadFragment(switcher);

            if (FragmentHistory?.Peek()?.Fragment == null)
            {
                throw new Exception("Fragment not founded");
            }

            //Can send data to fragment
            if (!string.IsNullOrEmpty(jsonModel))
                FragmentHistory?.Peek()?.SendData(jsonModel);

            //if (!FragmentHistory?.Peek()?.IsAdded)
            //{

            if (FragmentHistory.Peek().Fragment is Player.Player)
                FragmentHistory.Peek().LayoutId = Activity.GetFragmentViewId(true);
            else
                FragmentHistory.Peek().LayoutId = Activity.GetFragmentViewId();

            InsertFragment(FragmentHistory.Peek().LayoutId, FragmentHistory.Peek().Fragment);
            FragmentHistory.Peek().BackButton = () => { Activity.RemoveCurrentFragment(Activity.SupportFragmentManager, FragmentHistory.Peek().Fragment); };
            //}
            //else
            //{
            //    CurrentFragment.Show();
            //}
        }

        public void InsertFragment(int layoutId, FragmentBase fragment)
        {
            Activity.SupportFragmentManager.BeginTransaction()
            .Replace(layoutId, fragment)
            .Commit();
        }

        public void SetBackBtn(Action action)
        {
            if (FragmentBackBtn == null)
                FragmentBackBtn = new Stack<Action>();

            FragmentBackBtn.Push(action);
        }

        public bool CheckBackButton()
        {
            //if (FragmentBackBtn == null || FragmentBackBtn.Count == 0)
            //    return true;
            //FragmentBackBtn.Pop()?.Invoke();

            if (FragmentHistory == null && FragmentHistory.Count == 0)
            {
                return true;
            }

            var state = FragmentHistory.Pop();
            state.BackButton?.Invoke();

            if (state.LayoutId == int.MaxValue - 1)
                Activity.RemovePlayerView();

            if (FragmentHistory.Count != 0)
            {
                if (FragmentHistory.Peek().LayoutId == state.LayoutId)
                    InsertFragment(FragmentHistory.Peek().LayoutId, FragmentHistory.Peek().Fragment);
            }

            return false;
        }



        public void OnResume()
        {

        }

        public void OnStop()
        {

        }
    }
}