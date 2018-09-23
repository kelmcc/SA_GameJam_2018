using System;
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

	public CanvasGroup Overlay;

	public CanvasGroup MenuFade;

	public Vector2 spectrumDimensions;

	private BeatManager beatManager;

    public Image[] BeatLevel;

	// Use this for initialization
	void Start ()
	{
		GameManager.BeatManager.OnBeat += OnBeat;
		GameManager.BeatManager.OnSpectrumUpdated += SpectrumUpdate;
		Overlay.alpha = 0;
		MenuFade.alpha = 0;
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

	Coroutine c = null;
	public void ShowOverlayFor(float seconds)
	{
		if(c != null)
		{
			StopCoroutine(c);
		}
		c = StartCoroutine(PShowOverlayFor(seconds));
	}

	private IEnumerator PShowOverlayFor(float seconds)
	{
		Overlay.alpha = 1;

		yield return new WaitForSeconds(seconds);

		while(Overlay.alpha > 0)
		{
			Overlay.alpha -= Time.deltaTime;
			yield return null;
		}
	}

	Coroutine f = null;
	public void DoMenuFade(Action action)
	{
		if (f != null)
		{
			return;
		}
		MenuFade.alpha = 0;
		f = StartCoroutine(PDoMenuFade(action, 7));
	}

	private IEnumerator PDoMenuFade(Action action, float seconds)
	{
		
		while (Overlay.alpha < 1)
		{
			MenuFade.alpha += Time.deltaTime / seconds;
			yield return null;
		}

		f = null;
		action();	
	}
}
