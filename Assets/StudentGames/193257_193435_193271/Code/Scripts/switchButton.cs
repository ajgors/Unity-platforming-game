using UnityEngine;
using UnityEngine.UI;

namespace _193257_193435_193271
{
	public class SwitchButton : MonoBehaviour
	{
		public Sprite onTexture;
		public Sprite offTexture;
		public string pref;
		private int isOn = 1;

		private void Start()
		{
			isOn = PlayerPrefs.GetInt(pref.ToString());

			if (isOn == 0)
			{
				GetComponent<Image>().sprite = offTexture;
			}
		}

		public void ToggleSwitch()
		{
			// Toggle the state
			isOn = isOn == 1 ? isOn = 0 : isOn = 1;

			// Set the texture based on the state
			SetButtonTexture();
		}

		private void SetButtonTexture()
		{
			// Set the button texture based on the current state
			GetComponent<Image>().sprite = (isOn == 1) ? onTexture : offTexture;
		}
	}
}