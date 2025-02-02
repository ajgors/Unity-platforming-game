using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _193257_193435_193271
{
	public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER, GS_OPTIONS, GS_CONTROLS }

	public class GameManager : MonoBehaviour
	{
		public GameState currentGameState = GameState.GS_GAME;
		public static GameManager instance;

		//canvases
		public Canvas inGameCanvas;
		public Canvas pauseMenuCanvas;
		public Canvas LevelCompletedCanvas;
		public Canvas optionsCanvas;
		public Canvas gameOverCanvas;
		public Canvas controlsCanvas;

		//game canvas
		public TMP_Text scoreText;
		public TMP_Text timerText;
		public TMP_Text killsText;
		public TMP_Text bulletsText;

		//settings canvas
		public TMP_Text QualityText;

		//game complited canvas
		public TMP_Text overallScoreText;
		public TMP_Text highScoreText;

		//game canvas
		private int score = 0;
		public float timer = 0f;
		private int minutes = 0, seconds = 0;
		public UnityEngine.UI.Image[] keysTab;
		public UnityEngine.UI.Image[] heartTab;
		private int keysFound = 0;
		private int hp = 3;
		private int kills = 0;

		const string keyHighScore = "HighScore193257_193435_193271";
		const string keyVolume = "Volume";
		const string keyQuality = "Quality";
		const string keySound = "Sound";
		public Slider volumeSlider;


		void Start()
		{
			instance = this;

			//Shuffle the array of game music tracks
			AudioManager.instance.ShufflMusic();

			//Play the first track
			AudioManager.instance.PlayNextTrack();
		}

		void Update()
		{
			//pause music when pause menu is active
			if (currentGameState == GameState.GS_PAUSEMENU || currentGameState == GameState.GS_OPTIONS)
			{
				AudioManager.instance.PauseMusic();
			}

			if (currentGameState == GameState.GS_OPTIONS)
			{
				optionsMenu();
			}

			if (currentGameState == GameState.GS_GAME)
			{
				if (PlayerPrefs.GetInt(keySound) == 0)
				{
					AudioManager.instance.PauseMusic();
				}
				else
				{
					AudioManager.instance.ResumeMusic();
				}
				AddTime();
			}

			if (currentGameState == GameState.GS_GAME_OVER)
			{
				StartCoroutine(GameOver()); //add extra delay to show death animation
			}
		}


		void Awake()
		{
			instance = this;
			scoreText.text = score.ToString();
			updateQualityText();

			foreach (UnityEngine.UI.Image image in keysTab)
			{
				image.color = Color.grey;
			}

			InGame();

			if (!PlayerPrefs.HasKey(keySound))
			{
				PlayerPrefs.SetInt(keySound, 1);
			}

			if (!PlayerPrefs.HasKey(keyHighScore))
			{
				PlayerPrefs.SetInt(keyHighScore, 0);
			}

			if (volumeSlider != null)
			{
				volumeSlider.value = PlayerPrefs.GetFloat(keyVolume);
			}

			if (!PlayerPrefs.HasKey(keyQuality))
			{
				PlayerPrefs.SetInt(keyQuality, QualitySettings.GetQualityLevel());
			}
			else if (PlayerPrefs.HasKey(keyQuality))
			{
				int qualityLevel = PlayerPrefs.GetInt(keyQuality);
				setQualityText(qualityLevel);
				setQuality(qualityLevel);
			}
		}

		void setGameState(GameState newGameState)
		{
			currentGameState = newGameState;
			pauseMenuCanvas.enabled = currentGameState == GameState.GS_PAUSEMENU;
			LevelCompletedCanvas.enabled = currentGameState == GameState.GS_LEVELCOMPLETED;
			optionsCanvas.enabled = currentGameState == GameState.GS_OPTIONS;
			gameOverCanvas.enabled = currentGameState == GameState.GS_GAME_OVER;
			controlsCanvas.enabled = currentGameState == GameState.GS_CONTROLS;

			if (newGameState == GameState.GS_LEVELCOMPLETED)
			{
				Scene currentScene = SceneManager.GetActiveScene();

				if (currentScene.name == "193257_193435_193271")
				{
					int highScore = PlayerPrefs.GetInt(keyHighScore);

					if (highScore < score)
					{
						highScore = score;
						PlayerPrefs.SetInt(keyHighScore, highScore);
					}

					overallScoreText.text = "Your score: " + score.ToString();
					highScoreText.text = "High score: " + highScore.ToString();
				}
			}
		}


		public void optionsMenu()
		{
			setGameState(GameState.GS_OPTIONS);
			Time.timeScale = 0;
		}


		public void onReturnToPauseMenuButtonClicked()
		{
			AudioManager.instance.GuiPlayResume();
			PauseMenu();
		}


		public void PauseMenu()
		{
			setGameState(GameState.GS_PAUSEMENU);
			Time.timeScale = 0;
		}


		public void InGame()
		{
			setGameState(GameState.GS_GAME);
			Time.timeScale = 1f;
		}


		public void LevelCompleted()
		{
			setGameState(GameState.GS_LEVELCOMPLETED);
			Time.timeScale = 0;
		}


		public IEnumerator GameOver()
		{
			yield return new WaitForSeconds(0.8f);
			setGameState(GameState.GS_GAME_OVER);
			Time.timeScale = 0;
		}


		public void AddPoints(int points)
		{
			score += points;
			scoreText.text = score.ToString();
		}


		public void AddKey(String name)
		{
			Color color = Color.black;
			switch (name)
			{
				case "Gem-1":
					color = Color.green;
					break;
				case "Gem-2":
					color = Color.red;
					break;
				case "Gem-3":
					color = Color.blue;
					break;
			}

			keysFound++;
			keysTab[keysFound - 1].GetComponent<UnityEngine.UI.Image>().color = color;
		}


		public void AddTime()
		{
			timer += Time.deltaTime;

			if (timer >= 1f)
			{
				timer -= 1f;
				seconds += 1;
			}

			if (seconds == 60)
			{
				seconds = 0;
				minutes += 1;
			}

			timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
		}

		public void SetPlayerHP(int hp)
		{
			this.hp = hp;
		}

		public void AddHP()
		{
			if (hp < 3)
			{
				heartTab[hp].GetComponent<UnityEngine.UI.Image>().enabled = true;
				hp++;
			}
		}

		public void LoseHP()
		{
			hp--;
			heartTab[hp].GetComponent<UnityEngine.UI.Image>().enabled = false;

			if (hp == 0)
			{
				currentGameState = GameState.GS_GAME_OVER;
			}
		}

		public void AddKill()
		{
			kills++;
			killsText.text = kills.ToString();
		}

		public void onResumeToButtonClicked()
		{
			AudioManager.instance.GuiPlayResume();
			InGame();
		}

		public void onRestartButtonClicked()
		{
			AudioManager.instance.GuiPlayAccept();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		public void onReturnToMainMenuButtonClicked()
		{
			Time.timeScale = 1;
			AudioManager.instance.GuiPlayAccept();

			int sceneIndex = SceneUtility.GetBuildIndexByScenePath("Assets/StudentGames/193257_193435_193271/Level/Scenes/Main Menu.unity");
			Debug.Log(sceneIndex);
			if (sceneIndex >= 0)
			{
				SceneManager.LoadSceneAsync(sceneIndex); //³adowanie sceny ³¹cz¹cej gry
			}
			else
			{
				SceneManager.LoadScene("MainMenu");

			}
		}

		public void onOptionsButtonClicked()
		{
			AudioManager.instance.GuiPlayAccept();
			if (currentGameState == GameState.GS_PAUSEMENU)
			{
				setGameState(GameState.GS_OPTIONS);
			}
		}

		public void onControlsButtonClicked()
		{
			AudioManager.instance.GuiPlayAccept();
			if (currentGameState == GameState.GS_PAUSEMENU)
			{
				setGameState(GameState.GS_CONTROLS);
			}
		}


		public void onSoundButtonClicked()
		{
			PlayerPrefs.GetInt(keySound);
			if (PlayerPrefs.GetInt(keySound) == 1)
			{
				PlayerPrefs.SetInt(keySound, 0);
				AudioManager.instance.PauseMusic();
			}
			else
			{
				PlayerPrefs.SetInt(keySound, 1);
				AudioManager.instance.ResumeMusic();
			}
		}

		public void onDecreaseQualityButtonClicked()
		{
			QualitySettings.DecreaseLevel();
			PlayerPrefs.SetInt(keyQuality, QualitySettings.GetQualityLevel());
			updateQualityText();
		}

		public void onIncreaseQualityButtonClicked()
		{
			QualitySettings.IncreaseLevel();
			PlayerPrefs.SetInt(keyQuality, QualitySettings.GetQualityLevel());
			updateQualityText();
		}

		private void updateQualityText()
		{
			QualityText.text = "Quality: " + getQualityText();
		}

		private void setQualityText(int qualityLevel)
		{
			QualityText.text = "Quality: " + QualitySettings.names[qualityLevel];
		}

		private string getQualityText()
		{
			return QualitySettings.names[QualitySettings.GetQualityLevel()];
		}

		private void setQuality(int qualityLevel)
		{
			QualitySettings.SetQualityLevel(qualityLevel);
		}

		public void onSlideChange()
		{
			AudioManager.instance.setVolume(volumeSlider.value);
		}

		public int getHp()
		{
			return hp;
		}
	}
}