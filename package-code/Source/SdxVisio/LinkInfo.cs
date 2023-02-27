using SdxHelpers;
using System;
using System.Collections.Generic;

namespace SdxVisio
{
    /// <summary>
    /// Class to hold information about links
    /// </summary>
    public class LinkInfo
    {
        /// <summary>
        /// the link shape id.
        /// </summary>
        public int LinkId { get; set; }

        /// <summary>
        /// The shape we derive from
        /// </summary>
        public ShapeInfo MyShape { get; set; }

        /// <summary>
        /// The id of the object we are coming from
        /// </summary>
        public int FromObjectId { get; set; }

        /// <summary>
        /// The id of the object we are going to
        /// </summary>
        public int ToObjectId { get; set; }

        /// <summary>
        /// The object we are coming from
        /// </summary>
        public ShapeInfo FromObject { get; set; }

        public int FromIndex { get; set; }

        /// <summary>
        /// The object we are going to
        /// </summary>
        public ShapeInfo ToObject { get; set; }

        public int ToIndex { get; set; }

        /// <summary>
        /// The 'From' location (where this link begins)
        /// </summary>
        public VisioVertex BeginLocation {  get { return MyShape.BeginLocation; } }

        /// <summary>
        /// The simio properties that were found in the ShapeData
        /// </summary>
        public Dictionary<string,string> PropertyDict { get; set; }

        /// <summary>
        /// Properties associated with Simio links.
        /// </summary>
        public SimioLinkAttributes SimioLinkProps;

        /// <summary>
        /// The 'To' location (where this link ends)
        /// </summary>
        public VisioVertex EndLocation { get { return MyShape.EndLocation; } }

        public override string ToString()
        {
            string fromObj = FromObject?.ShapeName;
            string toObj = ToObject?.ShapeName;
            string ss = $"Link={MyShape.ShapeName} Class={MyShape.SimioClass}(Base={MyShape.SimioBaseClass}) From={fromObj} To={toObj}";
            return ss;
        }
        public LinkInfo(SimioAttributes simProps, int id, ShapeInfo si )
        {
            this.LinkId = id;
            this.MyShape = si;

            this.PropertyDict = si.PropertyDict;
            if ( this.PropertyDict.Count > 0 )
            {
                bool isDebug = true;
            }
       
            if (!simProps.LinkDict.TryGetValue(si.SimioBaseClass.ToLower(), out SimioLinkProps))
            {
                throw new ApplicationException($"Could not find Link Properties for Link={si.SimioClass}");
            }

        }
    }


}
