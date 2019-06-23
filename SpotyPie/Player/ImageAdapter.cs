using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Mobile_Api.Models;
using Realms;
using RestSharp;
using SpotyPie.Base;
using SpotyPie.Database.Helpers;
using Square.Picasso;

namespace SpotyPie.Player
{
    public class ImageAdapter : PagerAdapter
    {
        private object Locker { get; set; } = new object();
        private List<Songs> Songs;
        private Context Context;
        private ActivityBase Activity;

        public override int Count => int.MaxValue;

        public ImageAdapter(Context _context, ActivityBase activity)
        {
            Context = _context;
            Activity = activity;
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == ((ImageView)@object);
        }

        public override Java.Lang.Object InstantiateItem(View container, int position)
        {
            ImageView image = new ImageView(Context);
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

        public void LoadImage(ImageView image, int position)
        {
            lock (Locker)
            {
                if (Songs == null)
                {
                    using (var realm = Realm.GetInstance())
                    {
                        var songs = realm.All<Mobile_Api.Models.Realm.Music>().ToList().Select(x => new Songs(x.Song)).ToList();

                        Activity.RunOnUiThread(() =>
                        {
                            Songs = songs;
                        });
                    }
                    while (Songs == null)
                        Thread.Sleep(50);
                }
                int Count = Songs.Count;
                if (position > Songs.Count - 1)
                {
                    Realm_Songs nextSong = Activity.GetAPIService().GetNextSongAsync().Result;
                    var song = new Songs(nextSong);
                    Activity.RunOnUiThread(() =>
                    {
                        Songs.Add(song);
                    });
                    using (Realm innerRealm = Realm.GetInstance())
                    {
                        innerRealm.Write(() =>
                        {
                            innerRealm.Add(new Mobile_Api.Models.Realm.Music(nextSong, false));
                        });
                    }
                    LoadCustomImage(song, image);

                    while (Count == Songs.Count)
                        Thread.Sleep(125);
                }
                else
                {
                    LoadCustomImage(Songs[position], image);
                }
            }
        }

        private void LoadCustomImage(Songs song, ImageView image)
        {

            if (SettingHelper.IsCustomImageLoadingOn())
            {
                LoadOld();
            }
            else
            {
                List<Image> imageList = Activity.GetAPIService().GetNewImageForSongAsync(song.Id).Result;
                if (imageList == null || imageList.Count == 0)
                    LoadOld();
                else
                {
                    var img = imageList.OrderByDescending(x => x.Width).ThenByDescending(x => x.Height).First();
                    Activity.RunOnUiThread(() =>
                    {
                        Picasso.With(this.Context).Load(img.Url).Resize(1200, 1200).CenterCrop().Into((ImageView)image);
                    });
                }
            }

            void LoadOld()
            {
                Activity.RunOnUiThread(() => { Picasso.With(this.Context).Load(song.LargeImage).Resize(1200, 1200).CenterCrop().Into(image); });
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