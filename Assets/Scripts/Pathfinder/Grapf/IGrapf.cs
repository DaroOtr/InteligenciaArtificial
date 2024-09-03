using System;
using System.Collections.Generic;

namespace Pathfinder.Grapf
{
    public interface IGrapf<TNodeType> : ICollection<TNodeType>
    {
        public void SetgrapfParameters(Func<List<TNodeType>> grapfCrationMethod,Action addNodeNeighborsBehaviour);
        public void InitGrapf();
        public void AddNodeNeighbors();
        public TNodeType GetNode(int nodeId);
        public List<TNodeType> GetNodes();
        public void SetNodeCost(int nodeId,int nodeCost);
        public void SetNodeTransitionCost(int fromNodeId, int toNodeId,int transitionCost);
    }
}
