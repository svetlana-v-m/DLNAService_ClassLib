using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DLNAService_ClassLibrary
{
    internal class DLNAFile: DLNAObject
    {
        #region Properties
        private string artist = "";
        public string Artist { get { return artist; } }
        
        private string album = "";
        public string Album { get { return album; } }
        
        private string genre = "";
        public string Genre { get { return genre; } }
        
        private string thumbnailURI = null;
        public string ThumbnailURI { get { return thumbnailURI; } }
        
        private List<Resource> resource;
        public string MediaURL { get { return resource[0].ToString(); } }
        
        #endregion

        internal DLNAFile(XmlNode xmlNode) : base(xmlNode)
        {
            // Read the 'Dublin Core' & 'UPnP' attributes of the Object
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                switch (childNode.Name.ToLower())
                {
                    case "upnp:artist":
                        {
                            artist = Convert.ToString(childNode.InnerText);
                            break;
                        }

                    case "upnp:album":
                        {
                            album = Convert.ToString(childNode.InnerText);
                            break;
                        }

                    case "upnp:genre":
                        {
                            genre = Convert.ToString(childNode.InnerText);
                            break;
                        }

                    case "res":
                        {
                            if (resource == null)
                            {
                                resource = new List<Resource>();
                            }

                            resource.Add(new Resource(childNode));
                            break;
                        }
                    case "upnp:icon":
                        {
                            thumbnailURI = Convert.ToString(childNode.InnerText);
                            break;
                        }
                }
            }
        }
    }

    internal class Resource
    {
        public ulong size;
        public string duration;
        public uint bitrate;
        public uint sampleFrequency;
        public uint bitsPerSample;
        public uint nrAudioChannels;
        public string protocolInfo;
        public string protection;
        public string importUri;
        public Uri URI;
        
        public Resource(XmlNode xmlNode)
        {
            // Read the 'DIDL-Lite' attributes of the Object
            foreach (XmlAttribute tmpAttr in xmlNode.Attributes)
            {
                switch (tmpAttr.Name.ToLower())
                {
                    case "duration":
                        {
                            duration = Convert.ToString(tmpAttr.Value);
                            break;
                        }

                    case "protocolinfo":
                        {
                            protocolInfo = Convert.ToString(tmpAttr.Value);
                            break;
                        }

                    case "size":
                        {
                            size = ulong.Parse(tmpAttr.Value);
                            break;
                        }

                    case "bitrate":
                        {
                            bitrate = Convert.ToUInt32(tmpAttr.Value);
                            break;
                        }

                    case "samplefrequency":
                        {
                            sampleFrequency = Convert.ToUInt32(tmpAttr.Value);
                            break;
                        }

                    case "bitspersample":
                        {
                            bitsPerSample = Convert.ToUInt32(tmpAttr.Value);
                            break;
                        }

                    case "nrAudioChannels":
                        {
                            nrAudioChannels = Convert.ToUInt32(tmpAttr.Value);
                            break;
                        }

                    case "protection":
                        {
                            protection = Convert.ToString(tmpAttr.Value);
                            break;
                        }

                    case "importUri":
                        {
                            importUri = Convert.ToString(tmpAttr.Value);
                            break;
                        }
                }
            }

            URI = new Uri(xmlNode.InnerText); // the url which we will send to the media renderer to play it
        }

        public override string ToString()
        {
            return URI.ToString();
        }
    }
}
