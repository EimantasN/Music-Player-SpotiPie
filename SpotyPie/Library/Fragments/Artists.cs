using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Helpers;
using SpotyPie.RecycleView;
using Square.Picasso;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Library.Fragments
{
    public class Artists : SupportFragment
    {
        View RootView;

        public List<Artist> ArtistsLocal = new List<Artist>();
        public RvList<Artist> ArtistsData = new RvList<Artist>();
        private RecyclerView.LayoutManager ArtistsSongsLayoutManager;
        private RecyclerView.Adapter ArtistsSongsAdapter;
        private RecyclerView ArtistsSongsRecyclerView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.library_artist_layout, container, false);

            ArtistsSongsLayoutManager = new LinearLayoutManager(this.Activity);
            ArtistsSongsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.artists);
            ArtistsSongsRecyclerView.SetLayoutManager(ArtistsSongsLayoutManager);
            ArtistsSongsAdapter = new ArtistRV(ArtistsData, this.Context);
            ArtistsData.Adapter = ArtistsSongsAdapter;
            ArtistsSongsRecyclerView.SetAdapter(ArtistsSongsAdapter);
            ArtistsSongsRecyclerView.NestedScrollingEnabled = false;

            ArtistsSongsRecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (ArtistsSongsRecyclerView != null && ArtistsSongsRecyclerView.ChildCount != 0)
                {
                    Current_state.SetArtist(ArtistsLocal[position]);
                    FragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, MainActivity.Artist)
                    .Commit();
                }
            });

            return RootView;
        }

        public override void OnResume()
        {
            base.OnResume();
            Task.Run(() => LoadAlbumsAsync());
        }

        public override void OnStop()
        {
            base.OnStop();
        }

        public async Task LoadAlbumsAsync()
        {
            try
            {
                ArtistsData.Clear();
                ArtistsData.Add(null);
                var client = new RestClient("http://pie.pertrauktiestaskas.lt/api/Artist/Artists");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                IRestResponse response = await client.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {
                    var artists = JsonConvert.DeserializeObject<List<Artist>>(response.Content);
                    if (artists != null && artists.Count > 0)
                    {
                        if (artists.Count != ArtistsLocal.Count)
                        {
                            artists = artists.OrderByDescending(x => x.Popularity).ToList();
                            Application.SynchronizationContext.Post(_ =>
                            {
                                ArtistsLocal = artists;
                            }, null);

                            foreach (var x in artists.OrderByDescending(x => x.Popularity))
                            {
                                ArtistsData.Add(x);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                //ArtistsData.RemoveLoading();
            }
        }
    }

    public class ArtistRV : RecyclerView.Adapter
    {
        private RvList<Artist> Dataset;
        private Context Context;

        public ArtistRV(RvList<Artist> data, Context context)
        {
            Dataset = data;
            Context = context;
        }

        public class Loading : RecyclerView.ViewHolder
        {
            public View LoadingView { get; set; }

            public Loading(View view) : base(view)
            { }
        }

        public class BlockImage : RecyclerView.ViewHolder
        {
            public View EmptyTimeView { get; set; }

            public TextView Title { get; set; }

            public TextView SubTitile { get; set; }

            public ImageView Image { get; set; }

            public BlockImage(View view) : base(view) { }
        }

        public override int GetItemViewType(int position)
        {
            if (Dataset[position] == null)
            {
                return Resource.Layout.Loading;
            }
            else
            {
                return Resource.Layout.song_list_rv;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == Resource.Layout.Loading)
            {
                View Loading = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Loading, parent, false);

                Loading view = new Loading(Loading) { };

                return view;
            }
            else
            {
                View BoxView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.big_rv_list_one, parent, false);
                TextView mTitle = BoxView.FindViewById<TextView>(Resource.Id.textView10);
                TextView mSubTitle = BoxView.FindViewById<TextView>(Resource.Id.textView11);
                ImageView mImage = BoxView.FindViewById<ImageView>(Resource.Id.imageView5);

                BlockImage view = new BlockImage(BoxView)
                {
                    Image = mImage,
                    SubTitile = mSubTitle,
                    Title = mTitle,
                    IsRecyclable = false
                };

                return view;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is Loading)
            {
                ;
            }
            else if (holder is BlockImage)
            {
                BlockImage view = holder as BlockImage;
                view.Title.Text = Dataset[position].Name;
                if (Dataset[position].Genres != null)
                {
                    var GenresData = JsonConvert.DeserializeObject<List<string>>(Dataset[position].Genres);
                    if (GenresData != null && GenresData.Count != 0)
                        view.SubTitile.Text = GenresData.First();
                }
                if (Dataset[position].Images != null && Dataset[position].Images.Count != 0)
                    Picasso.With(Context).Load(Dataset[position].Images.First().Url).Resize(1200, 1200).CenterCrop().Into(view.Image);
                else
                    view.Image.SetImageResource(Resource.Drawable.noimg);
            }
        }

        public override int ItemCount
        {
            get { return Dataset.Count; }
        }
    }
}