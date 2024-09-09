using System;
using System.Collections;
using System.Collections.Generic;
using FSM;
using Pathfinder.Node;
using UnityEngine;

namespace _1Parcial_RTS.RTS_Entities.MIner.MinerStates
{
    public sealed class WalkState : State
    {
        private Vector2 _pos = new Vector2();
        private Transform _initalPos;
        private float _speed = 0.0f;
        private float _mineDistance = 0.0f;
        private float _nodeSeparation = 0.0f;
        private Vector2Int _destination = new Vector2Int();
        private Vector2Int _finalDestination = new Vector2Int();
        private Func<Vector2Int> getFinalDestination;

        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();

            behaviours.AddMultiThreadBehaviour(0, () =>
            {
                _speed = Convert.ToSingle(parameters[0]);
                _mineDistance = Convert.ToSingle(parameters[1]);
                _nodeSeparation = Convert.ToSingle(parameters[2]);
                getFinalDestination = parameters[3] as Func<Vector2Int>;
                _initalPos = parameters[4] as Transform;
            });


            return behaviours;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            Transform ownerTransform = parameters[0] as Transform;
            Func<Vector2Int> getDestination = parameters[1] as Func<Vector2Int>;
            
            BehaviourActions behaviours = new BehaviourActions();
            

            //behaviours.Add(() => { });  Exprecion Lambda o Metodo Anonimo 

            behaviours.AddMainThreadBehaviour(3,
                () =>
                {
                    //ownerTransform.position = new Vector3(_pos.x,_pos.y);
                    Vector3 newpos = new Vector3(_pos.x, _pos.y);
                    
                    ownerTransform.position = Vector3.Lerp(_initalPos.position, newpos,
                        _speed * Time.deltaTime);
                    
                    _initalPos = ownerTransform;
                });

            behaviours.AddMultiThreadBehaviour(0, () =>
            {   
                _destination = getDestination.Invoke();
            });
            
            behaviours.AddMultiThreadBehaviour(1,
                () =>
                {
                    _finalDestination = getFinalDestination.Invoke();
                });
            behaviours.AddMultiThreadBehaviour(2, () =>
            {
                _pos.x = _destination.x * _nodeSeparation;
                _pos.y = _destination.y * _nodeSeparation;
            });

            behaviours.SetTransitionBehaviour(() =>
            {
                if (Vector2.Distance(_pos, _finalDestination) < _mineDistance)
                    Debug.Log("_finalDestination " + _finalDestination);
                    //OnFlag?.Invoke(MinerFlags.OnWait);
                
            });

            return behaviours;
        }
    }
}