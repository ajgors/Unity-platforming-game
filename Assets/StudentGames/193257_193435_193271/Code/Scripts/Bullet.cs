using System.Collections;
using UnityEngine;

namespace _193257_193435_193271
{
	public class Bullet : MonoBehaviour
	{
		public int damage = 10;
		public float speed = 20f;
		public Rigidbody2D rb;
		private Animator anim;

		void Start()
		{
			anim = GetComponent<Animator>();

			//bullet sound
			AudioManager.instance.PlaySFX(SoundEffect.bulletSound);

			//bullet movement
			if (GameObject.Find("Player").GetComponent<PlayerController>().IsFacingRight())
			{
				rb.velocity = transform.right * speed;
			}
			else
			{
				rb.velocity = transform.right * (-speed);
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			//Ignore these collisions
			if (collision.tag == "Player" || collision.tag == "Cherry" ||
				collision.tag == "Key" || collision.tag == "Heart" ||
				collision.tag == "CheckPoint" || collision.tag == "Spring"
				|| collision.tag == "bullet")
			{
				return;
			}

			//Enemy collision 
			if (collision.tag == "Enemy")
			{
				collision.GetComponent<EnemyController>().Damage(damage);
			}

			//impact
			rb.velocity = Vector2.zero;
			AudioManager.instance.PlaySFX(SoundEffect.enemyHitSound);
			anim.SetBool("isImpact", true);
			StartCoroutine(KillOnAnimationEnd());
		}
		private void OnBecameInvisible()
		{
			// Destroy the bullet when it goes off-screen
			Destroy(gameObject);
		}

		private IEnumerator KillOnAnimationEnd()
		{
			yield return new WaitForSeconds(0.417f);
			Destroy(gameObject);
		}
	}
}