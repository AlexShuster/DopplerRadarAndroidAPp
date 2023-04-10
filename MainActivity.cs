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
using System.Collections.Generic;
using Android.Support.V4.Content;
using Android.Content.PM;
using DopplerRadarAndroidApp.ListView;
using static Android.Bluetooth.BluetoothClass;
using Android.Media;
using System.Text;

namespace DopplerRadarAndroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private const int BluetoothPermissionsRequestCode = 1000;

        private static readonly string[] BluetoothPermissions =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.BluetoothAdvertise,
            Manifest.Permission.BluetoothConnect,
            Manifest.Permission.BluetoothPrivileged,
            Manifest.Permission.BluetoothScan

        };

        // Bluetooth Variables
        private static MainActivity _instance;
        private bool _isReceiveredRegistered;
        private BluetoothHandler _handler;
        private BluetoothSocket _socket;

        public static MainActivity GetInstance()
        {
            return _instance;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            // SchiChart License Key
            SciChartSurface.SetRuntimeLicenseKey("SrDiaOt4OKLdVapiKRimqKbg/4T5ljr/pv4erw0WJa6YWIk0DIfv/kY/Yrhn87mcrHTWcSZz/8K/OMQwErwJiTwBtrKs2634dkIISi3Ti46yAg4KUPYrHDliaRvBWA0VzD8ScQSHrWa/GuDnkyzL1MQOuPb6dV6HJh3Q9zQtNtm7QIOyHQkUlevKXjEXiUo/G9yyNTE1c4FcdsaIP0DFMthVBzUnAuTIaogY6HvKLvPKDreLyw0wNE3CjDo/Hb8cF7DM/inkyrt/7NNjNe2qfLQsqUQlxLeNUWDWUZqSnNbil07g8qHprNjpCcDGlSQ8c0ySpfe+1LYSD2icWqC3158eUTKmAkAlXMp5EwCFGIwb0Q3ddu/HOaiHJAEe9Njxj/aNIvJKepk16XwZFci36pbFPUrR0kbNkBWLPC835G/X12ysfGWFjODS5rw31FUNeaO1frsKcLui2nVMJ+g4yp4qygGCQY09gKznDIA77aISdhFxddCZxm/SCweQ+8bmxmkvnsIxKvPobnJ7bNnPA81ZW8TTBc/XG/Fz2I5PNWTnEdELtt9n6IN0gt14J+LMtw3fdJfRp6r5o6etYBrE9ZEV6cT7");

            base.OnCreate(savedInstanceState);

            _instance = this;

            SetContentView(Resource.Layout.activity_main);

            // Bluetooth Permissions
            ActivityCompat.RequestPermissions(this, BluetoothPermissions, BluetoothPermissionsRequestCode);

            // Register for broadcasts when a device is discovered
            _handler = new BluetoothHandler();

            Button scanButton = FindViewById<Button>(Resource.Id.ScanButton);
            scanButton.Click += ScanButton_Click;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Make sure we're not doing discovery anymore
            CancelScanning();
        }

        protected override void OnPause()
        {
            base.OnPause();

            // Make sure we're not doing discovery anymore
            CancelScanning();
        }

        protected override void OnResume()
        {
            base.OnResume();

            StartScanning();
        }

        private static void StartScanning()
        {
            // if (!BluetoothHandler.Adapter.IsDiscovering) BluetoothHandler.Adapter.StartDiscovery();
        }

        private static void CancelScanning()
        {
            if (BluetoothHandler.Adapter.IsDiscovering) BluetoothHandler.Adapter.CancelDiscovery();
        }

        public void ScanButton_Click(object sender, System.EventArgs e)
        {
            // Get Bonded Device (RadarGunBluetooth)
            var connectedDevice = (from bd in _handler.deviceList where bd.Name == "RadarGunBluetooth" select bd).FirstOrDefault();// Return bonded device
            //connectedDevice.SetPairingConfirmation(false);

            //connectedDevice.SetPairingConfirmation(true);
            connectedDevice.CreateBond();

            // Create and Connect Bluetooth Socket
            _socket = connectedDevice.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb")); //the UUID of HC-05
            _socket.Connect();

            // Write data to the device
            _socket.OutputStream.Write(System.Text.Encoding.ASCII.GetBytes("Hello World!"));
            byte[] bytes = new byte[8];
            byte[] bytes2 = new byte[8];
            //float bytesString;
            //float bytesString2;
            string bytesString;
            string bytesString2;

            while (true)
            {
                _socket.InputStream.Read(bytes, 0, 4);
                // bytesString = System.BitConverter.ToSingle(bytes, 0);
                bytesString = System.Text.Encoding.ASCII.GetString(bytes);

                _socket.InputStream.Read(bytes2, 0, 4);
                // bytesString2 = System.BitConverter.ToSingle(bytes2, 0);
                bytesString2 = System.Text.Encoding.ASCII.GetString(bytes2);
            }
            
        }
    }
}
