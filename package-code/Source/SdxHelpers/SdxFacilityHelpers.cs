using SimioAPI;
using SimioAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdxHelpers
{
    /// <summary>
    /// Compute an AABB (Axis Aligned (minimum) Bounding Box) given location and size.
    /// Two vectors vMin and vMax define the AABB.
    /// </summary>
    public class AABB
    {
        public SdxVector PtMin { get; set; }
        public SdxVector PtMax { get; set; }

        public double XWidth { get { return Math.Abs(PtMax.X - PtMin.X); } }
        public double YHeight { get { return Math.Abs(PtMax.Y - PtMin.Y); } }
        public double ZLength { get { return Math.Abs(PtMax.Z - PtMin.Z); } }

        /// <summary>
        /// Compute the center of the box
        /// </summary>
        public SdxVector Center { get
            {
                return new SdxVector((PtMin.X + PtMax.X) / 2.0, (PtMin.Y + PtMax.Y) / 2.0, (PtMin.Z + PtMax.Z) / 2.0);
            } }

        /// <summary>
        /// Compute using a center point and size.
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="size"></param>
        public AABB(FacilityLocation loc, FacilitySize size)
        {
            PtMin = new SdxVector(double.MaxValue, double.MaxValue, double.MaxValue);
            PtMax = new SdxVector(double.MinValue, double.MinValue, double.MinValue);

            double deltaX = size.Width / 2.0;
            double deltaY = size.Length / 2.0;
            double deltaZ = size.Height / 2.0;

            PtMax = new SdxVector(loc.X + deltaX, loc.Y + deltaY, loc.Z + deltaZ);
            PtMin = new SdxVector(loc.X - deltaX, loc.Y - deltaY, loc.Z - deltaZ);

        }

        /// <summary>
        /// Compute using a list of vectors, and set the starting
        /// points to double.MinValues and double.MaxValues
        /// </summary>
        /// <param name="ptList"></param>
        public AABB(List<SdxVector> ptList)
        {
            ComputeNewAabb(true, ptList);
        } // method

        /// <summary>
        /// Compute a new AABB using a list of vectors, and compare against
        /// our internal PtMin and PtMax.
        /// If initialize is true, the the min/max points are initialized.
        /// </summary>
        /// <param name="ptList"></param>
        public void ComputeNewAabb(bool initialize, List<SdxVector> ptList)
        {
            if (initialize)
            {
                PtMin = new SdxVector(double.MaxValue, double.MaxValue, double.MaxValue);
                PtMax = new SdxVector(double.MinValue, double.MinValue, double.MinValue);
            }

            foreach (SdxVector pt in ptList)
            {
                if (pt.X > PtMax.X)
                    PtMax.X = pt.X;
                if (pt.X < PtMin.X)
                    PtMin.X = pt.X;

                if (pt.Y > PtMax.Y)
                    PtMax.Y = pt.Y;
                if (pt.Y < PtMin.Y)
                    PtMin.Y = pt.Y;

                if (pt.Z > PtMax.Z)
                    PtMax.Z = pt.Z;
                if (pt.Z < PtMin.Z)
                    PtMin.Z = pt.Z;

            } // for each point

        } // method

        public override string ToString()
        {
            return $"AABB=[Min={PtMin} Max={PtMax} W={XWidth.ToString("0.00")} H={YHeight.ToString("0.00")} L={ZLength.ToString("0.00")}]";
        }


    } // class aabb


    public static class SdxFacilityHelpers
    {

        /// <summary>
        /// Read a Simio Drawing Exchange (SDX) DataSet file, which will create the Facility.
        /// It is placed on the Facility View according to the transform.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ds"></param>
        /// <param name="transform"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static bool CreateFacilityFromDataSet(IDesignContext context, DataSet ds, SimioTransform transform, List<string> explanationList)
        {
            explanationList.Clear();

            if (ds == null)
            {
                explanationList.Add($"DataSet is null");
                return false;
            }

            try
            {
                var intelligentObjects = context.ActiveModel.Facility.IntelligentObjects;

                // Create a SDX context object from the dataset.
                SdxDataSetContext SdxContext = new SdxDataSetContext("Visio", "Facility", "", ds);
                string explanation = "";
                if (!SdxContext.ComputeBoundingBox(out explanation))
                {
                    explanationList.Add(explanation);
                    return false;
                }

                List<string> reasonList = new List<string>();
                if (!CreateSimioObjects(context, SdxContext, transform, reasonList))
                {
                    explanationList.AddRange(reasonList);
                }

                if (!CreateSimioLinks(context, SdxContext, transform, reasonList))
                {
                    explanationList.AddRange(reasonList);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Err={ex}");
            }
        }

        /// <summary>
        /// Get the facility location in simio units.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        private static FacilityLocation GetFacilityLoc(SdxObject obj, SimioTransform transform)
        {
            // Todo: Make transform
            double xx = obj.Center.X;
            double yy = obj.Center.Y;
            double zz = obj.Center.Z;

            FacilityLocation loc = new FacilityLocation(xx, yy, zz);
            return loc;
        }
        /// <summary>
        /// Get the facility location in simio units.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        private static FacilityLocation GetFacilityLoc(INodeObject obj, SimioTransform transform)
        {
            FacilityLocation loc = transform.TransformFacilityLocation(obj.Location);
            return loc;
        }

        /// <summary>
        /// Create Simio IFixedObjects from a DataTable
        /// </summary>
        /// <param name="context">Simio's Design-Time context</param>
        /// <param name="SdxContext">SDX context holding object, links, ...</param>
        /// <param name="transform">Spatial transforms</param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static bool CreateSimioObjects(IDesignContext context, SdxDataSetContext SdxContext, SimioTransform transform, List<string> explanationList)
        {
            explanationList.Clear();

            string marker = "Begin";
            try
            {
                // Compute a size scale by comparing the Visio height with the Simio
                //double sizeScale = 1.0;

                var intelligentObjects = context.ActiveModel.Facility.IntelligentObjects;

                // reference our bounding box
                AABB box = SdxContext.BBox;

                Dictionary<string, FacilitySize> TypeNameDict = new Dictionary<string, FacilitySize>();

                foreach (SdxObject obj in SdxContext.ObjectDict.Values)
                {
                    try
                    {
                        marker = $"Object={obj}";

                        // The location transformation is done by scaling at the origin, and then applying the offset.
                        SdxVector v1 = obj.Center.Subtract(box.PtMin);

                        v1 = v1.ScaleBy(transform.DrawingScale);
                        v1 = v1.Add(transform.LocationTranslate);

                        FacilityLocation oLoc = new FacilityLocation(v1.X, v1.Y, v1.Z);

                        //**** Create the object in Simio ****
                        obj.SimioObject = intelligentObjects.CreateObject(obj.ClassName, oLoc) as IFixedObject;

                        obj.OriginalSize = obj.SimioObject.Size; // remember the original size.
                        obj.SimioObject.ObjectName = obj.Name;

                        string typeName = obj.SimioObject.TypeName;
                        if (!TypeNameDict.ContainsKey(typeName))
                            TypeNameDict.Add(typeName, obj.OriginalSize);

                        // Convert from original units to Simio facility units.
                        if (obj.Size.yWidth == 0)
                            obj.Size.yWidth = 0.5;

                        if ( obj.ObjectPropertyList.Any() )
                        {
                            foreach ( SdxObjectProperty sdxProp in obj.ObjectPropertyList )
                            {
                                string propname = "";
                                try
                                {
                                    string name = sdxProp.Name.ToLower();
                                    if (!name.StartsWith("simio_") && name != "simio_")
                                        continue;

                                    propname = name.Substring("simio_".Length);

                                    // Skip over reserved SDX properties
                                    if (propname == "objectclass")
                                        continue;

                                    if (propname == "baseclass")
                                        continue;

                                    if (propname == "name")
                                        continue;

                                    //**** Set the Simio Property ****
                                    IProperty sProp = obj.SimioObject.Properties.SingleOrDefault(rr => rr.Name.ToLower() == propname);
                                    if (sProp != null)
                                    {
                                        sProp.Value = sdxProp.PropertyValue;
                                    }
                                    else
                                    {
                                        explanationList.Add($"Object={obj.Name}: Cannot find or clear Property={sdxProp.Name}");
                                    }

                                }
                                catch (Exception ex)
                                {
                                    explanationList.Add($"Could not set Simio Property={propname}. Err={ex}");
                                }
                            } // For each property found
                        }

                        FacilitySize oSize = obj.SimioObject.Size;
                        oLoc = obj.SimioObject.Location;

                        AABB oBox = new AABB(oLoc, oSize);

                        double xScale = transform.ObjectScale.X, yScale = transform.ObjectScale.Y, zScale = transform.ObjectScale.Z;

                        if (transform.TransformOptions.HasFlag(EnumTransformOptions.AutoScaleObjects))
                        {
                            xScale = obj.OriginalSize.Length / obj.Size.xLength;
                            yScale = obj.OriginalSize.Width / obj.Size.yWidth;
                            zScale = obj.OriginalSize.Height / obj.Size.zHeight;
                        }

                        // Set the new facility size
                        double len = obj.Size.xLength * xScale;
                        double wid = obj.Size.yWidth * yScale;
                        double hgt = obj.Size.zHeight * zScale;
                        oSize = new FacilitySize(len, hgt, wid);   // x-length, y-width, z-height ???

                        obj.SimioObject.Size = oSize;

                        IFixedObject fixedObj = obj.SimioObject;
                        int nodeCount = 0;
                        foreach (INodeObject node in fixedObj.Nodes)
                        {
                            nodeCount++;

                            FacilitySize nSize = node.Size;
                            FacilityLocation nLoc = node.Location;
                            node.Location = nLoc;

                            AABB nBox = new AABB(nLoc, nSize);

                            // Simio nodes are either to the 'left' or 'right' of the object. Determine which...
                            double leftSpace = oBox.PtMin.X - nBox.PtMax.X;
                            double rightSpace = nBox.PtMin.X - oBox.PtMax.X;

                            if ( obj.BaseClass.ToLower() == "source")
                            {
                                // Source has one node to the right.
                                node.Location = new FacilityLocation(oLoc.X + oSize.Length / 2, oLoc.Y, oLoc.Z);

                            }
                            else if ( obj.BaseClass.ToLower() == "server")
                            {
                                // Servers have one node to the right and one to the left
                                if ( nLoc.X > oLoc.X)
                                {
                                    node.Location = new FacilityLocation(oLoc.X + oSize.Length / 2, oLoc.Y, oLoc.Z);
                                }
                                else
                                {
                                    node.Location = new FacilityLocation(oLoc.X - oSize.Length / 2, oLoc.Y, oLoc.Z);
                                }
                            }
                            else if (obj.BaseClass == "sink")
                            {
                                // Sinks have one node to the left
                                node.Location = new FacilityLocation(oLoc.X - oSize.Length / 2, oLoc.Y, oLoc.Z);
                            }
                            else 
                            {
                                // General case for left and right
                                if (nLoc.X > oLoc.X)
                                {
                                    node.Location = new FacilityLocation(oLoc.X + oSize.Length / 2, oLoc.Y, oLoc.Z);
                                }
                                else
                                {
                                    node.Location = new FacilityLocation(oLoc.X - oSize.Length / 2, oLoc.Y, oLoc.Z);
                                }

                            }

                            foreach (ILinkObject link in node.InboundLinks)
                            {

                            }

                            foreach (ILinkObject link in node.OutboundLinks)
                            {

                            }

                        }

                        FacilityLocation floc = fixedObj.Location;
                        FacilitySize fsize = fixedObj.Size;

                    }
                    catch (Exception ex)
                    {
                        explanationList.Add( $"Object={obj}: Marker={marker} Err={ex}");
                    }
                } // for

                return true;
            }
            catch (Exception ex)
            {
                explanationList.Add( $"Marker={marker} Err={ex}");
                return false;
            }

        }


        /// <summary>
        /// Create Simio Links from a Simio DataTable.
        /// It is assumed that the objects have already been built.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dtObj"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static bool CreateSimioLinks(IDesignContext context, SdxDataSetContext SdxContext, SimioTransform transform, List<string> explanationList)
        {
            explanationList.Clear();

            string marker = "Begin";
            try
            {
                var intelligentObjects = context.ActiveModel.Facility.IntelligentObjects;
                // reference our bounding box
                AABB box = SdxContext.BBox;

                foreach (SdxLink link in SdxContext.LinkDict.Values)
                {
                    try
                    {
                        marker = $"Link={link}";
                        if (link.FromNode == null || link.ToNode == null)
                        {
                            explanationList.Add($"Link={link.Name}:Cannot find FromNode or ToNode");
                            goto GetNextLink;
                        }

                        int fromIndex = 0;
                        switch (link.FromNode.BaseClass.ToLower())
                        {
                            case "source":
                                {
                                    fromIndex = link.FromIndex;
                                }
                                break;

                            case "server":
                                {
                                    fromIndex = link.FromIndex + 1;
                                }
                                break;

                            case "separator":
                                {
                                    fromIndex = link.FromIndex + 1;
                                }
                                break;

                            case "combiner":
                                {
                                    fromIndex = link.FromIndex + 2;
                                }
                                break;

                            case "basicnode":
                                fromIndex = link.FromIndex;
                                break;

                            default:
                                fromIndex = link.FromIndex;
                                break;

                        }

                        int toIndex = 0;
                        switch (link.ToNode.BaseClass.ToLower())
                        {
                            case "sink":
                                {
                                    toIndex = link.ToIndex;
                                }
                                break;

                            case "server":
                                {
                                    toIndex = link.ToIndex;
                                }
                                break;

                            case "separator":
                                {
                                    toIndex = link.ToIndex;
                                }
                                break;

                            case "combiner":
                                {
                                    toIndex = link.ToIndex;
                                }
                                break;

                            case "BasicNode":
                                toIndex = link.ToIndex;
                                break;

                            default:
                                toIndex = link.ToIndex;
                                break;

                        }

                        marker = $"Link={link.Name} Indices:From={fromIndex} To={toIndex}";
                        int fromCount = link.FromNode.SimioObject.Nodes.Count();
                        int toCount = link.ToNode.SimioObject.Nodes.Count();

                        if (fromIndex < 0 || fromIndex > fromCount - 1)
                        {
                            marker = $"FromIndex={fromIndex} invalid. (Node count={fromCount})";
                            goto GetNextLink;
                        }

                        if (toIndex < 0 || toIndex > toCount - 1)
                        {
                            marker = $"ToIndex={toIndex} invalid. (Node count={toCount})";
                            goto GetNextLink;
                        }

                        INodeObject startNode = link.FromNode.SimioObject.Nodes[fromIndex];
                        INodeObject endNode = link.ToNode.SimioObject.Nodes[toIndex];

                        FacilityLocation fromLoc = startNode.Location; // GetFacilityLoc(startNode, transform);

                        FacilityLocation toLoc = endNode.Location; // GetFacilityLoc(endNode, transform);

                        List<FacilityLocation> pathList = new List<FacilityLocation>();
                        pathList.Add(fromLoc);

                        foreach (SdxVector vertex in link.VertexList)
                        {
                            SdxVector v1 = vertex.Subtract(box.PtMin);
                            v1 = v1.ScaleBy(transform.DrawingScale);
                            v1 = v1.Add(transform.LocationTranslate);

                            FacilityLocation loc = new FacilityLocation(v1.X, v1.Y, v1.Z);
                            pathList.Add(loc);
                        }

                        pathList.Add(toLoc);

                        intelligentObjects.CreateLink(link.ClassName, startNode, endNode, pathList);

                        GetNextLink:;
                    }
                    catch (Exception ex)
                    {
                        explanationList.Add( $"Link: Marker={marker} Err={ex}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                explanationList.Add( $"Link: Marker={marker} Err={ex}");
                return false;
            }

        }
    }

}
