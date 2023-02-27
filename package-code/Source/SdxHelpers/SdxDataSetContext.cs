//using MathNet.Numerics.LinearAlgebra;
using LoggertonHelpers;
using SimioAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdxHelpers
{
    /// <summary>
    /// The overall context for the Simio Drawing Exchange (SDX) DataSet.
    /// Holds collections of properties, objects, links, and vertices that
    /// are built from an SDX DataSet.
    /// It is just an in-memory dataset representation is easier to use than the raw dataset.
    /// </summary>
    public class SdxDataSetContext
    {
        public string Name { get; set; }

        /// <summary>
        /// What type of source was used to create this file?
        /// E.g. Visio, Excel, ...
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// When read in from a file, this contains the location of the file.
        /// If read in from a DataSet, then this is empty.
        /// </summary>
        public string XmlFilePath { get; set; }

        /// <summary>
        /// The file path to the application that (allegedly) created this dataset.
        /// For example "c:\(test)\visioTest.vsdx
        /// Stored in the Properties table
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// The referenced SDX DataSet
        /// </summary>
        public DataSet SdxDataSet { get; set; }

        /// <summary>
        /// The row from the Properties table.
        /// </summary>
        public DataRow PropertyRow { get; set; }

        /// <summary>
        /// The properties, which are all strings and keyed by the name.
        /// </summary>
        public Dictionary<string, SdxProperty> PropertyDict;

        /// <summary>
        /// Key is Name
        /// </summary>
        public Dictionary<string, SdxObject> ObjectDict;

        /// <summary>
        /// Key is ObjectName and Name
        /// </summary>
        public List<SdxObjectProperty> ObjectPropertyList = new List<SdxObjectProperty>();

        /// <summary>
        /// Key is name.
        /// These are non-simio shapes (artifacts)
        /// </summary>
        public Dictionary<string, SdxArtifact> ArtifactDict;

        /// <summary>
        /// Key is Name
        /// </summary>
        public Dictionary<string, SdxLink> LinkDict;

        /// <summary>
        /// Key is ObjectName and Name
        /// </summary>
        public List<SdxLinkProperty> LinkPropertyList = new List<SdxLinkProperty>();

        /// <summary>
        /// The list of vertices. Needs to be read before the Links.
        /// </summary>
        public List<SdxLinkVertex> VertexList;

        /// <summary>
        /// The bounding box calculated from objects, links(vertices), and artifacts
        /// </summary>
        public AABB BBox { get; set; }

        /// <summary>
        /// This constructor is used when we already have the dataset.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sourceType"></param>
        /// <param name="sourcePath"></param>
        /// <param name="ds"></param>
        public SdxDataSetContext(string name, string sourceType, string sourcePath, DataSet ds)
        {
            this.Name = name;
            this.SourcePath = sourcePath;
            this.SourceType = sourceType;

            this.SdxDataSet = ds;

            string explanation = "";
            if (!CreateFromDataSet(name, ds, out explanation))
                throw new ApplicationException(explanation);

        }

        /// <summary>
        /// Use this constructor if we are pulling the dataset from an SDX XML filepath.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="xmlFilePath"></param>
        public SdxDataSetContext(string name, string xmlFilePath)
        {
            if (!File.Exists(xmlFilePath))
                throw new ApplicationException($"Cannot find File={xmlFilePath}");

            string explanation = "";
            if (!CreateFromFilePath(name, xmlFilePath, out explanation))
                throw new ApplicationException(explanation);

        }

        /// <summary>
        /// Create our context from a SDX DataSet.
        /// The DataSet is checked for validity, and then read into our internal structures.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        private bool CreateFromDataSet(string name, DataSet ds, out string explanation)
        {
            explanation = "";
            try
            {
                this.Name = name;
                this.XmlFilePath = string.Empty;

                PropertyDict = new Dictionary<string, SdxProperty>();
                ObjectDict = new Dictionary<string, SdxObject>();
                LinkDict = new Dictionary<string, SdxLink>();
                ArtifactDict = new Dictionary<string, SdxArtifact>();

                if ( !IsValidSdx(ds, out explanation))
                    return false;

                this.SdxDataSet = ds;

                if (!BuildContextUsingInternalDataSet(out explanation))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Given a filepath to the dataset xml file., Create an SDX DataSet context.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        private bool CreateFromFilePath(string name, string xmlFilepath, out string explanation)
        {
            explanation = "";
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(xmlFilepath);

                if (!CreateFromDataSet(name, ds, out explanation))
                    return false;

                this.XmlFilePath = xmlFilepath;
                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }


        /// <summary>
        /// Do some simple checks to see if the file is in SDX format.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static bool IsValidSdx(string filepath, out string explanation)
        {
            explanation = "";

            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(filepath);

                return IsValidSdx(ds, out explanation);
            }
            catch (Exception ex)
            {
                explanation = $"Filepath={filepath} Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Do some simple checks to see if the file is in SDX format.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static bool IsValidSdx(DataSet ds, out string explanation)
        {
            explanation = "";

            try
            {
                DataTable dt = ds.Tables["Properties"];
                if (dt == null)
                {
                    explanation = $"A 'Properties' table is required, but none were found.";
                    return false;
                }

                const int MaxSdxTables = 7;
                if (ds.Tables.Count > MaxSdxTables)
                {
                    explanation = $"Expected no more than MaxSdxTables tables, found={ds.Tables.Count}";
                    return false;
                }

                if (ds.Tables.Count < 1)
                {
                    explanation = $"Expected at least 1 table, found={ds.Tables.Count}";
                    return false;
                }

                int nn = 0;
                if (ds.Tables[nn].TableName != "Properties")
                {
                    explanation = $"Table={nn} should be 'Properties', but found={ds.Tables[0].TableName}";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Get the columns from the Properties table (first row) as a multi-line string.
        /// </summary>
        /// <returns></returns>
        public string GetPropertiesAsString()
        {
            StringBuilder sb = new StringBuilder();

            foreach ( DataColumn dc in SdxDataSet.Tables["Properties"].Columns)
            {
                string key = $"{dc.ColumnName}";
                string value = PropertyRow[dc.Ordinal].ToString();
                sb.AppendLine($"{dc.ColumnName}={value}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Create the base information that should always be in the PropertyRow.
        /// You can add your own, but this need be be here for this version.
        /// </summary>
        /// <param name="DataSetType">e.g. Facility</param>
        /// <param name="SourceType">e.g. Visio</param>
        /// <param name="SourcePath">e.g. c:\(test)\myfile.vsdx</param>
        /// <returns></returns>
        public static void CreateBasicProperties(Dictionary<string, SdxProperty> propDict, string datasetType, string sourceType, string sourcePath)
        {
            SimioAttributes simProps = new SimioAttributes();

            foreach ( string prop in simProps.AttributeList)
            {
                switch ( prop.ToLower())
                {
                    case "sdxversion":
                        AddProperty(propDict, prop, "18.09.07");
                        break;
                    case "modifiedby":
                        AddProperty(propDict, prop, Environment.UserName);
                        break;
                    case "modifiedat":
                        AddProperty(propDict, prop, DateTime.UtcNow.ToString());
                        break;
                    case "datasettype":
                        AddProperty(propDict, prop, datasetType);
                        break;
                    case "sourcetype":
                        AddProperty(propDict, prop, sourceType);
                        break;
                    case "sourcepath":
                        AddProperty(propDict, prop, sourcePath);
                        break;
                    case "comments":
                        AddProperty(propDict, prop, "");
                        break;
                    case "simiooffset":
                        AddProperty(propDict, prop, "");
                        break;
                    case "boundingbox":
                        AddProperty(propDict, prop, "");
                        break;
                } // switch
            } // for each basic property

        } // method


        /// <summary>
        /// Add a property to the internal dictionary
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public static void AddProperty(Dictionary<string, SdxProperty> propDict, string propertyName, string propertyValue)
        {
            if (propDict == null)
                propDict = new Dictionary<string, SdxProperty>();

            if (!propDict.ContainsKey(propertyName.ToLower()))
            {
                SdxProperty prop = new SdxProperty(propertyName, propertyValue);
                propDict.Add(prop.Key, prop);
            }

        }

        /// <summary>
        /// Add a property to the internal dictionary
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void AddProperty(string propertyName, string propertyValue)
        {
            if (PropertyDict == null)
                PropertyDict = new Dictionary<string, SdxProperty>();

            AddProperty(PropertyDict, propertyName, propertyValue);

        }

        /// <summary>
        /// Create a DataRow using the Property information.
        /// </summary>
        /// <param name="DataTable"></param>
        /// <returns></returns>
        public static DataRow CreatePropertyRow(DataTable dt, Dictionary<string, SdxProperty> propDict)
        {
            DataRow dr = dt.NewRow();

            foreach (SdxProperty prop in propDict.Values)
            {
                dr[prop.Name] = prop.PropertyValue;
            }

            return dr;
        }

        /// <summary>
        /// Compute our internal bounding box.
        /// This considers all the objects and all the paths when
        /// constructing the box.
        /// </summary>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool ComputeBoundingBox(out string explanation)
        {
            explanation = "";
            string marker = "Begin";
            try
            {
                // Build a list
                List<SdxVector> vList = new List<SdxVector>();

                foreach (SdxObject so in ObjectDict.Values )
                {
                    List<SdxVector> vBox = so.CreateAabbVectors();
                    vList.AddRange(vBox);
                }

                foreach (SdxLink link in LinkDict.Values)
                {
                    vList.AddRange(link.VertexList);
                }
                vList.AddRange(ArtifactDict.Values.Select(rr => rr.Center).ToList());

                this.BBox = new AABB(vList);
                return true;

            }
            catch (Exception ex)
            {
                explanation = $"Marker={marker} Err={ex}";
                return false;
            }

        }
        /// <summary>
        /// Construct this context from our internal dataset SdxHelpers
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        private bool BuildContextUsingInternalDataSet(out string explanation)
        {
            explanation = "";
            string marker = "Begin";
            try
            {
                DataTable dt = null;

                dt = SdxDataSet.Tables["Properties"];
                if (dt != null)
                {
                    DataTable PropertyTable = dt;
                    PropertyRow = PropertyTable.Rows[0];
                    PropertyDict = new Dictionary<string, SdxProperty>();

                    marker = $"Add column names";
                    foreach (DataColumn dc in PropertyTable.Columns)
                    {
                        string propName = $"{dc.ColumnName}";
                        string propValue = PropertyRow[dc.Ordinal].ToString();

                        AddProperty(propName, propValue);
                    }
                }

                marker = " Add Vertices";
                this.VertexList = new List<SdxLinkVertex>();
                dt = SdxDataSet.Tables["Vertices"];
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SdxLinkVertex vv = new SdxLinkVertex();
                        if (!vv.BuildFromDataRow(this, dr, out explanation))
                            return false;

                        this.VertexList.Add(vv);
                    }
                }

                marker = "Add Object Properties";
                this.ObjectPropertyList.Clear();
                dt = SdxDataSet.Tables["ObjectProperties"];
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SdxObjectProperty prop = new SdxObjectProperty();
                        if (!prop.BuildFromDataRow(this, dr, out explanation))
                            return false;

                        this.ObjectPropertyList.Add(prop);
                    }
                }

                marker = "Add Objects";
                this.ObjectDict.Clear();
                dt = SdxDataSet.Tables["Objects"];
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SdxObject obj = new SdxObject();
                        if (!obj.BuildFromDataRow(this, dr, out explanation))
                            return false;

                        this.ObjectDict.Add(obj.Name, obj);
                    }
                }

                marker = "Add Link Properties";
                this.LinkPropertyList.Clear();
                dt = SdxDataSet.Tables["LinkProperties"];
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SdxLinkProperty prop = new SdxLinkProperty();
                        if (!prop.BuildFromDataRow(this, dr, out explanation))
                            return false;

                        this.LinkPropertyList.Add(prop);
                    }
                }

                marker = "Add Links";
                this.LinkDict.Clear();
                dt = SdxDataSet.Tables["Links"];
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SdxLink link = new SdxLink();
                        if (!link.BuildFromDataRow(this, dr, out explanation))
                            goto NextLinkRow;

                        if (this.LinkDict.ContainsKey(link.Name))
                        {
                            logit(EnumLogFlags.Warning, $"Duplicate LinkName={link.Name}");
                            goto NextLinkRow;
                        }

                        this.LinkDict.Add(link.Name, link);

                        NextLinkRow:;
                    } // for each link
                }

                marker = "Add Artifacts";
                this.ArtifactDict.Clear();
                dt = SdxDataSet.Tables["Artifacts"];
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SdxArtifact artifact = new SdxArtifact();
                        if (!artifact.BuildFromDataRow(this, dr, out explanation))
                            return false;

                        this.ArtifactDict.Add(artifact.Name, artifact);
                    } // for each artifact
                }

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Marker={marker} Err={ex}";
                return false;
            }
        }

        private void logit(EnumLogFlags flags, string msg)
        {
            Loggerton.Instance.LogIt(flags, msg);
        }

    }

    /// <summary>
    /// The properties for the SDX dataset.
    /// The key (for lookup) is lowercase so that the searches
    /// are case insensitive.
    /// </summary>
    public class SdxProperty
    {
        public string Name { get; set; }
        
        public string Key {  get { return Name.ToLower(); } }

        public string PropertyValue { get; set; }

        public SdxProperty(string name, string value)
        {
            this.Name = name;
            this.PropertyValue = value;
        }
    }

    /// <summary>
    /// The Simio properties for the SDX Object
    /// The key (for lookup) is lowercase so that the searches
    /// are case insensitive.
    /// </summary>
    public class SdxObjectProperty
    {
        public string Name { get; set; }

        /// <summary>
        /// The unique key is the owning object's name appended with the property name.
        /// </summary>
        public string Key { get { return $"{MyObject?.Name.ToLower()}_{Name.ToLower()}"; } }

        public string PropertyValue { get; set; }

        public string ObjectName { get; set; }

        public SdxObject MyObject { get; set; }

        public SdxObjectProperty()
        {
        }
        public SdxObjectProperty(SdxObject myObject, string name, string value)
        {
            this.MyObject = myObject;
            this.Name = name;
            this.PropertyValue = value;
        }

        /// <summary>
        /// Construct ObjectProperty from the dataset row.
        /// Get the Name and Value
        /// </summary>
        /// <param name="context">The context needed to get the vertices</param>
        /// <param name="dr">The row for th eLink</param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        internal bool BuildFromDataRow(SdxDataSetContext context, DataRow dr, out string explanation)
        {
            explanation = "";
            try
            {
                this.Name = dr["Name"] as string;
                this.PropertyValue = dr["Value"] as string;

                this.ObjectName = dr["ObjectName"] as string;

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }

    } // class


    /// <summary>
    /// Objects, such as Server, Combiner, Separator, BasicNode, ...
    /// </summary>
    public class SdxObject : SdxShape
    {
        /// <summary>
        /// What type of Simio object: Server, Sink, Source, ...
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// The Simio base class (Server, Sink, ...)
        /// </summary>
        public string BaseClass { get; set; }

        /// <summary>
        /// The Simio object created
        /// </summary>
        public IFixedObject SimioObject { get; set; }

        /// <summary>
        /// The original size according to Simio when the Simio bject is created.
        /// </summary>
        public FacilitySize OriginalSize { get; set; }

        public List<SdxObjectProperty> ObjectPropertyList { get; set; } = new List<SdxObjectProperty>();

        public override string ToString()
        {
            return $"Object={base.ToString()} ({ClassName})";
        }

        /// <summary>
        /// Construct Object from the dataset row.
        /// Get the location (x,y,z) and size (wid,height,length)
        /// </summary>
        /// <param name="context">The context needed to get the vertices</param>
        /// <param name="dr">The row for th eLink</param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        internal new bool BuildFromDataRow(SdxDataSetContext context, DataRow dr, out string explanation)
        {
            explanation = "";
            try
            {
                this.ClassName = dr["ObjectClass"] as string;
                this.BaseClass = dr["BaseClass"] as string;

                base.BuildFromDataRow(context, dr, out explanation);

                // Get properties
                List<SdxObjectProperty> propList = context.ObjectPropertyList
                    .Where(rr => rr.ObjectName == Name)
                    .ToList();

                this.ObjectPropertyList.Clear();
                foreach (SdxObjectProperty prop in propList)
                {
                    prop.MyObject = this;
                    ObjectPropertyList.Add(prop);
                }

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }

    }


    /// <summary>
    /// Shapes, as read from the file
    /// </summary>
    public class SdxShape
    {

        /// <summary>
        /// And the name assigned to it.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The location of the Center of the shape in original units. 
        /// I.e., it is probably not FacilityLocation
        /// </summary>
        public SdxVector Center { get; set; }
        
        /// <summary>
        /// The size in original units.
        /// </summary>
        public SdxSize Size { get; set; }


        public override string ToString()
        {
            return $"Shape={Name}";
        }

        internal double ConvertCellToDouble(object cell, double def)
        {
            try
            {
                return Convert.ToDouble(cell);
            }
            catch (Exception)
            {
                return def;
            }
        }

        /// <summary>
        /// Returns a list of 2 vectors created from the Center and Size.
        /// These are the vectors needed to define an Axis-Aligned Bounding Box (AABB).
        /// </summary>
        /// <returns></returns>
        internal List<SdxVector> CreateAabbVectors()
        {
            List<SdxVector> vList = new List<SdxVector>();
            SdxVector vv = Center;
            SdxSize vs = this.Size;

            double deltaX = vs.xLength / 2;
            double deltaY = vs.yWidth / 2;
            double deltaZ = vs.zHeight / 2;

            vList.Add(new SdxVector(vv.X + deltaX, vv.Y + deltaY, vv.Z + deltaZ));
            vList.Add(new SdxVector(vv.X - deltaX, vv.Y - deltaY, vv.Z - deltaZ));

            return vList;

        }

        /// <summary>
        /// Construct Shape from the dataset row.
        /// Get the location (x,y,z) and size (wid,height,length)
        /// </summary>
        /// <param name="context">The context needed to get the vertices</param>
        /// <param name="dr">The row for th eLink</param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        internal bool BuildFromDataRow(SdxDataSetContext context, DataRow dr, out string explanation)
        {
            explanation = "";
            try
            {
                this.Name = dr["Name"] as string;

                double xx = ConvertCellToDouble(dr["X"], 0);
                double yy = ConvertCellToDouble(dr["Y"], 0);
                double zz = ConvertCellToDouble(dr["Z"], 0);

                this.Center = new SdxVector(xx, yy, zz);

                double len = ConvertCellToDouble(dr["Length"], 0.5);
                double wid = ConvertCellToDouble(dr["Width"], 0.3);
                double hgt = ConvertCellToDouble(dr["Height"], 0.5);

                this.Size = new SdxSize(len, wid, hgt);
                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }

    }

    /// <summary>
    /// Class to hold information about Artifacts (that is non-Simio shapes)
    /// </summary>
    public class SdxArtifact : SdxShape
    {


        /// <summary>
        /// For Artifacts that have text
        /// </summary>
        public string TextData { get; set; }


        /// <summary>
        /// For Artifacts that have foreign data (like bitmaps)
        /// </summary>
        public string ForeignData { get; set; }


        public SdxArtifact()
        {

        }

        /// <summary>
        /// Construct Object from the dataset row.
        /// Get the location (x,y,z) and size (wid,height,length)
        /// </summary>
        /// <param name="context">The context needed to get the vertices</param>
        /// <param name="dr">The row for th eLink</param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        internal new bool BuildFromDataRow(SdxDataSetContext context, DataRow dr, out string explanation)
        {
            explanation = "";
            try
            {
                this.ForeignData = dr["ForeignData"] as string;
                this.TextData = dr["TextData"] as string;

                base.BuildFromDataRow(context, dr, out explanation);
                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }

    }

    /// <summary>
    /// The Simio properties for the SDX Link
    /// The key (for lookup) is lowercase so that the searches
    /// are case insensitive.
    /// </summary>
    public class SdxLinkProperty
    {
        public string Name { get; set; }

        /// <summary>
        /// The unique key is the owning Link's name appended with the property name.
        /// </summary>
        public string Key { get { return $"{MyLink?.Name.ToLower()}_{Name.ToLower()}"; } }

        public string PropertyValue { get; set; }

        public string LinkName { get; set; }

        public SdxLink MyLink { get; set; }

        public SdxLinkProperty()
        {
        }


        /// <summary>
        /// Construct ObjectProperty from the dataset row.
        /// Get the Name and Value
        /// </summary>
        /// <param name="context">The context needed to get the vertices</param>
        /// <param name="dr">The row for th eLink</param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        internal bool BuildFromDataRow(SdxDataSetContext context, DataRow dr, out string explanation)
        {
            explanation = "";
            try
            {
                this.Name = dr["Name"] as string;
                this.PropertyValue = dr["Value"] as string;

                LinkName = dr["LinkName"] as string;

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }

    }


    /// <summary>
    /// The SimioDataSet Link (connector)
    /// </summary>
    public class SdxLink
    {
        /// <summary>
        /// The class name (may be a subclass)
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// The simio base class (TimePath, ...)
        /// </summary>
        public string BaseClass { get; set; }

        /// <summary>
        /// The name of this link. All vertices reference this.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The object that begins the connector.
        /// It must be an object that *is* a node or contains nodes. 
        /// </summary>
        public SdxObject FromNode { get; set; }

        /// <summary>
        /// Our order (base 0) of 'from' links on the attached Object.
        /// </summary>
        public int FromIndex { get; set; }

        /// <summary>
        /// The node that terminates the connector
        /// It must be an object that *is* a node or contains nodes. 
        /// </summary>
        public SdxObject ToNode { get; set; }

        /// <summary>
        /// Our order (base 0) of 'to' links on the attached Object.
        /// </summary>
        public int ToIndex { get; set; }

        public string Network { get; set; }

        public List<SdxVector> VertexList;

        /// <summary>
        /// Size in the X direction
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Size in the Z direction
        /// </summary>
        public double Height { get; set; }

        public List<SdxLinkProperty> LinkPropertyList { get; set; } = new List<SdxLinkProperty>();

        /// <summary>
        /// The link created.
        /// </summary>
        public ILinkObject SimioLink { get; set; }

        public override string ToString()
        {
            return $"Link={Name} Class={ClassName} (Base={BaseClass}) From={FromNode} To={ToNode}";
        }

        private int ConvertDataRowCellToInt(object dr, int defaultInt)
        {
            int ii;
            try
            {
                ii = Convert.ToInt32(dr);
            }
            catch (Exception)
            {
                ii = defaultInt;
            }
            return ii;
        }
        private double ConvertDataRowCellToDouble(object dr, double defaultDouble)
        {
            double dd;
            try
            {
                dd = Convert.ToDouble(dr);
            }
            catch (Exception)
            {
                dd = defaultDouble;
            }
            return dd;
        }
        /// <summary>
        /// Build Link from the dataset row.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dr"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        internal bool BuildFromDataRow(SdxDataSetContext context, DataRow dr, out string explanation)
        {
            explanation = "";
            try
            {
                this.ClassName = dr["LinkClass"] as string;
                this.BaseClass = dr["BaseClass"] as string;
                this.Name = dr["Name"] as string;

                if ( this.Name == null )
                {
                    explanation = "Link Name is null";
                    return false;
                }

                string fromNode = dr["FromNode"] as string;
                if ( fromNode != null )
                    this.FromNode = context.ObjectDict.Values
                        .SingleOrDefault(rr => rr.Name.ToLower() == fromNode.ToLower());

                this.FromIndex = ConvertDataRowCellToInt(dr["FromIndex"], -1);
                this.ToIndex = ConvertDataRowCellToInt(dr["ToIndex"], -1);

                string toNode = dr["ToNode"] as string;
                if ( toNode != null )
                    this.ToNode = context.ObjectDict.Values
                        .SingleOrDefault(rr => rr.Name.ToLower() == toNode.ToLower());

                Width = ConvertDataRowCellToDouble(dr["Width"], 0.5);
                Height = ConvertDataRowCellToDouble(dr["Height"], 0.5);

                // Construct the vertices for this link.
                this.VertexList = new List<SdxVector>();
                List<SdxLinkVertex> vertList = context.VertexList
                    .Where(rr => rr.LinkName.ToLower() == this.Name.ToLower())
                    .ToList();

                foreach (SdxLinkVertex vert in vertList)
                {
                    this.VertexList.Add(vert.V3);
                }

                // Get properties
                List<SdxLinkProperty> propList = context.LinkPropertyList
                    .Where(rr => rr.LinkName == Name)
                    .ToList();

                this.LinkPropertyList.Clear();
                foreach ( SdxLinkProperty prop in propList )
                {
                    prop.MyLink = this;
                    LinkPropertyList.Add(prop);
                }

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        } // method


    } // class SdxLink



    /// <summary>
    /// A vertex as it exists in the SDX Vertex Table. It is a link name and a 3-tuple to hold x,y,z
    /// Also holds the LinkName;
    /// </summary>
    public class SdxLinkVertex
    {
        /// <summary>
        /// This just a number (0-..)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The link that owns this vertex.
        /// </summary>
        public string LinkName { get; set; }

        /// <summary>
        /// The xyz coordinates.
        /// </summary>
        public SdxVector V3 { get; set; }

        public SdxLinkVertex()
        {
        }

        /// <summary>
        /// The link vertext that holds the owning link name, and also the coordinates.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="linkName"></param>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="zz"></param>
        public SdxLinkVertex(string name, string linkName, double xx, double yy, double zz)
        {
            this.Name = name;
            this.LinkName = linkName;
            this.V3 = new SdxVector(xx, yy, zz);
        }

        public override string ToString()
        {
            return $"Name={Name} Link={LinkName} V=({V3.X.ToString("0.000")},{V3.Y.ToString("0.000")},{V3.Z.ToString("0.000")})";
        }

        private double ConvertCellToDouble(object cell, double def)
        {
            try
            {
                return Convert.ToDouble(cell);
            }
            catch (Exception)
            {
                return def;
            }
        }

        /// <summary>
        /// Construct a LinkVertex from a datarow.
        /// </summary>
        /// <param name="context">The context needed to get the vertices</param>
        /// <param name="dr">The row for th eLink</param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool BuildFromDataRow(SdxDataSetContext context, DataRow dr, out string explanation)
        {
            explanation = "";
            try
            {
                this.Name = dr["Name"] as string;
                this.LinkName = dr["LinkName"] as string;

                double xx = ConvertCellToDouble(dr["X"], 0);
                double yy = ConvertCellToDouble(dr["Y"], 0);
                double zz = ConvertCellToDouble(dr["Z"], 0);

                this.V3 = new SdxVector(xx, yy, zz);

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }



    } // class


    /// <summary>
    /// A size object to use.
    /// </summary>
    public class SdxSize
    {
        /// <summary>
        /// X Dimension
        /// </summary>
        public double xLength { get; set; }

        /// <summary>
        /// Y Dimension
        /// </summary>
        public double yWidth { get; set; }

        /// <summary>
        /// Z Dimension
        /// </summary>
        public double zHeight { get; set; }

        /// <summary>
        /// Constructor with necessary dimensions (Len=X, Wid=Y, Hgt=Z)
        /// </summary>
        /// <param name="wid"></param>
        /// <param name="hgt"></param>
        /// <param name="len"></param>
        public SdxSize(double len, double wid, double hgt)
        {
            this.xLength = len;
            this.yWidth = wid;
            this.zHeight = hgt;
        }
        public override string ToString()
        {
            return $"L(x)=({xLength.ToString("0.00")},W(y)={yWidth.ToString("0.00")},H(z)={zHeight.ToString("0.00")})";
        }

    }




    /// <summary>
    /// A 3-tuple to hold x,y,z.
    /// Creates a 3D Vertex with some vertex operations.
    /// </summary>
    public class SdxVector
    {

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public SdxVector()
        {
        }

        public SdxVector(double xx, double yy, double zz)
        {
            this.X = xx;
            this.Y = yy;
            this.Z = zz;
        }
        public override string ToString()
        {
            return $"V(x,y,z)=({X.ToString("0.000")},{Y.ToString("0.000")},{Z.ToString("0.000")})";
        }

        /// <summary>
        /// Vector length.
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            double len = Math.Sqrt(X * X + Y * Y + Z * Z);
            return len;
        }

        /// <summary>
        /// Scale all dimensions by the same number.
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public SdxVector ScaleBy(double scale)
        {
            SdxVector vScaled = new SdxVector(X * scale, Y * scale, Z * scale);
            return vScaled;
        }
        
        /// <summary>
        /// Scale using the 3 values in an SdxVector
        /// </summary>
        /// <param name="vScale"></param>
        /// <returns></returns>
        public SdxVector ScaleBy(SdxVector vScale)
        {
            SdxVector vScaled = new SdxVector(X * vScale.X, Y * vScale.Y, Z * vScale.Z);
            return vScaled;
        }

        public SdxVector Add(SdxVector vAdd)
        {
            SdxVector vAdded = new SdxVector(X + vAdd.X, Y + vAdd.Y, Z + vAdd.Z);
            return vAdded;
        }
        public SdxVector Subtract(SdxVector vSub)
        {
            SdxVector vDifference = new SdxVector(X - vSub.X, Y - vSub.Y, Z - vSub.Z);
            return vDifference;
        }

    }


}
