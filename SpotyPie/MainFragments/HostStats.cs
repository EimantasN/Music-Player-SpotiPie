using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Widget;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Data;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Interfaces.Datasets;
using MikePhil.Charting.Util;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.Monitoring;

namespace SpotyPie.MainFragments
{
    public class HostStats : FragmentBase
    {
        private bool IsAlive = false;

        public override int LayoutId { get; set; } = Resource.Layout.performance_layout;

        protected override LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Holder;

        private SystemInfo Info;

        private LineChart Chart;

        private PieChart RAMPieChart;

        private TextView RamValue;

        private List<Entry> CpuValues;

        private TextView TempValue;

        public void SubscribeForSystemInfo(SystemInfo m)
        {
            m.Handler += SendSystemInfoData;
        }

        private void UnSubscribeFromSystemInfo()
        {
            Info?.Stop();
            Info.Handler -= SendSystemInfoData;
        }

        private void SendSystemInfoData(dynamic m, EventArgs e)
        {
            UpdateTemperatureValue(m.cT.ToString());
            RunOnUiThread(() =>
            {
                UpdateCPUData((float)m.cU);
            });

            RunOnUiThread(() =>
            {
                UpdateRamData((float)m.rU);
            });
        }

        protected override void InitView()
        {
            Info = new SystemInfo(GetAPIService());
            SubscribeForSystemInfo(Info);

            TempValue = RootView.FindViewById<TextView>(Resource.Id.temp_value);
            RamValue = RootView.FindViewById<TextView>(Resource.Id.ram_value);
            RamValue.Text = "0.0";
            SetupCpuChart();
            SetupRamUsage();
        }

        public override void ForceUpdate()
        {
            Task.Run(async () =>
            {
                var systemInfo = await GetAPIService().GetSystemInfo();
            });
        }

        public override int GetParentView()
        {
            return Resource.Id.parent_view;
        }

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            return null;
        }

        public override void ReleaseData()
        {
            IsAlive = false;
        }

        private void SetupRamUsage()
        {
            RAMPieChart = RootView.FindViewById<PieChart>(Resource.Id.piechart);

            RAMPieChart.Description.Enabled = false;
            RAMPieChart.Legend.Enabled = false;

            float x = 85;
            RAMPieChart.DrawHoleEnabled = true;
            RAMPieChart.SetHoleColor(ColorTemplate.ColorNone);
            RAMPieChart.HoleRadius = x;

            RAMPieChart.SetUsePercentValues(true);

            //RAMPieChart.MaxAngle = 180f; // HALF CHART
            //RAMPieChart.RotationAngle = 180f;

            UpdateRamData(0);
        }

        private PieDataSet RAMDataSet;

        private void UpdateRamData(float value)
        {
            try
            {
                if (RAMPieChart == null)
                    SetupRamUsage();

                if (RAMDataSet == null || RAMDataSet.EntryCount == 0)
                {
                    IList<PieEntry> NoOfEmp = new List<PieEntry>();
                    NoOfEmp.Add(new PieEntry(0f, 0));
                    NoOfEmp.Add(new PieEntry(100f, 1));
                    RAMDataSet = new PieDataSet(NoOfEmp, "RAM USAGE");

                    IList<Java.Lang.Integer> colors = new List<Java.Lang.Integer>()
                {
                    (Java.Lang.Integer)ColorTemplate.Rgb("#EE9911"),
                    (Java.Lang.Integer)ColorTemplate.Rgb("#373B40")
                };

                    RAMDataSet.Colors = colors;
                    RAMDataSet.SetDrawValues(false);

                    PieData data = new PieData(RAMDataSet);
                    RAMPieChart.Data = data;

                    Highlight h = new Highlight(0f, 0, 0); // dataset index for piechart is always 0
                    RAMPieChart.HighlightValues(new Highlight[] { h });

                    RAMPieChart.AnimateXY(1000, 1000);
                }
                else
                {
                    RAMDataSet.Values = new List<PieEntry>()
                {
                    new PieEntry(value, 0),
                    new PieEntry(100 - value, 1)
                };
                    RAMDataSet?.NotifyDataSetChanged();
                    RAMPieChart?.Data?.NotifyDataChanged();
                    RAMPieChart?.NotifyDataSetChanged();
                    RAMPieChart?.Invalidate();
                    RamValue.Text = $"{value}.0";
                }
            }
            catch //Ignored
            {

            }
        }

        private void UpdateTemperatureValue(string value)
        {
            RunOnUiThread(() =>
            {
                TempValue.Text = value.Replace("°C", "").Replace("+", "").Trim();
            });
        }

        private void SetupCpuChart()
        {
            Chart = RootView.FindViewById<LineChart>(Resource.Id.chart1);
            Chart.Description.Enabled = false;
            Chart.Legend.Enabled = false;

            Chart.SetTouchEnabled(false);
            Chart.ScaleXEnabled = false;
            Chart.SetPinchZoom(true);

            int white = ColorTemplate.Rgb("#FFFFFF");
            Chart.SetNoDataTextColor(white);
            Chart.AxisLeft.TextColor = white;
            Chart.XAxis.TextColor = white;
            Chart.AxisRight.TextColor = white;
            Chart.Legend.TextColor = white;

            Chart.SetMaxVisibleValueCount(100);

            //Chart.SetScaleMinima(100, 100);
            Chart.MoveViewToX(100);

            InitCpuDataset();

            UpdateCPUData(0.0f);
        }

        private LineDataSet set1;

        private void InitCpuDataset()
        {
            CpuValues = new List<Entry>();
            for (int i = 0; i < 100; i++)
            {
                CpuValues.Add(new Entry(i, 0));
            }

            set1 = new LineDataSet(CpuValues, "CPU");

            set1.SetDrawIcons(false);
            set1.SetDrawValues(false);
            set1.SetDrawFilled(false);
            set1.SetDrawCircleHole(false);
            set1.SetDrawCircles(false);

            // draw selection line as dashed
            //set1.EnableDashedHighlightLine(10f, 5f, 0f);

            List<ILineDataSet> dataSets = new List<ILineDataSet>();
            dataSets.Add(set1); // add the data sets

            // create a data object with the data sets
            LineData data = new LineData(dataSets);
            // set data
            Chart.Data = data;
            Chart.Invalidate();
            Chart.AnimateY(1000);
        }

        private void UpdateCPUData(float value)
        {
            var entry = new Entry(CpuValues.Count, value);
            if (CpuValues.Count > 100)
            {
                for (int i = 1; i < CpuValues.Count; i++)
                {
                    CpuValues[i - 1].SetY(CpuValues[i].GetY());
                }
                CpuValues[CpuValues.Count - 1].SetY(entry.GetY());
            }
            else
            {
                CpuValues.Add(entry);
            }

            if (Chart.Data != null && set1 != null)
            {
                set1.Values = CpuValues;
                set1.NotifyDataSetChanged();
                Chart.Data.NotifyDataChanged();
                Chart.NotifyDataSetChanged();
                Chart.Invalidate();
            }
            else
            {
                InitCpuDataset();
            }
        }
    }
}