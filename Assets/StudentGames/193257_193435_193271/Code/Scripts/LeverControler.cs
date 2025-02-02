using UnityEngine;

namespace _193257_193435_193271
{
	public class LeverControler : MonoBehaviour
	{
		private Animator anim;
		private Rigidbody2D rb;
		public GameObject door;
		public GameObject enemyPrefab;
		private bool isSwitched = false;

		void Awake()
		{
			anim = GetComponent<Animator>();
			rb = GetComponent<Rigidbody2D>();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				anim.SetBool("isClosed", true);
				Destroy(door);

				if (!isSwitched)
				{
					SpawnEnemies(transform.position.x - 2.0f, transform.position.y + 5.0f);
					SpawnEnemies(transform.position.x + 5.0f, transform.position.y + 5.0f);
				}

				isSwitched = true;
			}
		}

		private void SpawnEnemies(float positionX, float positionY)
		{
			Vector3 spawnPosition = new Vector3(positionX, positionY, 0);

			if (enemyPrefab != null && spawnPosition != null)
			{
				Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

			}
		}
	}
}