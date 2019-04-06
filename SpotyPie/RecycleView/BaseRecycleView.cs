using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using SpotyPie.Base;
using SpotyPie.Helpers;
using System.Collections.Generic;

namespace SpotyPie.RecycleView
{
    public class BaseRecycleView<T>
    {
        private RvList<T> RvDataset;
        private RecyclerView.LayoutManager LayoutManager;
        private RecyclerView.Adapter RvAdapter;
        private RecyclerView RecyclerView;

        private FragmentBase Activity { get; set; }
        private int Id { get; set; }

        public BaseRecycleView(FragmentBase Activity, int RvId)
        {
            RvDataset = new RvList<T>();

            this.Activity = Activity;
            this.Id = RvId;
        }

        public RvList<T> Setup(int layoutPosition)
        {
            LayoutManager = new LinearLayoutManager(Activity.Activity, layoutPosition, false);
            RecyclerView = Activity.GetView().FindViewById<RecyclerView>(Id);
            RecyclerView.SetLayoutManager(LayoutManager);
            RvAdapter = new BaseRv<T>(RvDataset, RecyclerView, Activity.Context);
            RvDataset.Adapter = RvAdapter;
            RecyclerView.SetAdapter(RvAdapter);
            SetOnClick();
            return RvDataset;
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
                        Activity.LoadAlbum(RvDataset[position] as Album);
                    }
                    else if (RvDataset[position].GetType().Name == "Song")
                    {
                        Activity.GetState().SetSong(JsonConvert.DeserializeObject<List<Song>>(JsonConvert.SerializeObject(RvDataset.GetList())), position);
                    }
                }
            });
        }
    }
}