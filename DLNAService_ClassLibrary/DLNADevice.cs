using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UPNPLib;

namespace DLNAService_ClassLibrary
{
    //DLNA device class can browse in device content and get files metadata
    public class DLNADevice : UPnPDevice
    {
        public DLNADevice(UPnPDevice uPnPDevice)
        {
            isRootDevice = uPnPDevice.IsRootDevice;
            rootDevice = uPnPDevice.RootDevice;
            parentDevice = uPnPDevice.ParentDevice;
            hasChildren = uPnPDevice.HasChildren;
            children = uPnPDevice.Children;
            uniqueDeviceName = uPnPDevice.UniqueDeviceName;
            friendlyName = uPnPDevice.FriendlyName;
            type = uPnPDevice.Type;
            presentationURL = uPnPDevice.PresentationURL;
            manufacturerName = uPnPDevice.ManufacturerName;
            manufacturerURL = uPnPDevice.ManufacturerURL;
            modelName = uPnPDevice.ModelName;
            modelNumber = uPnPDevice.ModelNumber;
            description = uPnPDevice.Description;
            modelURL = uPnPDevice.ModelURL;
            upc = uPnPDevice.UPC;
            serialNumber = uPnPDevice.SerialNumber;
            services = uPnPDevice.Services;
            foreach (UPnPService myService in uPnPDevice.Services)
            {
                //define if found DLNA device has Content Directory Service
                if ((myService.ServiceTypeIdentifier ?? "") == "urn:schemas-upnp-org:service:ContentDirectory:1")
                {
                    ContentDirectory = myService;//if it is then ContentDirectory is not null
                    break;
                }
            }
        }

        #region Properties
        private UPnPService ContentDirectory = null;
        public UPnPDevice UPnPDevice { get; set; }
        private bool isRootDevice;
        public bool IsRootDevice { get { return isRootDevice; } }
        private UPnPDevice rootDevice;
        public UPnPDevice RootDevice { get { return rootDevice; } }
        private UPnPDevice parentDevice;
        public UPnPDevice ParentDevice { get { return parentDevice; } }
        private bool hasChildren;
        public bool HasChildren { get { return hasChildren; } }
        private UPnPDevices children;
        public UPnPDevices Children { get { return children; } }
        private string uniqueDeviceName;
        public string UniqueDeviceName { get { return uniqueDeviceName; } }
        private string friendlyName;
        public string FriendlyName { get { return friendlyName; } }
        private string type;
        public string Type { get { return type; } }
        private string presentationURL;
        public string PresentationURL { get { return presentationURL; } }
        private string manufacturerName;
        public string ManufacturerName { get { return manufacturerName; } }
        private string manufacturerURL;
        public string ManufacturerURL { get { return manufacturerURL; } }
        private string modelName;
        public string ModelName { get { return modelName; } }
        private string modelNumber;
        public string ModelNumber { get { return modelNumber; } }
        private string description;
        public string Description { get { return description; } }
        private string modelURL;
        public string ModelURL { get { return modelURL; } }
        private string upc;
        public string UPC { get { return upc; } }
        private string serialNumber;
        public string SerialNumber { get { return serialNumber; } }
        private UPnPServices services;
        public UPnPServices Services { get { return services; } }
        public string IconURL(string bstrEncodingFormat, int lSizeX, int lSizeY, int lBitDepth)
        {
            return rootDevice.IconURL(bstrEncodingFormat, lSizeX, lSizeY, lBitDepth);
        }
        #endregion
        
        //method gets list of folders/items in directory with ID=objectID. If it is root directory then objectID="0"
        internal List<DLNAObject> GetDeviceContent(string objectID)
        {
            List<DLNAObject> list = new List<DLNAObject>(GetContent(objectID));
            return list;
        }

        //method gets file metadata for DLNA device files
        internal DLNAFile GetFileInfo(string Id)
        {
            DLNAFile fileInfo = GetDLNAFileInformation(Id);
            return fileInfo;
        }
        private List<DLNAObject> GetContent(string parentID)
        {
            if (ContentDirectory == null) return null;//if DLNA device doesn't have Content Directory service, List<DLNAObjects>=null
            
            //collection for DLNA device objects
            List<DLNAObject> DLNAObjects = new List<DLNAObject>();

            #region Request
            string browseFlag = "BrowseDirectChildren"; // BrowseDirectChildren or BrowseMetadata as allowed values
            string filter = "";
            int startingIndex = 0;
            int requestedCount = 1000;
            string sortCriteria = "";

            object[] inArgs = new object[6];
            inArgs[0] = parentID;
            inArgs[1] = browseFlag;
            inArgs[2] = filter;
            inArgs[3] = startingIndex;
            inArgs[4] = requestedCount;
            inArgs[5] = sortCriteria;

            object outArgs = new object[4];
            ContentDirectory.InvokeAction("Browse", inArgs, ref outArgs);

            #endregion

            #region Responce
            object[] resultobj = (object[])outArgs;

            string result = (string)resultobj[0];
            int numberReturned = (int)(UInt32)resultobj[1];
            int totalMatches = (int)(UInt32)resultobj[2];
            int updateID = (int)(UInt32)resultobj[3];

            if (outArgs == null) return null;
            var myXMLDoc = new XmlDocument();
            #endregion

            //filling in DLNAObjects collection
            myXMLDoc.LoadXml(Convert.ToString(result));

            foreach (XmlNode xmlNode in myXMLDoc.DocumentElement.ChildNodes)
            {
                DLNAObjects.Add(new DLNAObject(xmlNode));
            }

            return DLNAObjects;
        }

        private DLNAFile GetDLNAFileInformation(string objectID)
        {
            if (ContentDirectory == null) return null;

            DLNAFile dlnaFile = null;

            #region Request
            string browseFlag = "BrowseMetadata"; // BrowseDirectChildren or BrowseMetadata as allowed values
            string filter = "upnp:album,upnp:artist,upnp:genre,upnp:title,upnp:icon,res@size,res@duration,res@bitrate,res@sampleFrequency,res@bitsPerSample,res@nrAudioChannels,res@protocolInfo,res@protection,res@importUri";
            int startingIndex = 0;
            int requestedCount = 1;
            string sortCriteria = "";

            object[] inArgs = new object[6];
            inArgs[0] = objectID;
            inArgs[1] = browseFlag;
            inArgs[2] = filter;
            inArgs[3] = startingIndex;
            inArgs[4] = requestedCount;
            inArgs[5] = sortCriteria;

            object outArgs = new object[4];
            ContentDirectory.InvokeAction("Browse", inArgs, ref outArgs);
            #endregion

            #region Responce
            object[] resultobj = (object[])outArgs;

            string result = (string)resultobj[0];
            int numberReturned = (int)(UInt32)resultobj[1];
            int totalMatches = (int)(UInt32)resultobj[2];
            int updateID = (int)(UInt32)resultobj[3];

            if (outArgs == null) return null;
            #endregion

            //extracting file metadata from responce
            var myXMLDoc = new XmlDocument();

            myXMLDoc.LoadXml(Convert.ToString(result));

            if (myXMLDoc.DocumentElement.ChildNodes.Count == 1)
                dlnaFile = (DLNAFile)DLNAObject.CreateDLNAObject(myXMLDoc.DocumentElement.ChildNodes[0]);

            return dlnaFile;
        }
    }
}
