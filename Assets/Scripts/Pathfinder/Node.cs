using System;
using System.Collections.Generic;

namespace Pathfinder
{
    [Serializable]
    public class Node<TCoordinate> : INode<TCoordinate>, IEquatable<Node<TCoordinate>>
        where TCoordinate : IEquatable<TCoordinate>
    {
        private bool _bloqued;
        private ICollection<INode<TCoordinate>> _neighbors = new List<INode<TCoordinate>>();
        private IDictionary<int, int> _transitionCost = new Dictionary<int, int>();
        private Func<TCoordinate, float> _distanceTo;
        private TCoordinate _coordinate;
        private int _nodeCost;
        private int _nodeID;
        

        public void AddNeighbor(int neighborID, int transitionCost)
        {
            _transitionCost.TryAdd(neighborID, transitionCost);
        }

        public ICollection<int> GetNeighbors()
        {
            return _transitionCost.Keys;
        }

        public int GetNeighborTransitionCost(int neighborID)
        {
            _transitionCost.TryGetValue(neighborID, out var cost);
            return cost;
        }

        public void SetNeighborTransitionCost(int neighborID,int transitionCost)
        {
            if (_transitionCost.ContainsKey(neighborID))
                _transitionCost[neighborID] = transitionCost;
        }

        public void MoveTo(TCoordinate coorninate)
        {
            _coordinate = coorninate;
        }

        public void SetDistanceMethod(Func<TCoordinate, float> DistanceTo)
        {
            _distanceTo = DistanceTo;
        }

        public float CalculateDistanceTo(TCoordinate coorninate)
        {
            return _distanceTo(coorninate);
        }

        public void SetCoordinate(TCoordinate coordinate)
        {
            _coordinate = coordinate;
        }

        public TCoordinate GetCoordinate()
        {
            return _coordinate;
        }

        public bool IsBloqued()
        {
            return _bloqued;
        }

        public void SetBlock(bool blockState)
        {
            _bloqued = blockState;
        }

        public void SetNodeCost(int cost)
        {
            _nodeCost = cost;
        }

        public int GetNodeCost()
        {
            return _nodeCost;
        }

        public void SetNodeID(int id)
        {
            _nodeID = id;
        }

        public int GetNodeID()
        {
            return _nodeID;
        }

        public bool Equals(Node<TCoordinate> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TCoordinate>.Default.Equals(_coordinate, other._coordinate) &&
                   _nodeCost == other._nodeCost && _bloqued == other._bloqued && Equals(_neighbors, other._neighbors);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Node<TCoordinate>)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_coordinate, _nodeCost, _bloqued, _neighbors);
        }

        public bool IsNull { get; }
    }
}