using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESC.Patron
{
    public static class EcsManager
    {
        private static readonly ParallelOptions ParallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 32 };

        private static ConcurrentDictionary<uint, EcsEntity> _entities = null;
        private static ConcurrentDictionary<Type, ConcurrentDictionary<uint, EcsComponent>> components = null;
        private static ConcurrentDictionary<Type, ECSSystem> _systems = null;

        public static void Init()
        {
            _entities = new ConcurrentDictionary<uint, EcsEntity>();
            components = new ConcurrentDictionary<Type, ConcurrentDictionary<uint, EcsComponent>>();
            _systems = new ConcurrentDictionary<Type, ECSSystem>();

            foreach (Type classType in typeof(ECSSystem).Assembly.GetTypes())
            {
                if (typeof(ECSSystem).IsAssignableFrom(classType) && !classType.IsAbstract)
                {
                    _systems.TryAdd(classType, Activator.CreateInstance(classType) as ECSSystem);
                }
            }

            foreach (KeyValuePair<Type, ECSSystem> system in _systems)
            {
                system.Value.Initialize();
            }

            foreach (Type classType in typeof(EcsComponent).Assembly.GetTypes())
            {
                if (typeof(EcsComponent).IsAssignableFrom(classType) && !classType.IsAbstract)
                {
                    components.TryAdd(classType, new ConcurrentDictionary<uint, EcsComponent>());
                }
            }
        }

        public static void Tick(float deltaTime)
        {
            Parallel.ForEach(_systems, ParallelOptions, system =>
            {
                system.Value.Run(deltaTime);
            });
        }

        public static uint CreateEntity()
        {
            EcsEntity ecsEntity;
            ecsEntity = new EcsEntity();
            _entities.TryAdd(ecsEntity.GetID(), ecsEntity);
            return ecsEntity.GetID();
        }

        public static void AddComponent<TComponentType>(uint entityID, TComponentType component) where TComponentType : EcsComponent
        {
            component.EntityOwnerID = entityID;
            _entities[entityID].AddComponentType(typeof(TComponentType));
            components[typeof(TComponentType)].TryAdd(entityID, component);
        }

        public static bool ContainsComponent<TComponentType>(uint entityID) where TComponentType : EcsComponent
        {
            return _entities[entityID].ContainsComponentType<TComponentType>();
        }

        public static IEnumerable<uint> GetEntitiesWhitComponentTypes(params Type[] componentTypes)
        {
            ConcurrentBag<uint> matchs = new ConcurrentBag<uint>();
            Parallel.ForEach(_entities, ParallelOptions, entity =>
            {
                for (int i = 0; i < componentTypes.Length; i++)
                {
                    if (!entity.Value.ContainsComponentType(componentTypes[i]))
                        return;
                }
                matchs.Add(entity.Key);
            });
            return matchs;
        }

        public static ConcurrentDictionary<uint, TComponentType> GetComponents<TComponentType>() where TComponentType : EcsComponent
        {
            if (components.ContainsKey(typeof(TComponentType)))
            {
                ConcurrentDictionary<uint, TComponentType> comps = new ConcurrentDictionary<uint, TComponentType>();

                Parallel.ForEach(components[typeof(TComponentType)], ParallelOptions, component => 
                { 
                    comps.TryAdd(component.Key, component.Value as TComponentType);
                });

                return comps;
            }

            return null;
        }

        public static TComponentType GetComponent<TComponentType>(uint entityID) where TComponentType : EcsComponent 
        {
            return components[typeof(TComponentType)][entityID] as TComponentType;
        }
    }
}
