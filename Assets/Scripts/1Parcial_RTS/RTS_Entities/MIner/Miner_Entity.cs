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
        [SerializeField] private float minerSpeed = 0.3f;
        [SerializeField] private float mineDistanceDetection = 0.5f;
        [SerializeField] private bool isminerInitialized = false;

        public void InitMiner()
        {
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
                        grapfView.Grapf
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
            _minerFsm.SetTransition(MinerBehaviours.Mine, MinerFlags.OnWait, MinerBehaviours.Wait,
                () => { Debug.Log("Espero"); });
            _minerFsm.ForceState(MinerBehaviours.Walk);
            isminerInitialized = true;
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