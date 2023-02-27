

using LoggertonHelpers;
using SdxHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SdxVisio
{
    /// <summary>
    /// The top layer for Visio has:
    /// Folders: _rels, masters, media, pages, theme
    /// Files: document.xml (StyleSheets, ...), windows.xml (stencils, etc)
    /// Under the folder 'pages' are the individual drawings (page) files: page1.xml, page2.xml, ...
    /// The page files contain Shapes and Connects.
    /// Under the Shapes XML are Sections (with Rows), Text, ...
    /// Sections have 'N' attributes such as User, Control, Connection, Paragraph, Character, ...
    /// The Visio coordinates have the origin at the lower left, so these will be 
    /// converted the Simio coordinate system with the origin at the upper left.
    /// Also, the Visio X,Y will be converted to the X,Z.
    /// As the time of this writing (Jul 2018) Visio is planned for 3D, but has not been released yet.
    /// </summary>
    public class SdxVisioApplication : SdxApplication
    {


        /// <summary>
        /// Dictionary to reference PageInfo using the key PageNumber (int) (*not* PageID)
        /// </summary>
        public Dictionary<int, PageInfo> PageDict { get; set; }

        /// <summary>
        /// Holds the referenced master templates information.
        /// These are the master{n}.xml files under the visio > masters folder.
        /// </summary>
        public Dictionary<int, MasterInfo> MasterTemplateDict { get; set; }

        /// <summary>
        /// Holds the master information.
        /// These are the master shapes from the visio masters.xml file.
        /// </summary>
        public Dictionary<int, MasterInfo> MasterSummaryDict { get; set; }

        /// <summary>
        /// Read the parts of a Package to get the information needed and produce
        /// a valid Simio Diagram DataSet so that Simio can use it to build Simio
        /// diagrams (e.g. Facility, Process Steps, ...).
        /// </summary>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool ReadPackage(Package package, List<string> explanationList)
        {
            string reason = "";
            explanationList.Clear();

            try
            {
                TableNameDict = new Dictionary<string, string>();
                MasterTemplateDict = new Dictionary<int, MasterInfo>();
                MasterSummaryDict = new Dictionary<int, MasterInfo>();
                PageDict = new Dictionary<int, PageInfo>();

                SdxDataSet = new DataSet();

                PackagePartCollection parts = package.GetParts();
                foreach (PackagePart part in parts)
                {
                    // We are only interested in XML parts
                    XDocument xmlPart = SdxApplication.GetXmlFromPart(part, out reason);
                    if (xmlPart == null)
                    {
                        //explanationList.Add(reason);
                        goto GetNextPart;
                    }

                    string uri = part.Uri.OriginalString.ToLower();

                    try
                    {
                        string masterPrefix = @"/visio/masters/master";
                        string pagePrefix = @"/visio/pages/page";
                        string suffix = ".xml";

                        // Parts beginning with 'page' which can also be pages.xml
                        // regardless of the internal page 'Name' the files appear as page1.xml, page2.xml, ...
                        if (uri.StartsWith(pagePrefix) && uri.EndsWith(suffix))
                        {
                            string pagePart = uri.Substring(pagePrefix.Length, uri.Length - (pagePrefix.Length + suffix.Length));

                            // The filename is either 'pages' or 'page{n}'
                            // We do not know the order that they arrive in, so the page
                            // objects must checked in the PageDict regardless.
                            if (pagePart == "s")
                            {
                                // Listing of the pages. Build our Page Dictionary
                                if (!ReadPagesPart(xmlPart, out reason))
                                {
                                    explanationList.Add($"Skipped Page={pagePart}. Reason={reason}");
                                    goto GetNextPart;
                                }
                            }
                            else // Not 'pages.xml', so it must be page{n}.xml, where n starts with 1.
                            {
                                int nn = 0;
                                if ( !int.TryParse(pagePart, out nn))
                                {
                                    explanationList.Add($"Cannot parse PageURI={uri}. Expected something like page1.xml");
                                    goto GetNextPart;
                                }

                                PageInfo page = null;
                                if (!PageDict.TryGetValue(nn, out page))
                                {
                                    page = new PageInfo(this, nn, "");
                                    page.XDoc = xmlPart;
                                    PageDict.Add(nn, page);
                                }

                                if (!ReadPagePart(page, xmlPart, out reason))
                                {
                                    explanationList.Add($"Skipped Page={page.PageNumber}. Reason={reason}");
                                    goto GetNextPart;
                                }

                            }

                        }
                        else if (uri.StartsWith(masterPrefix) && uri.EndsWith(suffix))
                        {
                            // Parts beginning with "Master"
                            string masterNumber = uri.Substring(masterPrefix.Length, uri.Length - (masterPrefix.Length + suffix.Length));

                            // If it is the masters file, the we need to run through them to get the reference to the master{n} file
                            if (masterNumber == "s")
                            {
                                if (!ReadMastersPart(xmlPart, out reason))
                                {
                                    explanationList.Add(reason);
                                    goto GetNextPart;
                                }
                            }
                            else // it must be a master{n} file, and hence a Master page template.
                            {
                                int nn = 0;

                                if (!int.TryParse(masterNumber, out nn))
                                {
                                    explanationList.Add( $"Malformed Page name={uri}");
                                    goto GetNextPart;
                                }

                                if (!ReadMasterPart(nn, xmlPart, out reason))
                                {
                                    explanationList.Add($"Skipped Master={nn}. Reason={reason}");
                                    goto GetNextPart;
                                }
                            }

                        }
                        else // Other components of the OPC package the we don't use right now.
                        {
                            switch (uri)
                            {
                                case @"/docprops/app.xml":
                                    break;
                                case @"/docprops/core.xml":
                                    break;
                                case @"/docprops/custom.xml":
                                    break;

                                case @"/visio/themes":
                                    break;
                                case @"/visio/document.xml":
                                    break;
                                case @"/visio/windows.xml":
                                    break;

                                case @"/xl/styles.xml":
                                    break;

                                case @"/xl/theme/theme1.xml":
                                    break;

                                default:
                                    break;

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        explanationList.Add( $"URI={uri} Err={ex}");
                    }

                    GetNextPart:;
                } // foreach part

                // Now everything is collected, so we can check the model, fix up the 
                // links and objects, and check for validation errors.

                if (FixupShapes(explanationList, includeOnlySimioClasses: false))
                    return true;
                else
                    return false;


            }
            catch (Exception ex)
            {
                reason = $"Err={ex}";
                explanationList.Add(reason);
                return false;
            }
        } // method

        /// <summary>
        /// Prompt for, and read a visio package file.
        /// A Visio app is created and returned.
        /// </summary>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool OpenAndReadPackage(string filepath, List<string> explanationList)
        {
            explanationList.Clear();

            try
            {
                Package package = null;
                this.FilePath = filepath;

                string explanation = "";
                using (package = SdxVisioApplication.OpenPackageFile(filepath, "Visio", System.IO.FileAccess.Read,out explanation))
                {
                    if (package == null)
                    {
                        explanationList.Add(explanation);
                        return false;
                    }

                    try
                    {
                        if (!this.ReadPackage(package, explanationList))
                            return false;
                    }
                    catch (Exception ex)
                    {
                        explanationList.Add($"File={filepath} Exception={ex}");
                        return false;
                    }
                    finally // gotta close it regardless
                    {
                        package.Close();
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                explanationList.Add( $"Exception={ex}");
                return false;
            }

        }



        /// <summary>
        /// Create a simio dataset.
        /// To do this, a valid PageDict key (pageNumber) must be supplied.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public override DataSet CreateSdxDataSet(string name, string sourcePath, int? pageNumber, out string explanation)
        {
            explanation = "";

            if ( pageNumber == null )
            {
                explanation = "PageNumber cannot be null";
                return null;
            }

            DataSet ds = new DataSet(name);

            try
            {
                PageInfo page = null;
                if ( !PageDict.TryGetValue(pageNumber.Value, out page) )
                {
                    explanation = $"Cannot find Page with PageNumber={pageNumber}";
                    return null;
                }

                ds = BuildDataSet(page, sourcePath, out explanation);

                return ds;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return null;
            }
        }

        /// <summary>
        /// Build a Simio Diagram DataSet for a given Visio page.
        /// </summary>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public DataSet BuildDataSet(PageInfo page, string filepath, out string explanation)
        {
            explanation = "";
            string marker = $"Begin. Page={page.PageName}";

            if (IsObjectNull(page, "Null Page", out explanation))
                return null;

            try
            {

                DataSet ds = new DataSet("TestFacility");
                if (!BuildDataSetTables(ds, out explanation))
                    return null;

                if ( !AddPropertyRow(ds.Tables["Properties"], filepath, out explanation))
                {
                    explanation = $"Cannot build PropertyRow. Reason={explanation}";
                    return null;
                }

                // Now get all the simio shapes
                List<ShapeInfo> shapeList = new List<ShapeInfo>();

                foreach (ObjectInfo oi in page.ObjectDict.Values)
                {
                    if (!AddObjectRow(ds, oi, out explanation))
                        logit($"Err={explanation}");
                }

                foreach (LinkInfo li in page.LinkDict.Values)
                {
                    if (!AddLinkRow(ds, li, out explanation))
                    {
                        logit($"Err={explanation}");
                        goto GetNextLink;
                    }

                    int nn = 0;
                    foreach (VisioVertex vv in li.MyShape.VertexGeometry.VList)
                    {
                        string key = (++nn).ToString();
                        if (!AddVertexRow(ds.Tables["Vertices"], key, li.MyShape.ShapeName, vv, out explanation))
                        {
                            logit($"Link={li.MyShape.ShapeName} Vertex");
                            goto GetNextLink;
                        }
                    }

                    GetNextLink:;
                }

                foreach (ArtifactInfo ai in page.ArtifactDict.Values)
                {
                    if (!AddArtifactRow(ds.Tables["Artifacts"], ai, out explanation))
                        logit($"Err={explanation}");
                }


                return ds;

            }
            catch (Exception ex)
            {
                explanation = $"Marker={marker} Err={ex}";
                return null;
            }

        }

        /// <summary>
        /// Use ShapeData to add a row to the Object table.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="oi"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool AddObjectRow(DataSet ds, ObjectInfo oi, out string explanation)
        {
            explanation = "";

            if (IsObjectNull(ds, "DataSet", out explanation))
                return false;

            DataTable dt = ds.Tables["Objects"];
            if (IsObjectNull(oi, "Object", out explanation))
                return false;

            if (dt == null)
            {
                explanation = "DataTable Objects is null.";
                return false;
            }

            try
            {
                DataRow dr = dt.Rows.Add();
                dr["ObjectClass"] = oi.MyShape.SimioClass;
                dr["BaseClass"] = oi.MyShape.SimioBaseClass;

                // The shape names must be unique
                dr["Name"] = oi.MyShape.Name;

                // Adjust dimensions
                double xx = oi.MyShape.Center.X, yy = oi.MyShape.Center.Y, zz=oi.MyShape.Center.Z;

                yy = -yy;

                double aaa = yy;
                yy = zz;
                zz = aaa;

                dr["X"] = xx; 
                dr["Y"] = yy;
                dr["Z"] = zz;

                if ( oi.MyShape.SimioClass.ToLower().Contains("separator"))
                {
                    bool isDebug = true;
                }

                double ParamMinDimension = 0.05;
                // Make sure each object has some presence
                double ww = Math.Max(oi.MyShape.Size.XWidth, ParamMinDimension);
                double hh = Math.Max(oi.MyShape.Size.YHeight, ParamMinDimension);
                double ll = Math.Max(oi.MyShape.Size.ZDepth, ParamMinDimension);


                dr["Length"] = ww; // Simio X
                dr["Width"] = ll; // Simio Y
                dr["Height"] = hh; // Simio Z

                // Not using this right now.
                dr["EntityType"] = "";
                dr["InitialNumberInSystem"] = "";
                dr["RideOnTransporter"] = "";
                dr["TransporterName"] = "";

                foreach (KeyValuePair<string,string> kvp in oi.PropertyDict )
                {
                    if (!AddObjectPropertyRow(ds, oi.Name, kvp, out explanation))
                        logit($"Err={explanation}");
                }

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"ObjectShapeData={oi.MyShape.ShapeName} Err={ex}";
                return false;
            }
        } // method

        /// <summary>
        /// Use KeyValue pair to add a row to the ObjectProperties table.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="objectName"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool AddObjectPropertyRow(DataSet ds, string objectName, KeyValuePair<string, string> kvp, out string explanation)
        {
            explanation = "";

            if (IsObjectNull(ds, "DataSet", out explanation))
                return false;

            if (IsObjectNull(kvp, "ObjectProperty KVP", out explanation))
                return false;

            DataTable dt = ds.Tables["ObjectProperties"];
            if (dt == null)
            {
                explanation = "DataTable ObjectProperties is null.";
                return false;
            }

            try
            {
                DataRow dr = dt.Rows.Add();
                dr["ObjectName"] = objectName;
                dr["Name"] = kvp.Key;
                dr["Value"] = kvp.Value;

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"ObjectShapeData={objectName} Err={ex}";
                return false;
            }
        } // method
        /// <summary>
        /// Use KeyValue pair to add a row to the ObjectProperties table.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="objectName"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool AddLinkPropertyRow(DataSet ds, string linkName, KeyValuePair<string, string> kvp, out string explanation)
        {
            explanation = "";

            if (IsObjectNull(ds, "DataSet", out explanation))
                return false;

            if (IsObjectNull(kvp, "LinkProperty KVP", out explanation))
                return false;

            DataTable dt = ds.Tables["LinkProperties"];
            if (dt == null)
            {
                explanation = "DataTable LinkProperties is null.";
                return false;
            }

            try
            {
                DataRow dr = dt.Rows.Add();
                dr["LinkName"] = linkName;
                dr["Name"] = kvp.Key;
                dr["Value"] = kvp.Value;

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"ObjectShapeData={linkName} Err={ex}";
                return false;
            }
        } // method

        /// <summary>
        /// Use ShapeData to add a row to the Artifact table.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="si"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool AddArtifactRow(DataTable dt, ArtifactInfo ai, out string explanation)
        {
            explanation = "";

            if (IsObjectNull(dt, "DataTable", out explanation))
                return false;

            if (IsObjectNull(ai, "ArtifactInfo", out explanation))
                return false;

            if (dt == null)
            {
                explanation = "DataTable is null.";
                return false;
            }

            try
            {
                DataRow dr = dt.Rows.Add();
                dr["Name"] = ai.MyShape.ShapeName;

                // Adjust dimensions
                double xx = ai.MyShape.Center.X, yy = ai.MyShape.Center.Y, zz = ai.MyShape.Center.Z;

                yy = -yy;

                double aaa = yy;
                yy = zz;
                zz = aaa;

                dr["X"] = xx;
                dr["Y"] = yy;
                dr["Z"] = zz;


                double ParamMinDimension = 0.05;
                // Make sure each object has some presence
                double ww = Math.Max(ai.MyShape.Size.XWidth, ParamMinDimension);
                double hh = Math.Max(ai.MyShape.Size.YHeight, ParamMinDimension);
                double ll = Math.Max(ai.MyShape.Size.ZDepth, ParamMinDimension);

                dr["Length"] = ww;
                dr["Width"] = ll;
                dr["Height"] = hh;

                // Debug
                if (ai.ForeignData != null || ai.TextBlock != null)
                {
                    string xxx = "";
                }

                // Additional data
                dr["ForeignData"] = ai.ForeignData;
                dr["TextData"] = ai.TextBlock;

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"ObjectShapeData={ai.MyShape.ShapeName} Err={ex}";
                return false;
            }
        } // method

        public bool AddPropertyRow(DataTable dt, string filepath, out string explanation)
        {
            explanation = "";
            try
            {
                dt.Rows.Clear();

                Dictionary<string, SdxProperty> propDict = new Dictionary<string, SdxProperty>();
                SdxDataSetContext.CreateBasicProperties(propDict, "Facility", "Visio", filepath);
                DataRow dr = SdxDataSetContext.CreatePropertyRow(dt, propDict);
                dt.Rows.Add(dr);

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Writing PropertyRow. Err={ex}";
                return false;
            }
        }
        /// <summary>
        /// Use ShapeData to add a row to the Link table.
        /// The Shape must be a Link (path, etc).
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="si"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool AddLinkRow(DataSet ds, LinkInfo li, out string explanation)
        {
            explanation = "";

            if (IsObjectNull(ds, "DataSet", out explanation))
                return false;

            if (IsObjectNull(li, "LinkInfo", out explanation))
                return false;

            DataTable dt = ds.Tables["Links"];
            if (dt == null)
            {
                explanation = "DataTable is null.";
                return false;
            }

            try
            {
                DataRow dr = dt.Rows.Add();
                dr["LinkClass"] = li.MyShape.SimioClass;
                dr["BaseClass"] = li.MyShape.SimioBaseClass;
                dr["Name"] = li.MyShape.ShapeName;
                dr["FromNode"] = li.MyShape.FromShape?.Name;
                dr["FromIndex"] = li.FromIndex;
                dr["ToNode"] = li.MyShape.ToShape?.Name;
                dr["ToIndex"] = li.ToIndex;
                dr["Network"] = "";
                dr["Length"] = li.MyShape.Size.XWidth;
                dr["Width"] = li.MyShape.Size.ZDepth;
                dr["Height"] = li.MyShape.Size.YHeight;

                // Not using this now.
                dr["Type"] = "Bidirectional"; // ??

                foreach ( KeyValuePair<string,string> kvp in li.PropertyDict)
                {
                    if (!AddLinkPropertyRow(ds, li.MyShape.ShapeName, kvp, out explanation))
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                explanation = $"LinkInfo. Name={li.MyShape.ShapeName} Err={ex}";
                return false;
            }
        } // method


        /// <summary>
        /// Use ShapeData to add a row to the Link table.
        /// The Shape must be a connector.
        /// The coordinates are changed from Visio to Simio, so Y and Z are swapped,
        /// and the new 'Z' is negated.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="lsi"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool AddVertexRow(DataTable dt, string name, string linkName, VisioVertex vertex, out string explanation)
        {
            explanation = "";

            if (IsObjectNull(dt, "DataTable", out explanation))
                return false;

            if (IsObjectNull(vertex, "vertex", out explanation))
                return false;

            if (dt == null)
            {
                explanation = "DataTable is null.";
                return false;
            }

            try
            {
                DataRow dr = dt.Rows.Add();

                // Adjustments to dimensions

                dr["Name"] = name;
                dr["LinkName"] = linkName;
                dr["X"] = vertex.X;
                dr["Y"] = vertex.Z;   // Y and Z are swapped...
                dr["Z"] = -vertex.Y;  // and the new 'Z' is negated

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Link Name={Name} Parent={linkName} Err={ex}";
                return false;
            }
        } // method


        /// <summary>
        /// Read the masters.xml file and
        /// </summary>
        /// <param name="filename"></param>
        public bool ReadMastersPart(XDocument xDoc, out string explanation)
        {
            explanation = "";

            string marker = "Begin";
            try
            {
                // Process the masters file and store select data from the Master XElement
                // in a dictionary.
                List<XElement> masterList = xDoc.Descendants()
                    .Where(rr => rr.Name.LocalName == "Master")
                    .ToList();

                MasterSummaryDict.Clear();

                int count = 0;
                foreach (XElement xMaster in masterList)
                {
                    int id = 0;
                    string name = "";

                    try
                    {
                        MasterInfo mi = new MasterInfo(null);
                        id = VisioHelpers.GetAttrAsInt32(xMaster, "ID", 0);
                        name = VisioHelpers.GetAttrAsString(xMaster, "Name", "");
                        count += 1;
                        marker = $"Master ID={id} name={name}";

                        mi.Id = id;
                        mi.Name = name;

                        mi.XMaster = xMaster;

                        XElement xRel = xMaster.Elements().SingleOrDefault(rr => rr.Name.LocalName == "Rel");
                        if (xRel != null)
                        {
                            XAttribute attr = xRel.Attributes().SingleOrDefault(rr => rr.Name.LocalName == "id");
                            if (attr != null)
                            {
                                string refId = attr.Value;
                                if (refId != null && refId.StartsWith("rId"))
                                {
                                    mi.TemplateId = refId.Substring("rId".Length);
                                }
                            }
                        }

                        if (!MasterSummaryDict.ContainsKey(mi.Id))
                        {
                            MasterSummaryDict.Add(mi.Id, mi);
                        }

                    }
                    catch (Exception ex)
                    {
                        explanation = $"Masters: Count={count} ID={id} Name={name} Err={ex}";
                        return false;
                    }
                } // for each master shape
                return true;

            }
            catch (Exception ex)
            {
                explanation = $"Masters. Marker={marker} Err={ex}";
                return false;
            }
        }


        /// <summary>
        /// Read a master{id}.xml file and create a new MasterData for the dictionary.
        /// </summary>
        /// <param name="filename"></param>
        public bool ReadMasterPart(int id, XDocument xDoc, out string explanation)
        {
            explanation = "";

            string marker = "Begin";
            try
            {
                marker = $"Looking for Shapes";
                List<XElement> shapesList = xDoc.Descendants()
                    .Where(rr => rr.Name.LocalName == "Shapes")
                    .ToList();

                if (!shapesList.Any())
                {
                    explanation = $"Master={id} had no 'Shapes' element.";
                    return false;
                }

                marker = $"Looking for Shape";
                XElement xeShapes = shapesList.First();
                // Get the Topmost Shape
                XElement xeShape = xeShapes.Elements()
                    .Where(rr => rr.Name.LocalName == "Shape")
                    .First();

                MasterInfo mi = new MasterInfo(xeShape);

                mi.Size = new VisioSize(0,0,0);
                mi.Size.XWidth = VisioHelpers.GetCellValueAsDouble(xeShape, "Width", 0.0);
                mi.Size.YHeight = VisioHelpers.GetCellValueAsDouble(xeShape, "Height", 0.0);

                // Does the master have simio properties and class information?
                Dictionary<string, string> propDict = null;
                if ( VisioHelpers.FindSimioProperties(xeShape, out propDict, out explanation))
                {
                    mi.PropertyDict = propDict;
                }

                string simioClass = "";
                string simioClassType = "";
                string simioBaseClass = "";
                if (VisioHelpers.FindSimioClass(propDict, out simioClassType, out simioClass, out simioBaseClass, out explanation))
                {
                    mi.SimioClassType = simioClassType;
                    mi.SimioClass = simioClass;
                    mi.SimioBaseClass = simioBaseClass;
                }

                int internalId = VisioHelpers.GetAttrAsInt32(xeShape, "ID", id);

                mi.Id = id; // Convert.ToInt32(shape.Attribute("ID")?.Value);
                mi.Name = VisioHelpers.GetAttrAsString(xeShape, "Name", "");
                marker = $"Building Master. ID={id}";

                if (!MasterTemplateDict.ContainsKey(id))
                {
                    MasterTemplateDict.Add(id, mi);
                }
                else
                {
                    explanation = $"Duplicate Master with ID={id}";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Marker={marker}. Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Read the page{n}.xml file, which consists mainly of 
        /// Shapes and Connect Elements.
        /// Info from the outermost Shapes are put into ShapeInfo objects
        /// and place in the pageInfo's ShapeDict dictionary. 
        /// </summary>
        /// <param name="filename"></param>
        public bool ReadPagePart(PageInfo pageInfo, XDocument xDoc, out string explanation)
        {
            explanation = "";

            string marker = "Begin";
            try
            {

                List<XElement> shapeList = xDoc.Descendants()
                    .Where(rr => rr.Name.LocalName == "Shapes")
                    .ToList();
                if (shapeList.Any())
                {
                    XElement xShapes = shapeList.First();

                    // Get the outermost Shape elements.
                    List<XElement> xeShapesList = xShapes.Elements()
                        .Where(rr => rr.Name.LocalName == "Shape")
                        .ToList();

                    pageInfo.ShapeDict = new Dictionary<int, ShapeInfo>();
                    int shapeNbr = 0;
                    foreach (XElement xeShape in xeShapesList)
                    {
                        shapeNbr++;

                        ShapeInfo si = new ShapeInfo(xeShape);
                        si.Id = VisioHelpers.GetAttrAsInt32(xeShape, "ID", -1);
                        if ( si.Id == -1 )
                        {
                            explanation = $"Invalid ID at Shape#={shapeNbr}";
                            goto GetNextShape;
                        }

                        string shapeName = VisioHelpers.GetAttrAsString(xeShape, "Name", "");
                        // If there is no name, then give a temporary "???" + guid
                        if (shapeName == "")
                        {
                            shapeName = $"Shape-{si.Id}";
                        }
                        marker = $"Shape={shapeName}";

                        string shapeType = VisioHelpers.GetAttrAsString(xeShape, "Type", "");
                        if ( shapeType != "Shape" && shapeType != "Group" && shapeType != "Foreign" )
                        {
                            logit($"Ignoring Shape={shapeName} with Type={shapeType}");
                            goto GetNextShape;
                        }

                        if ( shapeName.ToLower().Contains("callout") )
                        {
                            bool isDebug = true;
                        }
                        int masterId = VisioHelpers.GetAttrAsInt32(xeShape, "Master", 0);

                        si.ShapeType = shapeType;
                        si.ShapeName = shapeName;
                        si.MasterId = masterId;

                        // The 'pin' is the centerpoint of the shape
                        si.Center.X = VisioHelpers.GetCellValueAsDouble(xeShape, "PinX", 0.0);
                        si.Center.Y = VisioHelpers.GetCellValueAsDouble(xeShape, "PinY", 0.0);
                        si.Center.Z = VisioHelpers.GetCellValueAsDouble(xeShape, "PinZ", 0.0);

                        // Connectors have Begin and End
                        si.BeginLocation.X = VisioHelpers.GetCellValueAsDouble(xeShape, "BeginX", 0.0);
                        si.BeginLocation.Y = VisioHelpers.GetCellValueAsDouble(xeShape, "BeginY", 0.0);

                        si.EndLocation.X = VisioHelpers.GetCellValueAsDouble(xeShape, "EndX", 0.0);
                        si.EndLocation.Y = VisioHelpers.GetCellValueAsDouble(xeShape, "EndY", 0.0);

                        si.Size = new VisioSize(0,0,0);
                        si.Size.XWidth = VisioHelpers.GetCellValueAsDouble(xeShape, "Width", double.NaN);
                        si.Size.YHeight = VisioHelpers.GetCellValueAsDouble(xeShape, "Height", double.NaN);
                        si.Size.ZDepth = VisioHelpers.GetCellValueAsDouble(xeShape, "Length", 0.0);

                        si.XShape = xeShape;

                        if ( pageInfo.ShapeDict.ContainsKey(si.Id))
                        {
                            explanation = $"Shape duplicate key: ID={si.Id}";
                            goto GetNextShape;
                        }

                        // Get the properties
                        Dictionary<string, string> propDict = null;
                        if ( VisioHelpers.FindSimioProperties(xeShape, out propDict, out explanation))
                        {
                            si.PropertyDict = propDict;
                        }

                        pageInfo.ShapeDict.Add(si.Id, si);

                        GetNextShape:;
                    } // foreach shape
                }
                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Marker={marker}. Err={ex}";
                return false;
            }
        }
        /// <summary>
        /// Read the pages (summary) file, and create a list of pages.
        /// </summary>
        /// <param name="filename"></param>
        public bool ReadPagesPart(XDocument xDoc, out string explanation)
        {
            explanation = "";
            string marker = "Begin";
            try
            {
                List<XElement> pageList = xDoc.Descendants()
                    .Where(rr => rr.Name.LocalName == "Pages").ToList();
                if (!pageList.Any())
                {
                    explanation = $"Pages file had no Pages.";
                    return false;
                }

                XElement xPages = pageList.First();

                // Get the outermost Page elements.
                List<XElement> pages = xPages.Elements()
                    .Where(rr => rr.Name.LocalName == "Page")
                    .ToList();

                // Each page will have a "Rel" element at the end (e.g. {Rel r:id='rId3'}
                // and this will match it to the Page{n} file (e.g. Page3.xml)
                int pageNumber = 1;
                foreach (XElement xPage in pages)
                {
                    string name = VisioHelpers.GetAttrAsString(xPage, "Name", "");
                    marker = $"Page={name}";

                    // We expect that the PageInfo was already read in,
                    // but we'll log a message and create a new one if it isn't
                    PageInfo pi = null;
                    if ( !PageDict.TryGetValue(pageNumber, out pi))
                    {
                        pi = new PageInfo(this, pageNumber, name);
                        PageDict.Add(pageNumber, pi);
                    }

                    pi.PageName = name;

                    // Note: The id does *not* match it to the page{n}.xml file. See the Rel element instead.
                    int id = VisioHelpers.GetAttrAsInt32(xPage, "ID", -1);
                    pi.Id = id;

                    XElement xRel = xPage.Elements()
                        .SingleOrDefault(rr => rr.Name.LocalName == "Rel");
                    if ( xRel != null )
                    {
                        XAttribute attr = xRel.Attributes().SingleOrDefault(rr => rr.Name.LocalName == "id");
                        if (attr != null)
                        {
                            string digits = attr.Value.Substring("rId".Length);
                            pi.PageNumber = int.Parse(digits);
                        }

                    }

                    // Get the child PageSheet to pick up other data.
                    XElement pageSheet = xPage.Elements()
                        .SingleOrDefault(rr => rr.Name.LocalName == "PageSheet");

                    if ( pageSheet != null )
                    {
                        double pageWidth = VisioHelpers.GetCellValueAsDouble(pageSheet, "PageWidth", 8.5);
                        double pageHeight = VisioHelpers.GetCellValueAsDouble(pageSheet, "PageHeight", 11.0);
                        double pageLength = 0;

                        pi.PageSize = new VisioSize(pageWidth, pageHeight, pageLength);
                    }

                    pageNumber++;
                }

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Marker={marker}. Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Use the master and master template to fixup the shapes, connectors, ....
        /// Information that might be missing from the shape are sizes,
        /// in which case, we'll need to go to its Master.
        /// Even if succesful, the explanationList may contain warnings.
        /// </summary>
        /// <param name="explanation"></param>
        /// <returns></returns>
        private bool FixupShapes(List<string> explanationList, bool includeOnlySimioClasses)
        {
            string marker = "Begin";
            int count = 0;
            string explanation = "";

            try
            {

                foreach (PageInfo pi in PageDict.Values)
                {
                    marker = $"Page={pi.PageName}";
                    pi.ObjectDict = new Dictionary<int, ObjectInfo>();
                    logit($"Reading Page={pi.PageName} Number={pi.PageNumber} with {pi.ShapeDict.Values.Count} Shapes.");

                    Dictionary<string, string> objectNameDict = new Dictionary<string, string>();

                    // Take a look at each shape. 
                    foreach (ShapeInfo si in pi.ShapeDict.Values)
                    {
                        count++;

                        if (count == 23)
                            count = count;

                        marker = $"Page={pi.PageName} Shape={si.ShapeName} Count={count}";

                        try
                        {
                            // Locate the Shape's Master Summary (from masters.xml)
                            MasterInfo mi = null;
                            if (MasterSummaryDict.TryGetValue(si.MasterId, out mi))
                            {
                                // And then use the Master ID to get the Template (e.g. master05.xml)
                                MasterInfo mTemplate = null;
                                int tid = Convert.ToInt32(mi.TemplateId);
                                if (MasterTemplateDict.TryGetValue(tid, out mTemplate))
                                {
                                    // Look for missing info that we can get from the Master
                                    if (string.IsNullOrEmpty(si.ShapeName))
                                    {
                                        si.ShapeName = mTemplate.Name;
                                    }

                                    // Combine the properties from shape and its template, giving precedence to the shape (over the master)
                                    Dictionary<string, string> propDict = new Dictionary<string, string>();
                                    foreach (KeyValuePair<string, string> kvp in si.PropertyDict)
                                    {
                                        propDict.Add(kvp.Key, kvp.Value);
                                    }
                                    foreach (KeyValuePair<string, string> kvp in mTemplate.PropertyDict)
                                    {
                                        if ( !propDict.ContainsKey(kvp.Key) )
                                        {
                                            propDict.Add(kvp.Key, kvp.Value);
                                        }
                                    }

                                    // Set the combined properties.
                                    si.PropertyDict = propDict;


                                    // For Simio objects, we really need a SimioClassType (Object, Link,...) and SimioClass (Server, Path, ...)
                                    // So, if the Shape doesn't have it, then its master should
                                    string simioClassType = string.Empty;
                                    string simioClass = string.Empty;
                                    string simioBaseClass = string.Empty;

                                    if (VisioHelpers.FindSimioClass(si.PropertyDict, out simioClassType, out simioClass, out simioBaseClass, out explanation))
                                    {
                                        si.SimioClassType = simioClassType;
                                        si.SimioClass = simioClass;
                                        si.SimioBaseClass = simioBaseClass;
                                    }

                                    if (si.SimioClass == null && includeOnlySimioClasses)
                                    {
                                        explanationList.Add($"Shape={si.ShapeName} did not have a SimioClass");
                                        goto GetNextShape;
                                    }

                                    if ( !ValidateSize(si, mTemplate, out explanation))
                                    {
                                        explanationList.Add($"Shape ID={si.Id} Name={si.ShapeName} has no valid size={explanation}");
                                        goto GetNextShape;
                                    }

                                    if (string.IsNullOrEmpty(si.SimioClassType) && includeOnlySimioClasses)
                                    {
                                        si.SimioClassType = "??";
                                        explanationList.Add($"Shape ID={si.Id} Name={si.ShapeName} has no ClassType");
                                        goto GetNextShape;
                                    }

                                    // Fix the name if it was missing.
                                    if (si.ShapeName.StartsWith("???"))
                                        si.ShapeName = $"Simio.{si.SimioClass}.{si.Id}";

                                    // Links are also Shapes, with cells with 'N'=Begin(X/Y) and End(X/Y) and 'V' as double
                                    // They will also have a Geometry Section directly below the Shape
                                    if (si.SimioClassType == "Link")
                                    {
                                        VisioVertex vBegin = null;
                                        VisioVertex vEnd = null;

                                        try
                                        {
                                            if (VisioHelpers.FindLinkLocations(si.XShape, out vBegin, out vEnd, out explanation))
                                            {
                                                XElement xGeometry = si.XShape.Elements()
                                                    .Where(rr => rr.Name.LocalName == "Section" && rr.Attribute("N")?.Value == "Geometry")
                                                    .FirstOrDefault();
                                                if (xGeometry != null)
                                                {
                                                    si.VertexGeometry = new Geometry(xGeometry, vBegin, vEnd);
                                                }
                                            }
                                            else
                                                explanationList.Add($"FindLinkLocations. Err={explanation}");
                                        }
                                        catch (Exception ex)
                                        {
                                            explanationList.Add($"Cannot get Vertex info for Shape={si.ShapeName}. Err={ex}");
                                        }

                                    }
                                    else if ( si.SimioClassType == "Object" ) // ClassType Object
                                    {
                                        ObjectInfo oi = new ObjectInfo(SimProps, si.Id, si);
                                        oi.PropertyDict = si.PropertyDict;

                                        pi.ObjectDict.Add(oi.ObjectId, oi);

                                        si.SubShapeList = new List<ShapeInfo>();

                                        // See if the shape has text, meaning the user has renamed it.
                                        string nameText = FindText(si.XShape, 1, false);
                                        if (!string.IsNullOrEmpty(nameText))
                                            si.UserName = nameText;

                                        // Set the Username if we don't already have one and simio_name properties is found
                                        if (string.IsNullOrEmpty(si.UserName))
                                        {
                                            string username = "";
                                            if (propDict.TryGetValue("simio_name", out username))
                                            {
                                                si.UserName = username;
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(si.UserName))
                                            si.UserName = BuildUniqueUsername(objectNameDict, si.UserName);

                                        // Finally, add the name
                                        objectNameDict.Add(oi.Name, oi.Name);


                                        // Get Sub-Shapes. We only get ID and MasterShape for these.
                                        // We need them because the Connector Shape often references these sub-shapes.
                                        XElement xeShapes = si.XShape.Elements().SingleOrDefault(rr => rr.Name.LocalName == "Shapes");
                                        if (xeShapes != null)
                                        {
                                            List<XElement> subShapeList = xeShapes.Descendants()
                                                .Where(rr => rr.Name.LocalName == "Shape")
                                                .ToList();
                                            foreach (XElement xeSub in subShapeList)
                                            {
                                                ShapeInfo siSub = new ShapeInfo(null);
                                                siSub.Id = int.Parse(xeSub.Attribute("ID")?.Value);
                                                siSub.MasterId = int.Parse(xeSub.Attribute("MasterShape")?.Value);
                                                si.SubShapeList.Add(siSub);
                                            }
                                        }

                                    }
                                    else // Classify it as an artifact and add it.
                                    {
                                        ArtifactInfo ai = new ArtifactInfo(si.Id, si);
                                        pi.ArtifactDict.Add(ai.ArtifactId, ai);

                                        if ( ai.MyShape.ShapeType == "Shape")
                                        {
                                            ai.TextBlock = FindText(ai.XShape, 10, true);

                                        }
                                        else if ( ai.MyShape.ShapeType == "Foreign")
                                        {
                                            List<XElement> dataList = ai.XShape.Descendants()
                                                .Where(rr => rr.Name.LocalName == "ForeignData")
                                                .ToList();
                                            if (dataList.Any())
                                            {
                                                StringBuilder sb = new StringBuilder();
                                                foreach (XElement xe in dataList)
                                                {
                                                    string attr = VisioHelpers.GetAttrAsString(xe, "ForeignType", "");
                                                    sb.Append(attr);
                                                }
                                                ai.ForeignData = sb.ToString().Trim();
                                            }

                                        }
                                        else if ( ai.MyShape.ShapeType == "Group")
                                        {
                                            ai.TextBlock = FindText(ai.XShape, 10, true);
                                        }
                                        else
                                        {
                                            string xx = "";
                                        }

                                        goto GetNextShape;
                                    }
                                }
                                else
                                {
                                    explanationList.Add($"Marker={marker}. Shape {si.Id} Master={mi.Id} Could not find TemplateMasterId={tid}");
                                    goto GetNextShape;
                                }

                            }
                            else
                            {
                                explanationList.Add($"Marker={marker}.Shape {si.Id} Count not find MasterId={si.MasterId}");
                                goto GetNextShape;
                            }

                        }
                        catch (Exception ex)
                        {
                            explanationList.Add( $"Marker={marker}. FixupShapes: Count={count} Err={ex}");
                            goto GetNextShape;
                        }

                        GetNextShape:;
                    } // for each shape info

                    // Do the connects
                    // Find the Connect elements, which are in the 'Connects' section at the end of the page.
                    // The connection Shape will have 2 Connect elements, one for Begin and one for End, and
                    // each of those 'Connect' elements should reference a Shape that it being connected to/from.
                    // For example. If Shape 30 is a path connecting a Dog (Shape 9) to a Cat (Shape 19), then the connects are:
                    // Connect FromSheet=30 FromCell=BeginX ToSheet=9 ToCell=blah
                    // Connect FromSheet=30 FromCell=EndX   ToSheet=19 ToCell=blah
                    marker = "Connects";
                    if (pi.XDoc != null)
                    {
                        List<XElement> connectList = pi.XDoc.Descendants()
                            .Where(rr => rr.Name.LocalName == "Connects")
                            .ToList();

                        if (connectList.Any())
                        {
                            marker = $"Connects (found {connectList.Count} connects.";
                            XElement xConnects = connectList.First();

                            // Get the outermost Shape elements.
                            List<XElement> connects = xConnects.Elements()
                                .Where(rr => rr.Name.LocalName == "Connect")
                                .ToList();

                            List<ShapeInfo> removedLinks = new List<ShapeInfo>();

                            pi.ConnectDict = new Dictionary<string, ConnectInfo>();
                            pi.LinkDict = new Dictionary<int, LinkInfo>();

                            int connectCount = 0;
                            foreach (XElement xConnect in connects)
                            {
                                connectCount++;
                                marker = $"Processing Connect {connectCount} of {connectList.Count}.";
                                try
                                {

                                    ConnectInfo ci = new ConnectInfo(xConnect);
                                    string fromId = VisioHelpers.GetAttrAsString(xConnect, "FromSheet", "?");
                                    ci.FromShapeId = VisioHelpers.GetAttrAsInt32(xConnect, "FromSheet", 0);
                                    ci.FromCellName = VisioHelpers.GetAttrAsString(xConnect, "FromCell", "");

                                    string toId = VisioHelpers.GetAttrAsString(xConnect, "ToSheet", "?");
                                    ci.ToShapeId = VisioHelpers.GetAttrAsInt32(xConnect, "ToSheet", 0);
                                    ci.ToCellName = VisioHelpers.GetAttrAsString(xConnect, "ToCell", "");

                                    ShapeInfo siFrom = FindObjectShape(pi, ci.FromShapeId);
                                    ShapeInfo siTo = FindObjectShape(pi, ci.ToShapeId);

                                    if ( !ValidateConnectionShape(siFrom, out explanation))
                                    {
                                        explanationList.Add($"Connection Validation failed={explanation}");
                                        goto GetNextConnect;
                                    }

                                    if (!ValidateConnectionShape(siTo, out explanation))
                                    {
                                        logit($"Connection Validation failed={explanation}");
                                        goto GetNextConnect;
                                    }

                                    // Enough validation to add the connection to the dictionary
                                    string key = $"{fromId}-{toId}";
                                    pi.ConnectDict.Add(key, ci);

                                    // Both Links and Objects can have 'from' and 'to'
                                    LinkInfo li = null;

                                    // If the 'from' side is 'link', then the 'object' must be the 'to' side
                                    if (siFrom.SimioClassType == "Link")
                                    {

                                        if ( !pi.LinkDict.TryGetValue(siFrom.Id, out li))
                                        {
                                            li = new LinkInfo(this.SimProps, siFrom.Id, siFrom);
                                            pi.LinkDict.Add(li.LinkId, li);
                                        }

                                        if (ci.FromCellName == "EndX") // This is the end of the link
                                        {
                                            ShapeInfo endObject = siTo as ShapeInfo;
                                            siFrom.ToShape = endObject;
                                            li.ToObjectId = endObject.Id;
                                            li.ToObject = endObject;
                                        }
                                        else if (ci.FromCellName == "BeginX")
                                        {
                                            ShapeInfo beginObject= siTo as ShapeInfo;
                                            siFrom.FromShape = beginObject;
                                            li.FromObjectId = beginObject.Id;
                                            li.FromObject = beginObject;

                                        }
                                    }
                                    else if (siTo.SimioClassType == "Link") // The 'to' side is link, so the 'from is the object.
                                    {
                                        if (!pi.LinkDict.TryGetValue(siTo.Id, out li))
                                        {
                                            li = new LinkInfo(this.SimProps, siTo.Id, siTo);
                                            pi.LinkDict.Add(li.LinkId, li);
                                        }

                                        if (ci.ToCellName == "EndX")
                                        {
                                            ShapeInfo endObject = siFrom as ShapeInfo;
                                            siTo.ToShape = endObject;
                                            li.ToObjectId = endObject.Id;
                                            li.ToObject = endObject;

                                        }
                                        else if (ci.ToCellName == "BeginX")
                                        {
                                            ShapeInfo beginObject = siFrom as ShapeInfo;
                                            siTo.FromShape = beginObject;
                                            li.FromObjectId = beginObject.Id;
                                            li.FromObject = beginObject;
                                        }
                                    }

                                    if (li != null)
                                        marker = $"Connect #{connectCount} From={li.FromObject?.ShapeName} To={li.ToObject?.ShapeName}";
                                    else
                                        marker = $"Connect #{connectCount} is not a Simio connector";

                                }
                                catch (Exception ex)
                                {
                                    explanationList.Add($"Marker={marker} Err={ex}");
                                    goto GetNextConnect;
                                }

                                GetNextConnect:;
                            } // for each connect

                            // A link *should* have something on either end. 
                            // If it doesn't, then add an explanation, and remove the link.
                            foreach (ShapeInfo si in pi.ShapeDict.Values.Where(rr => rr.SimioClassType == "Link"))
                            {
                                if (si.ToShape == null || si.FromShape == null)
                                {
                                    explanationList.Add($"Marker={marker} Link={si.ShapeName} is not fully connected (one or both ends don't attach).");

                                    // Remove the faulty link
                                    LinkInfo li = null;
                                    if ( pi.LinkDict.TryGetValue(si.Id, out li))
                                    {
                                        pi.LinkDict.Remove(si.Id);
                                    }
                                }

                            } // For each ShapeInfo

                        } // Any Connects?

                        // Go through the links again and check its 'in' and 'out' objects.
                        // Make sure the objects found reference the link.
                        foreach (LinkInfo li in pi.LinkDict.Values)
                        {
                            ObjectInfo oi = null;

                            if ( li.FromObject?.Id != null && pi.ObjectDict.TryGetValue(li.FromObject.Id, out oi))
                            {
                                oi.OutLinkList.Add(li);
                            }

                            if ( li.ToObject?.Id != null &&  pi.ObjectDict.TryGetValue(li.ToObject.Id, out oi))
                            {
                                oi.InLinkList.Add(li);
                            }
                        }

                        // The links are all done, so check the node logic.
                        foreach ( ObjectInfo oi in pi.ObjectDict.Values )
                        {
                            string reason = "";
                            foreach (LinkInfo li in oi.InLinkList.OrderBy(rr => rr.EndLocation.Y) )
                            {
                                NodeInfo node = oi.FindOrAddNode(li.EndLocation, EnumNodeType.InBound, out reason);
                                if (node == null)
                                {
                                    explanationList.Add(reason);
                                    goto GetNextObject;
                                }
                                li.ToIndex = node.Index;
                            }

                            foreach (LinkInfo li in oi.OutLinkList.OrderBy(rr => rr.BeginLocation.Y) )
                            {
                                NodeInfo node = oi.FindOrAddNode(li.BeginLocation, EnumNodeType.OutBound, out reason);
                                if ( node == null )
                                {
                                    explanationList.Add(reason);
                                    goto GetNextObject;
                                }
                                li.FromIndex = node.Index;
                            }

                            GetNextObject:;
                        } // for each ObjectInfo
                    } // Does the page have an XML doc?

                }   // for each page

                return true;
            }
            catch (Exception ex)
            {
                explanationList.Add( $"Marker={marker} Err={ex}");
                return false;
            }

        }

        private string BuildUniqueUsername(Dictionary<string,string> dict, string username)
        {
            bool isUnique = false;
            int safety = 0;
            while (++safety < 1000 && !isUnique)
            {
                if (dict.ContainsKey(username))
                    username = $"{username}_{safety.ToString()}";
                else
                    isUnique = true;
            }
            return username;
        }

        /// <summary>
        /// Locate descendant Text elements and return the concatenation 
        /// of up to maxElements that are found.
        /// Concatenate as separate lines if buildMultiline is true.
        /// </summary>
        /// <returns></returns>
        private string FindText(XElement xShape, int maxElements, bool buildMultiline)
        {
            string txt = "";
            try
            {
                List<XElement> textList = xShape.Descendants()
                    .Where(rr => rr.Name.LocalName == "Text")
                    .ToList();
                if (textList.Any())
                {
                    StringBuilder sb = new StringBuilder();
                    int nn = 0;
                    foreach (XElement xe in textList)
                    {
                        if (buildMultiline)
                            sb.AppendLine(xe.Value);
                        else
                            sb.Append(xe.Value);

                        if (++nn >= maxElements)
                            break;
                    }
                    txt = sb.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                logit($"FindText Err={ex}");
            }
            return txt;
        }


        /// <summary>
        /// Get the size.
        /// Use the template if necessary.
        /// </summary>
        /// <param name="si"></param>
        /// <param name="mTemplate"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        private bool ValidateSize(ShapeInfo si, MasterInfo mTemplate, out string explanation)
        {
            explanation = "";

            if (si == null)
            {
                explanation = $"Connection Shape is null.";
                return false;
            }

            try
            {
                if (double.IsNaN(si.Size.XWidth))   // If no size, then go to the master
                {
                    si.Size.XWidth = mTemplate.Size.XWidth;
                }
                if (double.IsNaN(si.Size.YHeight))   // If no size, then go to the master
                {
                    si.Size.YHeight = mTemplate.Size.YHeight;
                }
                if (double.IsNaN(si.Size.ZDepth))   // If no size, then go to the master
                {
                    si.Size.ZDepth = mTemplate.Size.ZDepth;
                }


                return true;
            }
            catch (Exception ex)
            {
                explanation = $"ShapeInfo={si.ShapeName} Err={ex}";
                return false;
            }

        }

        /// <summary>
        /// Check the shape involved in a legitimate Simio connection.
        /// It must exist, and must be a Link or an Object
        /// </summary>
        /// <param name="si"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        private bool ValidateConnectionShape(ShapeInfo si, out string explanation)
        {
            explanation = "";

            if ( si == null )
            {
                explanation = $"Connection Shape is null.";
                return false;
            }

            try
            {
                if ( si.SimioClassType != "Object" && si.SimioClassType != "Link" )
                {
                    explanation = $"Shape={si.ShapeName} is neither Object nor Link";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"ShapeInfo={si.ShapeName} Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Look in the Shape Dictionary for the shape with 'id'
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static ShapeInfo FindObjectShape(PageInfo pi, int id)
        {
            foreach (ShapeInfo si in pi.ShapeDict.Values)
            {
                if (si.Id == id)
                    return si;

                if (si.SimioClassType == "Object")
                {
                    foreach (ShapeInfo sub in si.SubShapeList)
                    {
                        if (sub.Id == id)
                            return si;
                    }
                }
            }

            return null;
        }

        private void logit(string msg)
        {
            Loggerton.Instance.LogIt(EnumLogFlags.Error, msg);
        }

        /// In visio apps, many of the elements have 'N' and 'V' attributes,
        /// which correspond to Name and Value.
        /// Moreover, these can be in a nested Element structure, which 
        /// we'll call a 'path' and delimit with ','
        /// So, finding a Cell node with N='Value' which is under a Row node with N='SimioClass'
        /// Can be done like this. FindVForN(xe, "Row:SimioClass,Cell:Value")
        /// As we go through the path, we're always finding the first matched XElement. 
        /// If - at any point - there are no matches, then null is returned.


    } // class



}
