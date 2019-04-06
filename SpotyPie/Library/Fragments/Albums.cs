using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.RecycleView;
using Square.Picasso;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie.Library.Fragments
{
    public class Albums : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.library_album_layout;

        private RvList<dynamic> RvData { get; set; }

        protected override void InitView()
        {
            if (RvData == null)
            {
                var rvBase = new BaseRecycleView<dynamic>(this, Resource.Id.albums);
                RvData = rvBase.Setup(LinearLayoutManager.Vertical);
                rvBase.DisableScroolNested();
            }
            Task.Run(() => LoadAllAlbums());
        }

        public async Task LoadAllAlbums()
        {
            List<dynamic> data = new List<dynamic>() { null };
            RvData.AddList(data);
            var api = (AlbumService)GetService(ApiServices.Albums);

            data.AddRange(await api.GetAll());

            for (int i = 0; i < data.Count; i++)
                data[i].Type = RvType.AlbumBigOne;

            RvData.AddList(data);
            RvData.RemoveLoading(data);
        }

        public override void ForceUpdate()
        {
            Task.Run(() => LoadAllAlbums());
        }
    }

    public class AlbumRV : RecyclerView.Adapter, IFastScrollRecyclerViewAdapter
    {
        private RvList<Album> Dataset;
        Dictionary<string, int> MapIndex;
        private Context Context;

        public AlbumRV(RvList<Album> data, Context context)
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
                //view.SubTitile.Text = GetState().Current_Artist.Name;
                Picasso.With(Context).Load(Dataset[position].LargeImage).Resize(1200, 1200).CenterCrop().Into(view.Image);
            }
        }

        public Dictionary<string, int> GetMapIndex(RvList<Album> data)
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