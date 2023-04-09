using Android.Bluetooth;
using Android.Content;
using DopplerRadarAndroidApp;
using DopplerRadarAndroidApp.ListView;
using System.Collections.Generic;
using System.Linq;

namespace DopplerRadarAndroidApp
{
    public class BluetoothDeviceReceiver : BroadcastReceiver
    {
        public static BluetoothAdapter Adapter => BluetoothAdapter.DefaultAdapter;

        public List<BluetoothDevice> deviceList = new List<BluetoothDevice>();
        
        public BluetoothDeviceReceiver()
        {
            deviceList = Adapter.BondedDevices.ToList();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;

            // Found a device
            switch (action)
            {
                case BluetoothDevice.ActionFound:
                    // Get the device
                    var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);

                    // Only update the adapter with items which are not bonded
                    if (device.BondState != Bond.Bonded)
                    {
                        MainActivity.GetInstance().UpdateAdapter(new DataItem(device.Name, device.Address));
                        deviceList.Add(device);
                    }

                    break;
                case BluetoothAdapter.ActionDiscoveryStarted:
                    MainActivity.GetInstance().UpdateAdapterStatus("Discovery Started...");
                    break;
                case BluetoothAdapter.ActionDiscoveryFinished:
                    MainActivity.GetInstance().UpdateAdapterStatus("Discovery Finished.");
                    break;
                default:
                    break;
            }
        }
    }
}