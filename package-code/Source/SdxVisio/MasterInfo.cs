using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace SdxVisio
{
    /// <summary>
    /// Used to collect the Master information that we might need.
    /// </summary>
    public class MasterInfo
    {
        /// <summary>
        /// The Visio ID
        /// </summary>
        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// The name of the Simio class used for importing.
        /// It is a string of arbitrary name chosen for the import.
        /// </summary>
        public string SimioClass { get; set; }

        /// <summary>
        /// The type of class, e.g. Object, Link, ...
        /// </summary>
        public string SimioClassType { get; set; }

        /// <summary>
        /// What Simio class is this class based upon (e.g. Server, Sink, ...)
        /// </summary>
        public string SimioBaseClass { get; set; }

        /// <summary>
        /// A collection of properties.
        /// </summary>
        public Dictionary<string, string> PropertyDict = new Dictionary<string, string>();

        /// <summary>
        /// The referenceId, which points us to the individual Master template file.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Height, Width, Depth.
        /// </summary>
        public VisioSize Size { get; set; }

        /// <summary>
        /// A reference to the XElement for the Master
        /// </summary>
        public XElement XMaster { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="xe"></param>
        public MasterInfo(XElement xe)
        {
            this.XMaster = xe;
        }

        public override string ToString()
        {
            return $"Name={Name} ID={Id} Class={SimioClass} Template={TemplateId}";
        }

        /// <summary>
        /// Get data from the Master file.
        /// </summary>
        /// <param name="folderpath"></param>
        /// <param name="refId"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool GetMasterInfo(string folderpath, string refId, out string explanation)
        {
            explanation = "";
            string fullPath = Path.Combine(folderpath, $"Master{refId}.XML");

            if (!File.Exists(fullPath))
            {
                explanation = $"No such File={fullPath}";
                return false;
            }

            try
            {


                return true;
            }
            catch (Exception ex)
            {
                explanation = $"File={fullPath} Err={ex}";
                return false;
            }
        }
    }



}
