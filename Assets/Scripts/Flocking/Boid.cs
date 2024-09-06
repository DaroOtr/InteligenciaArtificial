using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flocking
{
    public class Boid : MonoBehaviour
    {
        public float speed = 2.5f;
        public float turnSpeed = 5f;
        public float detectionRadious = 3.0f;
        public float AlignmentRadious = 1f;
        public float CohesionRadious = 1f;
        public float separationRadious = 2f;
        public float DirectionRadious = 2f;

        private Func<Boid, Vector3> Alignment;
        private Func<Boid, Vector3> Cohesion;
        private Func<Boid, Vector3> Separation;
        private Func<Boid, Vector3> Direction;

        public void Init(Func<Boid, Vector3> Alignment, 
            Func<Boid, Vector3> Cohesion, 
            Func<Boid, Vector3> Separation, 
            Func<Boid, Vector3> Direction,
            List<float> multipliers) 
        {
            this.Alignment = Alignment;
            this.Cohesion = Cohesion;
            this.Separation = Separation;
            this.Direction = Direction;
            AlignmentRadious = multipliers[0];
            CohesionRadious = multipliers[1];
            separationRadious = multipliers[2];
            DirectionRadious = multipliers[3];
        }

        private void Update()
        {
            Vector3 acs = ACS();
            transform.forward = Vector3.Lerp(transform.forward, acs, turnSpeed * Time.deltaTime);
            transform.position +=  transform.forward * speed * Time.deltaTime;
        }

        public Vector3 ACS()
        {
            Vector3 ACS = (Alignment(this) * AlignmentRadious) + (Cohesion(this) * CohesionRadious) + (Separation(this) * separationRadious) + (Direction(this) * DirectionRadious);
            return ACS.normalized;
        }
    }
}