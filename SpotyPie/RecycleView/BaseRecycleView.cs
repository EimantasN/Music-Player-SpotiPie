﻿using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Widget;
using Mobile_Api.Interfaces;
using Mobile_Api.Models;
using Newtonsoft.Json;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.RecycleView.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotyPie.RecycleView
{
    public class BaseRecycleView<T> where T : IBaseInterface
    {
        private RvList<T> RvDataset;
        private RecyclerView.Adapter RvAdapter;
        private RecyclerView RecyclerView;
        private LayoutManagers Manager = LayoutManagers.Unseted;

        private FragmentBase Activity { get; set; }
        private int Id { get; set; }

        public BaseRecycleView(FragmentBase Activity, int RvId)
        {
            RvDataset = new RvList<T>();

            this.Activity = Activity;
            this.Id = RvId;
        }

        public RvList<T> Setup(LayoutManagers layout)
        {
            Init(layout);
            return RvDataset;
        }

        public void Init(LayoutManagers layout)
        {
            RecyclerView = Activity.GetView().FindViewById<RecyclerView>(Id);
            SetLayoutManager(layout);
            RvAdapter = new BaseRv<T>(RvDataset, RecyclerView, Activity.Context);
            RvDataset.Adapter = RvAdapter;
            RecyclerView.SetAdapter(RvAdapter);
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
                            RecyclerView.SetLayoutManager(new LinearLayoutManager(this.Activity.Activity, LinearLayoutManager.Vertical, false));
                        }
                        break;
                    }
                case LayoutManagers.Linear_horizontal:
                    {
                        if (Manager == LayoutManagers.Unseted || Manager != LayoutManagers.Linear_horizontal)
                        {
                            Manager = LayoutManagers.Linear_horizontal;
                            RecyclerView.SetLayoutManager(new LinearLayoutManager(this.Activity.Activity, LinearLayoutManager.Horizontal, false));
                        }
                        break;
                    }
                case LayoutManagers.Grind_2_col:
                    {
                        if (Manager == LayoutManagers.Unseted || Manager != LayoutManagers.Grind_2_col)
                        {
                            Manager = LayoutManagers.Grind_2_col;
                            RecyclerView.SetLayoutManager(new GridLayoutManager(this.Activity.Activity, 2));
                        }
                        break;
                    }
            }
        }

        public void DisableScroolNested()
        {
            RecyclerView.NestedScrollingEnabled = false;
            ViewCompat.SetNestedScrollingEnabled(RecyclerView, false);
        }

        public void SetOnClick()
        {
            RecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (RecyclerView != null && RecyclerView.ChildCount != 0)
                {
                    if (RvDataset[position].GetType().Name == "Album")
                    {
                        Task.Run(() => Activity.GetService().Update(RvDataset[position].GetId()));
                        Activity.LoadAlbum(RvDataset[position] as Album);
                    }
                    else if (RvDataset[position].GetType().Name == "Songs")
                    {
                        Activity.GetState().SetSong(JsonConvert.DeserializeObject<List<Songs>>(JsonConvert.SerializeObject(RvDataset.GetList())), position);
                    }
                    else
                    {
                        Activity.LoadArtist(RvDataset[position] as Artist);
                    }
                }
            });
        }
    }
}