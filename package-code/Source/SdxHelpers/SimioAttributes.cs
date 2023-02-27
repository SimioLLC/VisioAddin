using System;
using System.Collections.Generic;
using System.Text;

namespace SdxHelpers
{
    /// <summary>
    /// Contains a list of attributes used by SDX to handle and display Simio objects.
    /// These are scalar attributes
    /// such as SourceType and SourcePath, plus also lists of attributes
    /// for objects (Server, Source, Sink, ...)
    /// and links (Connector, Path, TimePath, ...)
    /// </summary>
    public class SimioAttributes
    {
        public List<string> AttributeList { get; set; }

        /// <summary>
        /// A dictionary of Simio base classes for objects
        /// </summary>
        public Dictionary<string, SimioObjectAttributes> ObjectDict { get; set; }

        /// <summary>
        /// A dictionary of Simio base classes for links
        /// </summary>
        public Dictionary<string, SimioLinkAttributes> LinkDict { get; set; }


        public SimioAttributes()
        {
            AttributeList = new List<string>();

            AttributeList.Add("SdxVersion");
            AttributeList.Add("ModifiedBy");
            AttributeList.Add("ModifiedAt");
            AttributeList.Add("DataSetType");
            AttributeList.Add("SourceType");
            AttributeList.Add("SourcePath");
            AttributeList.Add("SimioOffset");
            AttributeList.Add("BoundingBox");
            AttributeList.Add("Comments");

            // Create a dictionary of Simio base classes
            ObjectDict = new Dictionary<string, SimioObjectAttributes>();

            SimioObjectAttributes obj;
            obj = new SimioObjectAttributes { Name = "Source", InNodes = 0, OutNodes = 1 };
            ObjectDict.Add(obj.Name.ToLower(), obj);
            obj = new SimioObjectAttributes { Name = "Server", InNodes = 1, OutNodes = 1 };
            ObjectDict.Add(obj.Name.ToLower(), obj);
            obj = new SimioObjectAttributes { Name = "Sink", InNodes = 1, OutNodes = 0 };
            ObjectDict.Add(obj.Name.ToLower(), obj);
            obj = new SimioObjectAttributes { Name = "Separator", InNodes = 1, OutNodes = 2 };
            ObjectDict.Add(obj.Name.ToLower(), obj);
            obj = new SimioObjectAttributes { Name = "Combiner", InNodes = 2, OutNodes = 1 };
            ObjectDict.Add(obj.Name.ToLower(), obj);
            obj = new SimioObjectAttributes { Name = "BasicNode", InNodes = int.MaxValue, OutNodes = int.MaxValue };
            ObjectDict.Add(obj.Name.ToLower(), obj);
            obj = new SimioObjectAttributes { Name = "TransferNode", InNodes = int.MaxValue, OutNodes = int.MaxValue };
            ObjectDict.Add(obj.Name.ToLower(), obj);

            obj = new SimioObjectAttributes { Name = "Resource", InNodes = 0, OutNodes = 0 };
            ObjectDict.Add(obj.Name.ToLower(), obj);
            obj = new SimioObjectAttributes { Name = "Worker", InNodes = 0, OutNodes = 0 };
            ObjectDict.Add(obj.Name.ToLower(), obj);
            obj = new SimioObjectAttributes { Name = "Vehicle", InNodes = 0, OutNodes = 0 };
            ObjectDict.Add(obj.Name.ToLower(), obj);

            // Link base classes
            LinkDict = new Dictionary<string, SimioLinkAttributes>();

            SimioLinkAttributes link;
            link = new SimioLinkAttributes { Name = "Connector" };
            LinkDict.Add(link.Name.ToLower(), link);
            link = new SimioLinkAttributes { Name = "Path" };
            LinkDict.Add(link.Name.ToLower(), link);
            link = new SimioLinkAttributes { Name = "TimePath" };
            LinkDict.Add(link.Name.ToLower(), link);
            link = new SimioLinkAttributes { Name = "Conveyor" };
            LinkDict.Add(link.Name.ToLower(), link);


        }
    }

    /// <summary>
    /// Specific attributes specific to Simio smart objects
    /// </summary>
    public struct SimioObjectAttributes
    {
        public string Name { get; set; }
        public int InNodes { get; set; }
        public int OutNodes { get; set; }
    }

    /// <summary>
    /// Specific attributes specific to Simio links.
    /// </summary>
    public struct SimioLinkAttributes
    {
        public string Name { get; set; }
    }
}
