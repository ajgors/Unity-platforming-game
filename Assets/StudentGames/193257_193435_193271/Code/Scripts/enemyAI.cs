using Pathfinding;
using UnityEngine;

namespace _193257_193435_193271
{
	public class enemyAI : MonoBehaviour
	{
		public Transform target;
		public float speed = 200f;
		public float newWaypointDistance = 3f;
		public float detectionRange = 10f;

		Path path;
		int currentWayPoint = 0;
		bool reachedEndOfPath = false;
		Seeker seeker;
		Rigidbody2D rb;

		void Start()
		{
			seeker = GetComponent<Seeker>();
			rb = GetComponent<Rigidbody2D>();

			if (target == null)
			{
				target = GameObject.FindGameObjectWithTag("Player").transform;
			}

			InvokeRepeating("UpdatePath", 0f, 0.5f);
		}

		void UpdatePath()
		{
			float distanceToTarget = Vector2.Distance(rb.position, target.position);

			// Check if the player is within the detection range
			if (distanceToTarget <= detectionRange && seeker.IsDone())
			{
				Vector2 targetPosition = target.position;

				// Check if the player is above the AI
				if (targetPosition.y > rb.position.y)
				{
					targetPosition.y += 2.0f;

					seeker.StartPath(rb.position, targetPosition, OnPathComplete);
				}
				else
				{
					seeker.StartPath(rb.position, target.position, OnPathComplete);
				}
			}
		}

		void OnPathComplete(Path p)
		{
			if (!p.error)
			{
				path = p;
				currentWayPoint = 0;
			}
		}

		void Update()
		{
			if (path == null)
			{
				return;
			}

			if (currentWayPoint >= path.vectorPath.Count)
			{
				reachedEndOfPath = true;
				return;
			}
			else
			{
				reachedEndOfPath = false;
			}

			Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
			Vector2 force = direction * speed * Time.deltaTime;

			rb.AddForce(force);

			float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

			if (distance < newWaypointDistance)
			{
				currentWayPoint++;
			}

			if (force.x >= 0.01f)
			{
				transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			else if (force.x <= -0.01f)
			{
				transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}
}
