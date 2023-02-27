using System.Xml.Linq;

namespace SdxVisio
{
    /// <summary>
    /// Class to hold information about connectors information
    /// in the visio file. There are two Connect elements for each link.
    /// </summary>
    public class ConnectInfo
    {
        public int FromShapeId { get; set; }
        public string FromCellName { get; set; }

        public int ToShapeId { get; set; }
        public string ToCellName { get; set; }

        public ShapeInfo FromShape { get; set; }
        public ShapeInfo ToShape { get; set; }

        /// <summary>
        /// XElement that the Connect was derived from.
        /// </summary>
        public XElement XConnect { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="xe"></param>
        public ConnectInfo(XElement xe)
        {
            this.XConnect = xe;

        }

        public override string ToString()
        {
            return $"From:[ID={FromShapeId} Name={FromCellName}] To:[ID={ToShapeId} Name={ToCellName}]";
        }


    }



}
