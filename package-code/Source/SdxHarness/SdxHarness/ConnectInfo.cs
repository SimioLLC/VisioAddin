namespace SdxVisio
{
    /// <summary>
    /// Class to hold information about connectors.
    /// </summary>
    public class ConnectInfo
    {
        public int FromShapeId { get; set; }
        public string FromCellName { get; set; }

        public int ToShapeId { get; set; }
        public string ToCellName { get; set; }

        public ShapeInfo FromShape { get; set; }
        public ShapeInfo ToShape { get; set; }
    }



}
