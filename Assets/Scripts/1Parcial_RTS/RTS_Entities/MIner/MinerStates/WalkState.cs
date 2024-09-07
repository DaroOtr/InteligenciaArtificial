using System;
using FSM;
using UnityEngine;

namespace _1Parcial_RTS.RTS_Entities.MIner.MinerStates
{
    public class WalkState : State
    {
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            Transform ownerTransform = parameters[0] as Transform;
            Transform targetTransform = parameters[1] as Transform;

            float speed = Convert.ToSingle(parameters[2]);
            float explodeDistance = Convert.ToSingle(parameters[3]);
            float lostDistance = Convert.ToSingle(parameters[4]);

            BehaviourActions behaviours = new BehaviourActions();

            //behaviours.Add(() => { });  Exprecion Lambda o Metodo Anonimo 

            behaviours.AddMainThreadBehaviour(0,
                () =>
                {
                    ownerTransform.position += (targetTransform.position - ownerTransform.position).normalized * speed *
                                               Time.deltaTime;
                });

            behaviours.AddMultiThreadBehaviour(1, () => { Debug.Log("Whistle"); });

            behaviours.SetTransitionBehaviour(() =>
            {
                if (Vector3.Distance(targetTransform.position, ownerTransform.position) < explodeDistance)
                    OnFlag?.Invoke(MinerFlags.OnMineReach);

                else if (Vector3.Distance(targetTransform.position, ownerTransform.position) > lostDistance)
                    OnFlag?.Invoke(MinerFlags.OnRetreat);
            });

            return behaviours;
        }
    }
}