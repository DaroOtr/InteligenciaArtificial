using System;
using FSM;
using UnityEngine;

namespace _1Parcial_RTS.RTS_Entities.MIner.MinerStates
{
    public class WaitForOrdersState : State
    {
        private Func<bool> _onGetAlarmState;
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            _onGetAlarmState = parameters[0] as Func<bool>;
            return default;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            bool alarmState = _onGetAlarmState.Invoke();
            BehaviourActions behaviours = new BehaviourActions();
            behaviours.SetTransitionBehaviour(() =>
            {
                if (!alarmState)
                    OnFlag?.Invoke(MinerFlags.OnWaitingforOrders);
            });
            return behaviours;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            return default;
        }
    }
}