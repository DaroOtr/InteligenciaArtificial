using System.Collections.Generic;

namespace Pathfinder
{
    public interface IGrapf<TNodeType,TCoordinateType,TPathType>
    {
        public void InitGrapf();
        public void AddNodeNeighbors();
        public TNodeType GetNode(int nodeId);
        public List<TNodeType> GetNodes();
        public void SetNodeCost(int nodeId,int nodeCost);
        public void SetNodeTransitionCost(int fromNodeId, int toNodeId,int transitionCost);
    }
}
