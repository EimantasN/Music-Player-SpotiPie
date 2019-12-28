﻿using Android.Widget;
using SpotyPie.Base;
using SpotyPie.Enums;
using System;
using System.Collections.Generic;

namespace SpotyPie.Models
{
    public class SpotyPieFragmetManager
    {
        public IAutoFragmentManagement Host { get; set; }

        public FragmentState CurrentFragmentState { get; set; }

        public Stack<FragmentState> FragmentHistory { get; } = new Stack<FragmentState>();

        public Stack<FragmentEnum> FragmentStack { get; } = new Stack<FragmentEnum>();

        public Stack<Action> FragmentBackBtn { get; } = new Stack<Action>();

        private bool Loading { get; set; } = false;

        public SpotyPieFragmetManager(IAutoFragmentManagement host)
        {
            Host = host;
        }

        public bool CheckFragments()
        {
            if (FragmentStack == null || FragmentStack.Count <= 1)
                return true;

            FragmentEnum fragmentState = FragmentStack.Pop();
            return false;
        }

        //TODO Investigate
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



        public void LoadFragmentInner(FragmentEnum switcher, string jsonModel = null, bool AddToBackButtonStack = true, LayoutScreenState screen = LayoutScreenState.Holder)
        {
            try
            {
                if (Loading)
                    return;

                Loading = true;

                if (FragmentHistory == null)
                    return;

                FragmentState newFState = FormatFragmentState(AddToBackButtonStack, screen, jsonModel);

                //Custom Fragment Load Logic
                FragmentBase fragment = Host.LoadFragment(switcher);

                if (switcher == FragmentEnum.Player)
                    return;

                if (fragment != null)
                {
                    newFState.Fragment = fragment;
                    newFState.Fragment.SendData(newFState.JsonData);
                }
                else
                {
                    Toast.MakeText(
                        Host.Context,
                        $"Provider LoadFragment Implementation For {switcher.ToString()}",
                        ToastLength.Long
                        ).Show();

                    return;
                }

                if (InsertFragment(newFState.LayoutId, newFState.Fragment))
                {
                    FragmentHistory.Push(newFState);
                    FragmentStack.Push(switcher);
                    newFState.BackButtonAction =
                        () =>
                        {
                            Host.RemoveCurrentFragment(Host.SupportFragmentManager, newFState.Fragment);
                        };
                    Host.SetScreen(screen);
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                Loading = false;
            }
        }

        private FragmentState FormatFragmentState(bool AddToBackButtonStack, LayoutScreenState screen, string json)
        {
            FragmentState state = new FragmentState();
            state.AddToBackStack = AddToBackButtonStack;
            state.FatherState = CurrentFragmentState;
            state.ScreenState = screen;

            //Hide father fragment if exists
            state.FatherState?.Fragment?.Hide();

            //Can send data to fragment
            if (!string.IsNullOrEmpty(json))
            {
                state.SendData(json);
            }

            state.LayoutId = Host.GetFragmentViewId();

            return state;
        }

        public bool InsertFragment(int layoutId, FragmentBase fragment)
        {
            try
            {
                if (Host?.SupportFragmentManager != null)
                {
                    var transaction = Host.SupportFragmentManager.BeginTransaction();
                    if (transaction != null)
                    {
                        FragmentTransitions.SetCustomTransitions(ref transaction, fragment);
                        transaction.Replace(layoutId, fragment);
                        transaction.Commit();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckBackButton()
        {
            if (FragmentHistory == null || FragmentHistory.Count == 0)
            {
                return true;
            }

            FragmentState state = FragmentHistory.Peek();
            state.BackButtonAction?.Invoke();

            FragmentHistory.Pop();

            if (FragmentHistory.Count != 0)
                Host.SetScreen(FragmentHistory.Peek().ScreenState);

            if (FragmentHistory.Count != 0 && state.LayoutId == FragmentHistory.Peek().LayoutId)
                InsertFragment(FragmentHistory.Peek().LayoutId, FragmentHistory.Peek().Fragment);

            return false;
        }

        public void Reset()
        {
            CurrentFragmentState = null;
            FragmentHistory.Clear();
            FragmentStack.Clear();
            FragmentBackBtn.Clear();
        }
    }
}