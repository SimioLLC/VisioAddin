using SdxHelpers;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SdxVisio
{


    /// <summary>
    /// Used to collect the Shape Data we'll need in order
    /// to create the Simio DataSet.
    /// Remember that this includes not only objects but connectors.
    /// </summary>
    public class ShapeInfo
    {
        /// <summary>
        /// From the ID Attribute on the Shape Element.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// From the Name attribute on the Shape Element.
        /// </summary>
        public string ShapeName { get; set; }

        /// <summary>
        /// A name set by the user.
        /// This will be either the Text element of an object, 
        /// or a Simio_Name property (UserDefined Data)
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The name to use for display and export purposes.
        /// Use the Username, but if it is blank, then use the ShapeName
        /// </summary>
        public string Name { get {
                if (!string.IsNullOrEmpty(UserName))
                    return UserName;
                else
                    return ShapeName;
                } }

        /// <summary>
        /// The ShapeType from the "Shape" element and attribute "Type"
        /// </summary>
        public string ShapeType { get; set; }

        /// <summary>
        /// The Simio class name (e.g. Server, Sink, Path, ...)
        /// It can also be a derived class (e.g. MyServer, ...)
        /// </summary>
        public string SimioClass { get; set; }

        /// <summary>
        /// The Simio class name (e.g. Server, Sink, Path, ...)
        /// </summary>
        public string SimioBaseClass { get; set; }

        public SimioAttributes SimInfo { get; set; }

        /// <summary>
        /// What type of class (e.g. Object, Link, ...)
        /// </summary>
        public string SimioClassType { get; set; }

        /// <summary>
        /// A dictionary of properties found in the ShapeData
        /// </summary>
        public Dictionary<string, string> PropertyDict = new Dictionary<string, string>();

        /// <summary>
        /// The location of the Shape's Center.
        /// </summary>
        public VisioVertex Center { get; set; }

        /// <summary>
        /// For links, where it starts (BeginX, BeginY)
        /// </summary>
        public VisioVertex BeginLocation { get; set; }

        /// <summary>
        /// For links, where it ends (EndX, EndY)
        /// </summary>
        public VisioVertex EndLocation { get; set; }

        /// <summary>
        /// The size of the Shape
        /// </summary>
        public VisioSize Size { get; set; }

        /// <summary>
        /// The master (in masters.xml) that we reference
        /// </summary>
        public int MasterId { get; set; }

        /// <summary>
        /// XElement that the shape was derived from.
        /// </summary>
        public XElement XShape { get; set; }

        /// <summary>
        /// For Object only: SubShapes the the Link might reference.
        /// </summary>
        public List<ShapeInfo> SubShapeList;

        /// <summary>
        /// For Link only: the from object.
        /// </summary>
        public ShapeInfo FromShape { get; set; }
        /// <summary>
        /// For Link only: the to object.
        /// </summary>
        public ShapeInfo ToShape { get; set; }

        /// <summary>
        /// For Link only. The Vertex Geometry.
        /// </summary>
        public Geometry VertexGeometry { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="xe"></param>
        public ShapeInfo(XElement xe)
        {
            this.XShape = xe;

            SimioBaseClass = "";
            SimioClass = "";
            SimioClassType = "";

            Center = new VisioVertex(0,0,0);
            BeginLocation = new VisioVertex(0, 0, 0);
            EndLocation = new VisioVertex(0, 0, 0);
            Size = new VisioSize(0,0,0);
        }

        public override string ToString()
        {
            return $"Name={ShapeName} ID={Id} Class={SimioClass} ClassType={SimioClassType} Master={MasterId} Loc={Center}";
        }


    }


}
