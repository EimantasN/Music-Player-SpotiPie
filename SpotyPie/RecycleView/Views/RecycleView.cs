using Android.Support.V4.View;
using Android.Support.V7.Widget;
using SpotyPie.Base;
using SpotyPie.RecycleView.Enums;

namespace SpotyPie.RecycleView.Views
{
    public class SpotyPieRecycleView
    {
        private int _rvLayoutId { get; set; }

        private RecyclerView _rv { get; set; }

        private LayoutManagers Manager { get; set; } = LayoutManagers.Unseted;

        private FragmentBase _activity { get; set; }

        public SpotyPieRecycleView(FragmentBase activity, int layoutid)
        {
            this._activity = activity;
            this._rvLayoutId = layoutid;
        }

        public SpotyPieRecycleView Setup(LayoutManagers layout)
        {
            _rv = _activity.GetView().FindViewById<RecyclerView>(_rvLayoutId);
            SetLayoutManager(layout);
            return this;
        }

        public SpotyPieRecycleView SetAdapter(RecyclerView.Adapter adapter)
        {
            _rv?.SetAdapter(adapter);
            return this;
        }

        public RecyclerView.Adapter SetAdapter()
        {
            return _rv?.GetAdapter();
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
                            _rv.SetLayoutManager(new LinearLayoutManager(this._activity.Activity, LinearLayoutManager.Vertical, false));
                        }
                        break;
                    }
                case LayoutManagers.Linear_horizontal:
                    {
                        if (Manager == LayoutManagers.Unseted || Manager != LayoutManagers.Linear_horizontal)
                        {
                            Manager = LayoutManagers.Linear_horizontal;
                            _rv.SetLayoutManager(new LinearLayoutManager(this._activity.Activity, LinearLayoutManager.Horizontal, false));
                        }
                        break;
                    }
                case LayoutManagers.Grind_2_col:
                    {
                        if (Manager == LayoutManagers.Unseted || Manager != LayoutManagers.Grind_2_col)
                        {
                            Manager = LayoutManagers.Grind_2_col;
                            _rv.SetLayoutManager(new GridLayoutManager(this._activity.Activity, 2));
                        }
                        break;
                    }
            }
        }

        public SpotyPieRecycleView DisableScroolNested()
        {
            _rv.NestedScrollingEnabled = false;
            ViewCompat.SetNestedScrollingEnabled(_rv, false);
            return this;
        }

    }
}