using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace _193257_193435_193271
{
	public class MainMenu : MonoBehaviour
	{
		public Canvas ControlsCanvas;
		public Canvas MainMenuCanvas;
		public AudioClip menuMusic;
		public AudioClip resumeAudio;
		public AudioClip goAudio;

		private void Awake()
		{
			if (PlayerPrefs.GetInt("Sound") == 1)
			{
				AudioManager.instance.playRepetedly(menuMusic);
			}
		}

		void Update()
		{
			//chandle input for controls menu
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (ControlsCanvas.enabled)
				{
					AudioManager.instance.GuiPlayResume();
					ControlsCanvas.enabled = false;
					MainMenuCanvas.enabled = true;
				}
			}
		}

		public void OnLevel1ButtonPressed()
		{
			AudioManager.instance.GuiPlayAccept();
			SceneManager.LoadScene("193257_193435_193271");
		}

		public void OnLevel2ButtonPressed()
		{
			//menuEffecSource.PlayOneShot(goAudio, AudioListener.volume);
			//SceneManager.LoadScene("Level2");
		}

		public void onControlsButtonPressed()
		{
			if (MainMenuCanvas.enabled)
			{
				AudioManager.instance.GuiPlayAccept();
			}
			else
			{
				AudioManager.instance.GuiPlayResume();
			}
			//unselect button
			EventSystem.current.SetSelectedGameObject(null);

			//change canvas
			ControlsCanvas.enabled = !ControlsCanvas.enabled;
			MainMenuCanvas.enabled = !MainMenuCanvas.enabled;
		}

		public void OnExitToDesktopButtonPressed()
		{
			AudioManager.instance.GuiPlayAccept();
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
			Application.Quit();
		}
	}
}