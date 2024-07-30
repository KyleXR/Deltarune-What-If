using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class ProceduralPathFollower : MonoBehaviour
    {
        public ProceduralSplineGenerator splineGenerator;
        public float speed = 5.0f;

        private int currentWaypointIndex = 0;
        private Vector3 targetWaypoint;
        private bool isMoving = true;

        private void Start()
        {
            if (splineGenerator.GetWaypointCount() < 2)
            {
                Debug.LogError("Path generator needs at least two waypoints.");
                isMoving = false;
                return;
            }

            targetWaypoint = splineGenerator.GetNextWaypoint(currentWaypointIndex);
        }

        private void Update()
        {
            if (isMoving)
            {
                MoveTowardsWaypoint();
            }
        }

        private void MoveTowardsWaypoint()
        {
            if (targetWaypoint == Vector3.zero)
            {
                isMoving = false;
                return;
            }

            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, step);

            float distanceToTarget = Vector3.Distance(transform.position, targetWaypoint);
            float totalDistance = Vector3.Distance(splineGenerator.GetNextWaypoint(currentWaypointIndex), splineGenerator.GetNextWaypoint(currentWaypointIndex + 1));

            if (distanceToTarget / totalDistance <= 0.2f)
            {
                currentWaypointIndex++;
                targetWaypoint = splineGenerator.GetNextWaypoint(currentWaypointIndex);
                GenerateNewWaypointIfNecessary();
            }
        }

        private void GenerateNewWaypointIfNecessary()
        {
            float distanceFromStart = Vector3.Distance(splineGenerator.GetNextWaypoint(0), transform.position);
            if (distanceFromStart < splineGenerator.maxDistance)
            {
                Vector3 direction = (targetWaypoint - transform.position).normalized;
                Vector3 newWaypoint = transform.position + direction * splineGenerator.waypointDistance;
                splineGenerator.AddWaypoint(newWaypoint);
            }
        }
    }
}