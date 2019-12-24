using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using RestSharp;
using SpotyPie.Base;
using SpotyPie.Database.Helpers;
using SpotyPie.Music.Manager;
using Square.Picasso;

namespace SpotyPie.Player
{
    public class ImageAdapter : PagerAdapter
    {
        private object _locker { get; set; } = new object();

        private Context _context;

        private ActivityBase _activity;

        private bool _locked { get; set; }

        public override int Count 
        {
            get
            {
                return SongManager.SongQueue.Count;
            }
        }

        public ImageAdapter(Context context, ActivityBase activity)
        {
            _context = context;
            _activity = activity;
            SongManager.SongListHandler += OnSongListChange;
        }

        private void OnSongListChange(List<Songs> songs)
        {
            NotifyDataSetChanged();
        }

        public int GetCurrentItem()
        {
            return SongManager.Index;
        }

        public Songs GetRecentItem()
        {
            return SongManager.Song;
        }

        public Songs GetCurrentSong(int index) => SongManager.TryGetSongByIndex(index);

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == ((ImageView)@object);
        }

        public override Java.Lang.Object InstantiateItem(View container, int position)
        {
            ImageView image = new ImageView(_context);
            image.SetScaleType(ImageView.ScaleType.CenterCrop);
            image.SetImageResource(Resource.Drawable.img_loading);
            ((ViewPager)container).AddView(image, 0);
            Task.Run(() => LoadImage(image, position));
            //Toast.MakeText(this.Context, $"Loaded -> {position}", ToastLength.Short).Show();
            return image;
        }

        public override void DestroyItem(View container, int position, Java.Lang.Object @object)
        {
            ((ViewPager)container).RemoveView((ImageView)@object);
        }

        public async Task LoadImage(ImageView image, int position)
        {
            var loadSongIamge = false;
            lock (_locker)
            {
                if (!_locked)
                {
                    loadSongIamge = true;
                    _locked = true;
                }
            }
            if (loadSongIamge)
            {
                if (position > Count - 1 && Count != 0)
                {
                    //Load Song
                    _activity?.RunOnUiThread(() =>
                    {
                        Toast.MakeText(this._context, "Load Song", ToastLength.Long).Show();
                    });
                }
                else
                {
                    await LoadCustomImage(SongManager.SongQueue[position], image);
                }
                _locked = false;
            }
        }

        private async Task LoadCustomImage(Songs song, ImageView image)
        {

            if (SettingHelper.IsCustomImageLoadingOn())
            {
                LoadOld();
            }
            else
            {
                List<Image> imageList = await _activity.GetAPIService().GetNewImageForSongAsync(song.Id);
                if (imageList == null || imageList.Count == 0)
                    LoadOld();
                else
                {
                    var img = imageList.OrderByDescending(x => x.Width).ThenByDescending(x => x.Height).First();
                    _activity.RunOnUiThread(() =>
                    {
                        Picasso
                        .With(_context)
                        .Load(img.Url)
                        .Resize(1200, 1200)
                        .CenterCrop()
                        .Into(image);
                    });
                }
            }

            void LoadOld()
            {
                _activity.RunOnUiThread(() => { 
                    Picasso
                    .With(_context)
                    .Load(song.LargeImage)
                    .Resize(1200, 1200)
                    .CenterCrop()
                    .Into(image); 
                });
            }
        }

        public void GetLargeImage(ImageView imagenew, int position)
        {

            RestClient client = new RestClient("https://source.unsplash.com/random");
            RestRequest request = new RestRequest(Method.GET);
            byte[] image = client.DownloadData(request);
            if (image.Length > 1000)
            {
                Bitmap BitMap = BitmapFactory.DecodeByteArray(image, 0, image.Length);
                Application.SynchronizationContext.Post(_ =>
                {
                    if (imagenew != null)
                    {
                        imagenew.SetImageBitmap(BitMap);
                        //BitMap.Dispose();
                    }
                }, null);
            }
        }
    }
}