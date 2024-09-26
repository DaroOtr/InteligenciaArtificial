using System;
using FSM;
using UnityEngine;

namespace _1Parcial_RTS.RTS_Entities.MIner.MinerStates
{
    public class MineState : State
    {
        private Action _addGold;
        private Action _recalculatePath;
        private Action<int> _onEatFood;
        private Func<int> _getCurrentGold;
        private Func<int> _getCurrentFood;
        private Func<int> _getFoodGold;
        private Func<int> _getGoldFunc;
        private int _mineTime;
        private int _maxLoad;
        private int _maxGoldMinedToEat;
        private int _currentGoldMined = 0;
        private float _currentTime = 0;
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            
            behaviours.AddMultiThreadBehaviour(0, () =>
            {
                _addGold = parameters[0] as Action;
                _getCurrentGold = parameters[1] as Func<int>;
                _getCurrentFood = parameters[2] as Func<int>;
                _mineTime = Convert.ToInt32(parameters[3]);
                _maxLoad = Convert.ToInt32(parameters[4]);
                _getGoldFunc = parameters[5] as Func<int>;
                _onEatFood = parameters[6] as Action<int>;
                _maxGoldMinedToEat = Convert.ToInt32(parameters[7]);
                _currentTime = 0;
                _currentGoldMined = 0;
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
                    _currentGoldMined++;
                    _currentTime = 0;
                }
                if ((_currentGoldMined == _maxGoldMinedToEat) && (_getCurrentFood.Invoke() > 0))
                {
                    _onEatFood.Invoke(1);
                    _currentGoldMined = 0;
                }
                
                _currentTime += delta;
            });
            
            behaviours.SetTransitionBehaviour(() =>
            {
               
                
                if (_getCurrentGold.Invoke() == _maxLoad)
                    OnFlag?.Invoke(MinerFlags.OnMaxLoad);
                
                if (_getGoldFunc?.Invoke() == 0)
                    OnFlag?.Invoke(MinerFlags.OnEmptyMine);
                
                if (_getCurrentFood?.Invoke() <= 0)
                    OnFlag?.Invoke(MinerFlags.OnEmptyStomach);
            });
            return behaviours;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            
            behaviours.AddMultiThreadBehaviour(0, () =>
            {
            });
            
            return behaviours;
        }
        
       
    }
}