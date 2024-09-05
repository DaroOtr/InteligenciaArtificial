using System.Collections.Generic;
using UnityEngine;

namespace ESC.Example
{
    public class EcsExampleNoEcs : MonoBehaviour
    {
        public int entityCount = 100;
        public float velocity = 10.0f;
        public GameObject prefab;

        private List<GameObject> _entities;

        void Start()
        {
            _entities = new List<GameObject>();
            for (int i = 0; i < entityCount; i++)
            {
                GameObject go = Instantiate(prefab, new Vector3(0, -i, 0), Quaternion.identity);
                go.AddComponent<MovementMonoBehaviour>().velocity = velocity;
                _entities.Add(go);
            }
        }
    }
}
