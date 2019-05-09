using Android.OS;
using Android.Support.V7.App;
using System.Collections.Generic;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SpotyPie.Base
{
    public abstract class ActivityBase : AppCompatActivity
    {
        protected virtual int FirstLayerFragmentHolder { get; set; } = Resource.Id.content_frame;

        private Stack<dynamic> FragmentStack { get; set; }

        protected virtual FragmentBase CurrentFragment { get; set; }

        private API Api_service { get; set; }

        public SupportFragmentManager mSupportFragmentManager;

        public abstract dynamic GetInstance();

        protected abstract void InitFather();


        public API GetAPIService()
        {
            if (Api_service == null)
                return Api_service = new API(new Mobile_Api.Service(), this);
            return Api_service;
        }

        protected abstract void LoadFragment(dynamic switcher);

        public override void OnBackPressed()
        {
            if (CheckFragments())
                base.OnBackPressed();
            else
            {
                LoadFragmentInner(GetFragmentStack().Pop());
            }
        }

        private bool CheckFragments()
        {
            if (GetFragmentStack() == null || GetFragmentStack().Count <= 1)
                return true;

            dynamic fragmentState = GetFragmentStack().Pop();
            if (fragmentState == null)
                return true;
            return false;
        }

        private Stack<dynamic> GetFragmentStack()
        {
            if (FragmentStack == null)
                FragmentStack = new Stack<dynamic>();
            return FragmentStack;
        }

        public void LoadFragmentInner(dynamic switcher, string jsonModel = null)
        {
            if (CurrentFragment != null)
            {
                CurrentFragment.Hide();
            }

            CurrentFragment = null;

            GetFragmentStack().Push(switcher);
            LoadFragment(switcher);

            if (CurrentFragment == null)
            {
                throw new System.Exception("Fragment not founded");
            }

            //Can send data to fragment
            if (!string.IsNullOrEmpty(jsonModel))
                CurrentFragment.SendData(jsonModel);

            if (!CurrentFragment.IsAdded)
            {
                SupportFragmentManager.BeginTransaction()
                .Replace(FirstLayerFragmentHolder, CurrentFragment)
                .Commit();
            }
            else
            {
                CurrentFragment.Show();
            }
        }

        protected override void OnDestroy()
        {
            FragmentStack = null;
            base.OnDestroy();
        }

        public void RemoveCurrentFragment()
        {
            throw new System.Exception("not implemented");
            //if (FirstLayerFragment != null)
            //{
            //    FirstLayerFragment.ReleaseData();
            //    var transaction = mSupportFragmentManager.BeginTransaction();
            //    transaction.Remove(FirstLayerFragment);
            //    transaction.Commit();
            //    transaction.SetTransition(Android.Support.V4.App.FragmentTransaction.TransitFragmentClose);
            //    transaction = null;
            //    FirstLayerFragment = null;
            //    FirstLayer.TranslationX = widthInDp;
            //}
        }
    }
}