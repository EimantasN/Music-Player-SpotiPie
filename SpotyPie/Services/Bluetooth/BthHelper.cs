using Android.Bluetooth;
using System.Threading;

namespace SpotyPie.Services.Bluetooth
{
    public static class BthHelper
    {
        public static void RestartBluetooth()
        {
            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (bluetoothAdapter.IsEnabled)
            {
                bluetoothAdapter.Disable();
                Thread.Sleep(1500);
                while (bluetoothAdapter.IsEnabled) Thread.Sleep(250);
                bluetoothAdapter.Enable();
                Thread.Sleep(500);
                while (!bluetoothAdapter.IsEnabled) Thread.Sleep(250);
            }
            else
            {
                bluetoothAdapter.Enable();
            }
        }
    }
}