using BehaivioursActions;
using System;

public abstract class State
{
    public Action<Enum> OnFlag;
    public abstract BehaviourActions GetOnEnterBehaviours(params object[] parameters);
    public abstract BehaviourActions GetOnExitBehaviours(params object[] parameters);
    public abstract BehaviourActions GetTickBehaviours(params object[] parameters);
}

