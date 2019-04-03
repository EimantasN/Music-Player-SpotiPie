using Android.Support.V7.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.Models;
using System.Collections.Generic;

namespace SpotyPie.RecycleView
{
    public class BaseRecycleView<T, U>
    {
        private List<T> Dataset;
        private RvList<U> RvDataset;
        private RecyclerView.LayoutManager LayoutManager;
        private RecyclerView.Adapter RvAdapter;
        private RecyclerView RecyclerView;

        private FragmentBase Activity { get; set; }
        private int Id { get; set; }

        public BaseRecycleView(FragmentBase Activity, int RvId, List<T> data)
        {
            Dataset = data;
            RvDataset = new RvList<U>();

            this.Activity = Activity;
            this.Id = RvId;
        }

        public RvList<U> Setup(int layoutPosition)
        {
            LayoutManager = new LinearLayoutManager(Activity.Activity, layoutPosition, false);
            RecyclerView = Activity.GetView().FindViewById<RecyclerView>(Id);
            RecyclerView.SetLayoutManager(LayoutManager);
            RvAdapter = new BaseRv<U>(RvDataset, RecyclerView, Activity.Context);
            RvDataset.Adapter = RvAdapter;
            RecyclerView.SetAdapter(RvAdapter);
            SetOnClick();
            return RvDataset;
        }


        public void SetOnClick()
        {
            RecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (RecyclerView != null && RecyclerView.ChildCount != 0)
                {
                    MainActivity.Fragment.TranslationX = 0;
                    MainActivity.CurrentFragment = new AlbumFragment();
                    //Current_state.SetAlbum(Dataset[position]);
                    MainActivity.mSupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.song_options, MainActivity.CurrentFragment)
                        .Commit();
                }
            });
        }
    }
}