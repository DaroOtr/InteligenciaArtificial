using BehaivioursActions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FSM<EnumState, EnumFlag>
    where EnumState : Enum
    where EnumFlag : Enum
{
    public int currentState = 0;
    private int counter = 0;
    private Dictionary<int, State> behaviour;
    private Dictionary<int, Func<object[]>> behaviourTickParameters;
    private Dictionary<int, Func<object[]>> behaviourOnEnterParameters;
    private Dictionary<int, Func<object[]>> behaviourOnExitParameters;
    private ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 32 };

    private (int destinationState, Action onTransition)[,] transition;

    public BehaviourActions GetCurrentStateOnEnterBehaviours => behaviour[currentState].
        GetOnEnterBehaviours(behaviourOnEnterParameters[currentState]?.Invoke());

    public BehaviourActions GetCurrentStateTickBehaviours => behaviour[currentState].
        GetTickBehaviours(behaviourTickParameters[currentState]?.Invoke());

    public BehaviourActions GetCurrentStateOnExitBehaviours => behaviour[currentState].
        GetOnExitBehaviours(behaviourOnExitParameters[currentState]?.Invoke());

    public FSM()
    {
        int states = Enum.GetValues(typeof(EnumState)).Length;
        int flags = Enum.GetValues(typeof(EnumFlag)).Length;

        behaviour = new Dictionary<int, State>();
        transition = new (int, Action)[states, flags];

        for (int i = 0; i < states; i++)
        {
            for (int j = 0; j < flags; j++)
            {
                transition[i, j] = (-1, null);
            }
        }

        behaviourTickParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnEnterParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnExitParameters = new Dictionary<int, Func<object[]>>();
    }

    public void SetTrasnsition(EnumState originState, EnumFlag flags, EnumState destinationState, Action onTransition = null)
    {
        transition[Convert.ToInt32(originState), Convert.ToInt32(flags)] = (Convert.ToInt32(destinationState), onTransition);
    }

    public void Transition(Enum flags)
    {
        if (transition[currentState, Convert.ToInt32(flags)].destinationState != -1)
        {
            ExecuteBehaviours(GetCurrentStateOnExitBehaviours);

            transition[currentState, Convert.ToInt32(flags)].onTransition?.Invoke();

            currentState = transition[currentState, Convert.ToInt32(flags)].destinationState;

            ExecuteBehaviours(GetCurrentStateOnEnterBehaviours);
        }
    }

    public void ForceState(EnumState state)
    {
        currentState = Convert.ToInt32(state);
        ExecuteBehaviours(GetCurrentStateOnEnterBehaviours);
    }

    public void Tick()
    {
        if (behaviour.ContainsKey(currentState))
        {
            ExecuteBehaviours(GetCurrentStateTickBehaviours);
        }
    }

    public void AddBehaviour<T>(EnumState stateIndex, Func<object[]> onTickParameters = null,
        Func<object[]> onEnterParameters = null, Func<object[]> onExitParameters = null) where T : State, new()
    {
        int indexState = Convert.ToInt32(stateIndex);
        if (!behaviour.ContainsKey(indexState))
        {
            State newState = new T();
            newState.OnFlag += Transition;
            behaviour.Add(indexState, newState);
            behaviourTickParameters.Add(indexState, onTickParameters);
            behaviourOnEnterParameters.Add(indexState, onEnterParameters);
            behaviourOnExitParameters.Add(indexState, onExitParameters);
        }
    }

    public void ExecuteBehaviours(BehaviourActions behaviourActions)
    {
        if (behaviourActions.Equals(default(BehaviourActions)))
            return;

        int executionOrder = 0;

        while ((behaviourActions.MainThreadBahaviours != null && behaviourActions.MainThreadBahaviours.Count > 0) ||
                (behaviourActions.MultiThreadBehaviours != null && behaviourActions.MultiThreadBehaviours.Count > 0))
        {
            Task multithreadeableBehaviours = new Task(() =>
            {
                if (behaviourActions.MultiThreadBehaviours != null)
                {
                    if (behaviourActions.MultiThreadBehaviours.ContainsKey(executionOrder))
                    {
                        Parallel.ForEach(behaviourActions.MultiThreadBehaviours[executionOrder], parallelOptions, (behaviour) =>
                        {
                            behaviour?.Invoke();
                        });
                        behaviourActions.MultiThreadBehaviours.TryRemove(executionOrder, out _);
                    }
                }
            });

            multithreadeableBehaviours.Start();

            if (behaviourActions.MainThreadBahaviours != null)
            {
                if (behaviourActions.MainThreadBahaviours.ContainsKey(executionOrder))
                {
                    foreach (Action behaviours in behaviourActions.MainThreadBahaviours[executionOrder])
                    {
                        behaviours?.Invoke();
                    }
                    behaviourActions.MainThreadBahaviours.Remove(executionOrder);
                }
            }

            multithreadeableBehaviours.Wait();

            executionOrder++;
        }

        behaviourActions.TransitionBehaviours?.Invoke();
    }

}

