using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DLNAService_ClassLibrary
{
    internal class DLNAFolder : DLNAObject
    {
        #region Properties
        private int childCount = default(int);
        public int ChildCount { get { return childCount; } }
        
        private bool searchable = false;
        public bool Searchable { get { return searchable; } }
        
        #endregion
        internal DLNAFolder(XmlNode xmlNode) : base(xmlNode)
        {

            // Read the 'DIDL-Lite' attributes of the Object
            foreach (XmlAttribute tmpAttr in xmlNode.Attributes)
            {
                switch (tmpAttr.Name.ToLower())
                {
                    case "searchable":
                        {
                            searchable = Convert.ToBoolean(tmpAttr.Value);
                            break;
                        }

                    case "childcount":
                        {
                            childCount = Convert.ToInt32(tmpAttr.Value);
                            break;
                        }
                }
            }
        }

    }
}
