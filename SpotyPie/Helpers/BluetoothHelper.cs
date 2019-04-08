using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Content;
using Java.Util;

namespace SpotyPie.Helpers
{
    //public interface IBluetoothHelper
    //{
    //    bool IsConnected { get; set; }

    //    string DeviceName { get; set; }

    //    Task<bool> Connect(string deviceName);
    //}

    //public class BluetoothHelper : IBluetoothHelper
    //{
    //    private BluetoothSocket _socket = null;

    //    private CancellationTokenSource _ct { get; set; }

    //    private Context _context;

    //    public bool IsConnected { get; set; }

    //    public string DeviceName { get; set; }

    //    public BluetoothHelper(Context context)
    //    {
    //        _context = context;
    //    }

    //    public bool Connect(string name)
    //    {
    //        Task.Run(() => ConnectToDevice(name));
    //        return true;
    //    }

    //    private async Task<bool> ConnectToDevice(string name)
    //    {
    //        BluetoothDevice device = null;
    //        BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;

    //        if (IsConnected)
    //            return true;

    //        _ct = new CancellationTokenSource();
    //        while (_ct.IsCancellationRequested == false)
    //        {
    //            try
    //            {
    //                Thread.Sleep(200);

    //                adapter = BluetoothAdapter.DefaultAdapter;

    //                if (adapter?.IsEnabled == null)
    //                    return false;

    //                // paired devices
    //                adapter.BondedDevices.ToList();

    //                foreach (var bondedDevice in adapter.BondedDevices)
    //                {
    //                    if (bondedDevice.Name.ToLower().IndexOf(name.ToLower()) >= 0)
    //                    {
    //                        device = bondedDevice;
    //                        break;
    //                    }
    //                }

    //                if (device == null)
    //                    return false;

    //                var uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
    //                _socket = device.CreateInsecureRfcommSocketToServiceRecord(uuid);

    //                if (_socket == null)
    //                    return false;

    //                await _socket.ConnectAsync();

    //                IsConnected = _socket.IsConnected;
    //                if (IsConnected)
    //                    await Task.Run(TalkToDevice);

    //                var a2dp = adapter.GetProfileConnectionState(ProfileType.A2dp);
    //                var gatt = adapter.GetProfileConnectionState(ProfileType.Gatt);
    //                var gattserver = adapter.GetProfileConnectionState(ProfileType.GattServer);
    //                var headset = adapter.GetProfileConnectionState(ProfileType.Headset);
    //                var health = adapter.GetProfileConnectionState(ProfileType.Health);
    //                var sap = adapter.GetProfileConnectionState(ProfileType.Sap);

    //                return IsConnected;
    //            }
    //            catch
    //            {

    //            }

    //            return false;
    //        }

    //        return false;
    //    }

    //    private Task TalkToDevice()
    //    {
    //        // check _socket.IsConnected, read w/InputStreamReader, BufferedReader
    //        return Task.CompletedTask;
    //    }
    //}
}