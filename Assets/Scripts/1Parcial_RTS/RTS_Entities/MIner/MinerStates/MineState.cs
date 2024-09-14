using System;
using FSM;
using UnityEngine;

namespace _1Parcial_RTS.RTS_Entities.MIner.MinerStates
{
    public class MineState : State
    {
        private Action<int> _addGold;
        private Func<int> _getCurrentGold;
        private int _mineTime;
        private int _maxLoad;
        private float _currentTime = 0;
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            
            behaviours.AddMultiThreadBehaviour(0, () =>
            {
                _addGold = parameters[0] as Action<int>;
                _getCurrentGold = parameters[1] as Func<int>;
                _mineTime = Convert.ToInt32(parameters[2]);
                _maxLoad = Convert.ToInt32(parameters[3]);
                _currentTime = 0;
            });
            
            return behaviours;
        }
        
        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            
            behaviours.AddMultiThreadBehaviour(0, () =>
            {
                if (_currentTime >= _mineTime)
                {
                    _addGold.Invoke(1);
                    Debug.Log("Current Gold : " + _getCurrentGold.Invoke());
                    _currentTime = 0;
                }
            });
            behaviours.AddMainThreadBehaviour(0, () =>
            {
                _currentTime += Time.deltaTime;
            });
            behaviours.SetTransitionBehaviour(() =>
            {
                if (_getCurrentGold.Invoke() == _maxLoad)
                    OnFlag.Invoke(MinerFlags.OnMaxLoad);
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