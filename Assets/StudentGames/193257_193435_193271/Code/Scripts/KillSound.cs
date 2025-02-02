using UnityEngine;

namespace _193257_193435_193271
{
	//script to kill sound source after it's done playing
	public class KillSound : MonoBehaviour
	{
		AudioSource source;

		void Start()
		{
			source = GetComponent<AudioSource>();
		}

		void Update()
		{
			if (!source.isPlaying)
				Destroy(gameObject);
		}
	}
}