using System;
using FSM;

namespace _1Parcial_RTS.RTS_Entities.MIner.MinerStates
{
    public class WaitForFood : State
    {
        private Func<int> _getCurrentFood;
        private Action _addFood;
        private int _maxFoodLoad;
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            _getCurrentFood = parameters[0] as Func<int>;
            _addFood = parameters[1] as Action;
            _maxFoodLoad = Convert.ToInt32(parameters[2]);
            return default;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            behaviours.AddMultiThreadBehaviour(0, () =>
            {
                _addFood.Invoke();
            });
            behaviours.SetTransitionBehaviour(() =>
            {
                if (_getCurrentFood.Invoke() == _maxFoodLoad)
                    OnFlag?.Invoke(MinerFlags.OnFoodRecovered);
            });
            return behaviours;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            return default;
        }
    }
}