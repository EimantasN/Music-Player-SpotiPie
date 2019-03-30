using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Helpers;
using SpotyPie.Models;
using Square.Picasso;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Library.Fragments
{
    public class Albums : SupportFragment
    {
        View RootView;

        //Album Songs
        public List<Album> AlbumsLocal = new List<Album>();
        public RecycleViewList<Album> AlbumsData = new RecycleViewList<Album>();
        private RecyclerView.LayoutManager AlbumSongsLayoutManager;
        private RecyclerView.Adapter AlbumSongsAdapter;
        private RecyclerView AlbumSongsRecyclerView;
        private FastScrollRecyclerViewItemDecoration decoration;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.library_album_layout, container, false);

            //ALBUM song list
            AlbumsData = new RecycleViewList<Album>();
            AlbumSongsLayoutManager = new LinearLayoutManager(this.Activity);
            AlbumSongsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.albums);
            AlbumSongsRecyclerView.HasFixedSize = true;
            AlbumSongsRecyclerView.SetLayoutManager(AlbumSongsLayoutManager);
            AlbumSongsAdapter = new AlbumRV(AlbumsData, this.Context);
            AlbumsData.Adapter = AlbumSongsAdapter;
            AlbumSongsRecyclerView.SetAdapter(AlbumSongsAdapter);
            decoration = new FastScrollRecyclerViewItemDecoration(this.Context);

            AlbumSongsRecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (AlbumSongsRecyclerView != null && AlbumSongsRecyclerView.ChildCount != 0 && AlbumsData != null && AlbumsData.Count != 0 && AlbumsData.Count > position)
                {
                    if (AlbumsLocal.Count == AlbumsData.Count)
                    {
                        Current_state.SetAlbum(AlbumsLocal[position]);
                        FragmentManager.BeginTransaction()
                        .Replace(Resource.Id.content_frame, MainActivity.Album)
                        .Commit();
                    }
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
            //AlbumsData.Clear();
        }

        public async Task LoadAlbumsAsync()
        {
            try
            {
                await AlbumsData.ClearAsync();
                AlbumsData.Add(null);

                var client = new RestClient("http://pie.pertrauktiestaskas.lt/api/album/Albums");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                IRestResponse response = await client.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {
                    var albums = JsonConvert.DeserializeObject<List<Album>>(response.Content);
                    if (albums != null && albums.Count > 0)
                    {
                        if (albums.Count != AlbumsData.Count)
                        {

                            albums = albums.OrderByDescending(x => x.Name).ToList();
                            Application.SynchronizationContext.Post(_ =>
                            {
                                AlbumsLocal = albums;
                            }, null);
                            foreach (var x in albums)
                            {
                                AlbumsData.Add(x);
                                //if (AlbumsData.Count == 20)
                                //AlbumsData.RemoveLoading();
                            }
                            while (AlbumsData.Count != albums.Count)
                                await Task.Delay(50);
                            Application.SynchronizationContext.Post(_ =>
                            {
                                //AlbumSongsRecyclerView.AddItemDecoration(decoration);
                                //AlbumSongsRecyclerView.SetItemAnimator(new DefaultItemAnimator());
                            }, null);
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                //AlbumsData.RemoveLoading();
            }
        }
    }

    public class AlbumRV : RecyclerView.Adapter, IFastScrollRecyclerViewAdapter
    {
        private RecycleViewList<Album> Dataset;
        Dictionary<string, int> MapIndex;
        private Context Context;

        public AlbumRV(RecycleViewList<Album> data, Context context)
        {
            Dataset = data;
            MapIndex = GetMapIndex(data);
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
                view.SubTitile.Text = JsonConvert.DeserializeObject<List<Artist>>(Dataset[position].Artists).First().Name;
                if (Dataset[position].Images != null && Dataset[position].Images.Count != 0)
                    Picasso.With(Context).Load(Dataset[position].Images.First().Url).Resize(1200, 1200).CenterCrop().Into(view.Image);
                else
                    view.Image.SetImageResource(Resource.Drawable.noimg);
            }
        }

        public Dictionary<string, int> GetMapIndex(RecycleViewList<Album> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                string name = data[i].Name;
                string index = name.Substring(0, 1);
                index = index.ToUpper();

                if (!MapIndex.ContainsKey(index))
                {
                    MapIndex.Add(index, i);
                }
            }
            return MapIndex;
        }

        public Dictionary<string, int> GetMapIndex()
        {
            return this.MapIndex;
        }

        public override int ItemCount
        {
            get { return Dataset.Count; }
        }
    }
}