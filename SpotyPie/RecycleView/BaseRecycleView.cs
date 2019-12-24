using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Microsoft.Win32.SafeHandles;
using Mobile_Api.Interfaces;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.Enums.Activitys;
using SpotyPie.Helpers;
using SpotyPie.Music.Manager;
using SpotyPie.RecycleView.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SpotyPie.RecycleView
{
    public class BaseRecycleView<T> where T : IBaseInterface<T>
    {
        private int RvId { get; set; }

        private bool Disposed { get; set; } = false;

        public int LastPosition { get; private set; }

        public bool IgnoreClick { get; set; } = false;

        private RvList<T> RvDataset { get; set; }
        private SpotyPieRv CustomRecyclerView { get; set; }
        private LayoutManagers Manager { get; set; } = LayoutManagers.Unseted;
        private List<Action> CustomActions { get; set; }
        private FragmentBase Activity { get; set; }

        public BaseRecycleView(FragmentBase activity, int rvId)
        {
            RvDataset = new RvList<T>(activity);
            this.Activity = activity;
            this.RvId = rvId;
        }

        public RvList<T> Setup(LayoutManagers layout, List<Action> actionsToView = null)
        {
            if (actionsToView != null)
            {
                CustomActions = actionsToView;
                IgnoreClick = true;
            }

            Init(layout);
            return RvDataset;
        }

        public void Init(LayoutManagers layout)
        {
            CustomRecyclerView = new SpotyPieRv(Activity.GetView().FindViewById<RecyclerView>(RvId));
            SetLayoutManager(layout);
            CustomRecyclerView.GetRecycleView().SetAdapter(new BaseRv<T>(RvDataset, CustomRecyclerView.GetRecycleView(), Activity.Context, CustomActions));
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

        internal void SetFocusable(bool v)
        {
            CustomRecyclerView.GetRecycleView().Focusable = v;
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
                LastPosition = position;
                if (!RvDataset.Updating && !IgnoreClick && CustomRecyclerView != null && CustomRecyclerView.GetRecycleView().ChildCount != 0)
                {
                    if (RvDataset[position].GetType().Name == "Album")
                    {
                        Task.Run(() => Activity.GetAPIService().UpdateAsync<Album>(RvDataset[position].GetId()));
                        Activity.LoadAlbum(RvDataset[position] as Album);
                    }
                    else if (RvDataset[position].GetType().Name == "Songs")
                    {
                        Songs song = RvDataset[position] as Songs;
                        if (song.GetModelType() != Mobile_Api.Models.Enums.RvType.SongBindList)
                        {
                            SongManager.SetSongs(RvDataset.GetList() as List<Songs>, position);
                        }
                    }
                    else if (RvDataset[position].GetType().Name == "SongTag")
                    {
                        //Ignore i sending action only run fragment
                    }
                    else if (RvDataset[position].GetType().Name == "SongOptions")
                    {

                    }
                    else if (RvDataset[position].GetType().Name == "Artist")
                    {
                        try
                        {
                            Activity.LoadArtist(RvDataset[position] as Artist);
                        }
                        catch (Exception e)
                        {
                        }
                    }
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