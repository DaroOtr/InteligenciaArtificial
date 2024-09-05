using System.Collections.Generic;
using System.Threading.Tasks;
using ESC.Implementation;
using ESC.Patron;
using UnityEngine;
using UnityEngine.Serialization;

namespace ESC.Example
{
    public class EcsExampleEcsWhitoutGOs : MonoBehaviour
    {
        public int entityCount = 100;
        public float velocity = 0.1f;
        public float rotvelocity = 0.1f;
        public Vector3 prefabRotation;
        public GameObject prefab;

        private const int MAX_OBJS_PER_DRAWCALL = 1000;
        private Mesh _prefabMesh;
        private Material _prefabMaterial;
        private Vector3 _prefabScale;
    

        private List<uint> _entities;

        void Start()
        {
            EcsManager.Init();
            _entities = new List<uint>();
            for (int i = 0; i < entityCount; i++)
            {
                uint entityID = EcsManager.CreateEntity();
                EcsManager.AddComponent<PositionComponent>(entityID,
                    new PositionComponent(0, -i, 0));
                EcsManager.AddComponent<VelocityComponent>(entityID,
                    new VelocityComponent(velocity, Vector3.right.x, Vector3.right.y, Vector3.right.z));
                EcsManager.AddComponent<RotationComponent>(entityID,
                    new RotationComponent(rotvelocity,prefabRotation.x, prefabRotation.y, prefabRotation.z));
                _entities.Add(entityID);
            }

            _prefabMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            _prefabMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;
            _prefabScale = prefab.transform.localScale;
        }

        void Update()
        {
            EcsManager.Tick(Time.deltaTime);
        }

        void LateUpdate()
        {
            List<Matrix4x4[]> drawMatrix = new List<Matrix4x4[]>();
            int meshes = _entities.Count;
            for (int i = 0; i < _entities.Count; i += MAX_OBJS_PER_DRAWCALL)
            {
                drawMatrix.Add(new Matrix4x4[meshes > MAX_OBJS_PER_DRAWCALL ? MAX_OBJS_PER_DRAWCALL : meshes]);
                meshes -= MAX_OBJS_PER_DRAWCALL;
            }
            Parallel.For(0, _entities.Count, i =>
            {
                PositionComponent position = EcsManager.GetComponent<PositionComponent>(_entities[i]);
                Vector3 newPos = new Vector3(position.X, position.Y, position.Z);
                RotationComponent rotation = EcsManager.GetComponent<RotationComponent>(_entities[i]);
                Quaternion newRot = Quaternion.Euler(rotation.rotationX, rotation.rotationY, rotation.rotationZ);
            
                drawMatrix[(i / MAX_OBJS_PER_DRAWCALL)][(i % MAX_OBJS_PER_DRAWCALL)]
                    .SetTRS(newPos, Quaternion.Euler(rotation.rotationX,rotation.rotationY,rotation.rotationZ), _prefabScale);
            });
            for (int i = 0; i < drawMatrix.Count; i++)
            {
                Graphics.DrawMeshInstanced(_prefabMesh, 0, _prefabMaterial, drawMatrix[i]);
            }
        }
    }
}
