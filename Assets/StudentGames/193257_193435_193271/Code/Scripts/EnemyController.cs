using System;
using System.Collections;
using UnityEngine;

namespace _193257_193435_193271
{
	public class EnemyController : MonoBehaviour
	{
		public int healthPoints = 30;
		public int pointsForKill = 100;

		[Header("Movement parameters")]
		[Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f;

		private float startPositionX;
		public float moveRange = 1.0f;
		[SerializeField] private bool isMovingRight = false;

		private Animator animator;
		[SerializeField] private bool isFacingRight = false;
		public bool canMove = true;

		Rigidbody2D rb;
		private Collider2D[] enemyColliders; // Array to hold references to all colliders
		private float highestYOfColliders = float.MinValue;
		private bool isKilled = false;

		void Update()
		{
			//to stop moving when dead
			if (!CompareTag("NonViolent"))
			{
				if (animator.GetBool("isDead"))
				{
					return;
				}
			}

			if (canMove)
			{
				if (isMovingRight)
				{
					if (transform.position.x <= startPositionX + moveRange)
					{
						moveRight();
					}
					else
					{
						isMovingRight = false;
						flip();
					}
				}
				else
				{
					if (transform.position.x >= startPositionX - moveRange)
					{
						moveLeft();
					}
					else
					{
						isMovingRight = true;
						flip();
					}
				}
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (CompareTag("NonViolent")) return;

			if (other.CompareTag("Player"))
			{
				BoxCollider2D playerCollider = other.GetComponent<BoxCollider2D>();

				//update highest y of enemy colliders (ai enemies)
				foreach (var collider in enemyColliders)
				{
					if (collider.bounds.max.y > highestYOfColliders)
					{
						highestYOfColliders = collider.bounds.max.y;
					}
				}

				//if player bottom y collider + 0.45f > enemy highest y collider
				if (playerCollider.bounds.min.y + 0.45f > highestYOfColliders)
				{
					Kill();
				}
				else
				{
					//stop enemy when it kills player
					StartCoroutine(stopMovement());
				}
			}
		}

		private void Kill()
		{
			if (!isKilled)
			{
				isKilled = true;
				disableColliders();
				AudioManager.instance.PlaySFX(SoundEffect.enemyKillSound);
				GameManager.instance.AddPoints(pointsForKill);
				rb.velocity = new Vector2(0, 0);
				rb.isKinematic = true;
				animator.SetBool("isDead", true);
				GameManager.instance.AddKill();

				StartCoroutine(KillOnAnimationEnd());
			}
		}

		void disableColliders()
		{
			foreach (var collider in enemyColliders)
			{
				collider.enabled = false;
			}
		}

		private IEnumerator stopMovement()
		{
			Rigidbody2D rb = GetComponent<Rigidbody2D>();
			rb.velocity = new Vector2(0, 0);
			ToggleMove();
			yield return new WaitForSeconds(0.8f);
			ToggleMove();
		}

		private void ToggleMove()
		{
			canMove = !canMove;
		}

		private IEnumerator KillOnAnimationEnd()
		{
			yield return new WaitForSeconds(0.8f);
			gameObject.SetActive(false);
		}

		private void flip()
		{
			isFacingRight = !isFacingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}

		void Awake()
		{
			startPositionX = transform.position.x;
			animator = GetComponent<Animator>();
			rb = GetComponent<Rigidbody2D>();
			enemyColliders = GetComponentsInChildren<Collider2D>();
		}

		public void Damage(int damage)
		{
			healthPoints -= damage;
			if (healthPoints <= 0)
			{
				Kill();
			}
		}

		void moveRight()
		{
			transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
		}

		void moveLeft()
		{
			transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
		}
	}
}