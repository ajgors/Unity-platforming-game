using UnityEngine;

namespace _193257_193435_193271
{
	public class Shooting : MonoBehaviour
	{
		public Transform firePoint;
		public GameObject bulletPrefab;

		public float shootDelay = 0.5f; // Time delay between shots
		private float shootTimer = 0f;
		private int currentBullets;

		void Start()
		{
			currentBullets = 3;
			GameManager.instance.bulletsText.text = currentBullets.ToString();
		}

		void Update()
		{
			// Update shoot timer
			shootTimer += Time.deltaTime;

			// Check if player can shoot
			if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")) && CanShoot())
			{
				Shoot();
			}
		}

		private bool CanShoot()
		{
			// Check if enough time has passed since the last shot and if there are bullets remaining
			return shootTimer >= shootDelay && currentBullets > 0;
		}

		private void Shoot()
		{
			// Instantiate bullet
			Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

			// Reset shoot timer
			shootTimer = 0f;

			// Decrease bullet count
			currentBullets--;
			GameManager.instance.bulletsText.text = currentBullets.ToString();
		}

		// Call this method when the player picks up bullets
		public void PickUpBullets(int bulletsToGive)
		{
			// Increase current bullets, but not exceeding the maximum
			currentBullets += bulletsToGive;
			GameManager.instance.bulletsText.text = currentBullets.ToString();
		}
	}
}