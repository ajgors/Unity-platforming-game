using UnityEngine;

namespace _193257_193435_193271
{
	public class JumpPad : MonoBehaviour
	{
		public float forceX = 0.0f;
		public float forceY = 10.0f;
		public Rigidbody2D rb;
		private Animator animator;
		public bool onlyFromAbove = false;

		public void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			if (other.gameObject.tag == "Player")
			{
				PlayerController controller = other.gameObject.GetComponent<PlayerController>();
				Vector2 playerPosition = controller.GetPosition();
				AnimatorStateInfo currentAnimatorState = controller.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

				if ((currentAnimatorState.IsName("jump") || currentAnimatorState.IsName("falling")))
				{
					if (onlyFromAbove)
					{
						if (playerPosition.y > transform.position.y)
						{
							animator.Play("Work");
							controller.Bounce(forceX, forceY);
						}
					}
					else
					{
						animator.Play("Work");
						controller.Bounce(forceX, forceY);
					}
				}
			}
		}
	}
}
