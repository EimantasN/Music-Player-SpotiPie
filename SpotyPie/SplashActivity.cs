using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;

namespace SpotyPie
{
    [Activity(Label = "SplashActivity", MainLauncher = true)]
    public class SplashActivity : Activity
    {
        ProgressBar loading;
        Button skip;
        TextView Quote;
        ImageView image;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.splash);

            loading = FindViewById<ProgressBar>(Resource.Id.loading);
            skip = FindViewById<Button>(Resource.Id.skip);
            skip.Click += Skip_Click;
            skip.Visibility = Android.Views.ViewStates.Gone;
            Quote = FindViewById<TextView>(Resource.Id.quote);
            image = FindViewById<ImageView>(Resource.Id.imageView3);

            Task.Run(() => LoadQuote());
        }

        private void Skip_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }


        public async Task LoadQuote()
        {
            try
            {
                int count = 0;
                while (count <= 2)
                {
                    count++;
                    var random = new Random();
                    int value = random.Next(2, 28);
                    RestClient client = new RestClient("http://pie.pertrauktiestaskas.lt/api/sync/GetQuote/" + value);
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("cache-control", "no-cache");
                    IRestResponse response = await client.ExecuteTaskAsync(request);
                    Quote quote = JsonConvert.DeserializeObject<Quote>(response.Content);
                    Application.SynchronizationContext.Post(_ =>
                    {
                        Quote.Text = quote.Text;
                    }, null);

                    Application.SynchronizationContext.Post(_ =>
                    {
                        skip.Visibility = Android.Views.ViewStates.Visible;
                    }, null);

                    await Task.Delay(500);
                }

                Application.SynchronizationContext.Post(_ =>
                {
                    var intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                }, null);
            }
            catch
            {
                //IGNORE

                Application.SynchronizationContext.Post(_ =>
                {
                    Quote.Text = "Now I'm dead inside!";
                    var intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                }, null);
            }
        }
    }
}