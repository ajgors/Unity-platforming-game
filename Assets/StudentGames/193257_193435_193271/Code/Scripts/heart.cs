using System.Collections;
using UnityEngine;

namespace _193257_193435_193271
{
	public class heart : MonoBehaviour
	{
		private Animator animator;
		bool isCollected = false;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player") && !isCollected)
			{
				AudioManager.instance.PlaySFX(SoundEffect.keyPickUpSound);
				isCollected = true;

				//remove color from object
				gameObject.GetComponent<SpriteRenderer>().color = Color.white;
				gameObject.transform.Translate(0, 0.5f, 0);

				animator.SetBool("isCollected", true);
				StartCoroutine(HeartAnimation());
			}
		}

		private IEnumerator HeartAnimation()
		{
			yield return new WaitForSeconds(0.333f);
			gameObject.SetActive(false);
		}

		void Awake()
		{
			animator = GetComponent<Animator>();
		}
	}
}