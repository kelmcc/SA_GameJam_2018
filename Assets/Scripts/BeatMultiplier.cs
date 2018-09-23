using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatMultiplier : MonoBehaviour
{
    public PlayerSettings PlayerSettings;

    private float innerLevelProgress = 0f;
    public Image[] beatLevelUI;

    public int CurrentBeatKeeperLevel = 0;

    public Material SkyboxMat;

    public Color level1ColorA;
    public Color level1ColorB;

    public Color level2ColorA;
    public Color level2ColorB;

    public Color level3ColorA;
    public Color level3ColorB;


    private void Start()
    {
        foreach (Image levelMeter in beatLevelUI)
        {
            levelMeter.fillAmount = 0f;
        }
        CurrentBeatKeeperLevel = 0;
        SkyboxMat = RenderSettings.skybox;
        UpdateSkybox();
    }

    void Update()
    {
        if (innerLevelProgress > 0)
        {
            innerLevelProgress -= PlayerSettings.beatDecrease;
        }
        else
        {
            innerLevelProgress = 0f;
        }

        beatLevelUI[CurrentBeatKeeperLevel].fillAmount = Mathf.Lerp(beatLevelUI[CurrentBeatKeeperLevel].fillAmount, innerLevelProgress / 50f, 0.5f);

        if (Mathf.Approximately(beatLevelUI[CurrentBeatKeeperLevel].fillAmount, 1f) && CurrentBeatKeeperLevel < beatLevelUI.Length - 1)
        {
            CurrentBeatKeeperLevel++;
            beatLevelUI[CurrentBeatKeeperLevel].fillAmount = 5f;
            innerLevelProgress = 5f;

            //update the skybox too
            UpdateSkybox();
        }
        else if (Mathf.Approximately(beatLevelUI[CurrentBeatKeeperLevel].fillAmount, 0f) && CurrentBeatKeeperLevel > 0)
        {
            CurrentBeatKeeperLevel--;
            innerLevelProgress = 50f;

            //update the skybox too
            UpdateSkybox();
        }
    }

	public void AddLevelProgress(float increment)
	{
		innerLevelProgress += increment;
	}

	public void RemoveLevelProggress()
	{
		innerLevelProgress = 0;
	}

    public void UpdateSkybox()
    {
        switch (CurrentBeatKeeperLevel)
        {
            case 0:
                SkyboxMat.SetColor("_Color1", level1ColorA);
                SkyboxMat.SetColor("_Color2", level1ColorB);
                break;
            case 1:
                SkyboxMat.SetColor("_Color1", level2ColorA);
                SkyboxMat.SetColor("_Color2", level2ColorB);
                break;
            case 2:
                SkyboxMat.SetColor("_Color1", level3ColorA);
                SkyboxMat.SetColor("_Color2", level3ColorB);
                break;
        }
    }

    // Update is called once per frame
    public void Beat(float beatInterval)
    {
        innerLevelProgress += beatInterval;
    }

    public void MissedBeat()
    {
        innerLevelProgress -= PlayerSettings.beatMissedPenalty;
    }
}
