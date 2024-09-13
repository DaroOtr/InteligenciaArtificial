using System;
using FSM;

namespace _1Parcial_RTS.RTS_Entities.MIner.MinerStates
{
    public class MineState : State
    {
        private int currentGold;
        private Action<int> onMining;
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            
            behaviours.AddMultiThreadBehaviour(0, () =>
            {
                onMining = parameters[0] as Action<int>;
                currentGold = Convert.ToInt32(parameters[1]);
            });
            
            return behaviours;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            
            behaviours.AddMultiThreadBehaviour(0, () =>
            {
                // Add Timer
                // Mine
                //On end mining Go to Urban Center
            });
            
            return behaviours;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            return default;
        }
    }
}