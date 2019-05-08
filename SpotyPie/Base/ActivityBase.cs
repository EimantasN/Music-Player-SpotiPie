using Android.OS;
using Android.Support.V7.App;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SpotyPie.Base
{
    public abstract class ActivityBase : AppCompatActivity
    {
        protected virtual int FirstLayerFragmentHolder { get; set; } = Resource.Id.content_frame;

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

        public void LoadFragmentInner(dynamic switcher)
        {
            if (CurrentFragment != null)
            {
                CurrentFragment.Hide();
            }

            CurrentFragment = null;

            LoadFragment(switcher);

            if (CurrentFragment == null)
            {
                throw new System.Exception("Fragment not founded");
            }

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