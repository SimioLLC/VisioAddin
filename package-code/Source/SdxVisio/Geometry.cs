using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace SdxVisio
{
    /// <summary>
    /// A Section of the Shape Element is Geometry, which holds the
    /// vertices for a connector in a series of Row Elements. They
    /// are ordered by IX, with "T" specifying "LineTo"
    /// the rest 
    /// </summary>
    public class Geometry
    {
        private XElement GeometryElement;

        public List<VisioVertex> VList = new List<VisioVertex>();

        public Geometry(XElement xe, VisioVertex vBegin, VisioVertex vEnd)
        {
            GeometryElement = xe;

            string explanation = "";
            if (!GetVertices(xe, vBegin, vEnd, out explanation))
                throw new ApplicationException(explanation);
        }

        /// <summary>
        /// The assumption is that xeRoot is a 'Geometry' element, with child 'Row' elements
        /// containing IX attribute that will put them in the correct order, each
        /// 'Row' having 'Cell' children for X, Y, ...
        /// These usually start with 0,0 so we'll assume they are in relative
        /// coordinates.
        /// </summary>
        /// <param name="xeRoot"></param>
        /// <returns></returns>
        public bool GetVertices(XElement xeRoot, VisioVertex vBegin, VisioVertex vEnd, out string explanation)
        {
            return GetVertices(xeRoot, vBegin, vEnd, VList, out explanation);
        }

        /// <summary>
        /// The assumption is that xeRoot is a 'Geometry' element, with child 'Row' elements
        /// containing IX attribute that will put them in the correct order, each
        /// 'Row' having 'Cell' children for X, Y, ...
        /// These usually start with 0,0 so we'll assume they are in relative
        /// coordinates.
        /// </summary>
        /// <param name="xeRoot"></param>
        /// <returns></returns>
        public static bool GetVertices(XElement xeRoot, VisioVertex vBegin, VisioVertex vEnd, List<VisioVertex> vList, 
            out string explanation)
        {
            explanation = "";
            string marker = "Begin";
            try
            {
                List<XElement> vertexList = xeRoot.Elements()
                    .Where(rr => rr.Name.LocalName == "Row")
                    .OrderBy(rr => rr.Attribute("IX").Value)
                    .ToList();

                vList.Clear();

                // The connector points becomes the first vertex
                VisioVertex vvStart = new VisioVertex(vBegin.X, vBegin.Y, vBegin.Z);
                vList.Add(vvStart);

                int vertexNbr = 0;
                foreach (XElement xe in vertexList)
                {
                    vertexNbr++;
                    marker = $"Vertex#={vertexNbr}";

                    string sx = "", sy = "", sz = "";

                    foreach (XElement cell in xe.Elements()
                        .Where(rr => rr.Name.LocalName == "Cell"))
                    {
                        if (cell.Attribute("N").Value == "X")
                            sx = cell.Attribute("V").Value;
                        if (cell.Attribute("N").Value == "Y")
                            sy = cell.Attribute("V").Value;
                        if (cell.Attribute("N").Value == "Z")
                            sz = cell.Attribute("V").Value;
                    } // for each cell

                    VisioVertex vNew = new VisioVertex(sx, sy, sz);

                    VisioVertex vv = vvStart.Add(vNew);

                    vList.Add(vv);

                } // for each vertex

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Marker={marker} Err={ex}";
                return false;
            }

        }

    } // class Geometry

    /// <summary>
    /// A visio line is between 2 visio points.
    /// </summary>
    public class VisioLine
    {
        public VisioVertex P1 { get; set; }
        public VisioVertex P2 { get; set; }

        public VisioLine(VisioVertex v1, VisioVertex v2)
        {
            P1 = v1;
            P2 = v2;
        }

        /// <summary>
        /// Compute the length of the line
        /// </summary>
        public double Length
        {
            get
            {
                double len = Math.Pow(P2.X - P1.X,2) + Math.Pow(P2.Y - P1.Y,2) + Math.Pow(P2.Z - P1.Z,2);
                len = Math.Pow(len, 0.5);
                return len;
            }
        }


    }

    /// <summary>
    /// A Visio point or vertex (x,y,z)
    /// </summary>
    public class VisioVertex
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        /// <summary>
        /// Constructor with xyz assignments.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public VisioVertex(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Constructor with parsing of string xyz.
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <param name="sz"></param>
        public VisioVertex(string sx, string sy, string sz)
        {
            try
            {
                double xx = 0, yy = 0, zz = 0;

                double.TryParse(sx, out xx);
                double.TryParse(sy, out yy);
                double.TryParse(sz, out zz);

                X = xx; Y = yy; Z = zz;

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Converting SX={sx},SY={sy},SZ={sz}. Err={ex}");
            }
        }

        public override string ToString()
        {
            return $"X={X.ToString("0.000")} Y={Y.ToString("0.000")} Z={Z.ToString("0.000")}";
        }

        /// <summary>
        /// Add the given vertex (vAdd) to this vertex.
        /// </summary>
        /// <param name="vAdd"></param>
        /// <returns></returns>
        public VisioVertex Add(VisioVertex vAdd)
        {
            VisioVertex result = new VisioVertex(this.X + vAdd.X, this.Y + vAdd.Y, this.Z + vAdd.Z);
            return result;
        }

    } // Vertex

    /// <summary>
    /// Size where Width and Height 'looking down' (what we see in 2D Simio)
    /// </summary>
    public class VisioSize
    {
        /// <summary>
        /// The 'X' dimension. Visio calls Width, but Simio calls Length
        /// </summary>
        public double XWidth { get; set; }

        /// <summary>
        /// The 'Y' dimension. Visio calls Height, and this is Simio's Z, which Simio also calls height
        /// </summary>
        public double YHeight { get; set; }

        /// <summary>
        /// The 'z' component. Simio calls width (which we Visio already used for X)
        /// </summary>
        public double ZDepth { get; set; }

        public VisioSize( double width, double height, double depth)
        {
            XWidth = width;
            YHeight = height;
            ZDepth = depth;
        }

        public override string ToString()
        {
            return $"Width={XWidth.ToString("0.000")} Height={YHeight.ToString("0.000")} Length={ZDepth.ToString("0.000")}";
        }

    }


}
