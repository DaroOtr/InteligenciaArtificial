using System;
using FSM;
using UnityEngine;

namespace _1Parcial_RTS.RTS_Entities.MIner.MinerStates
{
    public class DepositState : State
    {
        private Action<int> _depositGold;
        private Func<int> _getCurrentGold;
        private int _depositTime;
        private float _currentTime = 0;
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            _depositGold = parameters[0] as Action<int>;
            _getCurrentGold = parameters[1] as  Func<int>;
            _depositTime =Convert.ToInt32(parameters[2]);
            return default;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            float delta = Convert.ToSingle(parameters[0]);
            BehaviourActions behaviours = new BehaviourActions();
            behaviours.AddMultiThreadBehaviour(0, () =>
            {
                if (_currentTime >= _depositTime)
                {
                    _depositGold.Invoke(1);
                    _currentTime = 0;
                }
                _currentTime += delta;
            });
            behaviours.SetTransitionBehaviour(() =>
            {
                if (_getCurrentGold.Invoke() == 0)
                    OnFlag?.Invoke(MinerFlags.OnEmptyLoad);
            });
            return behaviours;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            behaviours.AddMultiThreadBehaviour(0, () => { _currentTime = 0; });
            return behaviours;
        }
    }
}