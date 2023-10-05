using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialPartitionPattern
{
    public class Grid
    {
        int cellSize;
        float halfMapWidth;

        Boid[,,] cells;

        public Grid(int mapWidth, int cellSize)
        {
            this.cellSize = cellSize;
            this.halfMapWidth = mapWidth / 2;
            int numberOfCells = mapWidth / cellSize;
            cells = new Boid[numberOfCells, numberOfCells, numberOfCells];
        }
        public void Add(Boid boid)
        {
            int cellX = (int)((boid.boidTrans.position.x) / cellSize);
            int cellY = (int)((boid.boidTrans.position.y) / cellSize);
            int cellZ = (int)((boid.boidTrans.position.z) / cellSize);
            boid.prevBoid = null;
            boid.nextBoid = cells[cellX, cellY, cellZ];

            cells[cellX, cellY, cellZ] = boid;

            if (boid.nextBoid != null)
            {
                boid.nextBoid.prevBoid = boid;
            }
        }
        public void MovePartitioned(Boid boid)
        {
            int oldCellX = (int)(boid.oldPos.x / cellSize);
            int oldCellY = (int)(boid.oldPos.y / cellSize);
            int oldCellZ = (int)(boid.oldPos.z / cellSize);
            int cellX = (int)(boid.boidTrans.position.x / cellSize);
            int cellY = (int)(boid.boidTrans.position.y / cellSize);
            int cellZ = (int)(boid.boidTrans.position.z / cellSize);

            Vector3 seperationDirection = Vector3.zero;
            int seperationCount = 0;
            Vector3 alignmentDirection = Vector3.zero;
            int alignmentCount = 0;
            Vector3 cohesionDirection = Vector3.zero;
            Vector3 boundsDir = Vector3.zero;
            int cohesionCount = 0;

            Boid currentBoid = cells[cellX, cellY, cellZ];

            while (currentBoid != null)
            {
                if (currentBoid == boid) {
                    currentBoid = currentBoid.nextBoid;
                    continue;
                }

                float distance = Vector3.Distance(currentBoid.boidTrans.position, boid.boidTrans.position);

                if (distance < boid.NoClumpingRadius)
                {
                    seperationDirection += currentBoid.boidTrans.position - boid.boidTrans.position;
                    seperationCount++;
                }
                if (distance < boid.LocalAreaRadius)
                {
                    alignmentDirection += currentBoid.boidTrans.forward;
                    alignmentCount++;
                }
                if (distance < boid.LocalAreaRadius)
                {
                    cohesionDirection += currentBoid.boidTrans.position - boid.boidTrans.position;
                    cohesionCount++;
                }
                currentBoid = currentBoid.nextBoid;
            }

            if (seperationCount > 0) seperationDirection /= seperationCount;

            seperationDirection = -seperationDirection.normalized;

            if (Physics.Raycast(boid.boidTrans.position, boid.boidTrans.forward, out RaycastHit hitInfo, boid.LocalAreaRadius, boid.layerMask))
                boundsDir = -(hitInfo.point - boid.boidTrans.position).normalized * 10;

            if (boundsDir == Vector3.zero)
            {
                boid.steering = seperationDirection.normalized * 0.5f;
                boid.steering += alignmentDirection.normalized * 0.34f;
                boid.steering += cohesionDirection.normalized * 0.16f;
            }
            else
                boid.steering = boundsDir;

            if (oldCellX == cellX && oldCellY == cellY && oldCellZ == cellZ)
            {
                return;
            }

            if (boid.prevBoid != null)
            {
                boid.prevBoid.nextBoid = boid.nextBoid;
            }

            if (boid.nextBoid != null)
            {
                boid.nextBoid.prevBoid = boid.prevBoid;
            }
            if (cells[oldCellX, oldCellY, oldCellZ] == boid)
            {
                cells[oldCellX, oldCellY, oldCellZ] = boid.nextBoid;
            }

            Add(boid);
        }
        /*public Soldier FindClosestEnemy(Soldier friendlySoldier)
        {
            int cellX = (int)(friendlySoldier.soldierTrans.position.x / cellSize);
            int cellY = (int)(friendlySoldier.soldierTrans.position.y / cellSize);
            int cellZ = (int)(friendlySoldier.soldierTrans.position.z / cellSize);

            Soldier enemy = cells[cellX, cellY, cellZ];

            Soldier closestSoldier = null;

            float bestDistSqr = Mathf.Infinity;

            while (enemy != null)
            {
                float distSqr = (enemy.soldierTrans.position - friendlySoldier.soldierTrans.position).sqrMagnitude;

                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;

                    closestSoldier = enemy;
                }

                enemy = enemy.nextSoldier;
            }
            return closestSoldier;
        }*/
        //public void Move(Soldier soldier, Vector3 oldPos)
        //{
        //    int oldCellX = (int)(oldPos.x / cellSize);
        //    int oldCellY = (int)(oldPos.y / cellSize);
        //    int oldCellZ = (int)(oldPos.z / cellSize);
        //    int cellX = (int)(soldier.soldierTrans.position.x / cellSize);
        //    int cellY = (int)(soldier.soldierTrans.position.y / cellSize);
        //    int cellZ = (int)(soldier.soldierTrans.position.z / cellSize);

        //    if (oldCellX == cellX && oldCellY == cellY && oldCellZ == cellZ)
        //    {
        //        return;
        //    }

        //    if (soldier.previousSoldier != null)
        //    {
        //        soldier.previousSoldier.nextSoldier = soldier.nextSoldier;
        //    }

        //    if (soldier.nextSoldier != null)
        //    {
        //        soldier.nextSoldier.previousSoldier = soldier.previousSoldier;
        //    }
        //    if (cells[oldCellX, oldCellY, oldCellZ] == soldier)
        //    {
        //        cells[oldCellX, oldCellY, oldCellZ] = soldier.nextSoldier;
        //    }

        //    Add(soldier);
        //}
    }
}