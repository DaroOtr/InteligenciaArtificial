using System;
using System.Collections.Generic;
using _1Parcial_RTS.RTS_Entities.MIner.MinerStates;
using Pathfinder.Grapf;
using Pathfinder.Node;
using UnityEngine;

namespace _1Parcial_RTS.RTS_Entities.MIner
{
    public enum MinerBehaviours
    {
        Walk,
        Mine,
        Deposit,
        Wait,
        WaitForFood
    }

    public enum MinerFlags
    {
        OnMineReach,
        OnUrbanCenterReach,
        OnMaxLoad,
        OnEmptyLoad,
        OnEmptyMine,
        OnWaitingforOrders,
        OnFoodRecovered,
        OnEmptyStomach
    }

    public class Miner : MonoBehaviour
    {
        public Grapf<Node<Vector2Int>> _grapf;
        public int Minergold { get; private set; }
        public int Minerfood { get; private set; }
        public bool _isAlarmSounded { get; private set; }
        
        private FSM<MinerBehaviours, MinerFlags> _minerFsm = new FSM<MinerBehaviours, MinerFlags>();
        private uint _minerID;
        private uint _mineTime = 1;
        private uint _depositTime = 1;
        private Node<Vector2Int> _currentNode;
        private Node<Vector2Int> _destinationNode;
        private Func<int, int> _minefunction;
        private Func<int, int> _getGoldFunc;
        private Func<int, int> _getFoodFunc;
        private Action<int> _onDepositGold;
        
        [SerializeField] private int maxLoad = 15;
        [SerializeField] private int maxMinerFood = 3;
        [SerializeField] private float minerSpeed = 0.3f;
        [SerializeField] private float mineDistanceDetection = 0.5f;
        [SerializeField] private bool isminerInitialized = false;
        
        public void InitMiner(Grapf<Node<Vector2Int>> grapf, int nodeSeparation, Func<int, int> minefunction,
            Action<int> depositAct, Func<int, int> getGoldFunc,Func<int, int> getFoodFunc)
        {
            _grapf = grapf;
            _minefunction = minefunction;
            _onDepositGold = depositAct;
            _getGoldFunc = getGoldFunc;
            _getFoodFunc = getFoodFunc;
            _currentNode = _grapf.GetNode(RtsNodeType.UrbanCenter);
            SetMinerFood(maxMinerFood);
            GetClosestMine();
            SetFsmStates(nodeSeparation);
            SetFsmTransitions();
            _minerFsm.ForceState(MinerBehaviours.Walk);
            isminerInitialized = true;
        }

        public void TogleAlarm()
        {
            if (_destinationNode.GetNodeType() == RtsNodeType.UrbanCenter)
            {
                _isAlarmSounded = false;
                GetClosestMine();
            }
            else
            {
                _isAlarmSounded = true;
                SetDestination(RtsNodeType.UrbanCenter);
            }

            _minerFsm.ForceState(MinerBehaviours.Walk);
            Debug.LogWarning("ALARM");
        }

        private void SetFsmStates(int nodeSeparation)
        {
            Action<Node<Vector2Int>> onsetNode = SetCurrentNode;
            Action onAddGold = AddGold;
            Action onAddFood = AddMinerFood;
            Action<int> onDepositGold = DepositGold;
            Action<int> onEatFood = Eatfood;
            Func<int> onGetGold = () => { return Minergold; };
            Func<int> onGetFood = () => { return Minerfood; };
            Func<bool> onGetAlarmState = () => { return _isAlarmSounded; };
            Func<int> onGetMineGold = () => { return GetCurrentMineGold(); };
            Func<Node<Vector2Int>> onGetCurrentNode = GetCurrentNode;

            _minerFsm.Init();
            _minerFsm.AddBehaviour<WalkState>(MinerBehaviours.Walk,
                onTickParameters: () =>
                {
                    return new object[]
                    {
                        transform,
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
                        onGetCurrentNode,
                        onGetAlarmState
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
                        Time.deltaTime
                    };
                },
                onEnterParameters: () =>
                {
                    return new object[]
                    {
                        onAddGold,
                        onGetGold,
                        onGetFood,
                        _mineTime,
                        maxLoad,
                        onGetMineGold,
                        onEatFood,
                        maxMinerFood
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

            _minerFsm.AddBehaviour<WaitForOrdersState>(MinerBehaviours.Wait,
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
                        onGetAlarmState,
                    };
                },
                onExitParameters: () =>
                {
                    return new object[]
                    {
                    };
                });
            
            _minerFsm.AddBehaviour<WaitForFood>(MinerBehaviours.WaitForFood,
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
                        onGetFood,
                        onAddFood,
                        maxMinerFood
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
            _minerFsm.SetTransition(MinerBehaviours.Walk, MinerFlags.OnWaitingforOrders, MinerBehaviours.Wait,
                () => { Debug.Log("Esperando Ordenes"); });
            _minerFsm.SetTransition(MinerBehaviours.Wait, MinerFlags.OnWaitingforOrders, MinerBehaviours.Walk,
                () => { Debug.Log("Volvemos A laburar"); });
            _minerFsm.SetTransition(MinerBehaviours.WaitForFood, MinerFlags.OnFoodRecovered, MinerBehaviours.Mine,
                () => { Debug.Log("Food Recovered"); });
            _minerFsm.SetTransition(MinerBehaviours.Mine, MinerFlags.OnEmptyStomach, MinerBehaviours.WaitForFood,
                () => { Debug.Log("huelga"); });
            _minerFsm.SetTransition(MinerBehaviours.Mine, MinerFlags.OnMaxLoad, MinerBehaviours.Walk,
                () =>
                {
                    Debug.Log("Volvemo");
                    SetDestination(RtsNodeType.UrbanCenter);
                });
            _minerFsm.SetTransition(MinerBehaviours.Mine, MinerFlags.OnEmptyMine, MinerBehaviours.Walk,
                () =>
                {
                    Debug.Log("Vaciamo Todo");
                    GetClosestMine();
                });
            _minerFsm.SetTransition(MinerBehaviours.Deposit, MinerFlags.OnEmptyLoad, MinerBehaviours.Walk,
                () =>
                {
                    Debug.Log("Volvemo a minar");
                    GetClosestMine();
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
                _onDepositGold?.Invoke(value);
            }
        }

        private int GetCurrentMineGold()
        {
            return (int)_getGoldFunc?.Invoke(_currentNode.GetNodeID());
        }

        private int GetCurrentMineGold(int mineIndex)
        {
            return (int)_getGoldFunc?.Invoke(mineIndex);
        }

        private void GetClosestMine()
        {
            float distance = float.MaxValue;
            int closestMineID = -1;
            ICollection<Node<Vector2Int>> _mines = _grapf.GetNodesOfType(RtsNodeType.Mine);
            if (_mines != null && _mines.Count > 0)
            {
                foreach (Node<Vector2Int> mine in _mines)
                {
                    if (mine != _currentNode && GetCurrentMineGold(mine.GetNodeID()) > 0)
                    {
                        Vector3 minePos = new Vector3(mine.GetCoordinate().x, mine.GetCoordinate().y);
                        if (Vector3.Distance(transform.position, minePos) < distance)
                        {
                            distance = Vector3.Distance(transform.position, minePos);
                            closestMineID = mine.GetNodeID();
                        }
                    }
                }

                if (closestMineID != -1)
                    _destinationNode = _grapf.GetNode(closestMineID);
                else
                    SetDestination(RtsNodeType.UrbanCenter);
            }
            else
            {
                SetDestination(RtsNodeType.UrbanCenter);
            }
        }

        private void Eatfood(int value) => Minerfood -= value;

        private void SetMinerFood(int value)
        {
            Minerfood = value;
        }

        private void AddMinerFood()
        {
           Minerfood += _getFoodFunc.Invoke(_currentNode.GetNodeID());
        }

        private void Update()
        {
            if (isminerInitialized)
            {
                _minerFsm.Tick();
            }
        }
    }
}