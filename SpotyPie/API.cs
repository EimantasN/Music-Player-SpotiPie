using Android.App;
using Mobile_Api.Interfaces;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class API
    {
        private Mobile_Api.Service _service;
        private MainActivity _activity;

        public API(Mobile_Api.Service service, MainActivity activity)
        {
            _service = service;
            _activity = activity;
        }

        #region Library

        public async Task GetAll<T>(RvList<T> RvList, Action action, RvType type) where T : IBaseInterface
        {
            try
            {
                List<T> AlbumsData = new List<T>() { default(T) };
                InvokeOnMainThread(() => RvList.AddList(AlbumsData));

                AlbumsData = await _service.GetAll<T>();

                AlbumsData.ForEach(x => x.SetModelType(type));

                InvokeOnMainThread(() =>
                {
                    RvList.AddList(AlbumsData);
                    if (action != null)
                    {
                        action.Invoke();
                    }
                });
            }
            catch (Exception e)
            {

            }
        }

        #endregion

        #region MainFragment

        public async Task GetRecentAlbumsAsync(RvList<Album> RvList, Action action)
        {
            try
            {
                List<Album> AlbumData = await _service.GetRecent<Album>();
                InvokeOnMainThread(() =>
                {
                    if (action != null)
                    {
                        action.Invoke();
                    }
                    RvList.AddList(AlbumData);
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetPolularAlbumsAsync(RvList<Album> RvList, Action action)
        {
            try
            {
                List<Album> AlbumData = await _service.GetPopular<Album>();
                InvokeOnMainThread(() =>
                {
                    if (action != null)
                    {
                        action.Invoke();
                    }
                    RvList.AddList(AlbumData);
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetOldAlbumsAsync(RvList<Album> RvList, Action action)
        {
            try
            {
                List<Album> AlbumData = await _service.GetOld<Album>();
                InvokeOnMainThread(() =>
                {
                    if (action != null)
                    {
                        action.Invoke();
                    }
                    RvList.AddList(AlbumData);
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion
        private void InvokeOnMainThread(Action action)
        {
            Application.SynchronizationContext.Post(_ =>
            {
                action.Invoke();
            }, null);
        }

    }
}