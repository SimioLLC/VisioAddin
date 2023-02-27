using SimioAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Packaging;
using System.Xml.Linq;

namespace SdxHelpers
{
    public class SdxApplication
    {
        /// <summary>
        /// The full path to the package file.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The name, which is the filename of the path (with extension)
        /// </summary>
        public string Name { get { return Path.GetFileName(FilePath); } }

        /// <summary>
        /// What type of Application? Visio, Excel, ...
        /// </summary>
        public string FileType { get; set; }

        public SimioAttributes SimProps { get; set; }

        /// <summary>
        /// Holds the mapping for Sheet numbers to Sheet Names.
        /// Sheet numbers are referenced in the Sheet URI names, and
        /// the Sheet names are in the Workbook.xml Part.
        /// Key is SheetID (e.g. "1") and Value is name (e.g. Objects1)
        /// </summary>
        public Dictionary<string, string> TableNameDict { get; set; }

        /// <summary>
        /// The returned Simio DataSet with tables for Objects, Links, and Vertices.
        /// There is also a table for Properties
        /// </summary>
        public DataSet SdxDataSet { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SdxApplication()
        {
            SimProps = new SimioAttributes();
        }



        /// <summary>
        /// Open the package file, and 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="fileAccess"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static Package OpenPackageFile(string filepath, string fileType, FileAccess fileAccess, out string explanation)
        {
            explanation = "";
            try
            {
                if (!File.Exists(filepath))
                {
                    explanation = $"Cannot find Package FilePath={filepath}";
                    return null;
                }

                return Package.Open(filepath, FileMode.Open, fileAccess);

            }
            catch (Exception ex)
            {
                explanation = $"Cannot Open FilePath={filepath} Err={ex}";
                return null;
            }
        } // method

        /// <summary>
        /// Helper to check for a null and create a string if found.
        /// </summary>
        /// <param name="oo"></param>
        /// <param name="msg"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool IsObjectNull(object oo, string msg, out string explanation)
        {
            explanation = "";

            if (oo != null)
                return false;

            explanation = $"{msg} is null.";
            return true;

        }

        /// <summary>
        /// Write our internal dataset as XML
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="overwrite"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool WriteInternalDataSet(string filepath, bool overwrite, out string explanation)
        {
            explanation = "";

            return WriteDataSet(SdxDataSet, filepath, overwrite, out explanation);
        }

        /// <summary>
        /// Write the dataset as XML
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="overwrite"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static bool WriteDataSet(DataSet ds, string filepath, bool overwrite, out string explanation)
        {
            explanation = "";
            try
            {
                if (ds == null)
                {
                    explanation = $"Cannot write: DataSet is null";
                    return false;
                }

                if (File.Exists(filepath) && overwrite)
                    File.Delete(filepath);

                using (System.IO.FileStream fstream = new FileStream(filepath, FileMode.CreateNew))
                {
                    ds.WriteXml(fstream);
                    fstream.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Filepath={filepath} Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Read a dataset as XML into our internal DataSet.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="overwrite"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool ReadInternalDataSet(string filepath, out string explanation)
        {
            explanation = "";
            try
            {
                DataSet ds = null;
                if (!ReadDataSet(filepath, out ds, out explanation))
                    return false;

                this.SdxDataSet = ds;
                return true;

            }
            catch (Exception ex)
            {
                explanation = $"Filpath={filepath} Err={ex}";
                return false;
            }

        }

        /// <summary>
        /// Read a dataset as XML
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="overwrite"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static bool ReadDataSet(string filepath, out DataSet ds, out string explanation)
        {
            ds = null;
            explanation = "";

            try
            {
                if (!File.Exists(filepath))
                {
                    explanation = $"File-{filepath} does not exist.";
                    return false;
                }

                using (System.IO.FileStream fstream = new FileStream(filepath, FileMode.Open))
                {
                    ds = new DataSet();

                    fstream.Position = 0;
                    ds.ReadXml(fstream);
                    fstream.Close();
                }
                return false;
            }
            catch (Exception ex)
            {
                explanation = $"Cannot ReadDataSet: Filepath={filepath} Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Build the tables of the dataset.
        /// The Properties Table contains a single record with meta-data about the DataSet.
        /// The other tables (Objects, Links, Vertices, and Artifacts) are created and left empty.
        /// The fields used were obtained from the Excel import example.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool BuildDataSetTables(DataSet ds, out string explanation)
        {
            string marker = "";
            explanation = "";

            try
            {
                SimioAttributes simProps = new SimioAttributes();

                // Create the Properties table, which contains global information about the Simio DataSet
                marker = "Adding Properties";
                DataTable dt = new DataTable("Properties");

                //Note: These are preliminary. Todo: Add Reflection of Properties
                foreach ( string propName in SimProps.AttributeList)
                {
                    dt.Columns.Add(new DataColumn(propName, typeof(string))); // Facility, Process, Task, ...
                }

                ds.Tables.Add(dt);

                marker = "Adding Objects";
                dt = new DataTable("Objects");
                dt.Columns.Add(new DataColumn("ObjectClass", typeof(string)));
                dt.Columns.Add(new DataColumn("BaseClass", typeof(string)));
                dt.Columns.Add(new DataColumn("Name", typeof(string))); // Key
                dt.Columns.Add(new DataColumn("X", typeof(double)));
                dt.Columns.Add(new DataColumn("Y", typeof(double)));
                dt.Columns.Add(new DataColumn("Z", typeof(double)));
                dt.Columns.Add(new DataColumn("Length", typeof(double)));
                dt.Columns.Add(new DataColumn("Width", typeof(double)));
                dt.Columns.Add(new DataColumn("Height", typeof(double)));
                dt.Columns.Add(new DataColumn("EntityType", typeof(string)));
                dt.Columns.Add(new DataColumn("InitialNumberInSystem", typeof(string)));
                dt.Columns.Add(new DataColumn("RideOnTransporter", typeof(string)));
                dt.Columns.Add(new DataColumn("TransporterName", typeof(string)));
                ds.Tables.Add(dt);

                marker = "Adding Object Properties";
                dt = new DataTable("ObjectProperties");
                dt.Columns.Add(new DataColumn("ObjectName", typeof(string))); // Key
                dt.Columns.Add(new DataColumn("Name", typeof(string))); // Foreign-Key
                dt.Columns.Add(new DataColumn("Value", typeof(string))); // Value associated with key
                ds.Tables.Add(dt);

                marker = "Adding Links";
                dt = new DataTable("Links");
                dt.Columns.Add(new DataColumn("LinkClass", typeof(string)));
                dt.Columns.Add(new DataColumn("BaseClass", typeof(string)));
                dt.Columns.Add(new DataColumn("Name", typeof(string))); // Key
                dt.Columns.Add(new DataColumn("FromNode", typeof(string)));
                dt.Columns.Add(new DataColumn("FromIndex", typeof(int)));
                dt.Columns.Add(new DataColumn("ToNode", typeof(string)));
                dt.Columns.Add(new DataColumn("ToIndex", typeof(int)));
                dt.Columns.Add(new DataColumn("Network", typeof(string)));
                dt.Columns.Add(new DataColumn("Length", typeof(double)));
                dt.Columns.Add(new DataColumn("Width", typeof(double)));
                dt.Columns.Add(new DataColumn("Height", typeof(double)));
                dt.Columns.Add(new DataColumn("Type", typeof(string)));
                ds.Tables.Add(dt);

                marker = "Adding Link Properties";
                dt = new DataTable("LinkProperties");
                dt.Columns.Add(new DataColumn("Name", typeof(string))); // Key
                dt.Columns.Add(new DataColumn("LinkName", typeof(string))); // Foreign-Key refrences Link's Name
                dt.Columns.Add(new DataColumn("Value", typeof(string))); // Key
                ds.Tables.Add(dt);

                marker = "Adding Vertices";
                dt = new DataTable("Vertices");
                dt.Columns.Add(new DataColumn("Name", typeof(string))); // Which is just a number. Name+Linkname create a unique key
                dt.Columns.Add(new DataColumn("LinkName", typeof(string))); // Foreign Key reference
                dt.Columns.Add(new DataColumn("X", typeof(double)));
                dt.Columns.Add(new DataColumn("Y", typeof(double)));
                dt.Columns.Add(new DataColumn("Z", typeof(double)));
                ds.Tables.Add(dt);

                marker = "Adding Artifacts";
                dt = new DataTable("Artifacts");
                dt.Columns.Add(new DataColumn("Name", typeof(string)));
                dt.Columns.Add(new DataColumn("X", typeof(double)));
                dt.Columns.Add(new DataColumn("Y", typeof(double)));
                dt.Columns.Add(new DataColumn("Z", typeof(double)));
                dt.Columns.Add(new DataColumn("Length", typeof(double))); // Simio x size
                dt.Columns.Add(new DataColumn("Width", typeof(double)));    // Simio y size
                dt.Columns.Add(new DataColumn("Height", typeof(double)));   // Simio z size
                dt.Columns.Add(new DataColumn("TextData", typeof(string)));
                dt.Columns.Add(new DataColumn("ForeignData", typeof(string)));
                ds.Tables.Add(dt);

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Marker={marker} Err={ex}";
                return false;
            }

        }


        /// <summary>
        /// Create a simio SDX dataset.
        /// A selection ID is provided to determine what part of the application is to
        /// be used. For example, this would be the page for Visio.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public virtual DataSet CreateSdxDataSet(string name, string sourcePath, int? selectionId, out string explanation)
        {
            explanation = "";

            DataSet ds = new DataSet(name);

            try
            {
                //The subclass should build the dataset with tables for properties, objects, links, vertices
                return ds;
            }
            catch (Exception ex)
            {
                explanation = $"Name={name} Path={sourcePath} Err={ex}";
                return null;
            }
        } // method (virtual)

        /// <summary>
        /// Given a PackagePart, extract an XDocument.
        /// </summary>
        /// <param name="packagePart"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static XDocument GetXmlFromPart(PackagePart packagePart, out string explanation)
        {
            explanation = "";
            try
            {
                XDocument partXml = null;
                // Open the packagePart as a stream and then 
                // open the stream in an XDocument object.
                using (Stream partStream = packagePart.GetStream())
                {
                    try
                    {
                        partXml = XDocument.Load(partStream);
                        return partXml;
                    }
                    catch (Exception ex)
                    {
                        explanation = $"PackagePart. ContentTYpe={packagePart.ContentType} Err={ex}";
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return null;
            }
        }

    } // class
}
