using Android.Support.V7.Widget;
using SpotyPie.Base;
using SpotyPie.Helpers;

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