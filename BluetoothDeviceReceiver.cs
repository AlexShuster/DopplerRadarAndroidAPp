using Android.Bluetooth;
using Android.Content;
using DopplerRadarAndroidApp;
using DopplerRadarAndroidApp.ListView;
using System.Collections.Generic;
using System.Linq;

namespace DopplerRadarAndroidApp
{
    public class BluetoothHandler
    {
        public static BluetoothAdapter Adapter => BluetoothAdapter.DefaultAdapter;

        public List<BluetoothDevice> deviceList = new List<BluetoothDevice>();
        
        public BluetoothHandler()
        {
            deviceList = Adapter.BondedDevices.ToList();
        }
    }
}