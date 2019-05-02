using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Microsoft.Win32.SafeHandles;
using Mobile_Api.Interfaces;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.RecycleView.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SpotyPie.RecycleView
{
    public class BaseRecycleView<T> where T : IBaseInterface
    {
        private bool Disposed = false;
        private RvList<T> RvDataset;
        private SpotyPieRv CustomRecyclerView;
        private LayoutManagers Manager = LayoutManagers.Unseted;

        private Action CustomAction;

        private FragmentBase Activity { get; set; }

        private int Id { get; set; }

        public BaseRecycleView(FragmentBase Activity, int RvId)
        {
            RvDataset = new RvList<T>();

            this.Activity = Activity;
            this.Id = RvId;
        }

        internal void SetClickAction(Action p)
        {
            CustomAction = p;
        }

        public RvList<T> Setup(LayoutManagers layout)
        {
            Init(layout);
            return RvDataset;
        }

        public void Init(LayoutManagers layout)
        {
            CustomRecyclerView = new SpotyPieRv(Activity.GetView().FindViewById<RecyclerView>(Id));
            SetLayoutManager(layout);
            CustomRecyclerView.GetRecycleView().SetAdapter(new BaseRv<T>(RvDataset, CustomRecyclerView.GetRecycleView(), Activity.Context));
            RvDataset.Adapter = CustomRecyclerView.GetRecycleView().GetAdapter();
            SetOnClick();
        }

        public RvList<T> GetData()
        {
            return RvDataset;
        }

        public void SetLayoutManager(LayoutManagers layout)
        {
            switch (layout)
            {
                case LayoutManagers.Linear_vertical:
                    {
                        if (Manager == LayoutManagers.Unseted || Manager != LayoutManagers.Linear_vertical)
                        {
                            Manager = LayoutManagers.Linear_vertical;
                            CustomRecyclerView.GetRecycleView().SetLayoutManager(new LinearLayoutManager(this.Activity.Activity, LinearLayoutManager.Vertical, false));
                        }
                        break;
                    }
                case LayoutManagers.Linear_horizontal:
                    {
                        if (Manager == LayoutManagers.Unseted || Manager != LayoutManagers.Linear_horizontal)
                        {
                            Manager = LayoutManagers.Linear_horizontal;
                            CustomRecyclerView.GetRecycleView().SetLayoutManager(new LinearLayoutManager(this.Activity.Activity, LinearLayoutManager.Horizontal, false));
                        }
                        break;
                    }
                case LayoutManagers.Grind_2_col:
                    {
                        if (Manager == LayoutManagers.Unseted || Manager != LayoutManagers.Grind_2_col)
                        {
                            Manager = LayoutManagers.Grind_2_col;
                            CustomRecyclerView.GetRecycleView().SetLayoutManager(new GridLayoutManager(this.Activity.Activity, 2));
                        }
                        break;
                    }
            }
        }

        public void DisableScroolNested()
        {
            CustomRecyclerView.GetRecycleView().NestedScrollingEnabled = false;
            ViewCompat.SetNestedScrollingEnabled(CustomRecyclerView.GetRecycleView(), false);
        }

        public void SetOnClick()
        {
            CustomRecyclerView.GetRecycleView().SetItemClickListener((rv, position, view) =>
            {
                if (CustomRecyclerView != null && CustomRecyclerView.GetRecycleView().ChildCount != 0)
                {
                    if (RvDataset[position].GetType().Name == "Album")
                    {
                        Task.Run(() => Activity.GetAPIService().UpdateAsync<Album>(RvDataset[position].GetId()));
                        Activity.LoadAlbum(RvDataset[position] as Album);
                    }
                    else if (RvDataset[position].GetType().Name == "Songs")
                    {
                        Activity.GetState().SetSong(RvDataset.GetList() as List<Songs>, position);
                    }
                    else
                    {
                        Activity.LoadArtist(RvDataset[position] as Artist);
                    }

                    CustomAction?.Invoke();
                }
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                RvDataset.Clear();
                RvDataset = null;
                Activity = null;
                CustomRecyclerView.Dispose();
                CustomRecyclerView = null;
            }

            Disposed = true;
        }
    }
}