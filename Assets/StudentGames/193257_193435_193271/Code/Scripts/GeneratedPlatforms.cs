using UnityEngine;

namespace _193257_193435_193271
{
	public class GeneratedPlatforms : MonoBehaviour
	{
		[SerializeField]
		public GameObject platformPrefab;
		const int PLATFORMS_NUM = 3;
		GameObject[] platforms;
		Vector3[] positions;
		Vector3[] destPositions;
		Vector3[] currentTargets;

		float speed = 1f;

		void Awake()
		{
			platforms = new GameObject[PLATFORMS_NUM];
			positions = new Vector3[PLATFORMS_NUM];
			destPositions = new Vector3[PLATFORMS_NUM];
			currentTargets = new Vector3[PLATFORMS_NUM];

			for (int i = 0; i < PLATFORMS_NUM; i++)
			{
				positions[i] = new Vector3(41 + 3 * i, -18, 0);
				destPositions[i] = positions[i];
				currentTargets[i] = new Vector3(48 + 3 * i, -18, 0);
				platforms[i] = Instantiate(platformPrefab, positions[i], Quaternion.identity);
			}
		}

		void Update()
		{
			for (int i = 0; i < PLATFORMS_NUM; i++)
			{
				platforms[i].transform.position = Vector3.MoveTowards(platforms[i].transform.position, currentTargets[i], speed * Time.deltaTime);

				if (Vector3.Distance(platforms[i].transform.position, currentTargets[i]) < 0.001f)
				{
					Vector3 buf = currentTargets[i];
					currentTargets[i] = destPositions[i];
					destPositions[i] = buf;
				}
			}
		}
	}
}