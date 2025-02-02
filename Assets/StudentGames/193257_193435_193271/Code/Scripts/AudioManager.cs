using UnityEngine;

namespace _193257_193435_193271
{
	public enum SoundEffect
	{
		enemyKillSound,
		enemyHitSound,
		jumpSound,
		checkPointSound,
		dashSound,
		cherryPickSound,
		bulletSound,
		runningSound,
		landingSound,
		keyPickUpSound,
		footStepSound,
		playerDeathSound,
	}

	public class AudioManager : MonoBehaviour
	{
		public static AudioManager instance;
		public AudioSource GuiEffectSource; //source for effects
		public AudioSource MusicSource; //source for music
		private const float defaultVol = 0.5f;
		private const string keyVolume = "Volume";

		//music
		public AudioClip[] gameMusics;
		private int currentTrackIndex = 0;

		//sound
		[SerializeField] AudioClip enemyKillSound;
		[SerializeField] AudioClip enemyHitSound;
		[SerializeField] AudioClip jumpSound;
		[SerializeField] AudioClip checkPointSound;
		[SerializeField] AudioClip dashSound;
		[SerializeField] AudioClip cherryPickSound;
		[SerializeField] AudioClip bulletSound;
		[SerializeField] AudioClip runningSound;
		[SerializeField] AudioClip landingSound;
		[SerializeField] AudioClip keyPickUpSound;
		[SerializeField] AudioClip playerDeathSound;

		//gui
		[SerializeField] AudioClip GuiAcceptSound;
		[SerializeField] AudioClip GuiResumeSound;
		public GameObject soundObject;

		private bool isMusicPaused = false;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}

			if (!PlayerPrefs.HasKey(keyVolume))
			{
				setVolume(defaultVol);
			}
			else
			{
				//get volume from player prefs
				AudioListener.volume = PlayerPrefs.GetFloat(keyVolume);
			}

			GuiEffectSource.loop = false;
		}

		public void setVolume(float vol)
		{
			AudioListener.volume = vol;
			MusicSource.volume = vol;
			PlayerPrefs.SetFloat(keyVolume, vol); //save player prefs
		}

		public void PlaySFX(SoundEffect sfx)
		{
			switch (sfx)
			{
				case SoundEffect.enemyKillSound:
					SoundObjectCreation(enemyKillSound);
					break;

				case SoundEffect.enemyHitSound:
					SoundObjectCreation(enemyHitSound);
					break;

				case SoundEffect.jumpSound:
					SoundObjectCreation(jumpSound);
					break;

				case SoundEffect.checkPointSound:
					SoundObjectCreation(checkPointSound);
					break;

				case SoundEffect.dashSound:
					SoundObjectCreation(dashSound);
					break;

				case SoundEffect.cherryPickSound:
					SoundObjectCreation(cherryPickSound);
					break;

				case SoundEffect.bulletSound:
					SoundObjectCreation(bulletSound);
					break;

				case SoundEffect.landingSound:
					SoundObjectCreation(landingSound);
					break;

				case SoundEffect.keyPickUpSound:
					SoundObjectCreation(keyPickUpSound);
					break;

				case SoundEffect.runningSound:
					SoundObjectCreation(runningSound);
					break;

				case SoundEffect.playerDeathSound:
					SoundObjectCreation(playerDeathSound);
					break;

			}
		}

		void SoundObjectCreation(AudioClip clip)
		{
			GameObject newGameObject = Instantiate(soundObject);
			AudioSource audioSource = newGameObject.GetComponent<AudioSource>();
			audioSource.PlayOneShot(clip, AudioListener.volume);
		}

		// Method to pause the music
		public void PauseMusic()
		{
			MusicSource.Pause();
			isMusicPaused = true;
		}

		// Method to resume the music
		public void ResumeMusic()
		{
			MusicSource.UnPause();
			isMusicPaused = false;
		}

		public void playRepetedly(AudioClip newMusic)
		{
			ChangeMusic(newMusic);
			MusicSource.loop = true;
		}

		public void ChangeMusic(AudioClip newMusic)
		{
			MusicSource.loop = false;
			MusicSource.Stop();
			MusicSource.clip = newMusic;
			MusicSource.Play();
		}

		//gui sounds
		public void GuiPlayResume()
		{
			GuiEffectSource.PlayOneShot(GuiResumeSound, AudioListener.volume);
		}

		public void GuiPlayAccept()
		{
			GuiEffectSource.PlayOneShot(GuiAcceptSound, AudioListener.volume);
		}


		public void ShufflMusic()
		{
			System.Random random = new System.Random();

			int n = gameMusics.Length;
			for (int i = n - 1; i > 0; i--)
			{
				// Generate a random index between 0 and i (inclusive)
				int randomIndex = random.Next(0, i + 1);

				// Swap the elements at randomIndex and i
				AudioClip temp = gameMusics[i];
				gameMusics[i] = gameMusics[randomIndex];
				gameMusics[randomIndex] = temp;
			}
		}

		private void Update()
		{
			//change song when current song ends
			if (!MusicSource.isPlaying && !isMusicPaused && Application.isFocused)
			{
				currentTrackIndex++;

				// Check if there are more tracks to play
				if (currentTrackIndex < gameMusics.Length)
				{
					// Play the next track
					PlayNextTrack();
				}
				else
				{
					// If no more tracks, loop back to the first track
					currentTrackIndex = 0;
					PlayNextTrack();
				}
			}
		}

		public void PlayNextTrack()
		{
			// Ensure the current index is valid
			if (currentTrackIndex >= 0 && currentTrackIndex < gameMusics.Length)
			{
				// Change the music to the current track
				ChangeMusic(gameMusics[currentTrackIndex]);
			}
		}
	}
}
