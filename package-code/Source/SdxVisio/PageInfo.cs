using System.Collections.Generic;
using System.Xml.Linq;

namespace SdxVisio
{
    /// <summary>
    /// Information needed for each Visio Page.
    /// The page name/number corresponds to the tab at the bottom of the diagram, which
    /// defaults to Page-1, Page-2, ...
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// 1 for Page1.xml, 2 for Page2.xml, etc.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// From Pages.xml
        /// </summary>
        public VisioSize PageSize { get; set; }

        /// <summary>
        /// The ID from the Pages XML. Does *not* correspond to the page files/
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Visio name for each page, which default to Page-1, Page-2, ...
        /// but are often set to something else by the user.
        /// These do not necessary correspond to the xml page files (page1.xml, page2.xml, ...)
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// The owning Application
        /// </summary>
        public SdxVisioApplication MyApp { get; set; }

        /// <summary>
        /// The shapes for this page.
        /// </summary>
        public Dictionary<int, ShapeInfo> ShapeDict { get; set; }

        /// <summary>
        /// How many shapes on the page.
        /// </summary>
        public int ShapeCount { get { return ShapeDict.Count; } }

        /// <summary>
        /// The connection info for this page.
        /// Keyed by the combination of FromSheet and ToSheet
        /// </summary>
        public Dictionary<string, ConnectInfo> ConnectDict { get; set; }

        /// <summary>
        /// The connection info for this page.
        /// Keyed by the Link ID
        /// </summary>
        public Dictionary<int, LinkInfo> LinkDict { get; set; }

        /// <summary>
        /// Holds object by the visio ID
        /// </summary>
        public Dictionary<int, ObjectInfo> ObjectDict { get; set; }

        public Dictionary<int, ArtifactInfo> ArtifactDict { get; set; }

        /// <summary>
        /// How many connects on the page.
        /// </summary>
        public int ConnectCount { get { return ConnectDict.Count; } }

        /// <summary>
        /// Reference the XDocument for this page.
        /// </summary>
        public XDocument XDoc { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ownerApp"></param>
        /// <param name="page"></param>
        public PageInfo(SdxVisioApplication ownerApp, int pageNbr, string pageName)
        {
            this.MyApp = ownerApp;
            this.PageNumber = pageNbr;
            this.PageName = pageName;

            ShapeDict = new Dictionary<int, ShapeInfo>();
            ConnectDict = new Dictionary<string, ConnectInfo>();
            LinkDict = new Dictionary<int, LinkInfo>();
            ObjectDict = new Dictionary<int, ObjectInfo>();
            ArtifactDict = new Dictionary<int, ArtifactInfo>();
        }

        public override string ToString()
        {
            return $"Name={PageName} #={PageNumber} Shapes={ShapeCount} Connects={ConnectCount}";
        }

        /// <summary>
        /// Reference our owner's Master Summary Dictionary
        /// </summary>
        public Dictionary<int, MasterInfo> MasterSummaryDict
        {
            get { return MyApp.MasterSummaryDict; }
        }
        /// <summary>
        /// Reference our owner's Master Summary Dictionary
        /// </summary>
        public Dictionary<int, MasterInfo> MasterTemplateDict
        {
            get { return MyApp.MasterTemplateDict; }
        }

    }



}
