using System.Collections.Generic;

namespace ESC.Patron
{
    public abstract class EcsBoidSystem<TComponent> :  ECSSystem
    {
        public abstract ICollection<TComponent> GetBoidsInsideRadius();
    }
}