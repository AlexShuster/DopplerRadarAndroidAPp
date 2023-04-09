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
        private const int LocationPermissionsRequestCode = 1000;

        private static readonly string[] LocationPermissions =
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
        private BluetoothDeviceReceiver _receiver;
        private BluetoothSocket _socket;
        private BluetoothServerSocket _serverSocket;

        public static MainActivity GetInstance()
        {
            return _instance;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SciChartSurface.SetRuntimeLicenseKey("SrDiaOt4OKLdVapiKRimqKbg/4T5ljr/pv4erw0WJa6YWIk0DIfv/kY/Yrhn87mcrHTWcSZz/8K/OMQwErwJiTwBtrKs2634dkIISi3Ti46yAg4KUPYrHDliaRvBWA0VzD8ScQSHrWa/GuDnkyzL1MQOuPb6dV6HJh3Q9zQtNtm7QIOyHQkUlevKXjEXiUo/G9yyNTE1c4FcdsaIP0DFMthVBzUnAuTIaogY6HvKLvPKDreLyw0wNE3CjDo/Hb8cF7DM/inkyrt/7NNjNe2qfLQsqUQlxLeNUWDWUZqSnNbil07g8qHprNjpCcDGlSQ8c0ySpfe+1LYSD2icWqC3158eUTKmAkAlXMp5EwCFGIwb0Q3ddu/HOaiHJAEe9Njxj/aNIvJKepk16XwZFci36pbFPUrR0kbNkBWLPC835G/X12ysfGWFjODS5rw31FUNeaO1frsKcLui2nVMJ+g4yp4qygGCQY09gKznDIA77aISdhFxddCZxm/SCweQ+8bmxmkvnsIxKvPobnJ7bNnPA81ZW8TTBc/XG/Fz2I5PNWTnEdELtt9n6IN0gt14J+LMtw3fdJfRp6r5o6etYBrE9ZEV6cT7");

            base.OnCreate(savedInstanceState);

            _instance = this;

            SetContentView(Resource.Layout.activity_main);

            var coarseLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation);
            var fineLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);

            if (coarseLocationPermissionGranted != Permission.Denied ||
                fineLocationPermissionGranted == Permission.Denied)
                ActivityCompat.RequestPermissions(this, LocationPermissions, LocationPermissionsRequestCode);

            // Register for broadcasts when a device is discovered
            _receiver = new BluetoothDeviceReceiver();

            RegisterBluetoothReceiver();

            PopulateListView();

            Button scanButton = FindViewById<Button>(Resource.Id.ScanButton);
            scanButton.Click += ScanButton_Click;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Make sure we're not doing discovery anymore
            CancelScanning();

            // Unregister broadcast listeners
            UnregisterBluetoothReceiver();
        }

        protected override void OnPause()
        {
            base.OnPause();

            // Make sure we're not doing discovery anymore
            CancelScanning();

            // Unregister broadcast listeners
            UnregisterBluetoothReceiver();
        }

        protected override void OnResume()
        {
            base.OnResume();

            StartScanning();

            // Register broadcast listeners
            RegisterBluetoothReceiver();
        }

        public void UpdateAdapter(DataItem dataItem)
        {
            var lst = FindViewById<Android.Widget.ListView>(Resource.Id.lstview);
            var adapter = lst.Adapter as ListViewAdapter;

            var items = adapter?.Items.Where(m => m.GetListItemType() == ListItemType.DataItem).ToList();

            if (items != null && !items.Any(x =>
                    ((DataItem)x).Text == dataItem.Text && ((DataItem)x).SubTitle == dataItem.SubTitle))
            {
                adapter.Items.Add(dataItem);
            }

            lst.Adapter = new ListViewAdapter(this, adapter?.Items);
        }

        private static void StartScanning()
        {
            if (!BluetoothDeviceReceiver.Adapter.IsDiscovering) BluetoothDeviceReceiver.Adapter.StartDiscovery();
        }

        private static void CancelScanning()
        {
            if (BluetoothDeviceReceiver.Adapter.IsDiscovering) BluetoothDeviceReceiver.Adapter.CancelDiscovery();
        }

        private void RegisterBluetoothReceiver()
        {
            if (_isReceiveredRegistered) return;

            RegisterReceiver(_receiver, new IntentFilter(BluetoothDevice.ActionFound));
            RegisterReceiver(_receiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryStarted));
            RegisterReceiver(_receiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));
            _isReceiveredRegistered = true;
        }

        private void UnregisterBluetoothReceiver()
        {
            if (!_isReceiveredRegistered) return;

            UnregisterReceiver(_receiver);
            _isReceiveredRegistered = false;
        }

        private void PopulateListView()
        {
            var item = new List<IListItem>
            {
                new HeaderListItem("PREVIOUSLY PAIRED")
            };

            item.AddRange(
                BluetoothDeviceReceiver.Adapter.BondedDevices.Select(
                    bluetoothDevice => new DataItem(
                        bluetoothDevice.Name,
                        bluetoothDevice.Address
                    )
                )
            );

            StartScanning();

            item.Add(new StatusHeaderListItem("Scanning started..."));

            var lst = FindViewById<Android.Widget.ListView>(Resource.Id.lstview);
            lst.Adapter = new ListViewAdapter(this, item);
        }

        public void UpdateAdapterStatus(string discoveryStatus)
        {
            var lst = FindViewById<Android.Widget.ListView>(Resource.Id.lstview);
            var adapter = lst.Adapter as ListViewAdapter;

            var hasStatusItem = adapter?.Items?.Any(m => m.GetListItemType() == ListItemType.Status);

            if (hasStatusItem.HasValue && hasStatusItem.Value)
            {
                var statusItem = adapter.Items.Single(m => m.GetListItemType() == ListItemType.Status);
                statusItem.Text = discoveryStatus;
            }

            lst.Adapter = new ListViewAdapter(this, adapter?.Items);
        }



        public void ScanButton_Click(object sender, System.EventArgs e)
        {
            // Get Bonded Device (RadarGunBluetooth)
            var connectedDevice = (from bd in _receiver.deviceList where bd.Name == "RadarGunBluetooth" select bd).FirstOrDefault();// Return bonded device
            //connectedDevice.SetPairingConfirmation(false);

            //connectedDevice.SetPairingConfirmation(true);
            connectedDevice.CreateBond();

            // Create and Connect Bluetooth Socket
            _socket = connectedDevice.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb")); //the UUID of HC-05
            _socket.Connect();

            //var connectedDevice = _receiver.deviceList.Find(x => x.Name == "AlexLab4");
            ////_socket = connectedDevice.CreateL2capChannel(0x0080); c2478b88-3ede-4070-b011-3716be7c1aca
            //_serverSocket = BluetoothDeviceReceiver.Adapter.ListenUsingRfcommWithServiceRecord("PitchData", UUID.FromString("0000FFE0-0000-1000-8000-00805F9B34FB"));
            //_socket = _serverSocket.Accept();
            //_serverSocket.Close();

            //CancelScanning();
            ////_socket.Connect();

            //// Read data from the device
            //// _socket.InputStream.ReadByte();

            //// Write data to the device
            _socket.OutputStream.Write(System.Text.Encoding.ASCII.GetBytes("Hello World!"));
        }
    }
}
