using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SpatialPartitionPattern
{
    public class BoidController : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public bool seekMode = false;
        public LayerMask layerMask;

        public GameObject boidObj;
        public Transform boidParent;

        List<Boid> boids = new List<Boid>();

        [SerializeField] float mapWidth = 50f;
        [SerializeField] int cellSize = 10;

        [SerializeField] int numberOfSoldiers = 100;

        Grid grid;

        private void Start()
        {
            grid = new Grid((int)mapWidth, cellSize);

            for (int i = 0; i < numberOfSoldiers; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(0f, mapWidth), Random.Range(0f, mapWidth), Random.Range(0f, mapWidth));
                GameObject newBoid = Instantiate(boidObj, randomPos, Quaternion.identity);
                newBoid.transform.parent = boidParent;
                boids.Add(new Boid(newBoid, mapWidth, grid, layerMask));
            }
        }
        private void Update()
        {
            for (int i = 0; i < boids.Count; i++)
            {
                boids[i].Move(boids, Time.deltaTime, seekMode);
            }
        }

        private void LateUpdate()
        {
            text.text = seekMode ? "Partitioned Method:\n" : "Slow Method:\n" ;
            text.text += (1f / Time.deltaTime) + " FPS\n";
            text.text += (1000 * Time.deltaTime) + " milliseconds";
        }

        public void ToggleSeekMode()
        {
            seekMode = !seekMode;
        }
    }
}