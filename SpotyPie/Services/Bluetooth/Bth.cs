using System;
using Android.Bluetooth;
using Java.Util;
using System.Threading.Tasks;
using Java.IO;
using System.Threading;
using System.Collections.ObjectModel;
using Thread = System.Threading.Thread;
using Android.Runtime;
using SpotyPie.Services.Bluetooth.Enumerators;
using SpotyPie.Services.Bluetooth.Interfaces;
using SpotyPie.Services.Bluetooth;

namespace SpotyPie
{
    public class Bth : IBth
    {
        //For now hardcoded
        private string Name { get; set; } = "MDR-ZX330BT";

        private BlhStatus Status { get; set; } = BlhStatus.Started;

        private CancellationTokenSource _ct { get; set; }

        private const int ConnectedSleep = 2500;

        const int RequestResolveError = 1000;

        public Bth()
        {
        }

        public BlhStatus GetStatus()
        {
            return Status;
        }

        public void StopBth()
        {
            if (_ct != null)
            {
                _ct.Cancel();
            }
        }

        #region IBth implementation

        public void Start(int sleepTime = ConnectedSleep, bool readAsCharArray = false)
        {
            Task.Run(() => Loop(Name, sleepTime, readAsCharArray));
        }

        private void Loop(string name, int sleepTime, bool readAsCharArray)
        {
            BthHelper.RestartBluetooth();

            BluetoothDevice device = null;
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            BluetoothSocket BthSocket = null;

            Thread.Sleep(1000);
            _ct = new CancellationTokenSource();

            while (_ct.IsCancellationRequested == false)
            {
                try
                {
                    Thread.Sleep(sleepTime);
                    adapter = BluetoothAdapter.DefaultAdapter;
                    if (adapter == null)
                    {
                        Status = BlhStatus.AdapterNotFound;
                        return;
                    }

                    if (!adapter.IsEnabled)
                    {
                        Status = BlhStatus.AdapterDisabled;
                        return;
                    }

                    Status = BlhStatus.Connecting;
                    device = GetDevice(adapter, name);

                    if (device != null)
                    {
                        UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
                        if ((int)Android.OS.Build.VERSION.SdkInt >= 10) // Gingerbread 2.3.3 2.3.4
                            BthSocket = device.CreateInsecureRfcommSocketToServiceRecord(uuid);
                        else
                            BthSocket = device.CreateRfcommSocketToServiceRecord(uuid);

                        if (BthSocket != null)
                        {
                            IntPtr createRfcommSocket = JNIEnv.GetMethodID(device.Class.Handle, "createRfcommSocket", "(I)Landroid/bluetooth/BluetoothSocket;");
                            IntPtr _socket = JNIEnv.CallObjectMethod(device.Handle, createRfcommSocket, new global::Android.Runtime.JValue(1));
                            BthSocket = Java.Lang.Object.GetObject<BluetoothSocket>(_socket, JniHandleOwnership.TransferLocalRef);
                            BthSocket.Connect();

                            if (BthSocket.IsConnected)
                            {
                                Status = BlhStatus.Connected;

                                InputStreamReader mReader = new InputStreamReader(BthSocket.OutputStream);
                                BufferedReader buffer = new BufferedReader(mReader);

                                while (_ct.IsCancellationRequested == false)
                                {
                                    Thread.Sleep(ConnectedSleep);
                                    if (!BthSocket.IsConnected)
                                        Status = BlhStatus.Disconnected;
                                }
                            }
                        }
                        else
                        {
                            Status = BlhStatus.FailedCreateSocket;
                        }
                    }
                }
                catch
                {
                    Thread.Sleep(500);
                    //BthHelper.RestartBluetooth();
                    Status = BlhStatus.Error;
                }

                finally
                {
                    if (BthSocket != null)
                        BthSocket.Close();
                    device = null;
                    adapter = null;
                }
            }
        }

        public BluetoothDevice GetDevice(BluetoothAdapter adapter, string name)
        {
            foreach (var bd in adapter.BondedDevices)
            {
                System.Diagnostics.Debug.WriteLine("Paired devices found: " + bd.Name.ToUpper());
                if (bd.Name.ToUpper().IndexOf(name.ToUpper()) >= 0)
                {

                    System.Diagnostics.Debug.WriteLine("Found " + bd.Name + ". Try to connect with it!");
                    return bd;
                }
            }
            Status = BlhStatus.DeviceNotFound;
            return null;
        }

        public ObservableCollection<string> PairedDevices()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            ObservableCollection<string> devices = new ObservableCollection<string>();

            foreach (var bd in adapter.BondedDevices)
                devices.Add(bd.Name);

            return devices;
        }

        #endregion
    }
}
