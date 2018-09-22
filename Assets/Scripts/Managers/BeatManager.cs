using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour 
{
	public GameManager gameManager;
	public AudioSettings AudioSettings;

	private AudioSettings.Song currentSong;
	private List<ActiveAudioData> currentAudioData;
	private float currentBps;
	private int lastBeatSample;
	private float currentFrequencyInverse;

	public event Action OnBeat = delegate { };
	public event Action<float[]> OnSpectrumUpdated = delegate { };

	public class ActiveAudioData
	{
		public AudioSource source;

		public bool IsPrimary;
		public AudioProcessor processor;
	}

	void Start ()
	{
		Debug.Assert(AudioSettings.Songs.Length > 0, "[BeatManager] no songs in audiosettings");
	
		PlaySong(AudioSettings.Songs[UnityEngine.Random.Range(0, AudioSettings.Songs.Length)]);
	}

	private void PlaySong(AudioSettings.Song song)
	{
		AudioProcessor[] oldProcs = gameObject.GetComponents<AudioProcessor>();
		foreach(AudioProcessor proc in oldProcs)
		{
			proc.enabled = false;
			Destroy(proc);
		}
		AudioSource[] oldSources = gameObject.GetComponents<AudioSource>();
		foreach (AudioSource src in oldSources)
		{
			src.enabled = false;
			Destroy(src);
		}

		//initialize
		currentAudioData = new List<ActiveAudioData>();
		bool primarySet = false;
		currentSong = song;

		foreach(AudioSettings.AudioClipData clip in song.clips)
		{
			ActiveAudioData data = new ActiveAudioData();

			data.source = gameObject.AddComponent<AudioSource>();
			data.source.clip = clip.clip;

			if (clip.IsPrimary)
			{			
				if(primarySet)
				{
					Debug.LogError("There can only be one primary audio clip per song. ignoring");
				}
				else
				{
					//use this for beat detection		
					data.processor = gameObject.AddComponent<AudioProcessor>();
					data.processor.audioSource = data.source;
					data.processor.gThresh = clip.GThreshhold;
					data.processor.bufferSize = clip.BufferSize;

					//add when they are initialized
					data.processor.onBeat = new AudioProcessor.OnBeatEventHandler();
					data.processor.onSpectrum = new AudioProcessor.OnSpectrumEventHandler();

					data.processor.onBeat.AddListener(OnOnbeatDetected);				
					data.processor.onSpectrum.AddListener(OnSpectrum);
				}			
			}

			data.IsPrimary = clip.IsPrimary;
			currentAudioData.Add(data);
		}

		//audio does not care about time scale afaik? 
		ActiveAudioData primary = PrimaryAudioData();

		currentBps = song.Bpm / 60f;
		currentFrequencyInverse = 1f / primary.source.clip.frequency;

		StartCoroutine(PlayAfterDelay(primary, song.StartBeatDelaySeconds));
	}

	private IEnumerator PlayAfterDelay(ActiveAudioData primary, float delay)
	{
		if (delay > 0)
		{
			yield return new WaitForSeconds(delay);
		}

		foreach (ActiveAudioData data in currentAudioData)
		{
			data.source.Play();
		}
		lastBeatSample = primary.source.timeSamples;
	}

	public ActiveAudioData PrimaryAudioData()
	{
		foreach(ActiveAudioData  a in currentAudioData)
		{
			if(a.IsPrimary)
			{
				return a;
			}
		}

		Debug.LogError("No primary audio data");
		return null;
	}

	private void OnOnbeatDetected()
	{
		//yehatehahteahyeayea whateveryousay
	}

	//This event will be called every frame while music is playing
	private void OnSpectrum(float[] spectrum)
	{
		//The spectrum is logarithmically averaged
		//to 12 bands

		OnSpectrumUpdated(spectrum);
	}

	private void Update()
	{
		//check beat against bps to see if its a primary beat
		ActiveAudioData a = PrimaryAudioData();
		if (a.source.isPlaying)
		{
			double secondsPerBeat = 1f / currentBps;
			int currentSamples = a.source.timeSamples;
			if ((currentSamples - lastBeatSample) * currentFrequencyInverse > secondsPerBeat)
			{
				//we subtract the sample amount we are behind by so the time does not drift (since update will never run exactly on beat)
				int samplesPerBeat = (int)(a.source.clip.frequency * secondsPerBeat);
				int timeOverLastBeat = currentSamples % samplesPerBeat;

				int exactCurrentBeatTime = currentSamples - timeOverLastBeat;

				lastBeatSample = exactCurrentBeatTime;
				OnBeat();
				Debug.Log("Beat!");
			}
		}
		else
		{
			PlaySong(AudioSettings.Songs[UnityEngine.Random.Range(0, AudioSettings.Songs.Length)]);
		}
	}
}
