using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class SoundManager : MonoBehaviour {

	public AudioClip[] MusicPlaylist;

	[SerializeField]
	private float volume;

		
	public bool Shuffle;

	public RepeatMode Repeat;
	public float FadeDuration;
	public bool PlayOnAwake;

	[HideInInspector]
	public AudioSource source;


	public static SoundManager singleton { get; private set; } //Singleton


	private void Awake ()
	{

		if (singleton == null)
		{
			//If singleton doesnt exist then intialize it
			singleton = this;
			DontDestroyOnLoad (this.gameObject);
		}
		else
		{
			//Else destroy it. (This object will be present in all scenes)
			DestroyImmediate (this.gameObject);
		}
	}

	private void Start()
	{
		
		// create a game object and add an AudioSource to it, to play music on
		source = gameObject.GetComponent<AudioSource>();
		source.name = "SoundManager";
		source.playOnAwake = false;
		volume = 1f;
		if (FadeDuration > 0)
			source.volume = 0f;
		else
			source.volume = volume;
		if (MusicPlaylist == null)
			return;
		if (MusicPlaylist.Length > 0)
			source.clip = MusicPlaylist [Random.Range (0, MusicPlaylist.Length)];
		else {
			Debug.LogError ("There are no music in the list");
		}


		if (PlayOnAwake)
			Play ();
		StartCoroutine (PlayMusicList ());
	}
		
		


	public void Play()
	{
		StartCoroutine (PlayMusicList ());
	}

	public void Stop(bool fade)
	{
		StopAllCoroutines ();
		if (fade)
			StartCoroutine(StopWithFade());
		else
			source.Stop();
	}

	public void Next()
	{
		source.Stop ();
	}
		

	private IEnumerator StopWithFade()
	{
		if (FadeDuration > 0)
		{
			float lerpValue = 0f;
			while (lerpValue < 1f) {
				lerpValue += Time.deltaTime / FadeDuration;
				source.volume = Mathf.Lerp (volume, 0f, lerpValue);
				yield return null;
			}
		}
		source.Stop();
	}

	public void PlaySong(AudioClip song)
	{
		StartCoroutine (PlaySongE (song));
	}

	private IEnumerator PlaySongE(AudioClip clip)
	{
		source.Stop ();
		source.clip = clip;
		StartCoroutine (FadeIn ());
		source.Play ();
		while (source.isPlaying) {
			if (source.clip.length - source.time <= FadeDuration) {
				yield return StartCoroutine (FadeOut ());
			}
			yield return null;
		}
	}

	private int _counter = 0;
	private IEnumerator PlayMusicList()
	{
		while (true)
		{
			yield return StartCoroutine (PlaySongE (MusicPlaylist [_counter]));
			if (RepeatMode.Track == Repeat) 
			{
			}
			else if (Shuffle) {
				int newTrack = GetNewTrack ();
				while (newTrack == _counter) 
				{
					newTrack = GetNewTrack ();
				}
				_counter = newTrack;

			} 
			else 
			{
				_counter++;
				if (_counter >= MusicPlaylist.Length-1) 
				{
					if (Repeat == RepeatMode.Playlist)
						_counter = 0;
					else
						yield break;
				}
			}
		}
	}

	private IEnumerator FadeOut()
	{
		if (FadeDuration > 0f) {
			float startTime = source.clip.length - FadeDuration;
			float lerpValue = 0f;
			while (lerpValue < 1f) {
				lerpValue = Mathf.InverseLerp (startTime, source.clip.length, source.time);
				source.volume = Mathf.Lerp (volume, 0f, lerpValue);
				yield return null;
			}
		} else {
			yield break;
		}
	}

	private IEnumerator FadeIn()
	{
		if (FadeDuration > 0f)
		{
			float lerpValue = 0f;
			while (lerpValue < 1f) {
				lerpValue = Mathf.InverseLerp (0f, FadeDuration, source.time);
				source.volume = Mathf.Lerp (0f, volume, lerpValue);
				yield return null;
			}
		}
		else
		{
			yield break;
		}
	}

	private int GetNewTrack()
	{
		return Random.Range (0, MusicPlaylist.Length);
	}
		 
}


public enum RepeatMode
{
	Playlist,
	Track,
	None
}