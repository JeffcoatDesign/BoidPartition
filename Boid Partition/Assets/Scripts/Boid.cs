using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialPartitionPattern
{
    public class Boid
    {
        public Transform boidTrans;
        public float walkSpeed = 10f;
        public float NoClumpingRadius = 5f;
        public float LocalAreaRadius = 20f;
        public float SteeringSpeed = 100f;
        public float BoundaryDistance = 1;
        public float margin = 10;
        private Grid grid;
        private float mapWidth;
        public Boid nextBoid;
        public Boid prevBoid;
        public Vector3 steering;
        public Vector3 oldPos;
        public LayerMask layerMask;
        public Boid(GameObject oidObj, float mapWidth, Grid grid, LayerMask layerMask)
        {
            this.boidTrans = oidObj.transform;
            this.grid = grid;
            this.mapWidth = mapWidth;
            this.layerMask = layerMask;
            grid.Add(this);
        }

        public void Move(List<Boid> boids, float time, bool seekMode)
        {
            steering = Vector3.zero;

            if (!seekMode)
            {
                MoveSlow(boids);
            }
            else
            {
                grid.MovePartitioned(this);
            }

            if (boidTrans.position.x < 0 + margin)
                steering += new Vector3(1, 0, 0);
            if (boidTrans.position.x > mapWidth - margin)
                steering -= new Vector3(1, 0, 0);
            if (boidTrans.position.y < 0 + margin)
                steering += new Vector3(0, 1, 0);
            if (boidTrans.position.y > mapWidth - margin)
                steering -= new Vector3(0, 1, 0);
            if (boidTrans.position.z < 0 + margin)
                steering += new Vector3(0, 0, 1);
            if (boidTrans.position.z > mapWidth - margin)
                steering -= new Vector3(0, 0, 1);

            if (steering != Vector3.zero)
                boidTrans.rotation = Quaternion.RotateTowards(boidTrans.rotation, Quaternion.LookRotation(steering), SteeringSpeed * time);

            boidTrans.Translate(Vector3.forward * Time.deltaTime * walkSpeed);

            if (boidTrans.position.x < 0 || boidTrans.position.y < 0 || boidTrans.position.z < 0)
                GetNewRandomPos();
            if (boidTrans.position.x > mapWidth || boidTrans.position.y > mapWidth || boidTrans.position.z > mapWidth)
                GetNewRandomPos();
            oldPos = boidTrans.position;
        }

        void MoveSlow(List<Boid> boids)
        {
            Vector3 seperationDirection = Vector3.zero;
            int seperationCount = 0;
            Vector3 alignmentDirection = Vector3.zero;
            int alignmentCount = 0;
            Vector3 cohesionDirection = Vector3.zero;
            Vector3 boundsDir = Vector3.zero;
            int cohesionCount = 0;

            foreach (Boid b in boids)
            {
                if (b == this) continue;

                float distance = Vector3.Distance(b.boidTrans.position, boidTrans.position);

                if (distance < NoClumpingRadius)
                {
                    seperationDirection += b.boidTrans.position - boidTrans.position;
                    seperationCount++;
                }
                if (distance < LocalAreaRadius)
                {
                    alignmentDirection += b.boidTrans.forward;
                    alignmentCount++;
                }
                if (distance < LocalAreaRadius)
                {
                    cohesionDirection += b.boidTrans.position - boidTrans.position;
                    cohesionCount++;
                }
            }

            if (seperationCount > 0) seperationDirection /= seperationCount;

            seperationDirection = -seperationDirection.normalized;

            if (Physics.Raycast(boidTrans.position, boidTrans.forward, out RaycastHit hitInfo, LocalAreaRadius, layerMask))
            {
                boundsDir = -(hitInfo.point - boidTrans.position).normalized * 10;
            }

            if (boundsDir == Vector3.zero)
            {
                steering = seperationDirection.normalized * 0.5f;
                steering += alignmentDirection.normalized * 0.34f;
                steering += cohesionDirection.normalized * 0.16f;
            }
            else
                steering = boundsDir;
        }

        void GetNewRandomPos()
        {
            boidTrans.position = new Vector3(Random.Range(0, mapWidth), Random.Range(0, mapWidth), Random.Range(0, mapWidth));
        }
    }
}