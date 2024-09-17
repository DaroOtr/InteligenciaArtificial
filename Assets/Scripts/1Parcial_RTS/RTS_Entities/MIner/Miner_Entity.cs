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
        Deposit,
        Retreat,
        Eat,
        Wait
    }

    [Serializable]
    public enum MinerFlags
    {
        OnMineReach,
        OnUrbanCenterReach,
        OnMove,
        OnMine,
        OnMaxLoad,
        OnEmptyLoad,
        OnEat,
        OnWait,
        OnRetreat
    }

    public class Miner : MonoBehaviour
    {
        public Grapf<Node<Vector2Int>> _grapf;
        private FSM<MinerBehaviours, MinerFlags> _minerFsm = new FSM<MinerBehaviours, MinerFlags>();
        private uint _minerID;
        private uint _mineTime = 1;
        private uint _depositTime = 1;
        private Node<Vector2Int> _currentNode;
        private Node<Vector2Int> _destinationNode;
        private Func<int, int> _minefunction;
        private Action<int> onDepositGold;
        [SerializeField] private int _maxLoad = 15;
        [SerializeField] private int _maxMinerFood = 3;
        [SerializeField] private float minerSpeed = 0.3f;
        public int Minergold { get; private set; }
        public int Minerfood { get; private set; }
        [SerializeField] private float mineDistanceDetection = 0.5f;
        [SerializeField] private bool isminerInitialized = false;

        public void InitMiner(Grapf<Node<Vector2Int>> grapf, int nodeSeparation, Func<int, int> minefunction,
            Action<int> depositAct)
        {
            _grapf = grapf;
            _minefunction = minefunction;
            onDepositGold = depositAct;
            _currentNode = _grapf.GetNode(RtsNodeType.UrbanCenter);
            SetMinerFood(_maxMinerFood);
            SetDestination(RtsNodeType.Mine);
            SetFsmStates(nodeSeparation);
            SetFsmTransitions();
            _minerFsm.ForceState(MinerBehaviours.Walk);
            isminerInitialized = true;
        }

        private void SetFsmStates(int nodeSeparation)
        {
            Action<Node<Vector2Int>> onsetNode = SetCurrentNode;
            Action onAddGold = AddGold;
            Action<int> onDepositGold = DepositGold;
            Func<int> onGetGold = () => { return Minergold; };
            Func<Node<Vector2Int>> onGetCurrentNode = GetCurrentNode;

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
                        nodeSeparation,
                        _grapf,
                        _currentNode,
                        _destinationNode,
                        onsetNode,
                        onGetCurrentNode
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
            _minerFsm.AddBehaviour<DepositState>(MinerBehaviours.Deposit,
                onTickParameters: () =>
                {
                    return new object[]
                    {
                        Time.deltaTime,
                    };
                },
                onEnterParameters: () =>
                {
                    return new object[]
                    {
                        onDepositGold,
                        onGetGold,
                        _depositTime
                    };
                },
                onExitParameters: () =>
                {
                    return new object[]
                    {
                    };
                });
        }

        private void SetFsmTransitions()
        {
            _minerFsm.SetTransition(MinerBehaviours.Walk, MinerFlags.OnMineReach, MinerBehaviours.Mine,
                () => { Debug.Log("Minardium"); });
            _minerFsm.SetTransition(MinerBehaviours.Walk, MinerFlags.OnUrbanCenterReach, MinerBehaviours.Deposit,
                () => { Debug.Log("Depositanding"); });
            _minerFsm.SetTransition(MinerBehaviours.Mine, MinerFlags.OnMaxLoad, MinerBehaviours.Walk,
                () =>
                {
                    Debug.Log("Volvemo");
                    SetDestination(RtsNodeType.UrbanCenter);
                });
            _minerFsm.SetTransition(MinerBehaviours.Deposit, MinerFlags.OnEmptyLoad, MinerBehaviours.Walk,
                () =>
                {
                    Debug.Log("Volvemo a minar");
                    SetDestination(RtsNodeType.Mine);
                });
        }

        private void SetCurrentNode(Node<Vector2Int> newNode)
        {
            _currentNode = newNode;
        }

        private Node<Vector2Int> GetCurrentNode()
        {
            return _currentNode;
        }

        private void SetDestination(RtsNodeType nodeType)
        {
            if (!_grapf.GetNode(nodeType).IsBloqued())
                _destinationNode = _grapf.GetNode(nodeType);
        }

        private void AddGold()
        {
            Minergold += _minefunction.Invoke(_currentNode.GetNodeID());
        }

        private void DepositGold(int value)
        {
            if (Minergold >= 0)
            {
                Minergold -= value;
                onDepositGold?.Invoke(value);
            }
        }

        private void Eatfood(int value) => Minerfood -= value;
        private void SetMinerFood(int value) => Minerfood = value;

        private void Update()
        {
            if (isminerInitialized)
            {
                _minerFsm.Tick();
            }
        }
    }
}