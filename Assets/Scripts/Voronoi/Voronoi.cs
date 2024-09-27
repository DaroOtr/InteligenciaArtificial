using System;
using System.Collections.Generic;
using Pathfinder.Grapf;
using Pathfinder.Node;

namespace Voronoi
{
    public class Voronoi<TNodeType,TCoordinate> : Ivoronoid<TNodeType,TCoordinate> 
        where TCoordinate : IEquatable<TCoordinate> 
        where TNodeType : INode<TCoordinate>, new()
    {
        private List<MinesHolder> mines = new List<MinesHolder>();
        private Grapf<TNodeType> _grapf = new Grapf<TNodeType>();
        private Func<List<MinesHolder>> _creator;
        private Func<TNodeType,TCoordinate> _closestInterestPointFunc;
        private Action<MinesHolder,MinesHolder> _calculateSidesFunc;
        private Action _drawFunc;
        public int _grapfMaxWidth;
        public int _grapfMaxHeight;


        public void SetVoronoidCreationParams(params object[] parameters)
        {
            _grapfMaxWidth = Convert.ToInt32(parameters[0]);
            _grapfMaxHeight = Convert.ToInt32(parameters[1]);
            _grapf = parameters[2] as Grapf<TNodeType>;
            _creator = parameters[3] as Func<List<MinesHolder>>;
            _closestInterestPointFunc = parameters[4] as Func<TNodeType,TCoordinate>;
            _calculateSidesFunc = parameters[5] as Action<MinesHolder,MinesHolder>;
            _drawFunc = parameters[6] as Action;
        }

        public void InitVoronoid()
        {
            mines = _creator?.Invoke();
        }

        public TCoordinate GetClosestInterestPoint(TNodeType point)
        {
            return _closestInterestPointFunc.Invoke(point);
        }

        public void ReCalculateVoronoiMap()
        {
            mines.Clear();
            _creator?.Invoke();
        }

        public void CalculateSide(MinesHolder A, MinesHolder B)
        {
            _calculateSidesFunc?.Invoke(A,B);
        }

        public void DrawVoronoi()
        {
            _drawFunc?.Invoke();
        }

        public Grapf<TNodeType> GetVoronoiGrapf()
        {
            throw new NotImplementedException();
        }
    }
}