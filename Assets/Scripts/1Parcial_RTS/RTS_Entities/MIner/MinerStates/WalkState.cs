using System;
using System.Collections.Generic;
using FSM;
using Pathfinder.Algorithm;
using Pathfinder.Grapf;
using Pathfinder.Node;
using UnityEngine;

namespace _1Parcial_RTS.RTS_Entities.MIner.MinerStates
{
    public sealed class WalkState : State
    {
        private float _speed = 0.0f;
        private float _mineDistance = 0.0f;
        private float _nodeSeparation = 0.0f;
        private Node<Vector2Int> _startNode;
        private Node<Vector2Int> _destinationNode;

        private AStarPathfinder<Node<Vector2Int>, Vector2Int> _pathfinder =
            new AStarPathfinder<Node<Vector2Int>, Vector2Int>();

        List<Node<Vector2Int>> _path = new List<Node<Vector2Int>>();
        private Grapf<Node<Vector2Int>> _grapf = new Grapf<Node<Vector2Int>>();

        private Action<Node<Vector2Int>> _onSetCurrentnode;

        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            Transform ownerTransform = parameters[0] as Transform;

            BehaviourActions behaviours = new BehaviourActions();

            behaviours.AddMultiThreadBehaviour(0, () =>
            {
                _speed = Convert.ToSingle(parameters[1]);
                _mineDistance = Convert.ToSingle(parameters[2]);
                _nodeSeparation = Convert.ToSingle(parameters[3]);
                _grapf = parameters[4] as Grapf<Node<Vector2Int>>;
                _startNode = parameters[5] as Node<Vector2Int>;
                _destinationNode = parameters[6] as Node<Vector2Int>;
                _onSetCurrentnode = parameters[7] as Action<Node<Vector2Int>>;
            });
            behaviours.AddMainThreadBehaviour(1, () =>
            {
                ownerTransform.position = new Vector3(_startNode.GetCoordinate().x * _nodeSeparation, _startNode.GetCoordinate().y * _nodeSeparation);
                CalculatePath();
            });

            return behaviours;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            Transform ownerTransform = parameters[0] as Transform;
            Vector3 nextNode = Vector3.zero;
            Vector3 minerpos = Vector3.zero;


            BehaviourActions behaviours = new BehaviourActions();

            behaviours.AddMainThreadBehaviour(0, () =>
            {
                minerpos = ownerTransform.position;
            });

            behaviours.AddMultiThreadBehaviour(1, () =>
            {
                if (_path.Count > 0)
                {
                    nextNode = new Vector3(_nodeSeparation * _path[0].GetCoordinate().x,
                        _nodeSeparation * _path[0].GetCoordinate().y);
                    
                    _onSetCurrentnode.Invoke(_path[0]);
                    if (Vector3.Distance(minerpos, nextNode) < _mineDistance)
                        _path.Remove(_path[0]);
                }
            });

            behaviours.AddMainThreadBehaviour(2,
                () =>
                {
                    ownerTransform.position += (nextNode - ownerTransform.position).normalized * _speed * Time.deltaTime;
                });


            behaviours.SetTransitionBehaviour(() =>
            {
                if (_path.Count == 0)
                    OnFlag.Invoke(MinerFlags.OnMineReach);
            });

            return behaviours;
        }

        private void CalculatePath()
        {
            _path = _pathfinder.FindPath(_startNode, _destinationNode, _grapf);
        }
    }
}