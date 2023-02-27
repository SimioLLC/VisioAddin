using System.Xml.Linq;

namespace SdxVisio
{
    /// <summary>
    /// Class to hold information about Artifacts (that is non-Simio shapes)
    /// </summary>
    public class ArtifactInfo
    {
        /// <summary>
        /// the artifact shape id.
        /// </summary>
        public int ArtifactId { get; set; }

        /// <summary>
        /// The shape we derive from (this is one-to-one)
        /// </summary>
        public ShapeInfo MyShape { get; set; }

        /// <summary>
        /// Return our shapes name
        /// </summary>
        public string Name { get { return MyShape?.ShapeName; } }

        /// <summary>
        /// The master (in masters.xml) that we reference
        /// </summary>
        public int MasterId { get; set; }

        /// <summary>
        /// For Shapes that have text
        /// </summary>
        public string TextBlock { get; set; }


        /// <summary>
        /// For Shapes that have foreign data (like bitmaps)
        /// </summary>
        public string ForeignData { get; set; }

        /// <summary>
        /// XElement that the shape was derived from.
        /// </summary>
        public XElement XShape { get { return MyShape?.XShape; } }


        public ArtifactInfo(int id, ShapeInfo si)
        {
            this.ArtifactId = id;
            this.MyShape = si;

        }


    }


}
