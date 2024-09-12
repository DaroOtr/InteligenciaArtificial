using FSM;
using UnityEngine;

namespace _1Parcial_RTS.RTS_Entities.MIner.MinerStates
{
    public class MineState : State
    {
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            Debug.Log("Mine State :: Enter");
            return default;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            Debug.Log("Mine State :: Tick");
            return default;
        }
    }
}