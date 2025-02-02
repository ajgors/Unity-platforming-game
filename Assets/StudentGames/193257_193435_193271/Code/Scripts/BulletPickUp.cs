using UnityEngine;

namespace _193257_193435_193271
{
	public class BulletPickup : MonoBehaviour
	{
		public int bulletsToGive = 3; // Number of bullets to give to the player

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				Shooting shootingScript = other.GetComponent<Shooting>();
				if (shootingScript != null)
				{
					AudioManager.instance.PlaySFX(SoundEffect.cherryPickSound);

					//update number of bullets player have
					shootingScript.PickUpBullets(bulletsToGive);
					Destroy(gameObject);
				}
			}
		}
	}
}