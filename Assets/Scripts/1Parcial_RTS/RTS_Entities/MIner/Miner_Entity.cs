using System;
using System.Collections;
using System.Collections.Generic;
using _1Parcial_RTS.RTS_Entities.MIner.MinerStates;
using Pathfinder.Algorithm;
using Pathfinder.Grapf;
using Pathfinder.Node;
using UnityEngine;
using UnityEngine.Serialization;

namespace _1Parcial_RTS.RTS_Entities.MIner
{
    public enum MinerBehaviours
    {
        Walk,
        Mine,
        Retreat,
        Eat,
        Wait
    }

    public enum MinerFlags
    {
        OnMineReach,
        OnMove,
        OnMine,
        OnEat,
        OnWait,
        OnRetreat
    }

    public class Miner : MonoBehaviour
    {
        private AStarPathfinder<Node<Vector2Int>, Vector2Int> _pathfinder =
            new AStarPathfinder<Node<Vector2Int>, Vector2Int>();

        List<Node<Vector2Int>> _path = new List<Node<Vector2Int>>();
        public GrapfView grapfView;
        private Node<Vector2Int> _startNode;
        private Node<Vector2Int> _destinationNode;
        private Node<Vector2Int> _currentNode;
        private Vector2 minerPos;
        private FSM<MinerBehaviours, MinerFlags> _minerFsm = new FSM<MinerBehaviours, MinerFlags>();
        private uint _minerID;
        private int _currentNodeIndex = 0;
        [SerializeField] private float minerSpeed = 0.3f;
        [SerializeField] private float mineDistanceDetection = 0.5f;
        [SerializeField] private bool isminerInitialized = false;

        public void InitMiner()
        {
            SetStarterNode(grapfView.Grapf.GetNode(RtsNodeType.UrbanCenter));
            SetInitialPos();
            FindDestination();
            CalculatePath();
            
            Func<Vector2Int> GetDestinationCoords = () =>
            {
                if (_path[_currentNodeIndex] == _destinationNode)
                    return _destinationNode.GetCoordinate();
                
                if (minerPos == _path[_currentNodeIndex].GetCoordinate())
                {
                    _currentNodeIndex++;
                }
                
                return _path[_currentNodeIndex].GetCoordinate();
            };
            
            Func<Vector2Int> GetFinalDestinationCoords = () =>
            {
                return _destinationNode.GetCoordinate();
            };
            
            _minerFsm.Init();
            _minerFsm.AddBehaviour<WalkState>(MinerBehaviours.Walk,
                onTickParameters: () =>
                {
                    return new object[]
                    {
                        transform,
                        GetDestinationCoords,
                    };
                },
                onEnterParameters: () =>
                {
                    return new object[]
                    {
                        minerSpeed,
                        mineDistanceDetection,
                        grapfView._nodeSeparation,
                        GetFinalDestinationCoords,
                        transform
                    };
                },
                onExitParameters: () =>
                {
                    return new object[]
                    {
                    };
                });
            _minerFsm.SetTransition(MinerBehaviours.Walk, MinerFlags.OnWait, MinerBehaviours.Wait,
                () => { Debug.Log("Espero"); });
            _minerFsm.ForceState(MinerBehaviours.Walk);
            isminerInitialized = true;
        }

        private void Update()
        {
            if (isminerInitialized)
            {
                minerPos.y = transform.position.y;
                minerPos.x = transform.position.x;
                _minerFsm.Tick();
            }
        }

        private void SetInitialPos()
        {
            Vector3 newPos = new Vector3(_startNode.GetCoordinate().x * grapfView._nodeSeparation,
                _startNode.GetCoordinate().y * grapfView._nodeSeparation);
            transform.position = newPos;
            _currentNode = _startNode;
        }

        private void CalculatePath()
        {
            _path = _pathfinder.FindPath(_startNode, _destinationNode, grapfView.Grapf);
        }

        private void SetStarterNode(Node<Vector2Int> newStarterNode)
        {
            _startNode = newStarterNode;
        }

        private void SetDestinationNode(Node<Vector2Int> newDestinationNode)
        {
            _destinationNode = newDestinationNode;
        }

        private void FindDestination()
        {
            _destinationNode = grapfView.Grapf.GetNode(RtsNodeType.Mine);
        }
    }
}