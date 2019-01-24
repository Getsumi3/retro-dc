using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {
	public Text scoreText,scoreTextBG;
    public Image hpBar, manaBar, manaIcon;
	public GameObject gameOverPanel,knifeSelector,gunSelector, portalImg;
    public static GameObject portal;
    int currentScore=0;
	static GameManager myslf;
	public bool gameOver=false;
    public static int highScore;

    public static int levelIndex = 0;
    int enemyPerWave = 5;
    public static bool isBossRoom = false;
	static int enemyCount;

    public static int EnemyCount
    {
        get{return enemyCount;}
        set{enemyCount = value;}
    }

    void Awake(){
		myslf = this;
	}

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("highScore");
        myslf.currentScore = PlayerPrefs.GetInt("playerScore");
        levelIndex = PlayerPrefs.GetInt("level id");
        levelIndex++;
        myslf.scoreText.text = myslf.currentScore.ToString();
        myslf.scoreTextBG.text = myslf.currentScore.ToString();
        if (highScore < PlayerPrefs.GetInt("playerScore"))
        {
            highScore = PlayerPrefs.GetInt("playerScore");
            PlayerPrefs.SetInt("highScore", highScore);
        }
    }


    public static void AddScore(int pointsAdded){
		myslf.currentScore += pointsAdded;
        PlayerPrefs.SetInt("playerScore", myslf.currentScore);
		myslf.scoreText.text = myslf.currentScore.ToString ();
		myslf.scoreTextBG.text = myslf.currentScore.ToString ();
		myslf.scoreText.transform.localScale = Vector3.one * 2.5f;
		iTween.Stop (myslf.scoreText.gameObject);
		iTween.ScaleTo (myslf.scoreText.gameObject, iTween.Hash ("scale", Vector3.one, "time", 0.25f, "delay", 0.1f, "easetype", iTween.EaseType.spring));
        if (highScore < PlayerPrefs.GetInt("playerScore"))
        {
            highScore = PlayerPrefs.GetInt("playerScore");
            PlayerPrefs.SetInt("highScore", highScore);
        }
    }
	public static void RegisterPlayerDeath(){
        //disable player controller
        FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
        FindObjectOfType<PlayerBehavior>().enabled = false;

        if (highScore < PlayerPrefs.GetInt("playerScore"))
        {
            highScore = PlayerPrefs.GetInt("playerScore");
            PlayerPrefs.SetInt("highScore", highScore);
        }

        PlayerPrefs.SetInt("level id", 0);
        PlayerPrefs.SetInt("playerScore", 0);
        PlayerPrefs.SetFloat("starting health", 100);

        myslf.ResetAmmo();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        myslf.gameOverPanel.SetActive (true);
        myslf.gameOver = true;

	}

    private int EnemiesToKill()
    {
        float t = Mathf.Pow(1.15f, levelIndex);
        int nextWaveEnemies = (int)Mathf.Floor(enemyPerWave * t);
        print(nextWaveEnemies);
        return nextWaveEnemies;
    }

    private void ResetAmmo()
    {
        //reset ammo
        PlayerPrefs.SetInt("normal_ammo", PlayerBehavior.normalProjectile);
        PlayerPrefs.SetInt("special_ammo", (int)PlayerBehavior.specialProjectile);
        PlayerPrefs.SetInt("heavy_ammo", PlayerBehavior.heavyProjectile);
    }

    public void Restart()
    {
        if (isBossRoom)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public static void UpdateHealth(float health, float startingHealth)
    {
        myslf.hpBar.fillAmount = health / startingHealth;
    }

    public static void UpdateAmmo(float ammo, float maxAmmo, string prefsKey)
    {
        myslf.manaBar.fillAmount = ammo / maxAmmo;
        PlayerPrefs.SetInt(prefsKey, (int)ammo);
    }

    public static void UpdateAmmo(Color ammoTypeColor)
    {
        myslf.manaBar.color = ammoTypeColor;
        myslf.manaIcon.color = ammoTypeColor;
    }
    public static void AddToEnemyCount(){
        if (!isBossRoom)
        {
            EnemyCount++;
            if (EnemyCount >= myslf.EnemiesToKill())
            {
                portal.SetActive(true);
                myslf.portalImg.SetActive(true);
            }
        }
        else {
            Debug.LogWarning("Boss room - kill the boss to escape this room");
        }
	}
	public static void RemoveEnemy(){
		EnemyCount--;
		if (EnemyCount <= 0) {
			//myslf.endSection.SetActive(true);
		}

	}
}

