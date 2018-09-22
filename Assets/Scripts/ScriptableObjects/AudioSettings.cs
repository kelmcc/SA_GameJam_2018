using System;
using UnityEngine;

[CreateAssetMenu(fileName ="AudioSettings", menuName ="Jam/AudioSettings")]
public class AudioSettings : ScriptableObject
{
	[Serializable]
	public class AudioClipData
	{
		public AudioClip clip;

		[Space]
		//Only one song will be used for beat detection
		public bool IsPrimary;
		public float GThreshhold;
		public int BufferSize;
	}

	[Serializable]
	public class Song
	{
		public AudioClipData[] clips;
		
		[Space]
		public float Bpm;
		public float StartBeatDelaySeconds;
	}

	public Song[] Songs;
}
