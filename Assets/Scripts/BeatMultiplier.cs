using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatMultiplier : MonoBehaviour
{

    public PlayerSettings PlayerSettings;

    public float BeatLevel = 0f;
    public Image[] beatLevelUI;

    public int currentLevel = 0;

    private void Start()
    {
        foreach (Image levelMeter in beatLevelUI)
        {
            levelMeter.fillAmount = 0f;
        }
        currentLevel = 0;
    }

    void Update()
    {
        if (BeatLevel > 0)
        {
            BeatLevel -= PlayerSettings.beatDecrease;
        }
        else
        {
            BeatLevel = 0f;
        }

        beatLevelUI[currentLevel].fillAmount = Mathf.Lerp(beatLevelUI[currentLevel].fillAmount, BeatLevel/50f, 0.5f);

        if (Mathf.Approximately(beatLevelUI[currentLevel].fillAmount, 1f) && currentLevel < beatLevelUI.Length-1)
        {
            currentLevel++;
            beatLevelUI[currentLevel].fillAmount = 0f;
            BeatLevel = 0f;
        }
        else if (Mathf.Approximately(beatLevelUI[currentLevel].fillAmount, 0f) && currentLevel > 0)
        {
            currentLevel--;            
            BeatLevel = 50f;
        }
    }

    // Update is called once per frame
    public void Beat(float beatInterval)
    {
        BeatLevel += beatInterval;
    }

    public void MissedBeat()
    {
        BeatLevel -= PlayerSettings.beatMissedPenalty;
    }
}
