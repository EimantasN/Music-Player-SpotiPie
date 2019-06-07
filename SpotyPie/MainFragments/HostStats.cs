using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Formatter;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Interfaces.Datasets;
using MikePhil.Charting.Listener;
using SpotyPie.Base;
using SpotyPie.Enums;

namespace SpotyPie.MainFragments
{
    public class HostStats : FragmentBase, IOnChartValueSelectedListenerSupport
    {
        public override int LayoutId { get; set; } = Resource.Layout.performance_layout;

        protected override LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Holder;

        LineChart Chart;

        public override void ForceUpdate()
        {

        }

        public override int GetParentView()
        {
            return Resource.Id.parent_view;
        }

        public override void LoadFragment(dynamic switcher)
        {
        }

        public override void ReleaseData()
        {
        }

        protected override void InitView()
        {
            Chart = RootView.FindViewById<LineChart>(Resource.Id.chart1);
            // disable description text
            Chart.Description.Enabled = false;

            // enable touch gestures
            Chart.SetTouchEnabled(true);

            Chart.SetOnChartValueSelectedListener(this);
            Chart.SetDrawGridBackground(false);

            // create marker to display box when values are selected
            //MarkerView mv = new MyMarkerView(this, Resource .layout.custom_marker_view);

            // Set the marker to the chart
            //mv.setChartView(chart);
            //Chart.setMarker(mv);

            // enable scaling and dragging
            Chart.DragEnabled = true;
            Chart.ScaleXEnabled = true;
            // chart.setScaleXEnabled(true);
            // chart.setScaleYEnabled(true);

            // force pinch zoom along both axis
            Chart.SetPinchZoom(true);

            SetData(45, 180);

        }

        private void SetData(int count, float range)
        {

            List<Entry> values = new List<Entry>();
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {

                float val = (float)(random.NextDouble() * range) - 30;
                values.Add(new Entry(i, val));
            }

            LineDataSet set1;

            if (Chart.Data != null)
            {
                set1 = (LineDataSet)Chart.Data.GetDataSetByIndex(0);
                set1.Values = values;
                set1.NotifyDataSetChanged();
                Chart.Data.NotifyDataChanged();
                Chart.NotifyDataSetChanged();
            }
            else
            {
                // create a dataset and give it a type
                set1 = new LineDataSet(values, "DataSet 1");

                set1.SetDrawIcons(false);

                // draw dashed line
                set1.EnableDashedLine(10f, 5f, 0f);

                // black lines and points
                set1.SetColor(Resource.Color.black, 1);
                set1.SetCircleColor(Resource.Color.black);

                // line thickness and point size
                set1.LineWidth = 1f;
                set1.CircleRadius = 3f;

                // draw points as solid circles
                set1.SetDrawCircleHole(false);

                // customize legend entry
                set1.FormLineWidth = 1f;
                set1.FormLineDashEffect = new DashPathEffect(new float[] { 10f, 5f }, 0f);
                set1.FormSize = 15.0f;

                // text size of values
                set1.ValueTextSize = 9f;

                // draw selection line as dashed
                set1.EnableDashedHighlightLine(10f, 5f, 0f);

            //    // set the filled area
                set1.SetDrawFilled(true);
                //set1.SetFillFormatter(new IFillFormatter() {
            //    @Override
            //    public float getFillLinePosition(ILineDataSet dataSet, LineDataProvider dataProvider)
            //    {
            //        return chart.getAxisLeft().getAxisMinimum();
            //    }
            }

            //// set color of filled area
            //if (Utils.getSDKInt() >= 18)
            //{
            //    // drawables only supported on api level 18 and above
            //    Drawable drawable = ContextCompat.getDrawable(this, R.drawable.fade_red);
            //    set1.setFillDrawable(drawable);
            //}
            //else
            //{
            //    set1.FillColor(Color.BLACK);
            //}

            List<ILineDataSet> dataSets = new List<ILineDataSet>();
            dataSets.Add(set1); // add the data sets

            // create a data object with the data sets
            LineData data = new LineData(dataSets);

            // set data
            Chart.Data = data;
        }


        public void OnNothingSelected()
        {
            Toast.MakeText(Context, "Nothing selected", ToastLength.Short).Show();
        }

        public void OnValueSelected(Entry e, Highlight h)
        {
            Toast.MakeText(Context, "Selected", ToastLength.Short).Show();
        }
    }
}