using System.Collections.ObjectModel;

namespace SpotyPie.Services.Bluetooth.Interfaces
{
    public interface IBth
    {
        void Start(int sleepTime, bool readAsCharArray);
        void StopBth();
        ObservableCollection<string> PairedDevices();
    }
}