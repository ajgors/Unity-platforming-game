using UnityEngine;

namespace _193257_193435_193271
{
	public class Oscilating : MonoBehaviour
	{
		public float moveSpeed = 1.0f; // Speed of the up and down movement
		public float moveRange = 0.5f; // Range of the up and down movement

		private Vector3 originalPosition;

		private void Start()
		{
			originalPosition = transform.position;
		}

		private void Update()
		{
			// Perform the up and down movement
			float yOffset = Mathf.Sin(Time.time * moveSpeed) * moveRange;
			transform.position = originalPosition + new Vector3(0f, yOffset, 0f);
		}
	}
}