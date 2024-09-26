using System;
using FSM;
using UnityEngine;

namespace _1Parcial_RTS.RTS_Entities.MIner.MinerStates
{
    public class MineState : State
    {
        private Action _addGold;
        private Action _recalculatePath;
        private Func<int> _getCurrentGold;
        private Func<int> _getGoldFunc;
        private int _mineTime;
        private int _maxLoad;
        private float _currentTime = 0;
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            
            behaviours.AddMultiThreadBehaviour(0, () =>
            {
                _addGold = parameters[0] as Action;
                _getCurrentGold = parameters[1] as Func<int>;
                _mineTime = Convert.ToInt32(parameters[2]);
                _maxLoad = Convert.ToInt32(parameters[3]);
                _currentTime = 0;
                _getGoldFunc = parameters[4] as Func<int>;
            });
            
            return behaviours;
        }
        
        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            float delta = Convert.ToSingle(parameters[0]);
            BehaviourActions behaviours = new BehaviourActions();
            
            behaviours.AddMultiThreadBehaviour(0, () =>
            {
                if (_currentTime >= _mineTime)
                {
                    _addGold?.Invoke();
                    _currentTime = 0;
                }

                _currentTime += delta;
            });
            
            behaviours.SetTransitionBehaviour(() =>
            {
                if (_getCurrentGold.Invoke() == _maxLoad)
                    OnFlag?.Invoke(MinerFlags.OnMaxLoad);
                
                if (_getGoldFunc?.Invoke() == 0)
                    OnFlag?.Invoke(MinerFlags.OnEmptyMine);
            });
            return behaviours;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            
            behaviours.AddMultiThreadBehaviour(0, () =>
            {
                _currentTime = 0;
            });
            
            return behaviours;
        }
        
       
    }
}