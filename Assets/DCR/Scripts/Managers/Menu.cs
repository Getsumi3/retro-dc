using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	public GameObject mainMenuHolder;
	public GameObject optionsMenuHolder;
    public Text highScoreHolder;

	public Slider[] volumeSliders;
	public Toggle[] resolutionToggles;
    public Toggle[] qualityToggles;
    public Toggle fullscreenToggle;
	public int[] screenWidths;
	int activeScreenResIndex;
    int activeQualityIndex;

	void Start() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        activeScreenResIndex = PlayerPrefs.GetInt ("screen res index");
		bool isFullscreen = (PlayerPrefs.GetInt ("fullscreen") == 1)?true:false;
        activeQualityIndex = PlayerPrefs.GetInt("quality index");

		volumeSliders [0].value = AudioManager.instance.masterVolumePercent;
		volumeSliders [1].value = AudioManager.instance.musicVolumePercent;
		volumeSliders [2].value = AudioManager.instance.sfxVolumePercent;

		for (int i = 0; i < resolutionToggles.Length; i++) {
			resolutionToggles [i].isOn = i == activeScreenResIndex;
		}
        for (int i = 0; i < qualityToggles.Length; i++)
        {
            qualityToggles[i].isOn = i == activeQualityIndex;
        }

        if (isFullscreen)
        {
            for (int i = 0; i < resolutionToggles.Length; i++)
            {
                resolutionToggles[i].isOn = !isFullscreen;
            }
            fullscreenToggle.isOn = isFullscreen;
        }
        
        highScoreHolder.text = "High Score: " + PlayerPrefs.GetInt("highScore");

        //reset currentScore
        PlayerPrefs.SetInt("playerScore", 0);
        PlayerPrefs.SetInt("level id", 0);

        //reset ammo
        PlayerPrefs.SetInt("normal_ammo", PlayerBehavior.normalProjectile);
        PlayerPrefs.SetInt("special_ammo", (int)PlayerBehavior.specialProjectile);
        PlayerPrefs.SetInt("heavy_ammo", PlayerBehavior.heavyProjectile);

        PlayerPrefs.SetFloat("starting health", 100);

    }


	public void Play() {
		SceneManager.LoadScene (1);
	}

	public void Quit() {
		Application.Quit ();
	}

	public void OptionsMenu() {
		mainMenuHolder.SetActive (false);
		optionsMenuHolder.SetActive (true);
	}

	public void MainMenu() {
		mainMenuHolder.SetActive (true);
		optionsMenuHolder.SetActive (false);
	}

	public void SetScreenResolution(int i) {
		if (resolutionToggles [i].isOn) {
			activeScreenResIndex = i;
			float aspectRatio = 16 / 9f;
			Screen.SetResolution (screenWidths [i], (int)(screenWidths [i] / aspectRatio), false);
			PlayerPrefs.SetInt ("screen res index", activeScreenResIndex);
			PlayerPrefs.Save ();
		}
	}

    public void SetQuality(int i)
    {
        if (qualityToggles[i].isOn)
        {
            activeQualityIndex = i;
            QualitySettings.SetQualityLevel(activeQualityIndex);
            PlayerPrefs.SetInt("quality index", activeQualityIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetFullscreen(bool isFullscreen) {
		for (int i = 0; i < resolutionToggles.Length; i++) {
			resolutionToggles [i].isOn = !isFullscreen;
		}

		if (isFullscreen) {
			Resolution[] allResolutions = Screen.resolutions;
			Resolution maxResolution = allResolutions [allResolutions.Length - 1];
			Screen.SetResolution (maxResolution.width, maxResolution.height, true);
		} else {
			SetScreenResolution (activeScreenResIndex);
		}

		PlayerPrefs.SetInt ("fullscreen", ((isFullscreen) ? 1 : 0));
		PlayerPrefs.Save ();
	}

	public void SetMasterVolume() {
        AudioManager.instance.SetVolume (volumeSliders[0].value, AudioManager.AudioChannel.Master);
	}

	public void SetMusicVolume() {
		AudioManager.instance.SetVolume (volumeSliders[1].value, AudioManager.AudioChannel.Music);
	}

	public void SetSfxVolume() {
		AudioManager.instance.SetVolume (volumeSliders[2].value, AudioManager.AudioChannel.Sfx);
	}

}
