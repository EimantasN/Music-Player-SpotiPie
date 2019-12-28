using Android.Content;
using SpotyPie.Enums;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SpotyPie.Base
{
    public interface IAutoFragmentManagement
    {
        SupportFragmentManager SupportFragmentManager { get; }

        Context Context { get; }

        int GetFragmentViewId();

        void RemoveCurrentFragment(SupportFragmentManager supportFragmentManager, FragmentBase fragmentBase);

        FragmentBase LoadFragment(FragmentEnum switcher);

        void SetScreen(LayoutScreenState screen);
    }
}