using UnityEngine;

namespace _193257_193435_193271
{
	public class Parallax : MonoBehaviour
	{
		public Transform cam;
		public float relativeMove = .3f;
		public bool lockY = false;
		public float offsetY = 0f;

		void Update()
		{
			if (lockY)
			{
				transform.position = new Vector2(cam.position.x * relativeMove, transform.position.y + offsetY);
			}
			else
			{
				transform.position = new Vector2(cam.position.x * relativeMove, (cam.position.y + offsetY) * relativeMove);
			}
		}
	}
}
