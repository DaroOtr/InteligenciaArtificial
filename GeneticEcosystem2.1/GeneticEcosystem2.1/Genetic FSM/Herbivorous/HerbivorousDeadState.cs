using BehaivioursActions;

namespace herbivorous
{
    public class HerbivorousDeadState : State
    {
        private int lives;

        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            BehaviourActions behaviour = new BehaviourActions();

            lives = (int)parameters[0];

            behaviour.SetTransition(() =>
            {
                if (lives <= 0)
                {
                    OnFlag.Invoke(HerbivoreFlags.ToCorpse);
                }
            });

            return behaviour;
        }
    }

}