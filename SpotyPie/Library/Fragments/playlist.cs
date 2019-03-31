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
using Square.Picasso;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Library.Fragments
{
    public class PlaylistFragment : SupportFragment
    {
        View RootView;

        public List<Playlist> PlaylistLocal;
        public RvList<Playlist> PlaylistsData;
        private RecyclerView.LayoutManager PlaylistSongsLayoutManager;
        private RecyclerView.Adapter PlaylistSongsAdapter;
        private RecyclerView PlaylistsSongsRecyclerView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.library_playlist_layout, container, false);

            PlaylistLocal = new List<Playlist>();
            PlaylistsData = new RvList<Playlist>();
            PlaylistSongsLayoutManager = new LinearLayoutManager(this.Activity);
            PlaylistsSongsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.playlist);
            PlaylistsSongsRecyclerView.SetLayoutManager(PlaylistSongsLayoutManager);
            PlaylistSongsAdapter = new PlaylistRV(PlaylistsData, this.Context);
            PlaylistsData.Adapter = PlaylistSongsAdapter;
            PlaylistsSongsRecyclerView.SetAdapter(PlaylistSongsAdapter);
            PlaylistsSongsRecyclerView.NestedScrollingEnabled = false;

            PlaylistsSongsRecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (PlaylistsSongsRecyclerView != null && PlaylistsSongsRecyclerView.ChildCount != 0)
                {
                    //Current_state.SetArtist(PlaylistLocal[position]);
                    //FragmentManager.BeginTransaction()
                    //.Replace(Resource.Id.content_frame, MainActivity.Artist)
                    //.Commit();
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
                await PlaylistsData.ClearAsync();
                PlaylistsData.Add(null);

                var client = new RestClient("http://pie.pertrauktiestaskas.lt/api/Playlist/playlists");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                IRestResponse response = await client.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {
                    var playlists = JsonConvert.DeserializeObject<List<Playlist>>(response.Content);
                    if (playlists != null && playlists.Count > 0)
                    {
                        if (playlists.Count != PlaylistLocal.Count)
                        {
                            playlists = playlists.OrderByDescending(x => x.Popularity).ToList();
                            Application.SynchronizationContext.Post(_ =>
                            {
                                PlaylistLocal = playlists;
                            }, null);

                            foreach (var x in playlists.OrderByDescending(x => x.Popularity))
                            {
                                PlaylistsData.Add(x);
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
                //PlaylistsData.RemoveLoading();
            }
        }

        public class PlaylistRV : RecyclerView.Adapter
        {
            private RvList<Playlist> Dataset;
            private Context Context;

            public PlaylistRV(RvList<Playlist> data, Context context)
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
                    view.SubTitile.Text = "Atnaujinta " + Dataset[position].Created;
                    if (!string.IsNullOrEmpty(Dataset[position].ImageUrl))
                        Picasso.With(Context).Load(Dataset[position].ImageUrl).Resize(1200, 1200).CenterCrop().Into(view.Image);
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
}