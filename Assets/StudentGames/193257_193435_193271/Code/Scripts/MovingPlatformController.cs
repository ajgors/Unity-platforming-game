using UnityEngine;

namespace _193257_193435_193271
{
	public class MovingPlatformController : MonoBehaviour
	{
		[Header("Movement parameters")]
		[Range(0.01f, 20.0f)][SerializeField] private float platformMoveSpeed = 0.1f;

		private float startPositionX;
		public float platformMoveRange = 3.5f;
		private bool isMovingRight = true;

		void Update()
		{
			if (isMovingRight)
			{
				if (this.transform.position.x <= startPositionX + platformMoveRange)
				{
					moveRight();
				}
				else
				{
					isMovingRight = false;
				}
			}
			else
			{
				if (this.transform.position.x >= startPositionX - platformMoveRange)
				{
					moveLeft();
				}
				else
				{
					isMovingRight = true;
				}
			}
		}
		void Awake()
		{
			startPositionX = this.transform.position.x;
		}

		void moveRight()
		{
			transform.Translate(platformMoveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
		}

		void moveLeft()
		{
			transform.Translate(-platformMoveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
		}
	}
}