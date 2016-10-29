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
        /// <summary>
        /// The title of the audio track.
        /// </summary>
        [XmlElement(ElementName = "TIT2")]
        public String Track;
        
        /// <summary>
        /// The name of the track's artist.
        /// </summary>
        [XmlElement(ElementName = "TPE1")]
        public String Artist;

        /// <summary>
        /// The name of the track's album.
        /// </summary>
        [XmlElement(ElementName = "TALB")]
        public String Album;

        /// <summary>
        /// The name of the software acting as a source. Try to work the name "Netshout" in there <3
        /// </summary>
        [XmlElement(ElementName = "TENC")]
        public String SourceIdentifier;
        
        /// <summary>
        /// The name of the station's stream.
        /// </summary>
        [XmlElement(ElementName = "TRSN")]
        public String StreamTitle;
        
        /// <summary>
        /// A URL to the station's website.
        /// </summary>
        [XmlElement(ElementName = "WORS")]
        public String StationWebsite;

        /// <summary>
        /// The year this track was published.
        /// </summary>
        [XmlElement(ElementName = "TYER")]
        public String Year;
        
        /// <summary>
        /// The genre of the station.
        /// </summary>
        [XmlElement(ElementName = "TCON")]
        public String Genre;

        /// <summary>
        /// Comment.
        /// </summary>
        [XmlElement(ElementName = "COMM")]
        public String Comment;

        /// <summary>
        /// For use with sc_trans, this is the name of the current DJ.
        /// </summary>
        [XmlElement(ElementName = "DJ")]
        public String DjName;

        /// <summary>
        /// A list of the alternative titles, serialized under the <extension> tag.
        /// </summary>
        [XmlArray(ElementName="extension")]
        [XmlArrayItem(ElementName="title")]
        public Title[] Titles;

        /// <summary>
        /// Creates an object that can be serialized into an XML document formatted for the SHOUTcast metadata specification.
        /// </summary>
        public ShoutcastMetadata() { }

        /// <summary>
        /// Adds a title to the extension element of the metadata.
        /// </summary>
        /// <param name="NewTitle">The new title you want to add to the extension element.</param>
        public void AddTitle(String NewTitle)
        {
            if (Titles == null)
            {
                Titles = new Title[1];
            }

            else
            {
                Array.Resize<Title>(ref Titles, Titles.Length + 1);
            }

            Titles[Titles.Length - 1] = new Title();
            Titles[Titles.Length - 1].TrackTitle = NewTitle;

            Titles[Titles.Length - 1].Index = Titles.Length.ToString();
        }
    }

    public class Title
    {
        [XmlAttribute(AttributeName = "seq")]
        public String Index;

        [XmlText]
        public String TrackTitle;
        
        public Title() { }
    }
}
