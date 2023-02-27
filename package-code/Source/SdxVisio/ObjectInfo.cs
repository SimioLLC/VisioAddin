using SdxHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SdxVisio
{
    /// <summary>
    /// Class to hold information about objects
    /// </summary>
    public class ObjectInfo
    {
        /// <summary>
        /// the object shape id.
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// The shape we derive from (this is one-to-one)
        /// </summary>
        public ShapeInfo MyShape { get; set; }

        /// <summary>
        /// Return our shapes name
        /// </summary>
        public string Name {  get { return MyShape?.Name; } }

        /// <summary>
        /// A list of Links coming into this object
        /// </summary>
        public IList<LinkInfo> InLinkList { get; set; }

        /// <summary>
        /// The nodes associated with this object.
        /// These are links connected to this object.
        /// </summary>
        public IList<NodeInfo> NodeList { get; set; }

        /// <summary>
        /// The simio properties that were found in the ShapeData
        /// </summary>
        public Dictionary<string, string> PropertyDict { get; set; }

        /// <summary>
        /// Simio object properties, such as how many incoming
        /// and outgoing nodes are expected.
        /// </summary>

        public SimioObjectAttributes ObjectAttributes;

        /// <summary>
        /// A list of Links going out of this object
        /// </summary>
        public IList<LinkInfo> OutLinkList { get; set; }

        public ObjectInfo(SimioAttributes simProps, int id, ShapeInfo si)
        {
            this.ObjectId = id;
            this.MyShape = si;

            if ( !simProps.ObjectDict.TryGetValue(si.SimioBaseClass.ToLower(), out ObjectAttributes))
            {
                throw new ApplicationException($"Could not find Object Properties for Object={si.SimioClass}");
            }

            InLinkList = new List<LinkInfo>();
            OutLinkList = new List<LinkInfo>();
            NodeList = new List<NodeInfo>();
        }

        /// <summary>
        /// Look for a node at the given location.
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public NodeInfo FindOrAddNode(VisioVertex loc, EnumNodeType nodeType, out string explanation)
        {
            explanation = "";

            // Is it close to an existing node
            foreach ( NodeInfo ni in this.NodeList)
            {
                double distance = (new VisioLine(loc, ni.Location)).Length;
                if ( distance < 0.01 )
                { 
                    return ni;
                }
            }

            // None were found. Make a new one
            int nextIndex = this.NodeList.Where(nn => nn.NodeType == nodeType).Count();

            switch ( nodeType )
            {
                case EnumNodeType.InBound:
                    if ( nextIndex > ObjectAttributes.InNodes)
                    {
                        explanation = $"Object={ObjectAttributes.Name} Tried to exceed Max Inbound Nodes={ObjectAttributes.InNodes}";
                        return null;
                    }
                    break;
                case EnumNodeType.OutBound:
                    if (nextIndex > ObjectAttributes.OutNodes)
                    {
                        explanation = $"Object={ObjectAttributes.Name} Tried to exceed Max Outbound Nodes={ObjectAttributes.OutNodes}";
                        return null;
                    }
                    break;
                case EnumNodeType.InOrOutBound:
                    break;
            }
            NodeInfo newNode = new NodeInfo(nextIndex, loc, nodeType);
            NodeList.Add(newNode);
            return newNode;
        }

    }

    public enum EnumNodeType
    {
        UnknownType = 0,
        InBound = 1,
        OutBound = 2,
        InOrOutBound = 3
    }

    /// <summary>
    /// Nodes, that are generally attached to objects
    /// </summary>
    public class NodeInfo
    {
        /// <summary>
        /// Just a counter so that we have a reference.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Is node inbound, outbound, both, neither, ...
        /// </summary>
        public EnumNodeType NodeType { get; set; }

        /// <summary>
        /// The centerpoint of the node.
        /// </summary>
        public VisioVertex Location { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index"></param>
        /// <param name="loc"></param>
        /// <param name="nodeType"></param>
        public NodeInfo(int index, VisioVertex loc, EnumNodeType nodeType)
        {
            Index = index;
            Location = loc;
            NodeType = nodeType;
        }

        public override string ToString()
        {
            return $"Index={Index}, Type={NodeType} V={Location}";
        }
    }


}
