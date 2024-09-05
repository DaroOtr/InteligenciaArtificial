using System;

namespace ESC.Implementation.Interfaces
{
    public interface ITransitionable
    {
        public void SetTransitionBeahviour(Action transitionBehaviour);
        public Action GetTransitionBeahviour();
    }
}