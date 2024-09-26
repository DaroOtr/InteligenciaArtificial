using System;
using _1Parcial_RTS.RTS_Entities.MIner;
using _1Parcial_RTS.RTS_Entities.MIner.MinerStates;
using Pathfinder.Grapf;
using Pathfinder.Node;
using UnityEngine;
using UnityEngine.Serialization;

namespace _1Parcial_RTS.RTS_Entities.Caravan
{
    public enum CaravanBehaviours
    {
        Walk,
        Deposit,
        ReStock,
        Wait
    }

    public enum CaravanFlags
    {
        OnMineReach,
        OnUrbanCenterReach,
        OnMaxLoad,
        OnEmptyLoad,
        OnWaitingforOrders
    }

    public class Caravan : MonoBehaviour
    {
        public bool IsAlarmSounded { get; private set; }
        public Grapf<Node<Vector2Int>> Grapf;
        public int Caravanfood { get; private set; }
        [SerializeField] private uint depositTime = 1;
        [SerializeField] private float caravanSpeed = 0.3f;
        [SerializeField] private float nodeDistanceDetection = 0.5f;
        [SerializeField] private bool isCaravanInitialized = false;
        [SerializeField] private int maxLoad = 10;
        private Node<Vector2Int> _currentNode;
        private Node<Vector2Int> _destinationNode;
        private Action<int> _onDepositFood;
        private FSM<CaravanBehaviours, CaravanFlags> _caravanFsm = new FSM<CaravanBehaviours, CaravanFlags>();

        public void InitCaravan(Grapf<Node<Vector2Int>> grapf, int nodeSeparation,
            Action<int> depositAct)
        {
            Grapf = grapf;
            _onDepositFood = depositAct;
            Caravanfood = maxLoad;
            _currentNode = Grapf.GetNode(RtsNodeType.UrbanCenter);
            SetDestination(RtsNodeType.Mine);
            SetFsmStates(nodeSeparation);
            SetFsmTransitions();
            _caravanFsm.ForceState(CaravanBehaviours.Walk);
            isCaravanInitialized = true;
        }

        private void SetFsmStates(int nodeSeparation)
        {
            Action<Node<Vector2Int>> onsetNode = SetCurrentNode;
            Func<Node<Vector2Int>> onGetCurrentNode = GetCurrentNode;
            Action<int> onDepositFood = DepositFood;
            Func<int> onGetFood = () => { return Caravanfood; };
            Func<bool> onGetAlarmState = () => { return IsAlarmSounded; };

            _caravanFsm.Init();
            _caravanFsm.AddBehaviour<WalkState>(CaravanBehaviours.Walk,
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
                        caravanSpeed,
                        nodeDistanceDetection,
                        nodeSeparation,
                        Grapf,
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
            _caravanFsm.AddBehaviour<DepositState>(CaravanBehaviours.Deposit,
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
                        onDepositFood,
                        onGetFood,
                        depositTime
                    };
                },
                onExitParameters: () =>
                {
                    return new object[]
                    {
                    };
                });

            _caravanFsm.AddBehaviour<WaitForOrdersState>(CaravanBehaviours.Wait,
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
                        onGetAlarmState
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
            _caravanFsm.SetTransition(CaravanBehaviours.Walk, CaravanFlags.OnMineReach, CaravanBehaviours.Deposit,
                () => { Debug.Log("Depositando Morfi"); });
            _caravanFsm.SetTransition(CaravanBehaviours.Walk, CaravanFlags.OnUrbanCenterReach,
                CaravanBehaviours.Walk,
                () =>
                {
                    Caravanfood = maxLoad;
                    Debug.Log("Morfi Cargado");
                    SetDestination(RtsNodeType.Mine);
                });
            _caravanFsm.SetTransition(CaravanBehaviours.Deposit, CaravanFlags.OnEmptyLoad, CaravanBehaviours.Walk,
                () =>
                {
                    Debug.Log("Volviendo por Morfi");
                    SetDestination(RtsNodeType.UrbanCenter);
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

        private void DepositFood(int value)
        {
            if (Caravanfood >= 0)
            {
                Caravanfood -= value;
                _onDepositFood?.Invoke(value);
                Debug.Log("Depositando : " + value);
            }
        }

        private void SetDestination(RtsNodeType nodeType)
        {
            if (!Grapf.GetNode(nodeType).IsBloqued())
                _destinationNode = Grapf.GetNode(nodeType);
        }

        public void TogleAlarm()
        {
            if (_destinationNode.GetNodeType() == RtsNodeType.UrbanCenter)
            {
                IsAlarmSounded = false;
                SetDestination(RtsNodeType.Mine);
            }
            else
            {
                IsAlarmSounded = true;
                SetDestination(RtsNodeType.UrbanCenter);
            }

            _caravanFsm.ForceState(CaravanBehaviours.Walk);
            Debug.LogWarning("ALARM");
        }

        private void Update()
        {
            if (isCaravanInitialized)
            {
                _caravanFsm.Tick();
            }
        }
    }
}