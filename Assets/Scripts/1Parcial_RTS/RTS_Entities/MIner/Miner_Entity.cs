using System;
using System.Collections;
using System.Collections.Generic;
using _1Parcial_RTS.RTS_Entities.MIner.MinerStates;
using Pathfinder.Algorithm;
using Pathfinder.Grapf;
using Pathfinder.Node;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace _1Parcial_RTS.RTS_Entities.MIner
{
    [Serializable]
    public enum MinerBehaviours
    {
        Walk,
        Mine,
        Retreat,
        Eat,
        Wait
    }

    [Serializable]
    public enum MinerFlags
    {
        OnMineReach,
        OnMove,
        OnMine,
        OnMaxLoad,
        OnEat,
        OnWait,
        OnRetreat
    }

    public class Miner : MonoBehaviour
    {
        public GrapfView grapfView;
        private FSM<MinerBehaviours, MinerFlags> _minerFsm = new FSM<MinerBehaviours, MinerFlags>();
        private uint _minerID;
        private int _currentNodeIndex = 0;
        private uint _mineTime = 1;
        private Node<Vector2Int> _currentNode;
        private Node<Vector2Int> _destinationNode;
        [SerializeField] private int _maxLoad = 15;
        [SerializeField] private float minerSpeed = 0.3f;
        public int Minergold { get; private set; }
        [SerializeField] private float mineDistanceDetection = 0.5f;
        [SerializeField] private bool isminerInitialized = false;

        public void InitMiner()
        {
            _currentNode = grapfView.Grapf.GetNode(RtsNodeType.UrbanCenter);
            SetDestination(RtsNodeType.Mine);
            Action<Node<Vector2Int>> OnsetNode = SetCurrentNode;
            Action<int> onAddGold = AddGold;
            Func<int> onGetGold = () => { return Minergold;};
            _minerFsm.Init();
            _minerFsm.AddBehaviour<WalkState>(MinerBehaviours.Walk,
                onTickParameters: () =>
                {
                    return new object[]
                    {
                        transform
                    };
                },
                onEnterParameters: () =>
                {
                    return new object[]
                    {
                        transform,
                        minerSpeed,
                        mineDistanceDetection,
                        grapfView._nodeSeparation,
                        grapfView.Grapf,
                        _currentNode,
                        _destinationNode,
                        OnsetNode
                    };
                },
                onExitParameters: () =>
                {
                    return new object[]
                    {
                    };
                });
            _minerFsm.AddBehaviour<MineState>(MinerBehaviours.Mine,
                onTickParameters: () =>
                {
                    return new object[]
                    {

                    };
                },
                onEnterParameters: () =>
                {
                    return new object[]
                    {
                        onAddGold,
                        onGetGold,
                        _mineTime,
                        _maxLoad
                    };
                },
                onExitParameters: () =>
                {
                    return new object[]
                    {
                    };
                });


            _minerFsm.SetTransition(MinerBehaviours.Walk, MinerFlags.OnMineReach, MinerBehaviours.Mine,
                () => { Debug.Log("Minardium"); });
            _minerFsm.SetTransition(MinerBehaviours.Mine, MinerFlags.OnMaxLoad, MinerBehaviours.Walk,
                () =>
                {
                    Debug.Log("Volvemo");
                    SetDestination(RtsNodeType.UrbanCenter);
                });
            _minerFsm.ForceState(MinerBehaviours.Walk);
            isminerInitialized = true;
        }

        private void SetCurrentNode(Node<Vector2Int> newNode)
        {
            _currentNode = newNode;
        }

        private void SetDestination(RtsNodeType nodeType)
        {
            _destinationNode = grapfView.Grapf.GetNode(nodeType);
        }

        private void AddGold(int value) => Minergold += value;

        private void Update()
        {
            if (isminerInitialized)
            {
                _minerFsm.Tick();
            }
        }
    }
}