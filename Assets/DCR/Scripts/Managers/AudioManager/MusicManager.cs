using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{

    public AudioClip[] menuTheme;
    public AudioClip[] actionTheme;
    public AudioClip[] bossTheme;
    public AudioClip[] idleTheme;

    int sceneID;

    void Start()
    {
        //OnSceneLoaded(0);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int newSceneID = SceneManager.GetActiveScene().buildIndex;
        if (newSceneID != sceneID)
        {
            sceneID = newSceneID;
            Invoke("PlayMusic", .2f);

        }
        else {
            sceneID = newSceneID;
            Invoke("PlayMusic", .2f);
        }
    }

    void PlayMusic()
    {
        AudioClip clipToPlay = null;

        if (sceneID == 0)
        {
            clipToPlay = menuTheme[GetNewTrack(menuTheme)];
            
        }
        else if (sceneID == 1)
        {
            clipToPlay = actionTheme[GetNewTrack(actionTheme)];
        }
        else if (sceneID == 2)
        {
            clipToPlay = bossTheme[GetNewTrack(bossTheme)];
        }
        else if (sceneID == 3)
        {
            clipToPlay = idleTheme[GetNewTrack(idleTheme)];
        }

        if (clipToPlay != null)
        {
            AudioManager.instance.PlayMusic(clipToPlay, 2);
            Invoke("PlayMusic", clipToPlay.length);
        }

    }

    private int GetNewTrack(AudioClip[] theme)
    {
        return Random.Range(0, theme.Length);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
