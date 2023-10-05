using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SpatialPartitionPattern
{
    public class GameController : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public bool seekMode = false;

        public GameObject friendlyObj;
        public GameObject enemyObj;
        public Material enemyMat;
        public Material closestEnemyMat;
        public Transform enemyParent;
        public Transform friendlyParent;

        List<Soldier> enemySoldiers = new List<Soldier>();
        List<Soldier> friendlySoldiers = new List<Soldier>();

        List<Soldier> closestEnemies = new List<Soldier>();

        float mapWidth = 50f;
        int cellSize = 10;

        [SerializeField] int numberOfSoldiers = 100;

        Grid grid;

        private void Start()
        {
            grid = new Grid((int)mapWidth, cellSize);

            for (int i = 0; i < numberOfSoldiers; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(0f, mapWidth), Random.Range(0f, mapWidth), Random.Range(0f, mapWidth));
                GameObject newEnemy = Instantiate(enemyObj, randomPos, Quaternion.identity);
                newEnemy.transform.parent = enemyParent;
                enemySoldiers.Add(new Enemy(newEnemy, mapWidth, grid));


                randomPos = new Vector3(Random.Range(0f, mapWidth), Random.Range(0f, mapWidth), Random.Range(0f, mapWidth));

                GameObject newFriendly = Instantiate(friendlyObj, randomPos, Quaternion.identity);
                newFriendly.transform.parent = friendlyParent;

                friendlySoldiers.Add(new Friendly(newFriendly, mapWidth));

            }
        }
        private void Update()
        {
            for (int i = 0; i < enemySoldiers.Count; i++)
            {
                enemySoldiers[i].Move();
            }

            for (int i = 0; i < closestEnemies.Count; i++)
            {
                closestEnemies[i].soldierMeshRenderer.material = enemyMat;
            }

            closestEnemies.Clear();
            
            //for (int i = 0; i < friendlySoldiers.Count; i++)
            //{
            //    Soldier closestEnemy = seekMode ? grid.FindClosestEnemy(friendlySoldiers[i]) : FindClosestEnemySlow(friendlySoldiers[i]);

            //    if (closestEnemy != null)
            //    {
            //        closestEnemy.soldierMeshRenderer.material = closestEnemyMat;

            //        closestEnemies.Add(closestEnemy);

            //        friendlySoldiers[i].Move(closestEnemy);
            //    }
            //}
        }

        private void LateUpdate()
        {
            text.text = seekMode ? "Partitioned Method:\n" : "Slow Method:\n" ;
            text.text += (1f / Time.deltaTime) + " FPS\n";
            text.text += (1000 * Time.deltaTime) + " milliseconds";
        }

        Soldier FindClosestEnemySlow(Soldier soldier) {
            Soldier closestEnemy = null;
            float bestDistSqr = Mathf.Infinity;
            for (int i = 0; i < enemySoldiers.Count; i++)
            {
                float distSqr = (soldier.soldierTrans.position - enemySoldiers[i].soldierTrans.position).sqrMagnitude;

                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;

                    closestEnemy = enemySoldiers[i];
                }
            }
            return closestEnemy;
        }

        public void ToggleSeekMode()
        {
            seekMode = !seekMode;
        }
    }
}