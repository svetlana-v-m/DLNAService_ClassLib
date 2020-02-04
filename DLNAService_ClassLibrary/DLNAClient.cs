using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Xml;
using UPNPLib;

namespace DLNAService_ClassLibrary
{
    //DLNAClient can start/stop searching for DLNA devices in local network and add founded devices to DLNADevices observable collection
    public class DLNAClient
    {
        //collection for founded devices
        public ObservableCollection<DLNADevice> DLNADevices = new ObservableCollection<DLNADevice>();
        private DLNADevice selectedDevice;
        public DLNADevice SelectedDevice
        {
            get { return selectedDevice; }
        }

        #region HelpProps
        private UPnPDeviceFinder DeviceFinder = new UPnPDeviceFinder();
        internal static int MediaServers;
        private DLNADeviceFinderCallback deviceFinderCallback;
        internal DLNADeviceFinderCallback DeviceFinderCallBack
        {
            get { return deviceFinderCallback; }
            set
            {
                deviceFinderCallback = value;
                if (value != null)
                {
                    deviceFinderCallback.DeviceFound += DeviceFound;
                }
                deviceFinderCallback.SearchOperationCompleted += SearchCompleted;
            }
        }
        #endregion
        //commands when device was found
        internal void DeviceFound(int lFindData, IUPnPDevice pDevice)
        {
            DLNADevices.Add(new DLNADevice((UPnPDevice)pDevice));
        }

        //commands on search completed
        internal void SearchCompleted(int IFindData)
        {

        }

        public void ChooseDLNADevice(int selectedIndex)
        {
            if (selectedIndex >= 0)
            {
                selectedDevice = DLNADevices[selectedIndex];
            }
        }

        //method which starts searching for DLNA devices asynchronously
        public void StartSearchingForDevices()
        {
            MediaServers = DeviceFinder.CreateAsyncFind("urn:schemas-upnp-org:device:MediaServer:1", 0, DeviceFinderCallBack = new DLNADeviceFinderCallback());
            DeviceFinder.StartAsyncFind(MediaServers);
        }

        //method to stop asynchronous search
        public void StopSearchingForDevices()
        {
            DeviceFinder.CancelAsyncFind(MediaServers);
        }
    }

    #region Helpers
    [ComVisible(true)]
    [ComImport]
    [Guid("415A984A-88B3-49F3-92AF-0508BEDF0D6C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface DLNADeviceFinderCallBack
    {
        [PreserveSig()]
        int DeviceAdded(int lFindData, IUPnPDevice pDevice);
        [PreserveSig()]
        int DeviceRemoved(int lFindData, string bstrUDN);
        [PreserveSig()]
        int SearchComplete(int lFindData);
    }
    internal class DLNADeviceFinderCallback : DLNADeviceFinderCallBack
    {
        public event DeviceFoundEventHandler DeviceFound;

        public delegate void DeviceFoundEventHandler(int lFindData, IUPnPDevice pDevice);

        public event DeviceLostEventHandler DeviceLost;

        public delegate void DeviceLostEventHandler(int lFindData, string bstrUDN);

        public event SearchOperationCompletedEventHandler SearchOperationCompleted;

        public delegate void SearchOperationCompletedEventHandler(int lFindData);

        public int DeviceAdded(int lFindData, IUPnPDevice pDevice)
        {
            DeviceFound?.Invoke(lFindData, pDevice);
            return default(int);
        }
        public int DeviceRemoved(int lFindData, string bstrUDN)
        {
            DeviceLost?.Invoke(lFindData, bstrUDN);
            return default(int);
        }
        public int SearchComplete(int lFindData)
        {
            SearchOperationCompleted?.Invoke(lFindData);
            return default(int);
        }
    }
    #endregion

}
