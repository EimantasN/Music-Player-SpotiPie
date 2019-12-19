using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;

namespace SpotyPie
{
    [Activity(Label = "SplashActivity", MainLauncher = false)]
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
                    RestClient client = new RestClient($"{BaseClient.BaseUrl}api/sync/GetQuote/" + value);
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("cache-control", "no-cache");
                    IRestResponse response = await client.ExecuteTaskAsync(request);
                    Quote quote = JsonConvert.DeserializeObject<Quote>(response.Content);
                    RunOnUiThread(() =>
                    {
                        Quote.Text = quote.Text;
                    });

                    RunOnUiThread(() =>
                    {
                        skip.Visibility = Android.Views.ViewStates.Visible;
                    });

                    await Task.Delay(500);
                }

                RunOnUiThread(() =>
                {
                    var intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                });
            }
            catch
            {
                //IGNORE
                RunOnUiThread(() =>
                {
                    Quote.Text = "Now I'm dead inside!";
                    var intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                });
            }
        }
    }
}