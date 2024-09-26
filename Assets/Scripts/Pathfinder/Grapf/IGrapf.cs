using System;
using System.Collections.Generic;
using Pathfinder.Node;

namespace Pathfinder.Grapf
{
    public interface IGrapf<TNodeType> : ICollection<TNodeType>
    {
        public void SetgrapfParameters(Func<List<TNodeType>> grapfCrationMethod,Action addNodeNeighborsBehaviour,int grapfMaxWidth,int grapfMaxHeight);
        public void InitGrapf();
        public void AddNodeNeighbors();
        public TNodeType GetNode(int nodeId);
        public TNodeType GetNode(RtsNodeType nodeType);
        public List<TNodeType> GetNodes();
        public void SetNodeCost(int nodeId,int nodeCost);
        public void SetNodeTransitionCost(int fromNodeId, int toNodeId,int transitionCost);
    }

    public interface IGrapf<TNodeType,TCoordinateType> : IGrapf<TNodeType>
    where TCoordinateType : IEquatable<TCoordinateType>
    {
        public TNodeType GetNode(TCoordinateType nodeType);
    }
}
