using Mobile_Api.Models;
using Newtonsoft.Json;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotyPie.SongBinder.Fragments
{
    public class SongBindList : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.song_bind_list;

        protected override Enums.LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Default;

        private BaseRecycleView<SongTag> Songs { get; set; }

        protected override void InitView()
        {
        }

        public override void ForceUpdate()
        {
            if (Songs == null)
            {
                Songs = new BaseRecycleView<SongTag>(this, Resource.Id.song_list);
                Songs.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical,
                    new List<Action>()
                    {
                        () => GetActivity().LoadFragmentInner(FragmentEnum.SongDetailsFragment, JsonConvert.SerializeObject(Songs.GetData().GetList()[Songs.LastPosition]))
                    });
            }
            Task.Run(() => LoadUnbindedSongsAsync());
        }

        private async Task LoadUnbindedSongsAsync()
        {
            try
            {
                List<SongTag> unbindedSongs = await ParentActivity.GetAPIService().GetUnbindedSongList();
                if (unbindedSongs != null && unbindedSongs.Count != 0)
                {
                    Songs?.GetData()?.AddList(unbindedSongs);
                }
            }
            catch (Exception e)
            {

            }
        }

        public override void ReleaseData()
        {
            if (Songs != null)
            {
                Songs.Dispose();
                Songs = null;
            }
        }

        public override int GetParentView()
        {
            throw new NotImplementedException();
        }

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            return null;
        }
    }
}