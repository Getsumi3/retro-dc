using UnityEngine;
using UnityEngine.UI;

public class StatsKeeper : MonoBehaviour {

    public Text scoreText, healthText;

    private void Start()
    {
        scoreText = GetComponent<Text>();
        scoreText.text = ""+PlayerPrefs.GetInt("highScore", 0);
        healthText.text = "" + PlayerPrefs.GetFloat("starting health");

    }
}
