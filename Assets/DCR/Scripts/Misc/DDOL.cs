using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOL : MonoBehaviour {

    public static DDOL instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnApplicationQuit()
    {
        //reset ammo
        PlayerPrefs.SetInt("normal_ammo", PlayerBehavior.normalProjectile);
        PlayerPrefs.SetInt("special_ammo", (int)PlayerBehavior.specialProjectile);
        PlayerPrefs.SetInt("heavy_ammo", PlayerBehavior.heavyProjectile);
        PlayerPrefs.SetFloat("starting health", 100);
        //save score
        if (PlayerPrefs.GetInt("highScore") < PlayerPrefs.GetInt("playerScore"))
        {
            PlayerPrefs.SetInt("highScore", PlayerPrefs.GetInt("playerScore"));
        }
        PlayerPrefs.Save();
    }
}
