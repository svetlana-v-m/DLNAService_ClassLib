using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DLNAService_ClassLibrary
{
    //base class for DLNAFolder and DLNAFile
    internal class DLNAObject
    {
        #region Properties
        public enum DLNAObject_WriteStatus
        {
            WS_Writable,
            WS_Protected,
            WS_NotWritable,
            WS_Unknown,
            WS_Mixed
        }

        private string id;
        public string ID { get { return id; } }
        
        private string parentID;
        public string ParentID { get { return parentID; } }
        
        private string name;
        public string Name { get { return name; } }
        
        private string creator = null;
        public string Creator { get { return creator; } }
        
        private string xmlClass;
        public string ClassName { get { return xmlClass; } }
        
        private int restricted;
        public bool Restricted
        {
            get
            {
                bool restrict;
                if (restricted == 0) restrict = false;
                else restrict = true;
                return restrict;
            }
        }
        
        private DLNAObject_WriteStatus writeStatus = default(DLNAObject_WriteStatus);
        public DLNAObject_WriteStatus WriteStatus { get { return writeStatus; } }
        
        private string xmlDump; // Didl info to send to the media renderer when playing this item
        public string XMLDump { get { return xmlDump; } }
       
        #endregion
        
        internal DLNAObject(XmlNode xmlNode)
        {
            xmlDump = xmlNode.OuterXml;
            // Read the 'DIDL-Lite' attributes of the Object
            foreach (XmlAttribute tmpAttr in xmlNode.Attributes)
            {
                switch (tmpAttr.Name.ToLower())
                {
                    case "id":
                        {
                            id = tmpAttr.Value.ToString();
                            break;
                        }

                    case "parentid":
                        {
                            parentID = tmpAttr.Value.ToString();
                            break;
                        }

                    case "restricted":
                        {
                            restricted = Int32.Parse(tmpAttr.Value);
                            break;
                        }
                }
            }

            // Read the 'Dublin Core' & 'UPnP' attributes of the Object
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                switch (childNode.Name.ToLower())
                {
                    case "dc:title":
                        {
                            name = childNode.InnerText.ToString();
                            break;
                        }

                    case "dc:creator":
                        {
                            creator = childNode.InnerText.ToString();
                            break;
                        }

                    case "upnp:class":
                        {
                            xmlClass = childNode.InnerText.ToString();
                            break;
                        }
                    case "upnp:writestatus":
                        {
                            switch (Convert.ToString(childNode.InnerText).ToUpper())
                            {
                                case "WRITABLE":
                                    {
                                        writeStatus = DLNAObject_WriteStatus.WS_Writable;
                                        break;
                                    }

                                case "PROTECTED":
                                    {
                                        writeStatus = DLNAObject_WriteStatus.WS_Protected;
                                        break;
                                    }

                                case "NOT_WRITABLE":
                                    {
                                        writeStatus = DLNAObject_WriteStatus.WS_Writable;
                                        break;
                                    }

                                case "UNKNOWN":
                                    {
                                        writeStatus = DLNAObject_WriteStatus.WS_Unknown;
                                        break;
                                    }

                                case "MIXED":
                                    {
                                        writeStatus = DLNAObject_WriteStatus.WS_Mixed;
                                        break;
                                    }
                            }

                            break;
                        }
                }
            }
        }

        public static object CreateDLNAObject(XmlNode xmlNode)
        {
            DLNAObject newObject = null;
            switch (xmlNode.Name.ToLower())
            {
                case "item":
                    {
                        newObject = new DLNAFile(xmlNode);
                        break;
                    }

                case "container":
                    {
                        newObject = new DLNAFolder(xmlNode);
                        break;
                    }
            }
            return newObject;
        }
    }
}
