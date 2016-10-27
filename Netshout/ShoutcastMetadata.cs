using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Netshout
{
    [XmlRootAttribute("metadata", IsNullable = false)]
    public class ShoutcastMetadata
    {
        [XmlElement(ElementName = "TIT2")]
        public String TrackTitle;
        
        [XmlElement(ElementName = "TPE1")]
        public String Artist;

        [XmlElement(ElementName = "TALB")]
        public String Album;

        [XmlElement(ElementName = "TENC")]
        public String SourceIdentifier;
        
        [XmlElement(ElementName = "TRSN")]
        public String StreamTitle;
        
        [XmlElement(ElementName = "WORS")]
        public String StationWebsite;

        [XmlElement(ElementName = "TYER")]
        public String Year;
        
        [XmlElement(ElementName = "TCON")]
        public String Genre;

        [XmlElement(ElementName = "COMM")]
        public String Comment;

        [XmlElement(ElementName = "DJ")]
        public String DjName;

        /// <summary>
        /// Creates an object that can be serialized into an XML document formatted for the SHOUTcast metadata specification.
        /// </summary>
        public ShoutcastMetadata() { }
    }

    [XmlRootAttribute("extension", IsNullable = false)]
    class Extension
    {
        [XmlElement(ElementName = "TIT2")]
        public String TrackTitle;

        public Extension() { }

    }
}
