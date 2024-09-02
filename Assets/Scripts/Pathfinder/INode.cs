using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;

public interface INode : INullable
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
}

public interface INode<Coorninate> : INode 
    where  Coorninate : IEquatable<Coorninate>
{
    public Coorninate GetCoordinate();
    public void SetCoordinate(Coorninate coordinateType);
    public void MoveTo(Coorninate coorninate);
    public float CalculateDistanceTo(Coorninate coorninate);
    public void SetDistanceMethod(Func<Coorninate, float> DistanceTo);
}
