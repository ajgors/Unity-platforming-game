using System.Collections;
using UnityEngine;

namespace _193257_193435_193271
{
	public class PlatformFall : MonoBehaviour
	{
		private Rigidbody2D rb;
		private Vector3 startPosition;
		private Vector3 playerRelativePosition;
		public float shakeTime;
		public float shakeMagnitude;
		public float resetTime; // Time to wait before resetting the platform
		private Rigidbody2D playerRb;
		private bool isPlayerOnPlatform;
		private PlayerController playerController;
		void Start()
		{
			rb = GetComponent<Rigidbody2D>();
			startPosition = transform.position;

			// Set Rigidbody2D settings here
			rb.interpolation = RigidbodyInterpolation2D.Interpolate;
			rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		}


		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				isPlayerOnPlatform = false;
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				isPlayerOnPlatform = true;
			}
		}

		private void OnTriggerStay2D(Collider2D collision)
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				isPlayerOnPlatform = true;
			}
		}

		private void OnCollisionExit2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				isPlayerOnPlatform = false;
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			isPlayerOnPlatform = false;
			if (collision.gameObject.CompareTag("Player"))
			{
				playerController = collision.gameObject.GetComponent<PlayerController>();
				isPlayerOnPlatform = true;
				playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
				playerRelativePosition = playerRb.transform.position - transform.position;
				StartCoroutine(ShakeAndFallPlatform());
			}
		}

		private IEnumerator ShakeAndFallPlatform()
		{
			Vector3 originalPosition = transform.position;
			float elapsedTime = 0f;

			while (elapsedTime < shakeTime)
			{
				float y = originalPosition.y + Random.Range(-shakeMagnitude, shakeMagnitude);
				float x = originalPosition.x + Random.Range(-shakeMagnitude, shakeMagnitude);

				// Apply the shake effect to the platform
				transform.Translate(new Vector3(x - originalPosition.x, y - originalPosition.y, 0f));

				if (isPlayerOnPlatform && playerRb.velocity.y <= 0 && playerController.isGrounded())
				{
					// Apply the relative position to the player's y coordinate
					playerRb.transform.position = new Vector3(playerRb.transform.position.x, transform.position.y + playerRelativePosition.y, playerRb.transform.position.z);
				}

				elapsedTime += Time.deltaTime;

				yield return null;
			}

			StartCoroutine(Fall());
		}

		private IEnumerator Fall()
		{
			rb.bodyType = RigidbodyType2D.Dynamic;

			yield return new WaitForSeconds(resetTime);

			rb.bodyType = RigidbodyType2D.Kinematic;
			rb.velocity = Vector2.zero;
			transform.position = startPosition;
		}
	}
}