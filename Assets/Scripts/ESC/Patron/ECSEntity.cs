using System;
using System.Collections.Generic;

namespace ESC.Patron
{
    public class EcsEntity
    {
        private class EntityID
        {
            private static uint _lastEntityID = 0;
            internal static uint GetNew() => _lastEntityID++;
        }

        private uint ID;
        private List<Type> componentsType;

        public EcsEntity()
        {
            ID = EntityID.GetNew();
            componentsType = new List<Type>();
        }

        public uint GetID() => ID;

        public void Dispose()
        {
            componentsType.Clear();
        }

        public void AddComponentType<ComponentType>() where ComponentType : EcsComponent
        {
            AddComponentType(typeof(ComponentType));
        }

        public void AddComponentType(Type ComponentType)
        {
            componentsType.Add(ComponentType);
        }

        public bool ContainsComponentType<ComponentType>() where ComponentType : EcsComponent
        {
            return ContainsComponentType(typeof(ComponentType));
        }

        public bool ContainsComponentType(Type ComponentType)
        {
            return componentsType.Contains(ComponentType);
        }
    }
}
