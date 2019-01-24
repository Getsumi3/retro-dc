using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class CursorController : MonoBehaviour {

	public GameObject pauseMenu;
    public UnityStandardAssets.Characters.FirstPerson.MouseLook mouseLook;
	
	public bool isPaused = false;

    private void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Pause"))
        {
            Trigger_Pause();
        }
        else if (CrossPlatformInputManager.GetButtonDown("Play"))
        {
            Trigger_Continue();
        }
    }

    public void Trigger_Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        mouseLook.SetCursorLock(false);
    }

    public void Trigger_Continue()
    { 
		pauseMenu.SetActive (false);
		Time.timeScale = 1f;
		isPaused = false;
        mouseLook.SetCursorLock(true);
    }

    public void Trigger_MainMenu()
    {
        Time.timeScale = 1f;
        //reset currentScore
        PlayerPrefs.SetInt("playerScore", 0);
        PlayerPrefs.SetInt("level id", 0);
        //reset ammo
        PlayerPrefs.SetInt("normal_ammo", PlayerBehavior.normalProjectile);
        PlayerPrefs.SetInt("special_ammo", (int)PlayerBehavior.specialProjectile);
        PlayerPrefs.SetInt("heavy_ammo", PlayerBehavior.heavyProjectile);
        mouseLook.SetCursorLock(false);
        SceneManager.LoadScene(0);
       

    }
}
