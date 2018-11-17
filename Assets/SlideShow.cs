using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideShow : MonoBehaviour
{
	int currentIndex = 0;
	public CanvasGroup[] slides;

	int beat = 0;

	// Use this for initialization
	void Start ()
	{
		FindObjectOfType<BeatManager>().OnBeat += Onbeat;
		currentIndex = 0;
		slides[0].alpha = 1;
		slides[1].alpha = 0;
	}
	
	void Onbeat(long beatCount)
	{
		beat++;
		if(beat % 8 != 0)
		{
			return;
		}

		currentIndex++;
		if(currentIndex >= slides.Length)
		{
			currentIndex = 0;
		}
		for(int i = 0; i < slides.Length; i++)
		{
			if(i == currentIndex)
			{
				slides[i].alpha = 1;
			}
			else
			{
				slides[i].alpha = 0;
			}
		}
	}
}
