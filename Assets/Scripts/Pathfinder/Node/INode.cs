using System;
using System.Collections.Generic;

namespace Pathfinder.Node
{
    public interface INode 
    {
        public bool IsBloqued();
        public void SetBlock(bool blockState);
        public void SetNodeCost(int cost);
        public int GetNodeCost();
        public void SetNodeID(int ID);
        public int GetNodeID();
        public void AddNeighbor(int neighborID,int transitionCost);
        public ICollection<int> GetNeighbors();
        public int GetNeighborTransitionCost(int neighborID);
        public void SetNeighborTransitionCost(int neighborID,int transitionCost);
        public void SetNodeType(RtsNodeType nodeType);
        public RtsNodeType GetNodeType();
    }

    public interface INode<TCoorninate> : INode 
        where  TCoorninate : IEquatable<TCoorninate>
    {
        public TCoorninate GetCoordinate();
        public void SetCoordinate(TCoorninate coordinateType);
        public void MoveTo(TCoorninate coorninate);
        public float CalculateDistanceTo(TCoorninate coorninate);
        public void SetDistanceMethod(Func<TCoorninate, float> DistanceTo);
    }
}