using System;
using Pathfinder.Grapf;
using Pathfinder.Node;

namespace Voronoi
{
    public interface Ivoronoid<TNodeType,TCoordinate> where TNodeType : INode<TCoordinate>, new() 
        where TCoordinate : IEquatable<TCoordinate>
    {
        public void SetVoronoidCreationParams(params object[] parameters);
        public void InitVoronoid();
        public TCoordinate GetClosestInterestPoint(TNodeType point);
        public void ReCalculateVoronoiMap();
        public void CalculateSide(MinesHolder A, MinesHolder B);
        public void DrawVoronoi();
        public Grapf<TNodeType> GetVoronoiGrapf();
    }
}