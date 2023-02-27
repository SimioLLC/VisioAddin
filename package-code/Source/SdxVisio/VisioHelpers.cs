using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SdxVisio
{
    public static class VisioHelpers
    {
        /// <summary>
        /// Get the attribute with the given name as a Int32
        /// If there is a problem, then return the default.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="attrName"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static int GetAttrAsInt32(XElement xe, string attrName, int def)
        {
            XAttribute attr = xe.Attribute(attrName);
            if (attr == null)
                return def;

            int nn = 0;
            if (!Int32.TryParse(attr.Value, out nn))
                return def;

            return nn;
        }

        /// <summary>
        /// Get the attribute with the given name as a Int32
        /// If there is a problem, then return the default.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="attrName"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string GetAttrAsString(XElement xe, string attrName, string def)
        {
            XAttribute attr = xe.Attribute(attrName);
            if (attr == null)
                return def;

            return attr.Value;
        }

        /// <summary>
        /// Get the attribute with the given name as a double.
        /// If there is a problem, then return the default.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="attrName"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static double GetAttrAsDouble(XElement xe, string attrName, double def)
        {
            XAttribute attr = xe.Attribute(attrName);
            if (attr == null)
                return def;

            double dd = double.NaN;
            if (!double.TryParse(attr.Value, out dd))
                return def;

            return dd;
        }

        /// <summary>
        /// A Visio storage convention is to have several 'Cell' Elements below xe.
        /// And these have an 'N' (name) attribute, and a 'V' (value) attribute.
        /// Find the one where N is name, and return the value as double.
        /// The 'name' search is case insensitive. 
        /// If the value does not parse to a double, then NaN is returne.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="name"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static double GetCellValueAsDouble(XElement xeParent, string name, double def)
        {
            try
            {
                XElement xe = xeParent.Elements()
                    .SingleOrDefault(rr => rr.Name.LocalName == "Cell" 
                    && rr.Attribute("N").Value.ToLower() == name.ToLower());
                if (xe == null)
                    return def;

                string vv = xe.Attribute("V").Value;
                double dd = double.NaN;
                if (!double.TryParse(vv, out dd))
                    return double.NaN;

                return dd;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Getting Cell={name}. Err={ex}");
            }
        }

        /// <summary>
        /// Given an xe for a shape, find the Begin/End X/Y Cells.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="vBegin"></param>
        /// <param name="vEnd"></param>
        /// <returns></returns>
        public static bool FindLinkLocations(XElement xe, out VisioVertex vBegin, out VisioVertex vEnd, out string explanation)
        {
            explanation = "";
            vBegin = null;
            vEnd = null;

            try
            {
                double bx = GetCellValueAsDouble(xe, "BeginX", double.NaN);
                double by = GetCellValueAsDouble(xe, "BeginY", double.NaN);
                double bz = 0;

                double ex = GetCellValueAsDouble(xe, "EndX", double.NaN);
                double ey = GetCellValueAsDouble(xe, "EndY", double.NaN);
                double ez = 0;

                vBegin = new VisioVertex(bx, by, bz);
                vEnd = new VisioVertex(ex, ey, ez);

                if (double.IsNaN(bx) || double.IsNaN(by))
                    explanation += " Missing Begin.";
                if (double.IsNaN(ex) || double.IsNaN(ey))
                    explanation += " Missing End.";

                return String.IsNullOrEmpty(explanation);
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Given an xe for a shape, find the Begin/End X/Y Cells.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="vBegin"></param>
        /// <param name="vEnd"></param>
        /// <returns></returns>
        public static bool BuildPathGeometry(XElement xe, VisioVertex vBegin, VisioVertex vEnd, List<VisioVertex> vList)
        {
            vList.Clear();

            try
            {
                XElement xSection = xe.Elements()
                    .SingleOrDefault(rr => rr.Name.LocalName == "Section" && rr.Attribute("N").Value == "Geometry");
                if (xSection == null)
                    return false;

                // Assume rows are in order. Each row may have Cells with N=X or N=Y
                // A missing value means zero
                double xx = vBegin.X;
                double yy = vBegin.Y;
                double zz = 0;

                VisioVertex vv = new VisioVertex(xx, yy, zz);
                vList.Add(vv); // Starting position

                List<XElement> rowList = xSection.Elements().Where(rr => rr.Name.LocalName == "Row").ToList();
                foreach ( XElement xRow in rowList)
                {
                    double deltaX = GetCellValueAsDouble(xRow, "X", 0.0);
                    double deltaY = GetCellValueAsDouble(xRow, "Y", 0.0);
                    double deltaZ = GetCellValueAsDouble(xRow, "Z", 0.0);

                    xx += deltaX;
                    yy += deltaY;
                    zz += deltaZ;

                    vv = new VisioVertex(xx, yy, zz);
                    vList.Add(vv);
                }

                vList.Add(new VisioVertex(vEnd.X, vEnd.Y, vEnd.Z)); // terminator

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Err={ex}");
            }
        }

    /// <summary>
    /// In visio apps, many of the elements have 'N' and 'V' attributes,
    /// which correspond to Name and Value.
    /// Moreover, these can be in a nested Element structure, which 
    /// we'll call a 'path' and delimit with ','
    /// So, finding a Cell node with N='Value' which is under a Row node with N='SimioClass'
    /// Can be done like this. FindVForN(xe, "Row:SimioClass,Cell:Value")
    /// As we go through the path, we're always finding the first matched XElement. 
    /// If - at any point - there are no matches, then null is returned.
    /// </summary>
    /// <param name="searchPath"></param>
    /// <param name="N"></param>
    /// <param name=""></param>
    public static string FindVForN(XElement xeRoot, string path)
        {
            string result = "";
            try
            {
                int nn = path.IndexOf(",");
                string term = path.Trim();
                if (nn != -1)
                {
                    term = path.Substring(0, nn).Trim();
                }

                string[] tokens = term.Split(':');
                if (tokens.Length != 2)
                    throw new ApplicationException($"Term={term} must be colon-separated");

                string elementName = tokens[0].Trim();
                string attrName = tokens[1].Trim();
                List<XElement> xeList = xeRoot.Elements()
                    .Where(rr => rr.Name.LocalName == elementName && rr.Attribute("N")?.Value == attrName)
                    .ToList();

                if (!xeList.Any())
                    return null;

                XElement xe = xeList.First();

                if (nn != -1)
                    result = FindVForN(xe, path.Substring(nn + 1));
                else
                {
                    result = xe.Attribute("V")?.Value;
                }
                return result;

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Path={path} Err={ex}");
            }

        }

        /// <summary>
        /// In visio apps, there is a 'Section' element with attribute N='Property'
        /// Within the Section element are Row elements with attribute N='the-property-name'.
        /// By convention, it is a Simio property if it begins with "simio_".
        /// With the Row element in a Cell N='Value' with V='the-property-value'
        /// This method finds these simio properties and returns
        /// a propertyDictionary with the key ('N') and the value ('V')
        /// </summary>
        /// <param name="xeRoot">The XML doc</param>
        /// <param name="properties">Returned properties</param>
        /// <param name=""></param>
        public static bool FindSimioProperties(XElement xeRoot, out Dictionary<string,string> properties, out string explanation)
        {
            explanation = "";
            properties = new Dictionary<string, string>();
            string marker = "Begin";
            try
            {
                List<XElement> xeSections = xeRoot.Descendants()
                    .Where(rr => rr.Name.LocalName == "Section" && rr.Attribute("N")?.Value == "Property")
                    .ToList();

                if (!xeSections.Any())
                    return false;

                foreach (XElement xeSection in xeSections)
                {
                    List<XElement> xeRows = xeSection.Elements()
                        .Where(rr => rr.Name.LocalName == "Row")
                        .ToList();

                    foreach (XElement xeRow in xeRows )
                    {
                        string prefix = "simio_";
                        string name = xeRow.Attribute("N")?.Value;
                        if ( name.ToLower().StartsWith(prefix))
                        {
                            marker = $"PropertyName={name}";

                            if ( properties.ContainsKey(name))
                            {
                                logitStatic($"Duplicate Property found={name}");
                                goto GetNextRow;
                            }
                            // Todo: Consider validation options; both with Simio design context and without.

                            XElement valueCell = xeRow.Elements()
                                .SingleOrDefault(rr => rr.Name.LocalName == "Cell"
                                    && rr.Attribute("N")?.Value == "Value");
                            if (valueCell != null)
                            {
                                string value = valueCell.Attribute("V")?.Value;
                                properties.Add(name, value);
                            }
                        }

                        GetNextRow:;
                    } // for each Row element
                    
                } // for each 'Property' section

                return properties.Any();
            }
            catch (Exception ex)
            {
                explanation = $"Marker={marker} Err={ex}";
                return false;
            }

        }

        /// <summary>
        /// Given an element, find a Property Section, and determine if the a simio_objectclass or simio_linkclass occurs.
        /// return the simio class Type (e.g. Object, Link, ...)
        /// and its name (Objects can be Source, Server, Sink... and Links can be Path, TimePath ...)
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="simioClassType"></param>
        /// <param name="simioClass"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static bool FindSimioClass(XElement xe, out string simioClassType, out string simioClass, out string simioBaseClass, out string explanation)
        {
            explanation = "";
            simioClassType = "";
            simioClass = "";
            simioBaseClass = "";
            try
            {
                Dictionary<string, string> propertyDict = null;

                if (!VisioHelpers.FindSimioProperties(xe, out propertyDict, out explanation))
                    return false;

                return FindSimioClass(propertyDict, out simioClassType, out simioClass, out simioBaseClass, out explanation);
            }
            catch (Exception ex)
            {
               explanation = $"Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Given a simio property list, determine if we have a valid classtype and class.
        /// The simio class Type (e.g. Object, Link, ...)
        /// and its name (Objects can be Source, Server, Sink... and Links can be Path, TimePath ...)
        /// If neither simio_objectclass or simio_linkclass is found, then false is returned, since
        /// all other simio_{property} are invalid without it.
        /// </summary>
        /// <param name="propertyDict"></param>
        /// <param name="simioClassType"></param>
        /// <param name="simioClass"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static bool FindSimioClass(Dictionary<string,string> propertyDict, out string simioClassType, out string simioClass, out string simioBaseClass, out string explanation)
        {
            explanation = "";
            simioClassType = "";
            simioClass = "";
            simioBaseClass = "";

            if (propertyDict == null || !propertyDict.Any())
            {
                explanation = "No Property Dictionary.";
                return false;
            }

            try
            {

                foreach (KeyValuePair<string, string> pair in propertyDict)
                {
                    switch (pair.Key.ToLower())
                    {
                        case "simio_objectclass":
                            simioClassType = "Object";
                            simioClass = pair.Value;
                            // No validation of value right now due to subclass complexities.
                            goto CheckForBaseClass;

                        case "simio_linkclass":
                            simioClassType = "Link";
                            simioClass = pair.Value;
                            // No validation of value right now due to subclass complexities.
                            goto CheckForBaseClass;

                        default: // Could be other properties
                            break;
                    } // switch
                } // for each property

          CheckForBaseClass:

                if ( string.IsNullOrEmpty(simioClassType))
                {
                    explanation = "No simio_objectclass or simio_linkclass found.";
                    return false;
                }

                // Find or attempt to derive a base class
                foreach (KeyValuePair<string, string> pair in propertyDict)
                {
                    switch (pair.Key.ToLower())
                    {
                        case "simio_baseclass":
                            simioBaseClass = pair.Value;
                            // No validation of value right now due to subclass complexities.
                            return true;

                        default: // Could be other properties
                            break;
                    } // switch
                } // for each property

                // No property found, so derive
                // By default, a subclass prefixes the parent class with "My",
                // So derived from Server is MyServer, and derived from MyServer is MyMyServer, ...
                if ( string.IsNullOrEmpty(simioBaseClass))
                {
                    bool foundMy = false;
                    string baseClass = simioClass.ToLower();
                    do
                    {
                        if ( baseClass.StartsWith("my"))
                        {
                            baseClass = baseClass.Substring(2);
                        }
                    } while (foundMy);
                    simioBaseClass = baseClass;
                }

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }

        }

        /// </summary>
        /// <param name="searchPath"></param>
        /// <param name="N"></param>
        /// <param name=""></param>
        private static XElement FindElementForN(XElement xeRoot, string path)
        {
            XElement result = null;
            try
            {
                int nn = path.IndexOf(",");
                string term = path.Trim();
                if (nn != -1)
                {
                    term = path.Substring(0, nn).Trim();
                }

                string[] tokens = term.Split(':');
                if (tokens.Length != 2)
                    throw new ApplicationException($"Term={term} must be colon-separated");

                string elementName = tokens[0].Trim();
                string attrName = tokens[1].Trim();
                List<XElement> xeList = xeRoot.Elements()
                    .Where(rr => rr.Name.LocalName == elementName && rr.Attribute("N")?.Value == attrName)
                    .ToList();

                if (!xeList.Any())
                    return null;

                XElement xe = xeList.First();

                if (nn != -1)
                    result = VisioHelpers.FindElementForN(xe, path.Substring(nn + 1));
                else
                {
                    result = xe;
                }
                return result;

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Path={path} Err={ex}");
            }
        }
        
        private static void logitStatic(string msg)
        {
            string xx = msg;
        }

    } // class




}
