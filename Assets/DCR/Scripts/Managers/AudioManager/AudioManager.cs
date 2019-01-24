using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    public enum AudioChannel { Master, Sfx, Music };

    public float masterVolumePercent { get; private set; }
    public float sfxVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    public static AudioManager instance;

    //Transform audioListener;
    //Transform playerT;

    SoundLibrary library;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {

            instance = this;
            DontDestroyOnLoad(gameObject);

            library = GetComponent<SoundLibrary>();

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }
            GameObject newSfx2Dsource = new GameObject("2D sfx source");
            sfx2DSource = newSfx2Dsource.AddComponent<AudioSource>();
            newSfx2Dsource.transform.parent = transform;

            //audioListener = FindObjectOfType<AudioListener>().transform;
            //if (FindObjectOfType<PlayerBehavior>() != null)
            //{
            //    playerT = FindObjectOfType<PlayerBehavior>().transform;
            //}

            masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1f);
            sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 0.75f);
            musicVolumePercent = PlayerPrefs.GetFloat("music vol", 0.5f);
        }
    }

    //void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    if (playerT == null)
    //    {
    //        if (FindObjectOfType<PlayerBehavior>() != null)
    //        {
    //            playerT = FindObjectOfType<PlayerBehavior>().transform;
    //        }
    //    }
    //}

    //void Update()
    //{
    //    if (playerT != null)
    //    {
    //        audioListener.position = playerT.position;
    //    }
    //}


    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                AudioListener.volume = masterVolumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                sfx2DSource.volume = sfxVolumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                musicSources[0].volume = musicVolumePercent;
                musicSources[1].volume = musicVolumePercent;
                break;
        }

        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
        PlayerPrefs.Save();
    }

    public void PlayRngMusic(AudioClip clip, float fadeDuration = 1)
    {
        StartCoroutine(PlayMusicList());
        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].Stop();
        musicSources[activeMusicSourceIndex].clip = clip;
        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
        musicSources[activeMusicSourceIndex].Play();
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(library.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent);
    }

    public IEnumerator PlayMusicY(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].Stop();
        musicSources[activeMusicSourceIndex].clip = clip;
        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
        musicSources[activeMusicSourceIndex].Play();
        while (musicSources[activeMusicSourceIndex].isPlaying)
        {
            if (musicSources[activeMusicSourceIndex].clip.length - musicSources[activeMusicSourceIndex].time <= fadeDuration)
            {
                yield return StartCoroutine(AnimateMusicCrossfade(fadeDuration));
            }
            yield return null;
        }
    }

    int _counter = 0;
    public IEnumerator PlayMusicList()
    {
        while (true)
        {
            yield return StartCoroutine(PlayMusicY(library.soundGroups[0].group[GetNewTrack()], 2));

            int newTrack = GetNewTrack();
            while (newTrack == _counter)
            {
                newTrack = GetNewTrack();
            }
            _counter = newTrack;

        }
    }

    private int GetNewTrack()
    {
        return Random.Range(0, library.soundGroups[0].group.Length);
    }


    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent, 0, percent);
            yield return null;
        }
    }

    //void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}
}
