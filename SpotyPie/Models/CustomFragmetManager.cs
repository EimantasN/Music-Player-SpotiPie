using Android.App;
using SpotyPie.Base;
using SpotyPie.Enums;
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
            if (FragmentHistory.Count == 0)
                FragmentHistory.Push(new FragmentState());
            FragmentHistory.Peek().Fragment = fragment;
        }

        public FragmentBase GetCurrentFragment()
        {
            return FragmentHistory?.Peek()?.Fragment;
        }

        public void LoadFragmentInner(dynamic switcher, string jsonModel = null, bool AddToBackButtonStack = true, LayoutScreenState screen = LayoutScreenState.Holder)
        {
            FragmentHistory.Push(new FragmentState());
            FragmentHistory.Peek().AddToBackStack = AddToBackButtonStack;
            FragmentHistory.Peek().FatherState = CurrentFragmentState;
            FragmentHistory.Peek().ScreenState = screen;


            if (FragmentHistory.Peek()?.FatherState?.Fragment != null)
            {
                CurrentFragmentState?.FatherState?.Fragment.Hide();
            }

            GetFragmentStack().Push(switcher);

            Activity.LoadFragment(switcher);

            if (FragmentHistory?.Peek()?.Fragment == null)
            {
                throw new Exception("Fragment not founded");
            }

            //Can send data to fragment
            if (!string.IsNullOrEmpty(jsonModel))
                FragmentHistory?.Peek()?.SendData(jsonModel);

            if (FragmentHistory.Peek().Fragment is Player.Player)
                FragmentHistory.Peek().LayoutId = Activity.GetFragmentViewId(true);
            else
                FragmentHistory.Peek().LayoutId = Activity.GetFragmentViewId();

            InsertFragment(FragmentHistory.Peek().LayoutId, FragmentHistory.Peek().Fragment);
            FragmentHistory.Peek().BackButton = () => { Activity.RemoveCurrentFragment(Activity.SupportFragmentManager, FragmentHistory.Peek().Fragment); };
        }

        public void InsertFragment(int layoutId, FragmentBase fragment)
        {
            if (Activity?.SupportFragmentManager != null)
            {
                var transaction = Activity.SupportFragmentManager.BeginTransaction();
                if (transaction != null)
                {
                    transaction.SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_left);
                    transaction.Replace(layoutId, fragment);
                    //transaction.AddToBackStack(null);
                    transaction.Commit();
                }
            }
        }

        public void SetBackBtn(Action action)
        {
            if (FragmentBackBtn == null)
                FragmentBackBtn = new Stack<Action>();

            FragmentBackBtn.Push(action);
        }

        public bool CheckBackButton()
        {
            if (FragmentHistory == null || FragmentHistory.Count == 0)
            {
                return true;
            }

            FragmentState state = FragmentHistory.Peek();
            state.BackButton?.Invoke();

            FragmentHistory.Pop();

            if (FragmentHistory.Count != 0)
                Activity.SetScreen(FragmentHistory.Peek().ScreenState);

            if (FragmentHistory.Count != 0 && state.LayoutId == FragmentHistory.Peek().LayoutId)
                InsertFragment(FragmentHistory.Peek().LayoutId, FragmentHistory.Peek().Fragment);

            if (state.LayoutId == int.MaxValue - 1)
                Activity.RemovePlayerView();

            return false;
        }

        public void Reset()
        {
            CurrentFragmentState = null;
            FragmentHistory = new Stack<FragmentState>();
            FragmentStack = new Stack<dynamic>();
            FragmentBackBtn = new Stack<Action>();
        }
    }
}