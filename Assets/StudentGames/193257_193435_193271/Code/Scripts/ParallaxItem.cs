using UnityEngine;

namespace _193257_193435_193271
{
	public class ParallaxItem : MonoBehaviour
	{
		public Transform cam;
		public float relativeMove = 0.3f;
		public bool lockY = false;
		public float offsetY = 0f;

		private Vector3 initialPosition;

		void Start()
		{
			// Store the initial position of the background element
			initialPosition = transform.position;
		}

		void Update()
		{
			float parallaxX = (cam.position.x - initialPosition.x) * relativeMove;
			float parallaxY = (cam.position.y - initialPosition.y) * relativeMove;

			if (lockY)
			{
				transform.position = new Vector3(initialPosition.x + parallaxX, transform.position.y + offsetY);
			}
			else
			{
				transform.position = new Vector3(initialPosition.x + parallaxX, initialPosition.y + parallaxY + offsetY);
			}
		}
	}
}