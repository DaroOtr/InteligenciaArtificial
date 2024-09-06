using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Flocking
{
    public class FlockingManager : MonoBehaviour
    {
        public Transform target;
        public int boidCount = 50;
        public Boid boidPrefab;
        private List<Boid> boids = new List<Boid>();
        public float AlignmentRadious;
        public float CohesionRadious;
        public float separationRadious;
        public float DirectionRadious;

        private void Start()
        {
            List<float> multipliers = new List<float>();
            multipliers.Add(AlignmentRadious);
            multipliers.Add(CohesionRadious);
            multipliers.Add(separationRadious);
            multipliers.Add(DirectionRadious);
            for (int i = 0; i < boidCount; i++)
            {
                GameObject boidGO = Instantiate(boidPrefab.gameObject,
                    new Vector3(Random.Range(-10, 10), Random.Range(-10, 10)), Quaternion.identity);
                Boid boid = boidGO.GetComponent<Boid>();
                boid.Init(Alignment, Cohesion, Separation, Direction,multipliers);
                boids.Add(boid);
            }
        }

        public Vector3 Alignment(Boid boid)
        {
            List<Boid> insideRadiusBoids = GetBoidsInsideRadius(boid);
            
            if (insideRadiusBoids.Count == 0)   
                return transform.forward;            
            
            Vector3 avg = Vector3.zero;
            foreach (Boid b in insideRadiusBoids)
            {
                if (b == boid) 
                    continue;     
                
                avg += b.transform.forward;
            }

            avg /= insideRadiusBoids.Count;
            return avg.normalized;
        }

        public Vector3 Cohesion(Boid boid)
        {
            List<Boid> insideRadiusBoids = GetBoidsInsideRadius(boid);
            
            if (insideRadiusBoids.Count == 0)   
                return Vector3.zero;            
            
            
            Vector3 avg = Vector3.zero;
            foreach (Boid b in insideRadiusBoids)
            {
                if (b == boid) 
                    continue;  
                
                avg += b.transform.position;
            }

            avg /= insideRadiusBoids.Count;
            return (avg - boid.transform.position).normalized;
        }

        public Vector3 Separation(Boid boid)
        {
            List<Boid> insideRadiusBoids = GetBoidsInsideRadius(boid);
            
            if (insideRadiusBoids.Count == 0)
                return Vector3.zero;
            
            Vector3 avg = Vector3.zero;
            foreach (Boid b in insideRadiusBoids)
            {
                if (b == boid)
                    continue;
                
                avg += (boid.transform.position - b.transform.position);
            }

            avg /= insideRadiusBoids.Count;
            return avg.normalized;
        }

        public Vector3 Direction(Boid boid)
        {
            return (target.position - boid.transform.position).normalized;
        }

        public List<Boid> GetBoidsInsideRadius(Boid boid)
        {
            List<Boid> insideRadiusBoids = new List<Boid>();

            foreach (Boid b in boids)
            {
                if (b == boid)
                    continue;

                if (Vector3.Distance(boid.transform.position, b.transform.position) < boid.detectionRadious)
                {
                    insideRadiusBoids.Add(b);
                }
            }

            return insideRadiusBoids;
        }

        public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;


            foreach (Boid boid in boids)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawRay(boid.transform.position, boid.transform.forward);
                //Gizmos.color = Color.red;
                //Gizmos.DrawWireSphere(boid.transform.position,boid.detectionRadious);
            }
        }
    }
}