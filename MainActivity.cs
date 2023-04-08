using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Support.V4.App;
using Android;
using Android.Widget;
using Android.Bluetooth.LE;
using static AndroidX.RecyclerView.Widget.RecyclerView;
using System.Linq;
using Java.Util;
using Java.Nio;
using System.Threading.Tasks;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Axes;
using SciChart.Data.Model;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.PointMarkers;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Drawing.Common;
using System.Drawing;
using SciChart.Charting.Modifiers;
using SciChart.Charting3D.Modifiers;
using SciChart.Charting;
using Android.Bluetooth;
using Android.Content;
using static DopplerRadarAndroidAPp.BluetoothDeviceReceiver;
using System.Collections.Generic;

namespace DopplerRadarAndroidAPp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        [Obsolete]
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            // SciChart License Key
            SciChartSurface.SetRuntimeLicenseKey("SrDiaOt4OKLdVapiKRimqKbg/4T5ljr/pv4erw0WJa6YWIk0DIfv/kY/Yrhn87mcrHTWcSZz/8K/OMQwErwJiTwBtrKs2634dkIISi3Ti46yAg4KUPYrHDliaRvBWA0VzD8ScQSHrWa/GuDnkyzL1MQOuPb6dV6HJh3Q9zQtNtm7QIOyHQkUlevKXjEXiUo/G9yyNTE1c4FcdsaIP0DFMthVBzUnAuTIaogY6HvKLvPKDreLyw0wNE3CjDo/Hb8cF7DM/inkyrt/7NNjNe2qfLQsqUQlxLeNUWDWUZqSnNbil07g8qHprNjpCcDGlSQ8c0ySpfe+1LYSD2icWqC3158eUTKmAkAlXMp5EwCFGIwb0Q3ddu/HOaiHJAEe9Njxj/aNIvJKepk16XwZFci36pbFPUrR0kbNkBWLPC835G/X12ysfGWFjODS5rw31FUNeaO1frsKcLui2nVMJ+g4yp4qygGCQY09gKznDIA77aISdhFxddCZxm/SCweQ+8bmxmkvnsIxKvPobnJ7bNnPA81ZW8TTBc/XG/Fz2I5PNWTnEdELtt9n6IN0gt14J+LMtw3fdJfRp6r5o6etYBrE9ZEV6cT7");


            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            #region SciChart

            // Get our chart from the layout resource,
            var chart = FindViewById<SciChartSurface>(Resource.Id.Chart);

            // Create a numeric X axis
            var xAxis = new NumericAxis(this) { AxisTitle = "Pitch Number" };

            // Create a numeric Y axis
            var yAxis = new NumericAxis(this)
            {
                AxisTitle = "Pitch Velocity (mph)",
                VisibleRange = new DoubleRange(1, 150)
            };

            // Add xAxis to the XAxes collection of the chart
            chart.XAxes.Add(xAxis);

            // Add yAxis to the YAxes collection of the chart
            chart.YAxes.Add(yAxis);

            // Create interactivity modifiers
            var xAxisDragModifier = new XAxisDragModifier();
            xAxisDragModifier.SetReceiveHandledEvents(true);

            var zoomPanModifier = new ZoomPanModifier();
            zoomPanModifier.Direction = Direction2D.XDirection;
            zoomPanModifier.SetReceiveHandledEvents(true);

            var zoomExtentsModifier = new ZoomExtentsModifier();
            zoomExtentsModifier.SetReceiveHandledEvents(true);

            // Create RolloverModifier to show tooltips
            var rolloverModifier = new RolloverModifier();
            rolloverModifier.SetReceiveHandledEvents(true);

            // Create modifier group from declared modifiers
            var modifiers = new ModifierGroup(xAxisDragModifier, zoomPanModifier, zoomExtentsModifier, rolloverModifier);

            // Add the interactions to the ChartModifiers collection of the chart
            chart.ChartModifiers.Add(modifiers);

            // Data
            // Create XyDataSeries to host data for our chart
            var lineData = new XyDataSeries<double, double>() { SeriesName = "Sin(x)" };
            var scatterData = new XyDataSeries<double, double>() { SeriesName = "Cos(x)" };

            // Append data which should be drawn
            for (var i = 0; i < 100; i++)
            {
                lineData.Append(i, 60 + 50 * Math.Sin(i * 0.1));
                scatterData.Append(i, 60 + 50 * Math.Cos(i * 0.1));
            }

            // Create line series with data appended into lineData
            var lineSeries = new FastLineRenderableSeries()
            {
                DataSeries = lineData,
                StrokeStyle = new SolidPenStyle(Color.LightBlue, 2)
            };

            // Create scatter series with data appended into scatterData
            var pitchSpeedSeries = new FastLineRenderableSeries()
            {
                DataSeries = scatterData,
                StrokeStyle = new SolidPenStyle(Color.LightGreen, 2)
            };

            // Add the renderable series to the RenderableSeries collection of the chart
            chart.RenderableSeries.Add(lineSeries);
            chart.RenderableSeries.Add(pitchSpeedSeries);


            //// Spin Rate Chart
            //// Get our chart from the layout resource,
            //var spinRateChart = FindViewById<SciChartSurface>(Resource.Id.SpinRateChart);

            //// Create a numeric X axis
            //var spinRateXAxis = new NumericAxis(this) { AxisTitle = "Pitch Number" };

            //// Create a numeric Y axis
            //var spinRateYAxis = new NumericAxis(this)
            //{
            //    AxisTitle = "Pitch Spin Rate (RPM)",
            //    VisibleRange = new DoubleRange(1, 150)
            //};

            //// Add xAxis to the XAxes collection of the chart
            //spinRateChart.XAxes.Add(xAxis);

            //// Add yAxis to the YAxes collection of the chart
            //spinRateChart.YAxes.Add(yAxis);

            //// Create interactivity modifiers
            //var spinRateXAxisDragModifier = new XAxisDragModifier();
            //spinRateXAxisDragModifier.SetReceiveHandledEvents(true);

            //var spinRateZoomPanModifier = new ZoomPanModifier();
            //spinRateZoomPanModifier.Direction = Direction2D.XDirection;
            //spinRateZoomPanModifier.SetReceiveHandledEvents(true);

            //var spinRateZoomExtentsModifier = new ZoomExtentsModifier();
            //spinRateZoomExtentsModifier.SetReceiveHandledEvents(true);

            //// Create RolloverModifier to show tooltips
            //var spinRateRolloverModifier = new RolloverModifier();
            //spinRateRolloverModifier.SetReceiveHandledEvents(true);

            //// Create modifier group from declared modifiers
            //var spinRateModifiers = new ModifierGroup(spinRateXAxisDragModifier, spinRateZoomPanModifier, spinRateZoomExtentsModifier, spinRateRolloverModifier);

            //// Add the interactions to the ChartModifiers collection of the chart
            //spinRateChart.ChartModifiers.Add(spinRateModifiers);

            //// Data
            //// Create XyDataSeries to host data for our chart
            //var spinRateLineData = new XyDataSeries<double, double>() { SeriesName = "Sin(x)" };
            //var spinRateScatterData = new XyDataSeries<double, double>() { SeriesName = "Cos(x)" };

            //// Append data which should be drawn
            //for (var i = 0; i < 100; i++)
            //{
            //    spinRateLineData.Append(i, 60 + 50 * Math.Sin(i * 0.1));
            //    spinRateScatterData.Append(i, 60 + 50 * Math.Cos(i * 0.1));
            //}

            //// Create line series with data appended into lineData
            //var spinRateLineSeries = new FastLineRenderableSeries()
            //{
            //    DataSeries = spinRateLineData,
            //    StrokeStyle = new SolidPenStyle(Color.LightBlue, 2)
            //};

            //// Create scatter series with data appended into scatterData
            //var spinRateSeries = new FastLineRenderableSeries()
            //{
            //    DataSeries = scatterData,
            //    StrokeStyle = new SolidPenStyle(Color.LightGreen, 2)
            //};

            //// Add the renderable series to the RenderableSeries collection of the chart
            //spinRateChart.RenderableSeries.Add(spinRateLineSeries);
            //spinRateChart.RenderableSeries.Add(spinRateSeries);

            #endregion SciChart

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // Scan Button
            Button scanButton = FindViewById<Button>(Resource.Id.ScanButton);
            scanButton.Click += ScanButton_Click;

            #region OnCreate() Bluetooth

            // Request BluetoothPermission
            ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.BluetoothScan, 
                                                                   Manifest.Permission.BluetoothAdvertise, 
                                                                   Manifest.Permission.BluetoothConnect }, 1);


            // Register for broadcasts when a device is discovered
            _receiver = new BluetoothDeviceReceiver();
            RegisterBluetoothReceiver();




            #endregion OnCreate() Bluetooth
        }

        #region UI
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }
        #endregion UI

        #region Bluetooth
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private static MainActivity _instance;
        private bool _isReceiveredRegistered;
        private BluetoothDeviceReceiver _receiver;

        private static void StartScanning()
        {
            if (!BluetoothDeviceReceiver.Adapter.IsDiscovering) BluetoothDeviceReceiver.Adapter.StartDiscovery();
        }

        public static List<string> mDeviceList = new List<string>();

        #endregion Bluetooth
        // Scan Button
        public void ScanButton_Click(object sender, System.EventArgs e)
        {
            //var advertiseCheck = CheckSelfPermission(Manifest.Permission.BluetoothAdvertise);
            //var scanCheck = CheckSelfPermission(Manifest.Permission.BluetoothScan);
            //var connectCheck = CheckSelfPermission(Manifest.Permission.BluetoothConnect);

            //// Determine if BluetoothAdapter is Enabled
            //BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            //if (adapter == null)
            //    throw new Exception("No Bluetooth adapter found.");

            //if (!adapter.IsEnabled)
            //    throw new Exception("Bluetooth adapter is not enabled.");

            //// Get Bluetooth Device
            //BluetoothDevice device = (from bd in adapter.BondedDevices
            //                          where bd.Name == "Arduino Nano 33 BLE"
            //                          select bd).FirstOrDefault();

            //if (device == null)
            //    throw new Exception("Named device not found.");

            //// Create Bluetooth Socket Connection
            //var _socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            //// await _socket.ConnectAsync();

            //// Read/Write
            //// Read data from the device
            ////await _socket.InputStream.ReadAsync(buffer, 0, buffer.Length);

            //// Write data to the device
            ////await _socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            ///

            // Get Devices Already Paired with Phone
            if (BluetoothAdapter.DefaultAdapter != null && BluetoothAdapter.DefaultAdapter.IsEnabled)
            {
                foreach (var pairedDevice in BluetoothAdapter.DefaultAdapter.BondedDevices)
                {
                    Console.WriteLine(
                        $"Found device with name: {pairedDevice.Name} and MAC address: {pairedDevice.Address}");
                }
            }

            // Start Device Discovery
            StartScanning();
        }

        




    }
}
