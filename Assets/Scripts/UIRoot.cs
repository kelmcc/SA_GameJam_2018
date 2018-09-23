using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UIRoot : MonoBehaviour
{
	public GameManager GameManager;
	public GameObject ImageBeatLight;
	public GameObject ImageBeatDark;
	public UILineRenderer SpectrumLine;

	public Vector2 spectrumDimensions;

	private BeatManager beatManager;

    public Image[] BeatLevel;

	// Use this for initialization
	void Start ()
	{
		GameManager.BeatManager.OnBeat += OnBeat;
		GameManager.BeatManager.OnSpectrumUpdated += SpectrumUpdate;
	}

	void OnBeat()
	{
		ImageBeatDark.SetActive(ImageBeatLight.activeSelf);
		ImageBeatLight.SetActive(!ImageBeatLight.activeSelf);		
	}

	// Update is called once per frame
	void SpectrumUpdate(float[] spectrum)
	{
		List<Vector2> points = new List<Vector2>();
		for (int i = 0; i < spectrum.Length; ++i)
		{
			points.Add(new Vector2((i * (spectrumDimensions.x / spectrum.Length)) - (spectrumDimensions.x / 2), spectrum[i] * spectrumDimensions.y));
		}
		SpectrumLine.Points = points.ToArray();
		SpectrumLine.SetVerticesDirty();
	}
}
