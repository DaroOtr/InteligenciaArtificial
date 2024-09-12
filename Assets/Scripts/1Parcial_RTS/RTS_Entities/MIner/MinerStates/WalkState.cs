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
        private Vector2 _pos = new Vector2();
        private float _speed = 0.0f;
        private float _mineDistance = 0.0f;
        private float _nodeSeparation = 0.0f;
        private Node<Vector2Int> _startNode;
        private Node<Vector2Int> _destinationNode;
        private Node<Vector2Int> _currentNode;

        private AStarPathfinder<Node<Vector2Int>, Vector2Int> _pathfinder =
            new AStarPathfinder<Node<Vector2Int>, Vector2Int>();

        List<Node<Vector2Int>> _path = new List<Node<Vector2Int>>();
        private Grapf<Node<Vector2Int>> _grapf = new Grapf<Node<Vector2Int>>();

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
            });
            behaviours.AddMainThreadBehaviour(1, () =>
            {
                _startNode = _grapf.GetNode(RtsNodeType.UrbanCenter);
                ownerTransform.position = new Vector3(_startNode.GetCoordinate().x, _startNode.GetCoordinate().y);
                SetDestination();
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

            BehaviourActions behaviours = new BehaviourActions();


            //behaviours.Add(() => { });  Exprecion Lambda o Metodo Anonimo 

            behaviours.AddMainThreadBehaviour(0,
                () =>
                {
                    //ownerTransform.position = new Vector3(_pos.x,_pos.y);
                    if (_path != null && _path.Count > 0)
                    {
                        Debug.Log("Tick");
                        Vector3 aux = new Vector3(_nodeSeparation * _path[0].GetCoordinate().x,
                            _nodeSeparation * _path[0].GetCoordinate().y);

                        ownerTransform.position += (aux - ownerTransform.position).normalized * _speed * Time.deltaTime;

                        if (Vector3.Distance(ownerTransform.position, aux) < _mineDistance)
                            _path.Remove(_path[0]);
                    }
                });

            behaviours.SetTransitionBehaviour(() =>
            {
                Vector3 target = new Vector3(_destinationNode.GetCoordinate().x, _destinationNode.GetCoordinate().y);

                if (Vector3.Distance(ownerTransform.position, target) < _mineDistance)
                    Debug.Log("_finalDestination " + _destinationNode.GetCoordinate());

                //float testX = target.x - ownerTransform.position.x;
                //float testY = target.y - ownerTransform.position.y;
                //float testZ = target.z - ownerTransform.position.z;
                //
                //float sum = (testX + testY) / 2;
                //float epsilon = (Mathf.Epsilon) * 10;
                //
                //if (sum < 0)
                //    sum *= -1;
                //
                //if (sum < epsilon)
                //{
                //    Debug.Log("_finalDestination " + _destinationNode.GetCoordinate());
                //    //OnFlag?.Invoke(Flags.OnReadyToMine);
                //}

                //if (Vector2.Distance(_pos, _destinationNode.GetCoordinate()) < _mineDistance)
                //   
                //    //OnFlag?.Invoke(MinerFlags.OnWait);
            });

            return behaviours;
        }

        private void SetDestination()
        {
            _destinationNode = _grapf.GetNode(RtsNodeType.Mine);
        }

        private void CalculatePath()
        {
            _path = _pathfinder.FindPath(_startNode, _destinationNode, _grapf);
        }
    }
}