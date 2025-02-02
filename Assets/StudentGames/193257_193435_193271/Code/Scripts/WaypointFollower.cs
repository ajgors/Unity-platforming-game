using UnityEngine;

namespace _193257_193435_193271
{
	public class WaypointFollower : MonoBehaviour
	{
		[SerializeField] private GameObject[] waypoints;
		private int currentWaypoint = 0;
		[SerializeField] private float speed = 1.0f;

		void Update()
		{
			if (waypoints.Length == 0) return;

			// Calculate distance to the current waypoint
			float distance = Vector2.Distance(transform.position, waypoints[currentWaypoint].transform.position);

			// Move towards the waypoint
			transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypoint].transform.position, speed * Time.deltaTime);

			// Check if the waypoint is reached, considering a small threshold
			if (distance < 0.1f)
			{
				// Move to the next waypoint
				currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
			}
		}
	}
}
